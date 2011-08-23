using System;

namespace MTS.AdminModule {

    public class Channels : IModule
    {
        private IModule module;

        #region IModule Members

        public void LoadConfiguration(string filename)
        {
            module.LoadConfiguration(filename);
        }
        public void Connect()
        {
            module.Connect();
        }
        public void Initialize()
        {
            module.Initialize();

            // initialize channels: 
            // !!! PROPERTY NAME IS ALWAYS THE SAME AS CHANNEL NAME STRING !!!
            // Consider this when setting variable names in TwinCAT IO Server

            // ???
            TestingDeviceOpened = (IDigitalInput)module.GetChannelByName("TestingDeviceOpened");
            TestingDeviceClosed = (IDigitalInput)module.GetChannelByName("TestingDeviceClosed");
            StartButton = (IDigitalInput)module.GetChannelByName("StartButton");
            ErrorAcknButton = (IDigitalInput)module.GetChannelByName("ErrorAcknButton");
            ControlCurrentOn = (IDigitalInput)module.GetChannelByName("ControlCurrentOn");

            // powerfold
            PowerFoldUnfoldedPositionSensor1 = (IDigitalInput)module.GetChannelByName("PowerFoldUnfoldedPositionSensor1");
            PowerFoldUnfoldedPositionSensor2 = (IDigitalInput)module.GetChannelByName("PowerFoldUnfoldedPositionSensor2");
            PowerFoldFoldedPositionSensor = (IDigitalInput)module.GetChannelByName("PowerFoldFoldedPositionSensor");
            PowerFoldCurrent = (IAnalogInput)module.GetChannelByName("PowerFoldCurrent");
            Fold = (IDigitalOutput)module.GetChannelByName("Fold");
            Unfold = (IDigitalOutput)module.GetChannelByName("Unfold");

            // spiral
            HeatingFoilOn = (IDigitalOutput)module.GetChannelByName("HeatingFoilOn");
            HeatingFoilCurrent = (IAnalogInput)module.GetChannelByName("HeatingFoilCurrent");
            HeatingFoilCurrent.RealLow = 0;
            HeatingFoilCurrent.RealHigh = 10;
            HeatingFoilSignSensor = (IDigitalInput)module.GetChannelByName("HeatingFoilSignSensor");

            // mirror movement
            VerticalActuatorCurrent = (IAnalogInput)module.GetChannelByName("VerticalActuatorCurrent");
            HorizontalActuatorCurrent = (IAnalogInput)module.GetChannelByName("HorizontalActuatorCurrent");
            ActuatorPowerSupplyVoltage = (IAnalogInput)module.GetChannelByName("ActuatorPowerSupplyVoltage");
            OtherPowerSupplyVoltage = (IAnalogInput)module.GetChannelByName("OtherPowerSupplyVoltage");
            DistanceX = (IAnalogInput)module.GetChannelByName("DistanceX");
            DistanceY = (IAnalogInput)module.GetChannelByName("DistanceY");
            DistanceZ = (IAnalogInput)module.GetChannelByName("DistanceZ");
            MoveMirrorUp = (IDigitalOutput)module.GetChannelByName("MoveMirrorUp");
            MoveMirrorLeft = (IDigitalOutput)module.GetChannelByName("MoveMirrorLeft");
            MoveReverse = (IDigitalOutput)module.GetChannelByName("MoveReverse");

            // blinker
            DirectionLightCurrent = (IAnalogInput)module.GetChannelByName("DirectionLightCurrent");
            DirectionLightOn = (IDigitalOutput)module.GetChannelByName("DirectionLightOn");


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
        }
        public void Update()
        {   // update all channels: read in/out and write out channels
            module.Update();
        }
        public void Update(TimeSpan time)
        {   // for debug prpose only
            module.Update(time);
        }
        public void UpdateInputs()
        {
            module.UpdateInputs();
        }
        public void UpdateOutputs()
        {
            module.UpdateOutputs();
        }
        public void Disconnect()
        {
            module.Disconnect();
        }

        public IChannel GetChannelByName(string name)
        {
            return module.GetChannelByName(name);
        }

        #endregion

        #region Channels

        #region Digital inputs

        public IDigitalInput HeatingFoilSignSensor { get; set; }
        public IDigitalInput PowerFoldUnfoldedPositionSensor1 { get; set; }
        public IDigitalInput PowerFoldUnfoldedPositionSensor2 { get; set; }
        public IDigitalInput PowerFoldFoldedPositionSensor { get; set; }
        public IDigitalInput TestingDeviceOpened { get; set; }
        public IDigitalInput TestingDeviceClosed { get; set; }
        public IDigitalInput StartButton { get; set; }
        public IDigitalInput ErrorAcknButton { get; set; }

        public IDigitalInput ControlCurrentOn { get; set; }

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

        #endregion

        #region Digital outputs

        public IDigitalOutput AllowMirrorMovement { get; set; }
        public IDigitalOutput CONTROL_CURRENT_ALLOW { get; set; } // ??
        public IDigitalOutput MoveMirrorUp { get; set; }
        public IDigitalOutput MoveMirrorLeft { get; set; }
        public IDigitalOutput MoveReverse { get; set; }

        public IDigitalOutput Fold { get; set; }
        public IDigitalOutput Unfold { get; set; }
        public IDigitalOutput OpenTestingDevice { get; set; }
        public IDigitalOutput CloseTestingDevice { get; set; }

        public IDigitalOutput HeatingFoilOn { get; set; }
        public IDigitalOutput DirectionLightOn { get; set; }

        IDigitalOutput SG_GREEN { get; set; }
        IDigitalOutput SG_RED { get; set; }
        IDigitalOutput SG_AUDIO { get; set; }

        IDigitalOutput MARKER_LEFT { get; set; }
        IDigitalOutput MARKER_RIGHT { get; set; }
        IDigitalOutput MOVE_SENSOR_UP { get; set; }
        IDigitalOutput MOVE_SENSOR_DOWN { get; set; }
        IDigitalOutput MOVE_SUPPORT_IN { get; set; }
        IDigitalOutput MOVE_SUPPORT_UP { get; set; }
        IDigitalOutput MOVE_SUPPORT_DOWN { get; set; }

        #endregion

        #region Analog inputs

        public IAnalogInput DistanceX { get; set; }
        public IAnalogInput DistanceY { get; set; }
        public IAnalogInput DistanceZ { get; set; }

        public IAnalogInput VerticalActuatorCurrent { get; set; }
        public IAnalogInput HorizontalActuatorCurrent { get; set; }
        public IAnalogInput HeatingFoilCurrent { get; set; }
        public IAnalogInput PowerFoldCurrent { get; set; }
        public IAnalogInput DirectionLightCurrent { get; set; }
        public IAnalogInput ActuatorPowerSupplyVoltage { get; set; }
        public IAnalogInput OtherPowerSupplyVoltage { get; set; }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of wrapper over IModule impelmentation. It allows access to channels
        /// through properties, not throught names as IModule does.
        /// </summary>
        /// <param name="module">Impelmentation of <typeparamref name="IModule"/> for a particular
        /// protocol, such as EtherCAT or Modbus TCP/IP (...)</param>
        public Channels(IModule module)
        {
            this.module = module;
            
        }

        #endregion
    }
}
