using System;
using System.Collections.Generic;
using System.IO;
using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    public sealed class DummyModule : IModule
    {
        /// <summary>
        /// True if module is connected
        /// </summary>
        public bool IsConnected { get; set; }
        /// <summary>
        /// Collection of channels identified by (string) name
        /// </summary>
        private Dictionary<string, IChannel> channels = new Dictionary<string, IChannel>();
        private System.Timers.Timer timer = new System.Timers.Timer();

        #region IModule Members

        public void Initialize()
        {
            // analog inputs
            DistanceX = (IAnalogInput)GetChannelByName("DistanceX");
            DistanceX.SetValue(1200);
            DistanceY = (IAnalogInput)GetChannelByName("DistanceY");
            DistanceY.SetValue(1200);
            DistanceZ = (IAnalogInput)GetChannelByName("DistanceZ");
            DistanceZ.SetValue(1200);

            PowerfoldCurrent = (IAnalogInput)GetChannelByName("PowerfoldCurrent");
            HeatingFoilCurrent = (IAnalogInput)GetChannelByName("HeatingFoilCurrent");
            VerticalActuatorCurrent = (IAnalogInput)GetChannelByName("VerticalActuatorCurrent");
            HorizontalActuatorCurrent = (IAnalogInput)GetChannelByName("HorizontalActuatorCurrent");
            DirectionLightCurrent = (IAnalogInput)GetChannelByName("DirectionLightCurrent");

            PowerSupplyVoltage1 = (IAnalogInput)GetChannelByName("PowerSupplyVoltage1");
            PowerSupplyVoltage2 = (IAnalogInput)GetChannelByName("PowerSupplyVoltage2");
            // analog inputs

            // digital inputs
            IsDistanceSensorUp = (IDigitalInput)GetChannelByName("IsDistanceSensorUp");
            IsDistanceSensorDown = (IDigitalInput)GetChannelByName("IsDistanceSensorDown");
            IsSuckerUp = (IDigitalInput)GetChannelByName("IsSuckerUp");
            IsSuckerDown = (IDigitalInput)GetChannelByName("IsSuckerDown");
            IsVacuum = (IDigitalInput)GetChannelByName("IsVacuum");
            IsLeftRubberPresent = (IDigitalInput)GetChannelByName("IsLeftRubberPresent");

            IsPowerfoldDown = (IDigitalInput)GetChannelByName("IsPowerfoldDown");
            IsPowerfoldUp = (IDigitalInput)GetChannelByName("IsPowerfoldUp");
            IsOldPowerfoldUp = (IDigitalInput)GetChannelByName("IsOldPowerfoldUp");
            IsRightRubberPresent = (IDigitalInput)GetChannelByName("IsRightRubberPresent");

            IsLeftMirror = (IDigitalInput)GetChannelByName("IsLeftMirror");
            IsOldMirror = (IDigitalInput)GetChannelByName("IsOldMirror");
            IsOldPowerfoldDown = (IDigitalInput)GetChannelByName("IsOldPowerfoldDown");
            IsStartPressed = (IDigitalInput)GetChannelByName("IsStartPressed");
            IsAckPressed = (IDigitalInput)GetChannelByName("IsAckPressed");

            IsLocked = (IDigitalInput)GetChannelByName("IsLocked");
            IsOldLocked = (IDigitalInput)GetChannelByName("IsOldLocked");
            // digital inputs

            // digital outputs
            AllowMirrorMovement = (IDigitalOutput)GetChannelByName("AllowMirrorMovement");
            MoveMirrorVertical = (IDigitalOutput)GetChannelByName("MoveMirrorVertical");
            MoveMirrorHorizontal = (IDigitalOutput)GetChannelByName("MoveMirrorHorizontal");
            MoveMirrorReverse = (IDigitalOutput)GetChannelByName("MoveMirrorReverse");
            FoldPowerfold = (IDigitalOutput)GetChannelByName("FoldPowerfold");
            UnfoldPowerfold = (IDigitalOutput)GetChannelByName("UnfoldPowerfold");
            HeatingFoilOn = (IDigitalOutput)GetChannelByName("HeatingFoilOn");
            DirectionLightOn = (IDigitalOutput)GetChannelByName("DirectionLightOn");

            LockWeak = (IDigitalOutput)GetChannelByName("LockWeak");
            UnlockWeak = (IDigitalOutput)GetChannelByName("UnlockWeak");
            MoveDistanceSensorUp = (IDigitalOutput)GetChannelByName("MoveDistanceSensorUp");
            MoveDistanceSensorDown = (IDigitalOutput)GetChannelByName("MoveDistanceSensorDown");
            MoveSuckerUp = (IDigitalOutput)GetChannelByName("MoveSuckerUp");
            MoveSuckerDown = (IDigitalOutput)GetChannelByName("MoveSuckerDown");
            SuckOn = (IDigitalOutput)GetChannelByName("SuckOn");
            BlowOn = (IDigitalOutput)GetChannelByName("BlowOn");

            AllowPowerSupply = (IDigitalOutput)GetChannelByName("AllowPowerSupply");
            LockStrong = (IDigitalOutput)GetChannelByName("LockStrong");
            UnlockStrong = (IDigitalOutput)GetChannelByName("UnlockStrong");
            GreenLightOn = (IDigitalOutput)GetChannelByName("GreenLightOn");
            RedLightOn = (IDigitalOutput)GetChannelByName("RedLightOn");
            BuzzerOn = (IDigitalOutput)GetChannelByName("BuzzerOn");

            // digital outputs

            // ???
            TestingDeviceOpened = (IDigitalInput)GetChannelByName("TestingDeviceOpened");
            TestingDeviceClosed = (IDigitalInput)GetChannelByName("TestingDeviceClosed");
            ErrorAcknButton = (IDigitalInput)GetChannelByName("ErrorAcknButton");


            // powerfold
            PowerFoldUnfoldedPositionSensor1 = (IDigitalInput)GetChannelByName("PowerFoldUnfoldedPositionSensor1");
            PowerFoldUnfoldedPositionSensor2 = (IDigitalInput)GetChannelByName("PowerFoldUnfoldedPositionSensor2");
            PowerFoldFoldedPositionSensor = (IDigitalInput)GetChannelByName("PowerFoldFoldedPositionSensor");


            HeatingFoilSignSensor = (IDigitalInput)GetChannelByName("HeatingFoilSignSensor");

            SensorHeadOut = (IDigitalInput)GetChannelByName("SensorHeadOut");
            SensorHeadIn = (IDigitalInput)GetChannelByName("SensorHeadIn");
            InsCheck1 = (IDigitalInput)GetChannelByName("InsCheck1");
            InsCheck2 = (IDigitalInput)GetChannelByName("InsCheck2");
            InsCheck3 = (IDigitalInput)GetChannelByName("InsCheck3");
            InsCheck4 = (IDigitalInput)GetChannelByName("InsCheck4");
            InsCheck5 = (IDigitalInput)GetChannelByName("InsCheck5");
            InsCheck6 = (IDigitalInput)GetChannelByName("InsCheck6");
            InsCheck7 = (IDigitalInput)GetChannelByName("InsCheck7");
            InsCheck8 = (IDigitalInput)GetChannelByName("InsCheck8");
            InsCheck9 = (IDigitalInput)GetChannelByName("InsCheck9");
            InsCheck10 = (IDigitalInput)GetChannelByName("InsCheck10");
            InsCheck11 = (IDigitalInput)GetChannelByName("InsCheck11");
            InsCheck12 = (IDigitalInput)GetChannelByName("InsCheck12");
            InsCheck13 = (IDigitalInput)GetChannelByName("InsCheck13");
            InsCheck14 = (IDigitalInput)GetChannelByName("InsCheck14");
            InsCheck15 = (IDigitalInput)GetChannelByName("InsCheck15");
            InsCheck16 = (IDigitalInput)GetChannelByName("InsCheck16");
            IsPowerSupplyOff = (IDigitalInput)GetChannelByName("IsPowerSupplyOff");
        }

        public void Connect()
        {
            IsConnected = true;
        }

        /// <summary>
        /// Update outputs and inputs
        /// </summary>
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
            // lock/unlock
            if (LockWeak.Value)
            {
                IsLocked.SetValue(true);
                IsOldLocked.SetValue(true);
            }
            if (UnlockStrong.Value)
            {
                IsLocked.SetValue(false);
                IsOldLocked.SetValue(false);
            }

            // moveing distance sensors
            if (MoveDistanceSensorUp.Value && !MoveDistanceSensorDown.Value)
            {
                IsDistanceSensorUp.SetValue(true);
                IsDistanceSensorDown.SetValue(false);
            }
            if (MoveDistanceSensorDown.Value && !MoveDistanceSensorUp.Value)
            {
                IsDistanceSensorUp.SetValue(false);
                IsDistanceSensorDown.SetValue(true);
            }
            // movement
            if (IsDistanceSensorUp.Value)
            {
                const int step = 1;
                // down
                if (MoveMirrorVertical.Value && !MoveMirrorReverse.Value)
                {
                    if (DistanceX.Value > DistanceX.RawLow)
                        DistanceX.SetValue(DistanceX.Value - step);
                    
                }// up
                else if (MoveMirrorHorizontal.Value && MoveMirrorReverse.Value)
                {
                    if (DistanceX.Value < DistanceX.RawHigh)
                        DistanceX.SetValue(DistanceX.Value + step);
                }//right
                else if (MoveMirrorHorizontal.Value && !MoveMirrorReverse.Value)
                {
                    if (DistanceY.Value < DistanceY.RawHigh)
                        DistanceY.SetValue(DistanceY.Value + step);
                }//left
                else if (MoveMirrorVertical.Value && MoveMirrorReverse.Value)
                {
                    if (DistanceY.Value > DistanceY.RawLow)
                        DistanceY.SetValue(DistanceY.Value - step);
                }
            }

            // pull-off
            if (MoveSuckerUp.Value && !MoveSuckerDown.Value)
            {
                IsSuckerUp.SetValue(true);
                IsSuckerDown.SetValue(false);
            }
            else if (!MoveSuckerUp.Value && MoveSuckerDown.Value)
            {
                IsSuckerUp.SetValue(false);
                IsSuckerDown.SetValue(true);
            }
            if (IsSuckerUp.Value)
            {
                if (SuckOn.Value)
                    IsVacuum.SetValue(true);
                if (BlowOn.Value)
                    IsVacuum.SetValue(false);                
            }

            // heating is on
            if (HeatingFoilOn.Value)
                HeatingFoilCurrent.SetValue((uint)gen.Next(4095));
            else HeatingFoilCurrent.SetValue(0);
            // vertical acuator is on
            if (MoveMirrorVertical.Value)
                VerticalActuatorCurrent.SetValue((uint)gen.Next(4095));
            else VerticalActuatorCurrent.SetValue(0);
            // horizontal acutarot is on
            if (MoveMirrorHorizontal.Value)
                HorizontalActuatorCurrent.SetValue((uint)gen.Next(4095));
            else HorizontalActuatorCurrent.SetValue(0);
            // blinker is on
            if (DirectionLightOn.Value)
                DirectionLightCurrent.SetValue((uint)gen.Next(4095));
            else DirectionLightCurrent.SetValue(0);

            if (FoldPowerfold.Value && !UnfoldPowerfold.Value)
                PowerfoldCurrent.SetValue((uint)gen.Next(4095));
            else if (FoldPowerfold.Value && UnfoldPowerfold.Value)
                PowerfoldCurrent.SetValue((uint)gen.Next(2048));
            else PowerfoldCurrent.SetValue(0);

            // power supply
            if (!IsPowerSupplyOff.Value)
            {
                PowerSupplyVoltage1.SetValue((uint)gen.Next(4095));
                PowerSupplyVoltage2.SetValue((uint)gen.Next(4095));
            }
        }

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
            // nothig to write bacause there is no remote hardware
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

        /// <summary>
        /// Return an instance of channel identified by name or null if there is no such a channel
        /// </summary>
        /// <param name="name">Channel identifier</param>
        public IChannel GetChannelByName(string name)
        {
            for (int i = 0; i < inputs.Count; i++)
                if (inputs[i].Name == name)     // look for channel with paricular name
                    return inputs[i];
            return null;        // channel with name "name" was not found
        }

        private List<ChannelBase> inputs = new List<ChannelBase>();
        private List<ChannelBase> outputs = new List<ChannelBase>();

        private readonly char[] whiteSpaces = { ' ', '\t', '\r' };
        private readonly char[] csvSep = { ';' };
        private const int itemsPerLine = 5; // number of items per one line

        private const string inputString = "Input";
        private const string outputString = "Output";

        public void LoadConfiguration(string filename)
        {
            string str;         // temporary value for string while parsing
            int value;          // temporary value fot integer while parsing
            string[] items;     // parsed items on one line 

            // reference to just created channel
            ChannelBase channel;

            // open configuration file
            StreamReader reader;
            reader = new StreamReader(filename);

            // skip first line (.csv file format) and count number of items on first line
            //itemsPerLine = 
            reader.ReadLine().Split(csvSep, StringSplitOptions.RemoveEmptyEntries);
            // [Channel Name];[Slot Number];[Channel Number];[I/O type];[I/O Data Length(bits)];[Comment]

            while (!reader.EndOfStream)
            {
                // parse line with CSV separator
                items = reader.ReadLine().Split(csvSep, StringSplitOptions.RemoveEmptyEntries);
                // not enought items per line - skip it
                if (items.Length < itemsPerLine) continue;

                // parsing length of channel value (in bits and hexadecimal format)
                str = items[4].Substring(items[4].IndexOf('x') + 1);    // remove leading 0x
                if (!int.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out value))
                    continue;   // parsing data length failed - skip this line

                // create an instance of channel - depending on the channel type
                if (value == 1)   // digital channel is always of size 1 (bit)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new DigitalInput();
                    else if (items[3] == outputString)
                    {
                        channel = new DigitalOutput();
                        outputs.Add(channel);
                    }
                    else continue;// I/O type of channel is wrong - skip this line
                else              // analog channel (usually 16 length)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new AnalogInput();
                    else if (items[3] == outputString)
                    {
                        channel = new AnalogOutput();
                        outputs.Add(channel);
                    }
                    else continue;// I/O type of channel is wrong - skip this line

                // "parsing" channel name
                channel.Name = items[0];

                // add all channels to this collection
                inputs.Add(channel);
            }
            // close file - release resources
            reader.Close();
        }

        //public void LoadConfiguration(string filename)
        //{
        //    // heating foil
        //    HeatingFoilOn = new DummyDigitalOutput() { Name = "HeatingFoilOn" };
        //    channels.Add("HeatingFoilOn", HeatingFoilOn);
        //    HeatingFoilCurrent = new DummyAnalogInput() { Name = "HeatingFoilCurrent" };
        //    HeatingFoilCurrent.RawHigh = ushort.MaxValue;
        //    HeatingFoilCurrent.RealHigh = 100;
        //    channels.Add("HeatingFoilCurrent", HeatingFoilCurrent);

        //    // blinker
        //    DirectionLightOn = new DummyDigitalOutput() { Name = "DirectionLightOn" };
        //    channels.Add("DirectionLightOn", DirectionLightOn);
        //    DirectionLightCurrent = new DummyAnalogInput() { Name = "DirectionLightCurrent" };
        //    channels.Add("DirectionLightCurrent", DirectionLightCurrent);

        //    // powerfold
        //    PowerFoldUnfoldedPositionSensor1 = new DummyDigitalInput() { Name = "PowerFoldUnfoldedPositionSensor1" };
        //    channels.Add("PowerFoldUnfoldedPositionSensor1", PowerFoldUnfoldedPositionSensor1);
        //    PowerFoldUnfoldedPositionSensor2 = new DummyDigitalInput() { Name = "PowerFoldUnfoldedPositionSensor2" };
        //    channels.Add("PowerFoldUnfoldedPositionSensor2", PowerFoldUnfoldedPositionSensor2);
        //    PowerFoldFoldedPositionSensor = new DummyDigitalInput() { Name = "PowerFoldFoldedPositionSensor" };
        //    channels.Add("PowerFoldFoldedPositionSensor", PowerFoldFoldedPositionSensor);
        //    PowerfoldCurrent = new DummyAnalogInput() { Name = "PowerfoldCurrent" };
        //    channels.Add("PowerfoldCurrent", PowerfoldCurrent);
        //    FoldPowerfold = new DummyDigitalOutput() { Name = "FoldPowerfold" };
        //    channels.Add("FoldPowerfold", FoldPowerfold);
        //    UnfoldPowerfold = new DummyDigitalOutput() { Name = "UnfoldPowerfold" };
        //    channels.Add("UnfoldPowerfold", UnfoldPowerfold);

        //    IsPowerSupplyOn = new DummyDigitalInput() { Name = "IsPowerSupplyOn" };
        //    channels.Add("IsPowerSupplyOn", IsPowerSupplyOn);
        //    IsDistanceSensorUp = new DummyDigitalInput() { Name = "IsDistanceSensorUp" };
        //    channels.Add("IsDistanceSensorUp", IsDistanceSensorUp);
        //    IsDistanceSensorDown = new DummyDigitalInput() { Name = "IsDistanceSensorDown" };
        //    channels.Add("IsDistanceSensorDown", IsDistanceSensorDown);
        //    IsSuckerUp = new DummyDigitalInput() { Name = "IsSuckerUp" };
        //    channels.Add("IsSuckerUp", IsSuckerUp);
        //    IsSuckerDown = new DummyDigitalInput() { Name = "IsSuckerDown" };
        //    channels.Add("IsSuckerDown", IsSuckerDown);
        //    IsStartPressed = new DummyDigitalInput() { Name = "StartButton" };
        //    channels.Add("StartButton", IsStartPressed);

        //    // mirror movement
        //    VerticalActuatorCurrent = new DummyAnalogInput() { Name = "VerticalActuatorCurrent" };
        //    VerticalActuatorCurrent.RawHigh = ushort.MaxValue;
        //    VerticalActuatorCurrent.RealHigh = 100;
        //    channels.Add("VerticalActuatorCurrent", VerticalActuatorCurrent);
        //    HorizontalActuatorCurrent = new DummyAnalogInput() { Name = "HorizontalActuatorCurrent" };
        //    HorizontalActuatorCurrent.RawHigh = ushort.MaxValue;
        //    HorizontalActuatorCurrent.RealHigh = 100;
        //    channels.Add("HorizontalActuatorCurrent", HorizontalActuatorCurrent);

        //    PowerSupplyVoltage1 = new DummyAnalogInput() { Name = "PowerSupplyVoltage1" };
        //    channels.Add("PowerSupplyVoltage1", PowerSupplyVoltage1);
        //    PowerSupplyVoltage2 = new DummyAnalogInput() { Name = "PowerSupplyVoltage2" };
        //    channels.Add("PowerSupplyVoltage2", PowerSupplyVoltage2);
        //    IsStartPressed = new DummyDigitalInput() { Name = "IsStartPressed" };
        //    channels.Add("IsStartPressed", IsStartPressed);

        //    DistanceX = new DummyAnalogInput() { Name = "DistanceX" };
        //    channels.Add("DistanceX", DistanceX);
        //    DistanceY = new DummyAnalogInput() { Name = "DistanceY" };
        //    channels.Add("DistanceY", DistanceY);
        //    DistanceZ = new DummyAnalogInput() { Name = "DistanceZ" };
        //    channels.Add("DistanceZ", DistanceZ);
        //    MoveMirrorVertical = new DummyDigitalOutput() { Name = "MoveMirrorVertical" };
        //    channels.Add("MoveMirrorVertical", MoveMirrorVertical);
        //    MoveMirrorHorizontal = new DummyDigitalOutput() { Name = "MoveMirrorHorizontal" };
        //    channels.Add("MoveMirrorLeft", MoveMirrorHorizontal);
        //    MoveMirrorReverse = new DummyDigitalOutput() { Name = "MoveMirrorReverse" };
        //    channels.Add("MoveMirrorReverse", MoveMirrorReverse);

        //    AllowMirrorMovement = new DummyDigitalOutput() { Name = "AllowMirrorMovement" };
        //    channels.Add("AllowMirrorMovement", AllowMirrorMovement);
        //    LockWeak = new DummyDigitalOutput() { Name = "LockWeak" };
        //    channels.Add("LockWeak", LockWeak);
        //    UnlockWeak = new DummyDigitalOutput() { Name = "UnlockWeak" };
        //    channels.Add("UnlockWeak", UnlockWeak);
        //    MoveDistanceSensorUp = new DummyDigitalOutput() { Name = "MoveDistanceSensorUp" };
        //    channels.Add("MoveDistanceSensorUp", MoveDistanceSensorUp);
        //    MoveDistanceSensorDown = new DummyDigitalOutput() { Name = "MoveDistanceSensorDown" };
        //    channels.Add("MoveDistanceSensorDown", MoveDistanceSensorDown);
        //    MoveSuckerUp = new DummyDigitalOutput() { Name = "MoveSuckerUp" };
        //    channels.Add("MoveSuckerUp", MoveSuckerUp);
        //    MoveSuckerDown = new DummyDigitalOutput() { Name = "MoveSuckerDown" };
        //    channels.Add("MoveSuckerDown", MoveSuckerDown);
        //    SuckOn = new DummyDigitalOutput() { Name = "SuckOn" };
        //    channels.Add("SuckOn", SuckOn);

        //    AllowPowerSupply = new DummyDigitalOutput() { Name = "AllowPowerSupply" };
        //    channels.Add("AllowPowerSupply", AllowPowerSupply);
        //    LockStrong = new DummyDigitalOutput() { Name = "LockStrong" };
        //    channels.Add("LockStrong", LockStrong);
        //    UnlockStrong = new DummyDigitalOutput() { Name = "UnlockStrong" };
        //    channels.Add("UnlockStrong", UnlockStrong);
        //    GreenLightOn = new DummyDigitalOutput() { Name = "GreenLightOn" };
        //    channels.Add("GreenLightOn", GreenLightOn);
        //    RedLightOn = new DummyDigitalOutput() { Name = "RedLightOn" };
        //    channels.Add("RedLightOn", RedLightOn);

        //    BuzzerOn = new DummyDigitalOutput() { Name = "BuzzerOn" };
        //    channels.Add("BuzzerOn", BuzzerOn);
        //}

        #endregion

        Random gen = new Random();

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
