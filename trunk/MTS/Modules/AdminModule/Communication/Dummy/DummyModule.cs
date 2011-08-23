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

        public void Initialize() { }

        public void Connect()
        {
            IsConnected = true;
        }

        public void Update(TimeSpan time)
        {
            if (HeatingFoilOn.Value)
                HeatingFoilCurrent.SetValue((uint)gen.Next(0, 20000));
            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue((uint)gen.Next(0, 100));

            double duration = (time - powerfoldStarted).TotalMilliseconds;

            if (Fold.Value)
            {
                if (powerfoldStarted.Ticks == 0)
                    powerfoldStarted = time;

                if (duration > 100 && duration < 300)
                    PowerFoldCurrent.SetValue(200);
                else if (duration > 500 && duration < 600)
                    PowerFoldCurrent.SetValue(150);
                else
                    PowerFoldCurrent.SetValue((uint)gen.Next(-50, 50));
                if (duration > 1000)
                    PowerFoldFoldedPositionSensor.SetValue(true);
            }
            else if (!Fold.Value && Unfold.Value)
            {
                PowerFoldCurrent.SetValue((uint)gen.Next(-50, 50));
                if (duration > 2000)
                {
                    PowerFoldUnfoldedPositionSensor1.SetValue(true);
                    PowerFoldUnfoldedPositionSensor2.SetValue(true);
                }
            }
        }

        public void Update()
        {
            if (HeatingFoilOn.Value)
                HeatingFoilCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else HeatingFoilCurrent.SetValue(0);

            if (MoveMirrorUp.Value)
                VerticalActuatorCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else VerticalActuatorCurrent.SetValue(0);

            if (MoveMirrorLeft.Value)
                HorizontalActuatorCurrent.SetValue((uint)gen.Next(0, short.MaxValue));
            else HorizontalActuatorCurrent.SetValue(0);

            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue((uint)gen.Next(0, 100));
            DirectionLightCurrent.SetValue(0);

            const int step = 2;
            if (MoveMirrorUp.Value && !MoveReverse.Value)
            {
                if (DistanceZ.Value < 1000)
                    DistanceZ.SetValue(DistanceZ.Value + step);
            }
            else if (MoveMirrorUp.Value && MoveReverse.Value)
            {
                if (DistanceZ.Value > 0)
                    DistanceZ.SetValue(DistanceZ.Value - step);
            }
            else if (MoveMirrorLeft.Value && !MoveReverse.Value)
            {
                if (DistanceX.Value < 1000)
                    DistanceX.SetValue(DistanceX.Value + step);
            }
            else if (MoveMirrorLeft.Value && MoveReverse.Value)
            {
                if (DistanceX.Value > 0)
                    DistanceX.SetValue(DistanceX.Value - step);
            }

        }

        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        public void UpdateInputs()
        {
            //Update(DateTime.Now.TimeOfDay);
        }

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs() { Update(); }

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
            HeatingFoilCurrent.SetValue(0);
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
            PowerFoldCurrent = new DummyAnalogInput() { Name = "PowerFoldCurrent" };
            channels.Add("PowerFoldCurrent", PowerFoldCurrent);
            Fold = new DummyDigitalOutput() { Name = "Fold" };
            channels.Add("Fold", Fold);
            Unfold = new DummyDigitalOutput() { Name = "Unfold" };
            channels.Add("Unfold", Unfold);

            // mirror movement
            VerticalActuatorCurrent = new DummyAnalogInput() { Name = "VerticalActuatorCurrent" };
            VerticalActuatorCurrent.RawHigh = ushort.MaxValue;
            VerticalActuatorCurrent.RealHigh = 100;
            channels.Add("VerticalActuatorCurrent", VerticalActuatorCurrent);
            HorizontalActuatorCurrent = new DummyAnalogInput() { Name = "HorizontalActuatorCurrent" };
            HorizontalActuatorCurrent.RawHigh = ushort.MaxValue;
            HorizontalActuatorCurrent.RealHigh = 100;
            channels.Add("HorizontalActuatorCurrent", HorizontalActuatorCurrent);

            ActuatorPowerSupplyVoltage = new DummyAnalogInput() { Name = "ActuatorPowerSupplyVoltage" };
            OtherPowerSupplyVoltage = new DummyAnalogInput() { Name = "OtherPowerSupplyVoltage" };

            DistanceX = new DummyAnalogInput() { Name = "DistanceX" };
            channels.Add("DistanceX", DistanceX);
            DistanceY = new DummyAnalogInput() { Name = "DistanceY" };
            channels.Add("DistanceY", DistanceY);
            DistanceZ = new DummyAnalogInput() { Name = "DistanceZ" };
            channels.Add("DistanceZ", DistanceZ);
            MoveMirrorUp = new DummyDigitalOutput() { Name = "MoveMirrorUp" };
            channels.Add("MoveMirrorUp", MoveMirrorUp);
            MoveMirrorLeft = new DummyDigitalOutput() { Name = "MoveMirrorLeft" };
            channels.Add("MoveMirrorLeft", MoveMirrorLeft);
            MoveReverse = new DummyDigitalOutput() { Name = "MoveReverse" };
            channels.Add("MoveReverse", MoveReverse);
        }

        #endregion

        Random gen = new Random();

        // heating foil
        DummyDigitalOutput HeatingFoilOn;
        DummyAnalogInput HeatingFoilCurrent;

        // blinker
        DummyDigitalOutput DirectionLightOn;
        DummyAnalogInput DirectionLightCurrent;

        // powerfold
        DummyDigitalInput PowerFoldUnfoldedPositionSensor1;
        DummyDigitalInput PowerFoldUnfoldedPositionSensor2;
        DummyDigitalInput PowerFoldFoldedPositionSensor;
        DummyAnalogInput PowerFoldCurrent;
        DummyDigitalOutput Fold;
        DummyDigitalOutput Unfold;


        DummyAnalogInput VerticalActuatorCurrent;
        DummyAnalogInput HorizontalActuatorCurrent;
        DummyAnalogInput ActuatorPowerSupplyVoltage;
        DummyAnalogInput OtherPowerSupplyVoltage;

        DummyAnalogInput DistanceX;
        DummyAnalogInput DistanceY;
        DummyAnalogInput DistanceZ;

        DummyDigitalOutput MoveMirrorUp;
        DummyDigitalOutput MoveMirrorLeft;
        DummyDigitalOutput MoveReverse;
    }
}
