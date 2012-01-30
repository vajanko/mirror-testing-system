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
        /// <summary>
        /// Instance of layer that is responsible for communication with hardware
        /// </summary>
        private IModule module;

        private Dictionary<string, IChannel> specialChannels = new Dictionary<string, IChannel>();

        /// <summary>
        /// Collection of setting for all analog channels. This includes raw/real low/high values
        /// </summary>
        private ChannelSettings settings;

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

            // setting channels property here is only temporary a will be changed in the future
            // this could be also done with reflection
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

            //// analog inputs - also initialize its settings raw/real low/high values
            //DistanceX = module.GetChannel<IAnalogInput<TAddress>>("DistanceX");
            //initializeChannel(DistanceX, settings.GetSetting("DistanceX"));

            //DistanceY = module.GetChannel<IAnalogInput<TAddress>>("DistanceY");
            //initializeChannel(DistanceY, settings.GetSetting("DistanceY"));

            //DistanceZ = module.GetChannel<IAnalogInput<TAddress>>("DistanceZ");
            //initializeChannel(DistanceZ, settings.GetSetting("DistanceZ"));

            //PowerfoldCurrent = module.GetChannel<IAnalogInput<TAddress>>("PowerfoldCurrent");
            //initializeChannel(PowerfoldCurrent, settings.GetSetting("PowerfoldCurrent"));

            //HeatingFoilCurrent = module.GetChannel<IAnalogInput<TAddress>>("HeatingFoilCurrent");
            //initializeChannel(HeatingFoilCurrent, settings.GetSetting("HeatingFoilCurrent"));

            //VerticalActuatorCurrent = module.GetChannel<IAnalogInput<TAddress>>("VerticalActuatorCurrent");
            //initializeChannel(VerticalActuatorCurrent, settings.GetSetting("VerticalActuatorCurrent"));

            //HorizontalActuatorCurrent = module.GetChannel<IAnalogInput<TAddress>>("HorizontalActuatorCurrent");
            //initializeChannel(HorizontalActuatorCurrent, settings.GetSetting("HorizontalActuatorCurrent"));

            //DirectionLightCurrent = module.GetChannel<IAnalogInput<TAddress>>("DirectionLightCurrent");
            //initializeChannel(DirectionLightCurrent, settings.GetSetting("DirectionLightCurrent"));

            //PowerSupplyVoltage1 = module.GetChannel<IAnalogInput<TAddress>>("PowerSupplyVoltage1");
            //initializeChannel(PowerSupplyVoltage1, settings.GetSetting("PowerSupplyVoltage1"));

            //PowerSupplyVoltage2 = module.GetChannel<IAnalogInput<TAddress>>("PowerSupplyVoltage2");
            //initializeChannel(PowerSupplyVoltage2, settings.GetSetting("PowerSupplyVoltage2"));
            ////// analog inputs

            //// digital inputs
            //IsDistanceSensorUp = module.GetChannel<IDigitalInput<TAddress>>("IsDistanceSensorUp");
            //IsDistanceSensorDown = module.GetChannel<IDigitalInput<TAddress>>("IsDistanceSensorDown");
            //IsSuckerUp = module.GetChannel<IDigitalInput<TAddress>>("IsSuckerUp");
            //IsSuckerDown = module.GetChannel<IDigitalInput<TAddress>>("IsSuckerDown");
            //IsVacuum = module.GetChannel<IDigitalInput<TAddress>>("IsVacuum");
            //IsLeftRubberPresent = module.GetChannel<IDigitalInput<TAddress>>("IsLeftRubberPresent");

            //IsPowerfoldDown = module.GetChannel<IDigitalInput<TAddress>>("IsPowerfoldDown");
            //IsPowerfoldUp = module.GetChannel<IDigitalInput<TAddress>>("IsPowerfoldUp");
            //IsOldPowerfoldUp = module.GetChannel<IDigitalInput<TAddress>>("IsOldPowerfoldUp");
            //IsRightRubberPresent = module.GetChannel<IDigitalInput<TAddress>>("IsRightRubberPresent");

            //IsLeftMirror = module.GetChannel<IDigitalInput<TAddress>>("IsLeftMirror");
            //IsOldMirror = module.GetChannel<IDigitalInput<TAddress>>("IsOldMirror");
            //IsOldPowerfoldDown = module.GetChannel<IDigitalInput<TAddress>>("IsOldPowerfoldDown");
            //IsStartPressed = module.GetChannel<IDigitalInput<TAddress>>("IsStartPressed");
            //IsAckPressed = module.GetChannel<IDigitalInput<TAddress>>("IsAckPressed");

            //IsLocked = module.GetChannel<IDigitalInput<TAddress>>("IsLocked");
            //IsOldLocked = module.GetChannel<IDigitalInput<TAddress>>("IsOldLocked");

            //IsPowerSupplyOff = module.GetChannel<IDigitalInput<TAddress>>("IsPowerSupplyOff");
            //// digital inputs

            //// digital outputs
            //AllowMirrorMovement = module.GetChannel<IDigitalOutput<TAddress>>("AllowMirrorMovement");
            //MoveMirrorVertical = module.GetChannel<IDigitalOutput<TAddress>>("MoveMirrorVertical");
            //MoveMirrorHorizontal = module.GetChannel<IDigitalOutput<TAddress>>("MoveMirrorHorizontal");
            //MoveMirrorReverse = module.GetChannel<IDigitalOutput<TAddress>>("MoveMirrorReverse");
            //FoldPowerfold = module.GetChannel<IDigitalOutput<TAddress>>("FoldPowerfold");
            //UnfoldPowerfold = module.GetChannel<IDigitalOutput<TAddress>>("UnfoldPowerfold");
            //HeatingFoilOn = module.GetChannel<IDigitalOutput<TAddress>>("HeatingFoilOn");
            //DirectionLightOn = module.GetChannel<IDigitalOutput<TAddress>>("DirectionLightOn");

            //LockWeak = module.GetChannel<IDigitalOutput<TAddress>>("LockWeak");
            //UnlockWeak = module.GetChannel<IDigitalOutput<TAddress>>("UnlockWeak");
            //MoveDistanceSensorUp = module.GetChannel<IDigitalOutput<TAddress>>("MoveDistanceSensorUp");
            //MoveDistanceSensorDown = module.GetChannel<IDigitalOutput<TAddress>>("MoveDistanceSensorDown");
            //MoveSuckerUp = module.GetChannel<IDigitalOutput<TAddress>>("MoveSuckerUp");
            //MoveSuckerDown = module.GetChannel<IDigitalOutput<TAddress>>("MoveSuckerDown");
            //SuckOn = module.GetChannel<IDigitalOutput<TAddress>>("SuckOn");
            //BlowOn = module.GetChannel<IDigitalOutput<TAddress>>("BlowOn");

            //AllowPowerSupply = module.GetChannel<IDigitalOutput<TAddress>>("AllowPowerSupply");
            //LockStrong = module.GetChannel<IDigitalOutput<TAddress>>("LockStrong");
            //UnlockStrong = module.GetChannel<IDigitalOutput<TAddress>>("UnlockStrong");
            //GreenLightOn = module.GetChannel<IDigitalOutput<TAddress>>("GreenLightOn");
            //RedLightOn = module.GetChannel<IDigitalOutput<TAddress>>("RedLightOn");
            //BuzzerOn = module.GetChannel<IDigitalOutput<TAddress>>("BuzzerOn");

            // digital outputs

            //// ???
            //TestingDeviceOpened = module.GetChannel("TestingDeviceOpened");
            //TestingDeviceClosed = module.GetChannel("TestingDeviceClosed");
            //ErrorAcknButton = module.GetChannel("ErrorAcknButton");

            //// powerfold
            //PowerFoldUnfoldedPositionSensor1 = module.GetChannel("PowerFoldUnfoldedPositionSensor1");
            //PowerFoldUnfoldedPositionSensor2 = module.GetChannel("PowerFoldUnfoldedPositionSensor2");
            //PowerFoldFoldedPositionSensor = module.GetChannel("PowerFoldFoldedPositionSensor");
            

            //HeatingFoilSignSensor = module.GetChannel("HeatingFoilSignSensor");

            //SensorHeadOut = module.GetChannel("SensorHeadOut");
            //SensorHeadIn = module.GetChannel("SensorHeadIn");
            //InsCheck1 = module.GetChannel("InsCheck1");
            //InsCheck2 = module.GetChannel("InsCheck2");
            //InsCheck3 = module.GetChannel("InsCheck3");
            //InsCheck4 = module.GetChannel("InsCheck4");
            //InsCheck5 = module.GetChannel("InsCheck5");
            //InsCheck6 = module.GetChannel("InsCheck6");
            //InsCheck7 = module.GetChannel("InsCheck7");
            //InsCheck8 = module.GetChannel("InsCheck8");
            //InsCheck9 = module.GetChannel("InsCheck9");
            //InsCheck10 = module.GetChannel("InsCheck10");
            //InsCheck11 = module.GetChannel("InsCheck11");
            //InsCheck12 = module.GetChannel("InsCheck12");
            //InsCheck13 = module.GetChannel("InsCheck13");
            //InsCheck14 = module.GetChannel("InsCheck14");
            //InsCheck15 = module.GetChannel("InsCheck15");
            //InsCheck16 = module.GetChannel("InsCheck16");            
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

        public IEnumerable<IChannel> Inputs
        {
            get { return module.Inputs; }
        }

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
        /// Get all channels contained in this current module of <typeparamref name="TChannel"/> type
        /// </summary>
        /// <typeparam name="TChannel">Type of channels to get. For example <see cref="IAnalogInput"/>
        /// or <see cref="IDigitalOutput"/> channels.</typeparam>
        /// <returns>Collection of channel of type <typeparamref name="TChannel"/></returns>
        public IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : IChannel
        {
            foreach (var channel in module.GetChannels<TChannel>())
                yield return channel;
        }
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
        /// (Get) Value indicating whether mirror glass is moveing down
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
        /// (Get) Value indicating whether mirror glass is moveing left
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
        /// (Get) Value indicating whether mirror glass is moveing right
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
            normal.Normalize();
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

        public void AddChannel(IChannel channel)
        {
            specialChannels.Add(channel.Id, channel);
        }

        // TODO: add description of channels
        #region Channels

        #region Digital inputs

        public IDigitalInput IsDistanceSensorUp { get; set; }
        public IDigitalInput IsDistanceSensorDown { get; set; }
        public IDigitalInput IsSuckerUp { get; set; }
        public IDigitalInput IsSuckerDown { get; set; }
        public IDigitalInput IsVacuum { get; set; }
        public IDigitalInput IsLeftRubberPresent { get; set; }


        public IDigitalInput IsPowerfoldDown { get; set; }
        public IDigitalInput IsPowerfoldUp { get; set; }
        public IDigitalInput IsOldPowerfoldUp { get; set; }
        public IDigitalInput IsRightRubberPresent { get; set; }
        public IDigitalInput IsLeftMirror { get; set; }
        public IDigitalInput IsOldMirror { get; set; }
        public IDigitalInput IsOldPowerfoldDown { get; set; }
        public IDigitalInput IsStartPressed { get; set; }
        public IDigitalInput IsAckPressed { get; set; }

        public IDigitalInput IsLocked { get; set; }
        public IDigitalInput IsOldLocked { get; set; }

        public IDigitalInput HeatingFoilSignSensor { get; set; }
        public IDigitalInput PowerFoldUnfoldedPositionSensor1 { get; set; }
        public IDigitalInput PowerFoldUnfoldedPositionSensor2 { get; set; }
        public IDigitalInput PowerFoldFoldedPositionSensor { get; set; }
        public IDigitalInput TestingDeviceOpened { get; set; }
        public IDigitalInput TestingDeviceClosed { get; set; }
        
        public IDigitalInput ErrorAcknButton { get; set; }

        public IDigitalInput SensorHeadOut { get; set; }
        public IDigitalInput SensorHeadIn { get; set; }
        public IDigitalInput InsCheck1 { get; set; }
        public IDigitalInput InsCheck2 { get; set; }
        public IDigitalInput InsCheck3 { get; set; }
        public IDigitalInput InsCheck4 { get; set; }
        public IDigitalInput InsCheck5 { get; set; }
        public IDigitalInput InsCheck6 { get; set; }
        public IDigitalInput InsCheck7 { get; set; }
        public IDigitalInput InsCheck8 { get; set; }
        public IDigitalInput InsCheck9 { get; set; }
        public IDigitalInput InsCheck10 { get; set; }
        public IDigitalInput InsCheck11 { get; set; }
        public IDigitalInput InsCheck12 { get; set; }
        public IDigitalInput InsCheck13 { get; set; }
        public IDigitalInput InsCheck14 { get; set; }
        public IDigitalInput InsCheck15 { get; set; }
        public IDigitalInput InsCheck16 { get; set; }
        public IDigitalInput IsPowerSupplyOff { get; set; }


        #endregion

        #region Digital outputs

        public IDigitalOutput AllowMirrorMovement { get; set; }
        public IDigitalOutput MoveMirrorVertical { get; set; }
        public IDigitalOutput MoveMirrorHorizontal { get; set; }
        public IDigitalOutput MoveMirrorReverse { get; set; }
        public IDigitalOutput FoldPowerfold { get; set; }
        public IDigitalOutput UnfoldPowerfold { get; set; }
        public IDigitalOutput HeatingFoilOn { get; set; }
        public IDigitalOutput DirectionLightOn { get; set; }

        public IDigitalOutput LockWeak { get; set; }
        public IDigitalOutput UnlockWeak { get; set; }
        public IDigitalOutput MoveDistanceSensorUp { get; set; }
        public IDigitalOutput MoveDistanceSensorDown { get; set; }
        public IDigitalOutput MoveSuckerUp { get; set; }
        public IDigitalOutput MoveSuckerDown { get; set; }
        public IDigitalOutput SuckOn { get; set; }
        public IDigitalOutput BlowOn { get; set; }

        public IDigitalOutput AllowPowerSupply { get; set; }
        public IDigitalOutput LockStrong { get; set; }
        public IDigitalOutput UnlockStrong { get; set; }
        public IDigitalOutput GreenLightOn { get; set; }
        public IDigitalOutput RedLightOn { get; set; }
        public IDigitalOutput BuzzerOn { get; set; }

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
