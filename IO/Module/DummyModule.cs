using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    public sealed class DummyModule : IModule
    {
        private const string protocol = "Dummy";

        /// <summary>
        /// True if module is Listening
        /// </summary>
        public bool IsConnected { get; set; }
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private int port = 1234;
        private ASCIIEncoding enc = new ASCIIEncoding();

        TcpClient master;

        #region IModule Members

        public void Initialize()
        {
        }

        public void Connect()
        {
            try
            {
                master = new TcpClient();
                //new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                master.Connect(new IPEndPoint(IPAddress.Loopback, port));
                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
                throw;  // re-throw this exception
            }
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
           // read command
            NetworkStream stream = master.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("read");
            writer.Flush();

            // wait for response (read)
            StreamReader reader = new StreamReader(stream);
            string line;
            while ((line = reader.ReadLine()) != "end")
            {
                string[] tmp = line.Split(':');
                if (tmp.Length > 1)
                {
                    string id = tmp[0];
                    string value = tmp[1];
                    //List<byte> val = new List<byte>();
                    //foreach (char c in value)
                    //    val.Add((byte)c);

                    IChannel channel = GetChannel(id);
                    if (channel != null)
                    {
                        if (channel is IAnalogInput)
                            (channel as IAnalogInput).SetValue(uint.Parse(value));
                        else if (channel is IDigitalInput)
                            (channel as IDigitalInput).SetValue(bool.Parse(value));
                        //channel.ValueBytes = enc.GetBytes(tmp[1]);
                    }
                }
            }
        }

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
            // write command
            NetworkStream stream = master.GetStream();
            StreamWriter writer = new StreamWriter(stream);

            // send write command
            writer.WriteLine("write");
            // write all !!! output !!! channels to stream
            foreach (IChannel channel in outputs)
            {
                if (channel is IAnalogInput)
                    writer.Write("{0}:{1}\n", channel.Id, (channel as IAnalogInput).Value);
                else if (channel is IDigitalInput)
                    writer.Write("{0}:{1}\n", channel.Id, (channel as IDigitalInput).Value);
            }
            // closing command
            writer.WriteLine("end");
            writer.Flush();
        }

        public void Disconnect()
        {
            if (master != null)
                master.Close();
            IsConnected = false;
        }

        /// <summary>
        /// Return an instance of channel identified by name or null if there is no such a channel
        /// </summary>
        /// <param name="id">Channel identifier</param>
        public IChannel GetChannel(string id)
        {
            for (int i = 0; i < inputs.Count; i++)
                if (inputs[i].Id == id)     // look for channel with particular name
                    return inputs[i];
            throw new ChannelException(Resource.ChannelNotFoundMsg) { ChannelName = id, ProtocolName = this.ProtocolName };
        }

        private readonly List<ChannelBase<DummyAddress>> inputs = new List<ChannelBase<DummyAddress>>();
        private readonly List<ChannelBase<DummyAddress>> outputs = new List<ChannelBase<DummyAddress>>();

        private readonly char[] csvSep = { ';' };
        private const int itemsPerLine = 5; // number of items per one line

        private const string inputString = "Input";
        private const string outputString = "Output";

        public void LoadConfiguration(string filename)
        {
            string str;         // temporary value for string while parsing
            int value;          // temporary value for integer while parsing
            string[] items;     // parsed items on one line 

            // reference to just created channel
            ChannelBase<DummyAddress> channel;

            // open configuration file
            StreamReader reader = new StreamReader(filename);

            // skip first line (.csv file format) and count number of items on first line
            //itemsPerLine = 
            reader.ReadLine().Split(csvSep, StringSplitOptions.RemoveEmptyEntries);
            // [Channel Name];[Slot Number];[Channel Number];[I/O type];[I/O Data Length(bits)];[Comment]

            while (!reader.EndOfStream)
            {
                // parse line with CSV separator
                items = reader.ReadLine().Split(csvSep, StringSplitOptions.RemoveEmptyEntries);
                // not enough items per line - skip it
                if (items.Length < itemsPerLine) continue;

                // parsing length of channel value (in bits and hexadecimal format)
                str = items[4].Substring(items[4].IndexOf('x') + 1);    // remove leading 0x
                if (!int.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out value))
                    continue;   // parsing data length failed - skip this line

                // create an instance of channel - depending on the channel type
                if (value == 1)   // digital channel is always of size 1 (bit)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new DigitalInput<DummyAddress>();
                    else if (items[3] == outputString)
                    {
                        channel = new DigitalOutput<DummyAddress>();
                        outputs.Add(channel);
                    }
                    else continue;// I/O type of channel is wrong - skip this line
                else              // analog channel (usually 16 length)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new AnalogInput<DummyAddress>();
                    else if (items[3] == outputString)
                    {
                        channel = new AnalogOutput<DummyAddress>();
                        outputs.Add(channel);
                    }
                    else continue;// I/O type of channel is wrong - skip this line

                // "parsing" channel name
                channel.Id = items[0];
                channel.Name = items[0];

                // add all channels to this collection
                inputs.Add(channel);
            }
            // close file - release resources
            reader.Close();
        }

        /// <summary>
        /// (Get) Name of this module communication protocol
        /// </summary>
        public string ProtocolName { get { return protocol; } }

        /// <summary>
        /// Enumerate collection of input channels. All outputs are inputs as well
        /// </summary>
        public IEnumerable<IChannel> Inputs
        {
            get
            {
                foreach (var channel in inputs)
                    if (channel is IAnalogInput || channel is IDigitalInput)
                        yield return channel;
            }
        }
        /// <summary>
        /// Enumerate collection of output channels
        /// </summary>
        public IEnumerable<IChannel> Outputs
        {
            get
            {
                foreach (var channel in inputs)
                    if (channel is IDigitalOutput || channel is IAnalogOutput)
                        yield return channel;
            }
        }
        /// <summary>
        /// Enumerate collection of channels of given type. For example all analog input channels
        /// </summary>
        /// <typeparam name="TChannel">Type of channel to enumerate</typeparam>
        /// <returns>Collection of channels of particular type</returns>
        public IEnumerable<TChannel> GetChannels<TChannel>() where TChannel : IChannel
        {
            foreach (var channel in Inputs)
                if (channel is TChannel)
                    yield return (TChannel)channel;
        }

        public TChannel GetChannel<TChannel>(string id) where TChannel : IChannel
        {
            foreach (var channel in GetChannels<TChannel>())
                if (channel.Id == id)
                    return channel;
            throw new ChannelException(string.Format(Resource.ChannelOfTypeNotFoundMsg, typeof(TChannel), id)) { ChannelName = id, ProtocolName = this.ProtocolName };
        }

        #endregion


        //#region Channels

        //#region Digital inputs

        //public IDigitalInput IsDistanceSensorUp { get; set; }
        //public IDigitalInput IsDistanceSensorDown { get; set; }
        //public IDigitalInput IsSuckerUp { get; set; }
        //public IDigitalInput IsSuckerDown { get; set; }
        //public IDigitalInput IsVacuum { get; set; }
        //public IDigitalInput IsLeftRubberPresent { get; set; }


        //public IDigitalInput IsPowerfoldDown { get; set; }
        //public IDigitalInput IsPowerfoldUp { get; set; }
        //public IDigitalInput IsOldPowerfoldUp { get; set; }
        //public IDigitalInput IsRightRubberPresent { get; set; }
        //public IDigitalInput IsLeftMirror { get; set; }
        //public IDigitalInput IsOldMirror { get; set; }
        //public IDigitalInput IsOldPowerfoldDown { get; set; }
        //public IDigitalInput IsStartPressed { get; set; }
        //public IDigitalInput IsAckPressed { get; set; }

        //public IDigitalInput IsLocked { get; set; }
        //public IDigitalInput IsOldLocked { get; set; }

        //public IDigitalInput HeatingFoilSignSensor { get; set; }
        //public IDigitalInput PowerFoldUnfoldedPositionSensor1 { get; set; }
        //public IDigitalInput PowerFoldUnfoldedPositionSensor2 { get; set; }
        //public IDigitalInput PowerFoldFoldedPositionSensor { get; set; }
        //public IDigitalInput TestingDeviceOpened { get; set; }
        //public IDigitalInput TestingDeviceClosed { get; set; }

        //public IDigitalInput ErrorAcknButton { get; set; }

        //public IDigitalInput SensorHeadOut { get; set; }
        //public IDigitalInput SensorHeadIn { get; set; }
        //public IDigitalInput InsCheck1 { get; set; }
        //public IDigitalInput InsCheck2 { get; set; }
        //public IDigitalInput InsCheck3 { get; set; }
        //public IDigitalInput InsCheck4 { get; set; }
        //public IDigitalInput InsCheck5 { get; set; }
        //public IDigitalInput InsCheck6 { get; set; }
        //public IDigitalInput InsCheck7 { get; set; }
        //public IDigitalInput InsCheck8 { get; set; }
        //public IDigitalInput InsCheck9 { get; set; }
        //public IDigitalInput InsCheck10 { get; set; }
        //public IDigitalInput InsCheck11 { get; set; }
        //public IDigitalInput InsCheck12 { get; set; }
        //public IDigitalInput InsCheck13 { get; set; }
        //public IDigitalInput InsCheck14 { get; set; }
        //public IDigitalInput InsCheck15 { get; set; }
        //public IDigitalInput InsCheck16 { get; set; }
        //public IDigitalInput IsPowerSupplyOff { get; set; }


        //#endregion

        //#region Digital outputs

        //public IDigitalOutput AllowMirrorMovement { get; set; }
        //public IDigitalOutput MoveMirrorVertical { get; set; }
        //public IDigitalOutput MoveMirrorHorizontal { get; set; }
        //public IDigitalOutput MoveMirrorReverse { get; set; }
        //public IDigitalOutput FoldPowerfold { get; set; }
        //public IDigitalOutput UnfoldPowerfold { get; set; }
        //public IDigitalOutput HeatingFoilOn { get; set; }
        //public IDigitalOutput DirectionLightOn { get; set; }

        //public IDigitalOutput LockWeak { get; set; }
        //public IDigitalOutput UnlockWeak { get; set; }
        //public IDigitalOutput MoveDistanceSensorUp { get; set; }
        //public IDigitalOutput MoveDistanceSensorDown { get; set; }
        //public IDigitalOutput MoveSuckerUp { get; set; }
        //public IDigitalOutput MoveSuckerDown { get; set; }
        //public IDigitalOutput SuckOn { get; set; }
        //public IDigitalOutput BlowOn { get; set; }

        //public IDigitalOutput AllowPowerSupply { get; set; }
        //public IDigitalOutput LockStrong { get; set; }
        //public IDigitalOutput UnlockStrong { get; set; }
        //public IDigitalOutput GreenLightOn { get; set; }
        //public IDigitalOutput RedLightOn { get; set; }
        //public IDigitalOutput BuzzerOn { get; set; }


        //public IDigitalOutput OpenTestingDevice { get; set; }
        //public IDigitalOutput CloseTestingDevice { get; set; }

        //#endregion

        //#region Analog inputs

        //public IAnalogInput DistanceX { get; set; }
        //public IAnalogInput DistanceY { get; set; }
        //public IAnalogInput DistanceZ { get; set; }
        //public IAnalogInput VerticalActuatorCurrent { get; set; }
        //public IAnalogInput HorizontalActuatorCurrent { get; set; }
        //public IAnalogInput HeatingFoilCurrent { get; set; }
        //public IAnalogInput PowerfoldCurrent { get; set; }
        //public IAnalogInput DirectionLightCurrent { get; set; }

        //public IAnalogInput PowerSupplyVoltage1 { get; set; }
        //public IAnalogInput PowerSupplyVoltage2 { get; set; }

        //#endregion

        //#endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return inputs.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();
            if (master != null)
                master.Close();

            timer.Dispose();
        }

        #endregion

        public DummyModule(int port = 1234)
        {
            this.port = port;
        }
    }
}
