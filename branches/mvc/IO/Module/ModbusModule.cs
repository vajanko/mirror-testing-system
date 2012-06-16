using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    public sealed class ModbusModule : IModule
    {
        private const string protocol = "Modbus";

        public ushort Port { get; private set; }
        public string IpAddress { get; private set; }

        private const uint timeout = 5000;
        private int hConnection;

        private readonly Dictionary<int, ModbusSlot<ModbusAddress>> inputs = new Dictionary<int, ModbusSlot<ModbusAddress>>();
        private readonly Dictionary<int, ModbusSlot<ModbusAddress>> outputs = new Dictionary<int, ModbusSlot<ModbusAddress>>();

        #region IModule Members

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
            ChannelBase<ModbusAddress> channel;
            // at the beginning all channels are created and inserted to this collection
            // after that slots are created and channels are inserted to them
            List<ChannelBase<ModbusAddress>> channels = new List<ChannelBase<ModbusAddress>>();

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
                        channel = new DigitalInput<ModbusAddress>();
                    else if (items[3] == outputString)
                        channel = new DigitalOutput<ModbusAddress>();
                    else continue;// I/O type of channel is wrong - skip this line
                else              // analog channel (usually 16 length)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new AnalogInput<ModbusAddress>();
                    else if (items[3] == outputString)
                        channel = new AnalogOutput<ModbusAddress>();
                    else continue;// I/O type of channel is wrong - skip this line

                // create a particular type of address for this king of channel
                ModbusAddress addr = new ModbusAddress();

                // now instance of channel is created - save parsed size
                channel.Size = value;
                // "parsing" channel name
                channel.Name = items[0];

                // parsing channel number
                if (!int.TryParse(items[2], out value))
                    continue;   // parsing channel number failed - skip this line
                addr.Channel = (byte)value;

                // parsing slot number
                if (!int.TryParse(items[1], out value))
                    continue;   // parsing slot number failed - skip this line
                addr.Slot = (byte)value;

                // set channel address
                channel.Address = addr;

                // add all channels to this collection
                channels.Add(channel);
            }
            // close file - release resources
            reader.Close();

            // now all channels are created - add them to slots
            while (channels.Count > 0)
            {
                // this item must exist, otherwise channels.Count == 0
                byte slot = channels[0].Address.Slot;
                ModbusSlot<ModbusAddress> mSlot = null;

                var addrs = from c in channels select (c.Address as ModbusAddress);
                // get minimum number of channel in the current slot
                int startChannel = (from a in addrs
                                    where a.Slot == slot
                                    select a.Channel).Min();
                // get maximum number of channel in the current slot
                int channelsCount = (from a in addrs
                                     where a.Slot == slot
                                     select a.Channel).Max();
                // this is the number of reserved channels - there may be some free slots between
                // startChannel and channel with max number, but we must leave them free
                // add 1 because the numbers of channel starts with 0
                channelsCount = channelsCount - startChannel + 1;

                Type type = channels[0].GetType();
                if (type == typeof(DigitalInput<ModbusAddress>))
                    mSlot = new ModbusDISlot<ModbusAddress>(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(DigitalOutput<ModbusAddress>))
                    mSlot = new ModbusDOSlot<ModbusAddress>(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(AnalogInput<ModbusAddress>))
                    mSlot = new ModbusAISlot<ModbusAddress>(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(AnalogOutput<ModbusAddress>))
                    mSlot = null;       // ModbusAOSlot not implemented yet

                if (mSlot == null)    // slot could not be created
                {
                    // skip this channels, but before remove all channels that belongs to this slot
                    channels.RemoveAll(c => (c.Address as ModbusAddress).Slot == slot);
                    continue;
                }

                // add all channels with same slot number and same type to created slot
                for (int i = 0; i < channels.Count; i++)
                    if (channels[i].Address.Slot == slot && channels[i].GetType() == type)
                        mSlot.AddChannel(channels[i]);

                // add created slot to inputs - all slots are inputs also
                inputs.Add(slot, mSlot);
                // if it is also an output slot - add it to outputs
                if (mSlot is ModbusOutputSlot<ModbusAddress>)
                    outputs.Add(slot, mSlot);

                // channels are added to slot - remove them from temporary collection
                channels.RemoveAll(c => c.Address.Slot == slot);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="MTS.IO.Module.ConnectionException">Connection could not be established</exception>
        public void Connect()
        {
            if (IsConnected)
                return;

            try
            {
                // initialize Modbus connection - allocate Modbus resources
                Mxio.MXEIO_Init();
                // create a windows socket on port: "Port", with timeout: "timeout"
                // hConnection is a connection handle that identifies this connection
                Mxio.MXEIO_Connect(System.Text.Encoding.UTF8.GetBytes(IpAddress), Port, timeout, ref hConnection);
                IsConnected = true;
            }
            catch (Exception ex)
            {   // establishing connection failed
                throw new ConnectionException(Resource.ConnectionFailedMsg, ex) { ProtocolName = this.ProtocolName };
            }
        }
        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        public void Initialize()
        {
            // do nothing
        }
        /// <summary>
        /// Read all input and write all output channels
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
            // read values from all slots
            foreach (ModbusSlot<ModbusAddress> slot in inputs.Values)
                if (slot != null)
                    slot.Read(hConnection);
        }
        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
            // write values to all output slots
            foreach (ModbusOutputSlot<ModbusAddress> slot in outputs.Values)
                if (slot != null)
                    slot.Write(hConnection);
        }

        public void Disconnect()
        {
            // release windows socket identified by connection handle
            Mxio.MXEIO_Disconnect(hConnection);
            // release Modbus resources
            Mxio.MXEIO_Exit();
            IsConnected = false;
        }

        public IChannel GetChannel(string id)
        {
            foreach (var channel in this.Inputs)
                if (channel.Id == id)
                    return channel;
            // channel with required name has not been found
            throw new ChannelException(Resource.ChannelNotFoundMsg) { ChannelName = id, ProtocolName = this.ProtocolName };
        }
        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        public bool IsConnected { get; private set; }

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
                foreach (var slot in inputs.Values)
                    foreach (var channel in slot.Channels)
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
                foreach (var slot in inputs.Values)
                    foreach (var channel in slot.Channels)
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
            foreach (var slot in inputs.Values)
                foreach (var channel in slot.Channels)
                    if (channel is TChannel)
                        yield return (TChannel)(IChannel)channel;
        }

        public TChannel GetChannel<TChannel>(string id) where TChannel : IChannel
        {
            // all channels are in input slots
            foreach (TChannel channel in GetChannels<TChannel>())
                if (channel.Id == id)
                    return channel;
            // channel with required name has not been found
            throw new ChannelException(Resource.ChannelNotFoundMsg) { ChannelName = id, ProtocolName = this.ProtocolName };
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return inputs.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (IsConnected)
                Disconnect();
        }

        #endregion

        #endregion

        #region Constructors

        public ModbusModule(string ipAddress, ushort port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        #endregion
    }
}
