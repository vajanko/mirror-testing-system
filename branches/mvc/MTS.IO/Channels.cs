using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace MTS.IO
{
    /// <summary>
    /// This class is only a thin cover over some IModule implementation
    /// It allows access to channels directly by properties
    /// </summary>
    public class Channels : IModule
    {
        #region Fields

        /// <summary>
        /// Instance of layer that is responsible for communication with hardware
        /// </summary>
        private IModule module;

        /// <summary>
        /// Collection of special channels. Special channels are defined by the application to
        /// allow uniform communication of execution logic with the rest of this application. All test have a special
        /// channel of name Is{testName}Enabled. Special channels are not transferred over network.
        /// </summary>
        private Dictionary<string, IChannel> specialChannels = new Dictionary<string, IChannel>();

        /// <summary>
        /// Collection of setting for all analog channels. This includes raw/real low/high values
        /// </summary>
        private ChannelSettings settings;

        #endregion

        #region IModule Members

        /// <summary>
        /// Load configuration of channels form file. At this time connection must not be established
        /// </summary>
        /// <param name="filename">Path to file where configuration of channels is stored</param>
        public void LoadConfiguration(string filename)
        {
            module.LoadConfiguration(filename);
        }
        /// <summary>
        /// Create a new connection between local computer and some hardware component. At the beginning of
        /// the communication this method must be called.
        /// </summary>
        /// <exception cref="MTS.IO.Module.ConnectionException">Connection could not be established</exception>
        public void Connect()
        {
            module.Connect();
        }
        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        /// <exception cref="MTS.IO.Address.AddressException">Address of some channel does not exists</exception>
        public void Initialize()
        {
            module.Initialize();

            // initialize channels: 
            // !!! PROPERTY NAME IS ALWAYS THE SAME AS CHANNEL NAME STRING !!!
            // Consider this when setting variable names in TwinCAT IO Server or 
            // editing configuration file for Moxa

            Type thisType = this.GetType();
            foreach (var channel in module.Inputs)
            {
                PropertyInfo prop = thisType.GetProperty(channel.Id);
                prop.SetValue(this, channel, null);
            }
            foreach (var channel in module.GetChannels<IAnalogInput>())
            {
                PropertyInfo prop = thisType.GetProperty(channel.Id);
                initializeChannel((IAnalogInput)prop.GetValue(this, null), settings.GetSetting(channel.Id));
            }          
        }
        /// <summary>
        /// Write all output and read all input channels (in this order)
        /// </summary>
        public void Update()
        {   // update all channels: read in/out and write out channels
            module.Update();
        }
        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        public void UpdateInputs()
        {
            module.UpdateInputs();
        }
        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
            module.UpdateOutputs();
        }
        /// <summary>
        /// Close connection between local computer and some hardware component. Call this method at the end of
        /// the communication to release resources.
        /// </summary>
        public void Disconnect()
        {   // prevent from disconnecting multiple times
            if (module != null && !module.IsConnected)
                module.Disconnect();
        }
        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        public bool IsConnected { get { return module.IsConnected; } }

        /// <summary>
        /// Switch all channels to safe state. This includes operations such as stopping mirror movement,
        /// powerfold, heating foil, ..., or moving calibrators down, ...
        /// </summary>
        public void SetupSafeState()
        {
            HeatingFoilOn.Off();
            DirectionLightOn.Off();
            StopPowerfold();
            StopMirror();
            StopAir();
            MoveCalibratorsDown();
            AllowMirrorMovement.Off();

            // write just changed values
            UpdateOutputs();
        }

        /// <summary>
        /// (Get) Name of underlying module communication protocol
        /// </summary>
        public string ProtocolName { get { return module.ProtocolName; } }
        /// <summary>
        /// (Get) Enumerate all input channels
        /// </summary>
        public IEnumerable<IChannel> Inputs
        {
            get { return module.Inputs; }
        }
        /// <summary>
        /// (Get) Enumerate all output channels
        /// </summary>
        public IEnumerable<IChannel> Outputs
        {
            get { return module.Outputs; }
        }
        /// <summary>
        /// Get an instance of particular channel identified by its name. Return null if there is no such a channel
        /// </summary>
        /// <param name="id">Unique name (identifier) of required channel</param>
        public IChannel GetChannel(string id)
        {
            if (specialChannels.ContainsKey(id))
                return specialChannels[id];
            return module.GetChannel(id);
        }
        /// <summary>
        /// Get all channels contained in this module of <typeparamref name="TChannel"/> type
        /// </summary>
        /// <typeparam name="TChannel">Type of channels to get. For example <see cref="IAnalogInput"/>
        /// or <see cref="IDigitalOutput"/> channels.</typeparam>
        /// <returns>Collection of channel of type <typeparamref name="TChannel"/></returns>
        public IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : IChannel
        {
            foreach (var channel in module.GetChannels<TChannel>())
                yield return channel;
        }
        /// <summary>
        /// Get channel of <typeparamref name="TChannel"/> type with given id.
        /// </summary>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="id"></param>
        /// <returns>Channel of <typeparamref name="TChannel"/> type with <paramref name="id"/> or
        /// null if there is no such a channel</returns>
        public TChannel GetChannel<TChannel>(string id) where TChannel : IChannel
        {
            if (specialChannels.ContainsKey(id))
            {
                TChannel ch = (TChannel)specialChannels[id];
                if (ch != null)
                    return ch;
            }
            return module.GetChannel<TChannel>(id);
        }

        #region IEnumerable Members

        /// <summary>
        /// Enumerate all channels in this module
        /// </summary>
        /// <returns>Enumerator of all channels</returns>
        public System.Collections.IEnumerator GetEnumerator()
        {
            return module.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release resources allocated by underlying module instance. Disconnect module if it is connected
        /// </summary>
        public void Dispose()
        {
            if (module != null)
            {
                Disconnect();
                module.Dispose();
            }
        }

        #endregion

        #endregion

        #region Public methods

        #region Lock/Unlock

        /// <summary>
        /// Get value indicating if device is closed (locked). This value depends on mirror type
        /// (old or new)
        /// </summary>
        /// <returns>Value indicating if device is closed</returns>
        public bool IsClosed()
        {
            return IsOldMirror.Value ? IsOldLocked.Value : IsLocked.Value;
        }

        #endregion

        #region Mirror glass

        /// <summary>
        /// (Get) Value indicating whether mirror glass is moving up
        /// </summary>
        public bool IsMirrorMoveingUp
        {
            get
            {
                return !MoveMirrorVertical.Value && MoveMirrorReverse.Value &&
                    MoveMirrorHorizontal.Value;
            }
        }
        /// <summary>
        /// (Get) Value indicating whether mirror glass is moving down
        /// </summary>
        public bool IsMirrorMoveingDown
        {
            get
            {
                return MoveMirrorVertical.Value && !MoveMirrorReverse.Value &&
                    !MoveMirrorHorizontal.Value;
            }
        }
        /// <summary>
        /// (Get) Value indicating whether mirror glass is moving left
        /// </summary>
        public bool IsMirrorMoveingLeft
        {
            get
            {
                return MoveMirrorVertical.Value && MoveMirrorReverse.Value &&
                    !MoveMirrorHorizontal.Value;
            }
        }
        /// <summary>
        /// (Get) Value indicating whether mirror glass is moving right
        /// </summary>
        public bool IsMirrorMoveingRight
        {
            get
            {
                return !MoveMirrorVertical.Value && !MoveMirrorReverse.Value &&
                    MoveMirrorHorizontal.Value;
            }
        }

        /// <summary>
        /// Start to move up mirror glass
        /// </summary>
        public void MoveMirrorUp()
        {
            MoveMirrorVertical.Value = false;
            MoveMirrorReverse.Value = true;
            MoveMirrorHorizontal.Value = true;
        }
        /// <summary>
        /// Start to move down mirror glass
        /// </summary>
        public void MoveMirrorDown()
        {
            MoveMirrorVertical.Value = true;
            MoveMirrorReverse.Value = false;
            MoveMirrorHorizontal.Value = false;
        }
        /// <summary>
        /// Start to move left mirror glass
        /// </summary>
        public void MoveMirrorLeft()
        {
            MoveMirrorVertical.Value = true;
            MoveMirrorReverse.Value = true;            
            MoveMirrorHorizontal.Value = false;
        }
        /// <summary>
        /// Start to move right mirror glass
        /// </summary>
        public void MoveMirrorRight()
        {
            MoveMirrorVertical.Value = false;
            MoveMirrorReverse.Value = false;
            MoveMirrorHorizontal.Value = true;
        }
        /// <summary>
        /// Stop moveing of mirror glass
        /// </summary>
        public void StopMirror()
        {
            MoveMirrorHorizontal.Value = false;
            MoveMirrorVertical.Value = false;
            MoveMirrorReverse.Value = false;
        }
        /// <summary>
        /// Start to move mirror glass in a particular direction
        /// </summary>
        /// <param name="dir">Direction in which to move the mirror</param>
        public void MoveMirror(MoveDirection dir)
        {
            switch (dir)
            {
                case MoveDirection.Up: MoveMirrorUp(); break;
                case MoveDirection.Down: MoveMirrorDown(); break;
                case MoveDirection.Left: MoveMirrorLeft(); break;
                case MoveDirection.Right: MoveMirrorRight(); break;
                default: StopMirror(); break;
            }
        }

        /// <summary>
        /// Start to move calibrators used for measuring mirror rotation angle up
        /// </summary>
        public void MoveCalibratorsUp()
        {
            MoveDistanceSensorUp.On();
            MoveDistanceSensorDown.Off();
        }
        /// <summary>
        /// Start to move calibrators used for measuring mirror rotation angle down
        /// </summary>
        public void MoveCalibratorsDown()
        {
            MoveDistanceSensorDown.On();
            MoveDistanceSensorUp.Off();
        }
             

        #endregion

        #region Sucker

        /// <summary>
        /// Start to move sucker down. Does not need to be stopped.
        /// </summary>
        public void SuckerDown()
        {
            MoveSuckerUp.Value = false;
            MoveSuckerDown.Value = true;
        }
        /// <summary>
        /// Start to move sucker up. Does not need to be stopped.
        /// </summary>
        public void SuckerUp()
        {
            MoveSuckerUp.Value = true;
            MoveSuckerDown.Value = false;
        }
        /// <summary>
        /// Start to suck air from the sucker disk. If air is being blew, it will be stopped
        /// </summary>
        public void StartSucking()
        {
            // for sure setup all values - so that never can happen that both values will be true
            SuckOn.Value = true;
            BlowOn.Value = false;
        }
        /// <summary>
        /// Start to blow air form the sucker disk. If air is being sucked, it will be stopped
        /// </summary>
        public void StartBlowing()
        {
            // for sure setup all values - so that never can happen that both values will be true
            SuckOn.Value = false;
            BlowOn.Value = true;
        }
        /// <summary>
        /// Stop to suck or blow air from sucker disk.
        /// </summary>
        public void StopAir()
        {
            SuckOn.Value = false;
            BlowOn.Value = false;
        }

        #endregion

        #region Powerfold

        /// <summary>
        /// Start to fold the powerfold. This method switch on actuators so that entire mirror is
        /// being moved in the direction to the car
        /// </summary>
        public void StartFolding()
        {
            UnfoldPowerfold.Value = false;
            FoldPowerfold.Value = true;
        }
        /// <summary>
        /// Start to unfold the powerfold. This method switch on actuators so that entire mirror is
        /// being moved in the direction from the car
        /// </summary>
        public void StartUnfolding()
        {
            UnfoldPowerfold.Value = true;
            FoldPowerfold.Value = false;
        }
        /// <summary>
        /// Stop to move mirror with powerfold. This method switch off actuators
        /// </summary>
        public void StopPowerfold()
        {
            FoldPowerfold.Value = false;
            UnfoldPowerfold.Value = false;
        }
        /// <summary>
        /// Get value indicating whether is powerfold folded. This value depends on type of mirror which is recognized
        /// by value of <see cref="IsOldMirror"/> channel
        /// </summary>
        public bool IsFolded()
        {   // if it is old mirror type, check for old powerfold sensor
            if (IsOldMirror.Value)
                return IsOldPowerfoldDown.Value;
            else
                return IsPowerfoldDown.Value;
        }
        /// <summary>
        /// Get value indicating whether is powerfold unfolded. This value depends on type of mirror which is recognized
        /// by value of <see cref="IsOldMirror"/> channel
        /// </summary>
        public bool IsUnfolded()
        {
            if (IsOldMirror.Value)
                return IsOldPowerfoldUp.Value;
            else
                return IsPowerfoldUp.Value;
        }

        #endregion

        // TODO: Optimize calculating of angles. YAxis and XAxis is not exact
        #region Rotation

        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde X
        /// </summary>
        private Point3D PointX;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Y
        /// </summary>
        private Point3D PointY;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Z
        /// </summary>
        private Point3D PointZ;

        private Vector3D YAxis = new Vector3D(0, 1, 0);     // this is not exactly
        private Vector3D XAxis = new Vector3D(1, 0, 0);
        /// <summary>
        /// (Get) Normal vector of mirror plane in the zero position. This is the moment when mirror
        /// is not rotated
        /// </summary>
        private Vector3D ZeroPlaneNormal { get; set; }

        /// <summary>
        /// Any 3D plane is determined by 3 points. Calculate normal vector to plane determined by this three points
        /// </summary>
        /// <param name="x">X-coordinate of plane</param>
        /// <param name="y">Y-coordinate of plane</param>
        /// <param name="z">Z-coordinate of plane</param>
        private static Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        {   // Get two vectors from tree points. Cross product gives us a perpendicular vector to both of them
            return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        }

        public double GetRotationAngle()
        {
            Vector3D normal = GetMirrorNormal();
            double angle = Vector3D.AngleBetween(normal, ZeroPlaneNormal);
            return angle;
        }

        /// <summary>
        /// Vector laying on the intersection of current mirror surface and zero position mirror surface
        /// This is the vector around which mirror is rotated
        /// </summary>
        public Vector3D GetRotationAxis()
        {
            Vector3D normal = GetMirrorNormal();
            Vector3D axis = Vector3D.CrossProduct(normal, ZeroPlaneNormal);            
            return axis;
        }

        public double GetHorizontalAngle()
        {
            double rotationAngle = GetRotationAngle();
            Vector3D rotationAxis = GetRotationAxis();

            double horizontalAngle = rotationAngle * Math.Cos(Vector3D.AngleBetween(rotationAxis, YAxis) / 180 * Math.PI);
            if (double.IsNaN(horizontalAngle))
                horizontalAngle = 0;
            return horizontalAngle;
            //return GetRotationAngle() * Math.Cos(Vector3D.AngleBetween(GetRotationAxis(), YAxis) / 180 * Math.PI);
        }
        public double GetVerticalAngle()
        {
            double rotationAngle = GetRotationAngle();
            Vector3D rotationAxis = GetRotationAxis();
            double verticalAngle = rotationAngle * Math.Cos(Vector3D.AngleBetween(rotationAxis, XAxis) / 180 * Math.PI);
            if (double.IsNaN(verticalAngle))
                verticalAngle = 0;
            return verticalAngle;
            //return -GetRotationAngle() * Math.Cos(Vector3D.AngleBetween(GetRotationAxis(), XAxis) / 180 * Math.PI);
        }

        /// <summary>
        /// Calculate normal vector to the current position of mirror glass
        /// </summary>
        /// <returns>Normal vector of mirror glass surface</returns>
        public Vector3D GetMirrorNormal()
        {
            // read distance values
            PointX.Z = DistanceX.RealValue;
            PointY.Z = DistanceY.RealValue;
            PointZ.Z = DistanceZ.RealValue;
            Vector3D normal = getPlaneNormal(PointX, PointY, PointZ);
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// This method should be called when channels for measuring mirror rotation will be used.
        /// It initialize setting which come from hardware settings and should be configured by user.
        /// Their are necessary for correct measurement of mirror glass rotation.
        /// </summary>
        /// <param name="zeroPlaneNormal">Normal of plane which is considered to be a default position. 
        /// In this position mirror is centered</param>
        /// <param name="calibratorX">Position of X calibrator in 2D space. Z coordinate of this structure 
        /// is ignored</param>
        /// <param name="calibratorY">Position of Y calibrator in 2D space. Z coordinate of this structure
        /// is ignored</param>
        /// <param name="calibratorZ">Position of Z calibrator in 2D space. Z coordinate of this structure
        /// is ignored</param>
        public void InitializeCalibratorsSettings(Vector3D zeroPlaneNormal, Point3D calibratorX,
            Point3D calibratorY, Point3D calibratorZ)
        {
            // Normal of plane which is considered to be a default position. In this position
            // mirror is centered
            this.ZeroPlaneNormal = zeroPlaneNormal;
            // These setting comes from application settings
            // X and Y coordinates of these points are positions of measuring calibrators
            // Z coordinates are distances of the mirror surface
            this.PointX = calibratorX;
            this.PointY = calibratorY;
            this.PointZ = calibratorZ;
        }

        public double GetRotationAngle(MoveDirection dir)
        {
            if (dir.IsHorizontal())
                return dir == MoveDirection.Left ? GetHorizontalAngle() : -GetHorizontalAngle();
            else
                return dir == MoveDirection.Up ? -GetVerticalAngle() : GetVerticalAngle();
        }

        #endregion

        #endregion

        // TODO: add description of channels
        #region Channels

        #region Digital inputs

        /// <summary>
        /// (Get) Channel indicating whether calibrators are in upper position. If true measurement of
        /// mirror glass can be performed.
        /// </summary>
        public IDigitalInput IsDistanceSensorUp { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether calibrators are in lower position. If true it is possible
        /// to manipulate with the mirror or pull off test can be performed. At the end of testing sequence
        /// calibrators must be moved down.
        /// </summary>
        public IDigitalInput IsDistanceSensorDown { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether sucker disk is in upper position. If true pull of test
        /// can be performed.
        /// </summary>
        public IDigitalInput IsSuckerUp { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether sucker disk is in lower position. If true it is possible to 
        /// manipulate with the mirror or mirror glass movement can be performed. At the end of testing 
        /// sequence sucker disk must be moved down.
        /// </summary>
        public IDigitalInput IsSuckerDown { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether vacuum is created in the space between sucker disk and mirror
        /// glass. If true the glass pull off process can be performed.
        /// </summary>
        public IDigitalInput IsVacuum { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether rubber on mirror cable is present. This value is only significant
        /// for left mirror. For right mirror always is false.
        /// </summary>
        public IDigitalInput IsLeftRubberPresent { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether rubber on mirror cable is present. This value is only significant
        /// for right mirror. For left mirror always is false.
        /// </summary>
        public IDigitalInput IsRightRubberPresent { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether tester device is closed. This value is significant only for
        /// new mirror types. For old mirror it is unpredictable.
        /// </summary>
        public IDigitalInput IsLocked { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether tester device is closed. This value is significant only for
        /// old mirror types. For new mirror it is unpredictable.
        /// </summary>
        public IDigitalInput IsOldLocked { get; private set; }
        /// <summary>
        /// (Get) Channel indicating the orientation of mirror inserted to tester. If true it is left
        /// otherwise it is right.
        /// </summary>
        public IDigitalInput IsLeftMirror { get; private set; }
        /// <summary>
        /// (Get) Channel indicating the type of mirror inserted to tester. If true it is old
        /// otherwise if is new type.
        /// </summary>
        public IDigitalInput IsOldMirror { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether START button on the tester is pressed. If true it is pressed
        /// otherwise it is released.
        /// </summary>
        public IDigitalInput IsStartPressed { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether power supply of tester is switched off. If true power supply
        /// is off otherwise it is on.
        /// </summary>
        public IDigitalInput IsPowerSupplyOff { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether mirror is folded. If true mirror is folded (on the car 
        /// it will be closer to the window) otherwise it is unfolded. This value is significant only
        /// for new mirror types. For old mirror type it is unpredictable.
        /// </summary>
        public IDigitalInput IsPowerfoldDown { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether mirror is unfolded. If true mirror is unfolded (on the car
        /// it will be in its default position) otherwise it is folded. This value is significant only
        /// for new mirror types. For old mirror type it is unpredictable.
        /// </summary>
        public IDigitalInput IsPowerfoldUp { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether mirror is folded. If true mirror is folded (on the car 
        /// it will be closer to the window) otherwise it is unfolded. This value is significant only
        /// for old mirror types. For new mirror type it is unpredictable.
        /// </summary>
        public IDigitalInput IsOldPowerfoldDown { get; private set; }
        /// <summary>
        /// (Get) Channel indicating whether mirror is unfolded. If true mirror is unfolded (on the car
        /// it will be in its default position) otherwise it is folded. This value is significant only
        /// for new mirror types. For old mirror type it is unpredictable.
        /// </summary>
        public IDigitalInput IsOldPowerfoldUp { get; private set; }
        
        /// <summary>
        /// (Get) Channel indicating whether ERROR ACKNOWLEDGMENT button on the tester is pressed. If true it
        /// is pressed otherwise it is released.
        /// </summary>
        public IDigitalInput IsAckPressed { get; private set; }

        //public IDigitalInput HeatingFoilSignSensor { get; private set; }
        //public IDigitalInput PowerFoldUnfoldedPositionSensor1 { get; private set; }
        //public IDigitalInput PowerFoldUnfoldedPositionSensor2 { get; private set; }
        //public IDigitalInput PowerFoldFoldedPositionSensor { get; private set; }
        //public IDigitalInput TestingDeviceOpened { get; private set; }
        //public IDigitalInput TestingDeviceClosed { get; private set; }

        //public IDigitalInput ErrorAcknButton { get; private set; }

        //public IDigitalInput SensorHeadOut { get; private set; }
        //public IDigitalInput SensorHeadIn { get; private set; }
        public IDigitalInput InsCheck1 { get; private set; }
        public IDigitalInput InsCheck2 { get; private set; }
        public IDigitalInput InsCheck3 { get; private set; }
        public IDigitalInput InsCheck4 { get; private set; }
        public IDigitalInput InsCheck5 { get; private set; }
        public IDigitalInput InsCheck6 { get; private set; }
        public IDigitalInput InsCheck7 { get; private set; }
        public IDigitalInput InsCheck8 { get; private set; }
        public IDigitalInput InsCheck9 { get; private set; }
        public IDigitalInput InsCheck10 { get; private set; }
        public IDigitalInput InsCheck11 { get; private set; }
        public IDigitalInput InsCheck12 { get; private set; }
        public IDigitalInput InsCheck13 { get; private set; }
        public IDigitalInput InsCheck14 { get; private set; }
        public IDigitalInput InsCheck15 { get; private set; }
        public IDigitalInput InsCheck16 { get; private set; }

        #endregion

        #region Digital outputs

        public IDigitalOutput AllowMirrorMovement { get; private set; }
        public IDigitalOutput MoveMirrorVertical { get; private set; }
        public IDigitalOutput MoveMirrorHorizontal { get; private set; }
        public IDigitalOutput MoveMirrorReverse { get; private set; }
        public IDigitalOutput FoldPowerfold { get; private set; }
        public IDigitalOutput UnfoldPowerfold { get; private set; }
        public IDigitalOutput HeatingFoilOn { get; private set; }
        public IDigitalOutput DirectionLightOn { get; private set; }

        public IDigitalOutput LockWeak { get; private set; }
        public IDigitalOutput UnlockWeak { get; private set; }
        public IDigitalOutput MoveDistanceSensorUp { get; private set; }
        public IDigitalOutput MoveDistanceSensorDown { get; private set; }
        /// <summary>
        /// (Get) Channel that controls sucker disk. If true support with the sucker disk is being
        /// moved up otherwise no action is performed.
        /// </summary>
        public IDigitalOutput MoveSuckerUp { get; private set; }
        /// <summary>
        /// (Get) Channel that controls sucker disk. If true support with the sucker disk is being
        /// moved down otherwise no action is performed.
        /// </summary>
        public IDigitalOutput MoveSuckerDown { get; private set; }
        /// <summary>
        /// (Get) Channel that controls air sucking on sucker disk. If true the air is being sucked,
        /// otherwise no action is performed. Do not use together with <see cref="BlowOn"/>.
        /// </summary>
        public IDigitalOutput SuckOn { get; private set; }
        /// <summary>
        /// (Get) Channel that controls air blowing on sucker disk. If true the air is being blown,
        /// otherwise on action is performed. Do not user together with <see cref="SuckOn"/>.
        /// </summary>
        public IDigitalOutput BlowOn { get; private set; }

        public IDigitalOutput AllowPowerSupply { get; private set; }
        public IDigitalOutput LockStrong { get; private set; }
        public IDigitalOutput UnlockStrong { get; private set; }

        /// <summary>
        /// (Get) Channel that controls the green light on tester. If true green light is switched on
        /// otherwise it is off.
        /// </summary>
        public IDigitalOutput GreenLightOn { get; private set; }
        /// <summary>
        /// (Get) Channel that controls the red light on tester. If true red light is switched on
        /// otherwise it is off.
        /// </summary>
        public IDigitalOutput RedLightOn { get; private set; }
        /// <summary>
        /// (Get) Channel that controls the buzzer on tester. If true buzzer is switched on
        /// otherwise it is off.
        /// </summary>
        public IDigitalOutput BuzzerOn { get; private set; }

        #endregion

        #region Analog inputs

        public IAnalogInput DistanceX { get; set; }
        public IAnalogInput DistanceY { get; set; }
        public IAnalogInput DistanceZ { get; set; }
        public IAnalogInput VerticalActuatorCurrent { get; set; }
        public IAnalogInput HorizontalActuatorCurrent { get; set; }
        public IAnalogInput HeatingFoilCurrent { get; set; }
        public IAnalogInput PowerfoldCurrent { get; set; }
        public IAnalogInput DirectionLightCurrent { get; set; }

        public IAnalogInput PowerSupplyVoltage1 { get; set; }
        public IAnalogInput PowerSupplyVoltage2 { get; set; }

        #endregion

        #region Special Channels

        /// <summary>
        /// Remove all special channels from this module. Special channels are defined by the application to
        /// allow uniform communication of execution logic with the rest of this application. All test have a special
        /// channel of name Is{testName}Enabled. Special channels are not transferred over network.
        /// </summary>
        public void ClearSpecialChannels()
        {
            specialChannels.Clear();
        }
        /// <summary>
        /// Add a new special channel to this module. Special channels are defined by the application to
        /// allow uniform communication of execution logic with the rest of this application. All test have a special
        /// channel of name Is{testName}Enabled. Special channels are not transferred over network.
        /// </summary>
        /// <param name="channel">Instance of special channel to be added</param>
        public void AddSpecialChannel(IChannel channel)
        {
            specialChannels.Add(channel.Id, channel);
        }

        #endregion

        /// <summary>
        /// Initialize given channel with given channel settings
        /// </summary>
        /// <param name="channel">Analog channel to initialize</param>
        /// <param name="setting">Setting to apply on given analog channel</param>
        private static void initializeChannel(IAnalogInput channel, ChannelSetting setting)
        {
            channel.RawLow = setting.RawLow;
            channel.RawHigh = setting.RawHigh;
            channel.RealLow = setting.RealLow;
            channel.RealHigh = setting.RealHigh;
            channel.Name = setting.Name;
            channel.Description = setting.Description;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of wrapper over IModule implementation. It allows access to channels
        /// through properties, not thought names as IModule does.
        /// </summary>
        /// <param name="module">Implementation of <typeparamref name="IModule"/> for a particular
        /// protocol, such as EtherCAT or Modbus TCP/IP (...)</param>
        /// <param name="settings">Collection of setting for all analog channels</param>
        public Channels(IModule module, ChannelSettings settings)
        {
            this.module = module;
            this.settings = settings;
        }

        #endregion
    }
}
