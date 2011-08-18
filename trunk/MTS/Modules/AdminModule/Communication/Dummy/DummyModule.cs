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

        public void Connect()
        {
            IsConnected = true;
        }

        public void Update(TimeSpan time)
        {
            if (HeatingFoilOn.Value)
                HeatingFoilCurrent.SetValue(gen.Next(10, 100));
            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue(gen.Next(0, 100));

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
                    PowerFoldCurrent.SetValue(gen.Next(-50, 50));
                if (duration > 1000)
                    PowerFoldFoldedPositionSensor.SetValue(true);
            }
            else if (!Fold.Value && Unfold.Value)
            {
                PowerFoldCurrent.SetValue(gen.Next(-50, 50));
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
                HeatingFoilCurrent.SetValue(gen.Next(10, 100));
            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue(gen.Next(0, 100));
            
        }

        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        public void UpdateInputs() { }

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs() { }

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
            HorizontalActuatorCurrent = new DummyAnalogInput() { Name = "HorizontalActuatorCurrent" };
            ActuatorPowerSupplyVoltage = new DummyAnalogInput() { Name = "ActuatorPowerSupplyVoltage" };
            OtherPowerSupplyVoltage = new DummyAnalogInput() { Name = "OtherPowerSupplyVoltage" };
            DistanceX = new DummyAnalogInput() { Name = "DistanceX" };
            DistanceY = new DummyAnalogInput() { Name = "DistanceY" };
            DistanceZ = new DummyAnalogInput() { Name = "DistanceZ" };
            MoveMirrorUp = new DummyDigitalOutput() { Name = "MoveMirrorUp" };
            MoveMirrorLeft = new DummyDigitalOutput() { Name = "MoveMirrorLeft" };
            MoveReverse = new DummyDigitalOutput() { Name = "MoveReverse" };
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
