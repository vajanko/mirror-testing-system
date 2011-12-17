using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

using MTS.IO.Properties;

namespace MTS.IO 
{

    /// <summary>
    /// This class is only a thin cover oover some IModule implementation
    /// It allows access to channels directrly by properties
    /// </summary>
    public partial class Channels : IModule
    {
        /// <summary>
        /// Instance of layer that is resposible for communication with hardware
        /// </summary>
        private IModule module;

        /// <summary>
        /// Collection of setting for all analog channels. This includes raw/real low/high vlaues
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
        public void Connect()
        {
            module.Connect();
        }
        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        public void Initialize()
        {
            module.Initialize();

            // initialize channels: 
            // !!! PROPERTY NAME IS ALWAYS THE SAME AS CHANNEL NAME STRING !!!
            // Consider this when setting variable names in TwinCAT IO Server or 
            // editing configuration file for Moxa

            // setting channels propety here is only temporary a will be changed in the future

            //// analog inputs - also initialize its settings raw/real low/high values
            DistanceX = (IAnalogInput)module.GetChannelByName("DistanceX");
            initializeChannel(DistanceX, settings.GetSetting("DistanceX"));

            DistanceY = (IAnalogInput)module.GetChannelByName("DistanceY");
            initializeChannel(DistanceY, settings.GetSetting("DistanceY"));

            DistanceZ = (IAnalogInput)module.GetChannelByName("DistanceZ");
            initializeChannel(DistanceZ, settings.GetSetting("DistanceZ"));

            PowerfoldCurrent = (IAnalogInput)module.GetChannelByName("PowerfoldCurrent");
            initializeChannel(PowerfoldCurrent, settings.GetSetting("PowerfoldCurrent"));

            HeatingFoilCurrent = (IAnalogInput)module.GetChannelByName("HeatingFoilCurrent");
            initializeChannel(HeatingFoilCurrent, settings.GetSetting("HeatingFoilCurrent"));

            VerticalActuatorCurrent = (IAnalogInput)module.GetChannelByName("VerticalActuatorCurrent");
            initializeChannel(VerticalActuatorCurrent, settings.GetSetting("VerticalActuatorCurrent"));

            HorizontalActuatorCurrent = (IAnalogInput)module.GetChannelByName("HorizontalActuatorCurrent");
            initializeChannel(HorizontalActuatorCurrent, settings.GetSetting("HorizontalActuatorCurrent"));

            DirectionLightCurrent = (IAnalogInput)module.GetChannelByName("DirectionLightCurrent");
            initializeChannel(DirectionLightCurrent, settings.GetSetting("DirectionLightCurrent"));

            PowerSupplyVoltage1 = (IAnalogInput)module.GetChannelByName("PowerSupplyVoltage1");
            initializeChannel(PowerSupplyVoltage1, settings.GetSetting("PowerSupplyVoltage1"));

            PowerSupplyVoltage2 = (IAnalogInput)module.GetChannelByName("PowerSupplyVoltage2");
            initializeChannel(PowerSupplyVoltage2, settings.GetSetting("PowerSupplyVoltage2"));
            //// analog inputs

            // digital inputs
            IsDistanceSensorUp = (IDigitalInput)module.GetChannelByName("IsDistanceSensorUp");
            IsDistanceSensorDown = (IDigitalInput)module.GetChannelByName("IsDistanceSensorDown");
            IsSuckerUp = (IDigitalInput)module.GetChannelByName("IsSuckerUp");
            IsSuckerDown = (IDigitalInput)module.GetChannelByName("IsSuckerDown");
            IsVacuum = (IDigitalInput)module.GetChannelByName("IsVacuum");
            IsLeftRubberPresent = (IDigitalInput)module.GetChannelByName("IsLeftRubberPresent");

            IsPowerfoldDown = (IDigitalInput)module.GetChannelByName("IsPowerfoldDown");
            IsPowerfoldUp = (IDigitalInput)module.GetChannelByName("IsPowerfoldUp");
            IsOldPowerfoldUp = (IDigitalInput)module.GetChannelByName("IsOldPowerfoldUp");
            IsRightRubberPresent = (IDigitalInput)module.GetChannelByName("IsRightRubberPresent");

            IsLeftMirror = (IDigitalInput)module.GetChannelByName("IsLeftMirror");
            IsOldMirror = (IDigitalInput)module.GetChannelByName("IsOldMirror");
            IsOldPowerfoldDown = (IDigitalInput)module.GetChannelByName("IsOldPowerfoldDown");
            IsStartPressed = (IDigitalInput)module.GetChannelByName("IsStartPressed");
            IsAckPressed = (IDigitalInput)module.GetChannelByName("IsAckPressed");

            IsLocked = (IDigitalInput)module.GetChannelByName("IsLocked");
            IsOldLocked = (IDigitalInput)module.GetChannelByName("IsOldLocked");
            // digital inputs

            // digital outputs
            AllowMirrorMovement = (IDigitalOutput)module.GetChannelByName("AllowMirrorMovement");
            MoveMirrorVertical = (IDigitalOutput)module.GetChannelByName("MoveMirrorVertical");
            MoveMirrorHorizontal = (IDigitalOutput)module.GetChannelByName("MoveMirrorHorizontal");
            MoveMirrorReverse = (IDigitalOutput)module.GetChannelByName("MoveMirrorReverse");
            FoldPowerfold = (IDigitalOutput)module.GetChannelByName("FoldPowerfold");
            UnfoldPowerfold = (IDigitalOutput)module.GetChannelByName("UnfoldPowerfold");
            HeatingFoilOn = (IDigitalOutput)module.GetChannelByName("HeatingFoilOn");
            DirectionLightOn = (IDigitalOutput)module.GetChannelByName("DirectionLightOn");

            LockWeak = (IDigitalOutput)module.GetChannelByName("LockWeak");
            UnlockWeak = (IDigitalOutput)module.GetChannelByName("UnlockWeak");
            MoveDistanceSensorUp = (IDigitalOutput)module.GetChannelByName("MoveDistanceSensorUp");
            MoveDistanceSensorDown = (IDigitalOutput)module.GetChannelByName("MoveDistanceSensorDown");
            MoveSuckerUp = (IDigitalOutput)module.GetChannelByName("MoveSuckerUp");
            MoveSuckerDown = (IDigitalOutput)module.GetChannelByName("MoveSuckerDown");
            SuckOn = (IDigitalOutput)module.GetChannelByName("SuckOn");
            BlowOn = (IDigitalOutput)module.GetChannelByName("BlowOn");

            AllowPowerSupply = (IDigitalOutput)module.GetChannelByName("AllowPowerSupply");
            LockStrong = (IDigitalOutput)module.GetChannelByName("LockStrong");
            UnlockStrong = (IDigitalOutput)module.GetChannelByName("UnlockStrong");
            GreenLightOn = (IDigitalOutput)module.GetChannelByName("GreenLightOn");
            RedLightOn = (IDigitalOutput)module.GetChannelByName("RedLightOn");
            BuzzerOn = (IDigitalOutput)module.GetChannelByName("BuzzerOn");

            // digital outputs

            // ???
            TestingDeviceOpened = (IDigitalInput)module.GetChannelByName("TestingDeviceOpened");
            TestingDeviceClosed = (IDigitalInput)module.GetChannelByName("TestingDeviceClosed");
            ErrorAcknButton = (IDigitalInput)module.GetChannelByName("ErrorAcknButton");
            

            // powerfold
            PowerFoldUnfoldedPositionSensor1 = (IDigitalInput)module.GetChannelByName("PowerFoldUnfoldedPositionSensor1");
            PowerFoldUnfoldedPositionSensor2 = (IDigitalInput)module.GetChannelByName("PowerFoldUnfoldedPositionSensor2");
            PowerFoldFoldedPositionSensor = (IDigitalInput)module.GetChannelByName("PowerFoldFoldedPositionSensor");
            

            HeatingFoilSignSensor = (IDigitalInput)module.GetChannelByName("HeatingFoilSignSensor");

            SensorHeadOut = (IDigitalInput)module.GetChannelByName("SensorHeadOut");
            SensorHeadIn = (IDigitalInput)module.GetChannelByName("SensorHeadIn");
            InsCheck1 = (IDigitalInput)module.GetChannelByName("InsCheck1");
            InsCheck2 = (IDigitalInput)module.GetChannelByName("InsCheck2");
            InsCheck3 = (IDigitalInput)module.GetChannelByName("InsCheck3");
            InsCheck4 = (IDigitalInput)module.GetChannelByName("InsCheck4");
            InsCheck5 = (IDigitalInput)module.GetChannelByName("InsCheck5");
            InsCheck6 = (IDigitalInput)module.GetChannelByName("InsCheck6");
            InsCheck7 = (IDigitalInput)module.GetChannelByName("InsCheck7");
            InsCheck8 = (IDigitalInput)module.GetChannelByName("InsCheck8");
            InsCheck9 = (IDigitalInput)module.GetChannelByName("InsCheck9");
            InsCheck10 = (IDigitalInput)module.GetChannelByName("InsCheck10");
            InsCheck11 = (IDigitalInput)module.GetChannelByName("InsCheck11");
            InsCheck12 = (IDigitalInput)module.GetChannelByName("InsCheck12");
            InsCheck13 = (IDigitalInput)module.GetChannelByName("InsCheck13");
            InsCheck14 = (IDigitalInput)module.GetChannelByName("InsCheck14");
            InsCheck15 = (IDigitalInput)module.GetChannelByName("InsCheck15");
            InsCheck16 = (IDigitalInput)module.GetChannelByName("InsCheck16");
            IsPowerSupplyOff = (IDigitalInput)module.GetChannelByName("IsPowerSupplyOff");
        }
        /// <summary>
        /// Write all ouput and read all input channels (in this order)
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
        /// Get an instance of paricular channel identified by its name. Return null if ther is no such a channel
        /// </summary>
        /// <param name="name">Unic name (identifier) of required channel</param>
        public IChannel GetChannelByName(string name)
        {
            return module.GetChannelByName(name);
        }
        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        public bool IsConnected { get { return module.IsConnected; } }

        /// <summary>
        /// Not implemented yet
        /// </summary>
        public void SwitchOffDigitalOutputs()
        {
            throw new NotImplementedException("Method which switch all ouputs to safe state is not implemented yet!");
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return module.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Release resources allocated by underlaying module instance. Disconnect module if it is connected
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

        private void initializeChannel(IAnalogInput channel, ChannelSetting setting)
        {
            channel.RawLow = setting.RawLow;
            channel.RawHigh = setting.RawHigh;
            channel.RealLow = setting.RealLow;
            channel.RealHigh = setting.RealHigh;
        }        

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
        /// (Get) Value indicating whether mirror glass is moveing up
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

        #endregion

        #region Sucker

        /// <summary>
        /// Start to move sucker down. Does not need to be stoped.
        /// </summary>
        public void SuckerDown()
        {
            MoveSuckerUp.Value = false;
            MoveSuckerDown.Value = true;
        }
        /// <summary>
        /// Start to move sucker up. Does not need to be stoped.
        /// </summary>
        public void SuckerUp()
        {
            MoveSuckerUp.Value = true;
            MoveSuckerDown.Value = false;
        }
        /// <summary>
        /// Start to suck air from the sucker disk. If air is being blowed, it will be stoped
        /// </summary>
        public void StartSucking()
        {
            // for sure setup all values - so that never can happen that both values will be true
            SuckOn.Value = true;
            BlowOn.Value = false;
        }
        /// <summary>
        /// Start to blow air form the sucker disk. If air is being sucked, it will be stoped
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
        /// Get value indicating whether is powerfold folded
        /// </summary>
        public bool IsFolded()
        {   // if it is old mirror type, check for old powerfold sensor
            if (IsOldMirror.Value)
                return IsOldPowerfoldDown.Value;
            else
                return IsPowerfoldDown.Value;
        }
        /// <summary>
        /// Get value indicating whether is powerfold unfolded
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
        private Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        {   // Get two vectors from tree points. Cross product gives us a pependicular vector to both of them
            return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        }

        public double GetRotationAngle()
        {
            return Vector3D.AngleBetween(GetMirrorNormal(), ZeroPlaneNormal);

            //// read distance values
            //PointX.Z = DistanceX.RealValue;
            //PointY.Z = DistanceY.RealValue;
            //PointZ.Z = DistanceZ.RealValue;

            //return Vector3D.AngleBetween(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal);
        }

        /// <summary>
        /// Vector laying on the intersection of current mirror surface and zero position mirror surface
        /// This is the vector around which mirror is rotated
        /// </summary>
        public Vector3D GetRotationAxis()
        {
            return Vector3D.CrossProduct(GetMirrorNormal(), ZeroPlaneNormal);
            //return Vector3D.CrossProduct(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal);
        }

        public double GetHorizontalAngle()
        {
            return GetRotationAngle() * Math.Cos(Vector3D.AngleBetween(GetRotationAxis(), YAxis) / 180 * Math.PI);
        }
        public double GetVerticalAngle()
        {
            return -GetRotationAngle() * Math.Cos(Vector3D.AngleBetween(GetRotationAxis(), XAxis) / 180 * Math.PI);
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
            return getPlaneNormal(PointX, PointY, PointZ);
        }

        /// <summary>
        /// This method should be called when channels for measuring mirror rotation will be used.
        /// It initialize setting which come from hardware settings and should be configured by user.
        /// Their are necessary for correct measurement of mirror glass rotation.
        /// </summary>
        /// <param name="zeroPlaneNormal">Normal of plane which is considered to be a default position. 
        /// In this position mirror is centred</param>
        /// <param name="calibratorX">Position of X calibrator in 2D space. Z coordinate of thie structure 
        /// is ignored</param>
        /// <param name="calibratorY">Position of Y calibrator in 2D space. Z coordinate of thie structure
        /// is ignored</param>
        /// <param name="calibratorZ">Position of Z calibrator in 2D space. Z coordinate of thie structure
        /// is ignored</param>
        public void InitializeCalibratorsSettings(Vector3D zeroPlaneNormal, Point3D calibratorX,
            Point3D calibratorY, Point3D calibratorZ)
        {
            // Normal of plane which is considered to be a default position. In this position
            // mirror is centred
            this.ZeroPlaneNormal = zeroPlaneNormal;
            // These setting comes from application settings
            // X and Y coordinates of these points are positions of measuring sonds
            // Z cooridnates are distances of the mirror surface
            this.PointX = calibratorX;
            this.PointY = calibratorY;
            this.PointZ = calibratorZ;
        }

        #endregion

        #endregion

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

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of wrapper over IModule impelmentation. It allows access to channels
        /// through properties, not throught names as IModule does.
        /// </summary>
        /// <param name="module">Impelmentation of <typeparamref name="IModule"/> for a particular
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
