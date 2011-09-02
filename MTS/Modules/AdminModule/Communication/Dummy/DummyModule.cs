using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.AdminModule
{
    class DummyModule : IModule
    {
        public bool IsConnected { get; set; }
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();
        private TimeSpan powerfoldStarted = new TimeSpan(0);

        #region IModule Members

        public void Initialize() 
        {
            DistanceX.SetValue(100);        // channel initial value
            DistanceY.SetValue(100);
            DistanceZ.SetValue(100);

            HeatingFoilCurrent.RealHigh = 100;
            VerticalActuatorCurrent.RealHigh = 100;
            HorizontalActuatorCurrent.RealHigh = 100;
            DirectionLightCurrent.RealHigh = 100;
            PowerfoldCurrent.RealHigh = 100;
        }

        public void Connect()
        {
            IsConnected = true;
        }

        public void Update()
        {
            UpdateOutputs();
            UpdateInputs();
        }

        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        public void UpdateInputs()
        {
            if (HeatingFoilOn.Value)
                HeatingFoilCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else HeatingFoilCurrent.SetValue(0);

            if (MoveMirrorVertical.Value)
                VerticalActuatorCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else VerticalActuatorCurrent.SetValue(0);

            if (MoveMirrorHorizontal.Value)
                HorizontalActuatorCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else HorizontalActuatorCurrent.SetValue(0);

            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else DirectionLightCurrent.SetValue(0);

            if (FoldPowerfold.Value && !UnfoldPowerfold.Value)
                PowerfoldCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else if (FoldPowerfold.Value && UnfoldPowerfold.Value)
                PowerfoldCurrent.SetValue((uint)gen.Next(0, short.MaxValue / 2));
            else PowerfoldCurrent.SetValue(0);

            const int step = 2;
            if (MoveMirrorVertical.Value && !MoveMirrorReverse.Value)
            {
                if (DistanceZ.Value < 1000)
                    DistanceZ.SetValue(DistanceZ.Value + step);
            }
            else if (MoveMirrorVertical.Value && MoveMirrorReverse.Value)
            {
                if (DistanceZ.Value > 0)
                    DistanceZ.SetValue(DistanceZ.Value - step);
            }
            else if (MoveMirrorHorizontal.Value && !MoveMirrorReverse.Value)
            {
                if (DistanceX.Value < 1000)
                    DistanceX.SetValue(DistanceX.Value + step);
            }
            else if (MoveMirrorHorizontal.Value && MoveMirrorReverse.Value)
            {
                if (DistanceX.Value > 0)
                    DistanceX.SetValue(DistanceX.Value - step);
            }
            //foreach (DummyChannel chan in channels.Values)
            //{   // raise events
            //    DummyDigitalInput c = chan as DummyDigitalInput;
            //    if (c != null)
            //        c.SetValue(c.Value);
            //    DummyAnalogInput d = chan as DummyAnalogInput;
            //    if (d != null)
            //        d.SetValue(d.Value);
            //}
        }

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

        public IChannel GetChannelByName(string name)
        {
            if (channels.ContainsKey(name))
                return channels[name];
            else return null;
        }

        public void LoadConfiguration(string filename)
        {
            // heating foil
            HeatingFoilOn = new DummyDigitalOutput() { Name = "HeatingFoilOn" };
            channels.Add("HeatingFoilOn", HeatingFoilOn);
            HeatingFoilCurrent = new DummyAnalogInput() { Name = "HeatingFoilCurrent" };
            HeatingFoilCurrent.RawHigh = ushort.MaxValue;
            HeatingFoilCurrent.RealHigh = 100;
            channels.Add("HeatingFoilCurrent", HeatingFoilCurrent);

            // blinker
            DirectionLightOn = new DummyDigitalOutput() { Name = "DirectionLightOn" };
            channels.Add("DirectionLightOn", DirectionLightOn);
            DirectionLightCurrent = new DummyAnalogInput() { Name = "DirectionLightCurrent" };
            channels.Add("DirectionLightCurrent", DirectionLightCurrent);

            // powerfold
            PowerFoldUnfoldedPositionSensor1 = new DummyDigitalInput() { Name = "PowerFoldUnfoldedPositionSensor1" };
            channels.Add("PowerFoldUnfoldedPositionSensor1", PowerFoldUnfoldedPositionSensor1);
            PowerFoldUnfoldedPositionSensor2 = new DummyDigitalInput() { Name = "PowerFoldUnfoldedPositionSensor2" };
            channels.Add("PowerFoldUnfoldedPositionSensor2", PowerFoldUnfoldedPositionSensor2);
            PowerFoldFoldedPositionSensor = new DummyDigitalInput() { Name = "PowerFoldFoldedPositionSensor" };
            channels.Add("PowerFoldFoldedPositionSensor", PowerFoldFoldedPositionSensor);
            PowerfoldCurrent = new DummyAnalogInput() { Name = "PowerfoldCurrent" };
            channels.Add("PowerfoldCurrent", PowerfoldCurrent);
            FoldPowerfold = new DummyDigitalOutput() { Name = "FoldPowerfold" };
            channels.Add("FoldPowerfold", FoldPowerfold);
            UnfoldPowerfold = new DummyDigitalOutput() { Name = "UnfoldPowerfold" };
            channels.Add("UnfoldPowerfold", UnfoldPowerfold);

            IsPowerSupplyOn = new DummyDigitalInput() { Name = "IsPowerSupplyOn" };
            channels.Add("IsPowerSupplyOn", IsPowerSupplyOn);
            IsDistanceSensorUp = new DummyDigitalInput() { Name = "IsDistanceSensorUp" };
            channels.Add("IsDistanceSensorUp", IsDistanceSensorUp);
            IsDistanceSensorDown = new DummyDigitalInput() { Name = "IsDistanceSensorDown" };
            channels.Add("IsDistanceSensorDown", IsDistanceSensorDown);
            IsSuckerUp = new DummyDigitalInput() { Name = "IsSuckerUp" };
            channels.Add("IsSuckerUp", IsSuckerUp);
            IsSuckerDown = new DummyDigitalInput() { Name = "IsSuckerDown" };
            channels.Add("IsSuckerDown", IsSuckerDown);
            IsStartPressed = new DummyDigitalInput() { Name = "StartButton" };
            channels.Add("StartButton", IsStartPressed);

            // mirror movement
            VerticalActuatorCurrent = new DummyAnalogInput() { Name = "VerticalActuatorCurrent" };
            VerticalActuatorCurrent.RawHigh = ushort.MaxValue;
            VerticalActuatorCurrent.RealHigh = 100;
            channels.Add("VerticalActuatorCurrent", VerticalActuatorCurrent);
            HorizontalActuatorCurrent = new DummyAnalogInput() { Name = "HorizontalActuatorCurrent" };
            HorizontalActuatorCurrent.RawHigh = ushort.MaxValue;
            HorizontalActuatorCurrent.RealHigh = 100;
            channels.Add("HorizontalActuatorCurrent", HorizontalActuatorCurrent);

            PowerSupplyVoltage1 = new DummyAnalogInput() { Name = "PowerSupplyVoltage1" };
            channels.Add("PowerSupplyVoltage1", PowerSupplyVoltage1);
            PowerSupplyVoltage2 = new DummyAnalogInput() { Name = "PowerSupplyVoltage2" };
            channels.Add("PowerSupplyVoltage2", PowerSupplyVoltage2);
            IsStartPressed = new DummyDigitalInput() { Name = "IsStartPressed" };
            channels.Add("IsStartPressed", IsStartPressed);

            DistanceX = new DummyAnalogInput() { Name = "DistanceX" };
            channels.Add("DistanceX", DistanceX);
            DistanceY = new DummyAnalogInput() { Name = "DistanceY" };
            channels.Add("DistanceY", DistanceY);
            DistanceZ = new DummyAnalogInput() { Name = "DistanceZ" };
            channels.Add("DistanceZ", DistanceZ);
            MoveMirrorVertical = new DummyDigitalOutput() { Name = "MoveMirrorVertical" };
            channels.Add("MoveMirrorVertical", MoveMirrorVertical);
            MoveMirrorHorizontal = new DummyDigitalOutput() { Name = "MoveMirrorHorizontal" };
            channels.Add("MoveMirrorLeft", MoveMirrorHorizontal);
            MoveMirrorReverse = new DummyDigitalOutput() { Name = "MoveMirrorReverse" };
            channels.Add("MoveMirrorReverse", MoveMirrorReverse);

            AllowMirrorMovement = new DummyDigitalOutput() { Name = "AllowMirrorMovement" };
            channels.Add("AllowMirrorMovement", AllowMirrorMovement);
            LockWeak = new DummyDigitalOutput() { Name = "LockWeak" };
            channels.Add("LockWeak", LockWeak);
            UnlockWeak = new DummyDigitalOutput() { Name = "UnlockWeak" };
            channels.Add("UnlockWeak", UnlockWeak);
            MoveDistanceSensorUp = new DummyDigitalOutput() { Name = "MoveDistanceSensorUp" };
            channels.Add("MoveDistanceSensorUp", MoveDistanceSensorUp);
            MoveDistanceSensorDown = new DummyDigitalOutput() { Name = "MoveDistanceSensorDown" };
            channels.Add("MoveDistanceSensorDown", MoveDistanceSensorDown);
            MoveSuckerUp = new DummyDigitalOutput() { Name = "MoveSuckerUp" };
            channels.Add("MoveSuckerUp", MoveSuckerUp);
            MoveSuckerDown = new DummyDigitalOutput() { Name = "MoveSuckerDown" };
            channels.Add("MoveSuckerDown", MoveSuckerDown);
            SuckOn = new DummyDigitalOutput() { Name = "SuckOn" };
            channels.Add("SuckOn", SuckOn);

            AllowPowerSupply = new DummyDigitalOutput() { Name = "AllowPowerSupply" };
            channels.Add("AllowPowerSupply", AllowPowerSupply);
            LockStrong = new DummyDigitalOutput() { Name = "LockStrong" };
            channels.Add("LockStrong", LockStrong);
            UnlockStrong = new DummyDigitalOutput() { Name = "UnlockStrong" };
            channels.Add("UnlockStrong", UnlockStrong);
            GreenLightOn = new DummyDigitalOutput() { Name = "GreenLightOn" };
            channels.Add("GreenLightOn", GreenLightOn);
            RedLightOn = new DummyDigitalOutput() { Name = "RedLightOn" };
            channels.Add("RedLightOn", RedLightOn);

            BuzzerOn = new DummyDigitalOutput() { Name = "BuzzerOn" };
            channels.Add("BuzzerOn", BuzzerOn);
        }

        public void SwitchOffDigitalOutputs() { }

        #endregion

        Random gen = new Random();

        #region Channels

        #region Digital inputs

        public IDigitalInput IsDistanceSensorUp { get; set; }
        public IDigitalInput IsDistanceSensorDown { get; set; }
        public IDigitalInput IsSuckerUp { get; set; }
        public IDigitalInput IsSuckerDown { get; set; }

        public IDigitalInput IsStartPressed { get; set; }

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
        public IDigitalInput IsPowerSupplyOn { get; set; }


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

        public IDigitalOutput AllowPowerSupply { get; set; }
        public IDigitalOutput LockStrong { get; set; }
        public IDigitalOutput UnlockStrong { get; set; }
        public IDigitalOutput GreenLightOn { get; set; }
        public IDigitalOutput RedLightOn { get; set; }
        public IDigitalOutput BuzzerOn { get; set; }


        public IDigitalOutput OpenTestingDevice { get; set; }
        public IDigitalOutput CloseTestingDevice { get; set; }

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
        public IAnalogInput PowerfoldCurrent { get; set; }
        public IAnalogInput DirectionLightCurrent { get; set; }

        public IAnalogInput PowerSupplyVoltage1 { get; set; }
        public IAnalogInput PowerSupplyVoltage2 { get; set; }

        #endregion

        #endregion
    }
}
