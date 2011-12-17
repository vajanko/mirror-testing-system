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
        public ushort Port { get; private set; }
        public string IpAddress { get; private set; }

        private const uint timeout = 5000;
        private int hConnection;

        private readonly Dictionary<int, ModbusSlot> inputs = new Dictionary<int, ModbusSlot>();
        private readonly Dictionary<int, ModbusSlot> outputs = new Dictionary<int, ModbusSlot>();

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
            ChannelBase channel;
            // at the beginning all channels are created and inserted to this collection
            // after that slots are created and channels are inserted to them
            List<ChannelBase> channels = new List<ChannelBase>();

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
                        channel = new DigitalOutput();
                    else continue;// I/O type of channel is wrong - skip this line
                else              // analog channel (usually 16 length)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new AnalogInput();
                    else if (items[3] == outputString)
                        channel = new AnalogOutput();
                    else continue;// I/O type of channel is wrong - skip this line

                // create a particular type of address fot this king of channel
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
                ModbusAddress addr = channels[0].Address as ModbusAddress;
                if (addr == null) continue; // skip this channel if no (or not correct) address is present
                byte slot = addr.Slot;   
                ModbusSlot mSlot = null;

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
                if (type == typeof(DigitalInput))
                    mSlot = new ModbusDISlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(DigitalOutput))
                    mSlot = new ModbusDOSlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(AnalogInput))
                    mSlot = new ModbusAISlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(AnalogOutput))
                    mSlot = null;       // ModbusAOSlot not implemented yet

                if (mSlot == null)    // slot could not be created
                {
                    // skip this channels, but before remove all channels that belongs to this slot
                    channels.RemoveAll(c => (c.Address as ModbusAddress).Slot == slot);
                    continue;
                }

                // add all channels with same slot number and same type to created slot
                for (int i = 0; i < channels.Count; i++)
                    if ((channels[i].Address as ModbusAddress).Slot == slot && channels[i].GetType() == type)
                        mSlot.AddChannel(channels[i]);

                // add created slot to inputs - all slots are inputs also
                inputs.Add(slot, mSlot);
                // if it is also an output slot - add it to outputs
                if (mSlot is ModbusOutputSlot)
                    outputs.Add(slot, mSlot);

                // channels are added to slot - remove them from temporary collection
                channels.RemoveAll(c => (c.Address as ModbusAddress).Slot == slot);
            }
        }

        public void Connect()
        {
            // initialize Modbus connection - allocate Modbus resources
            Mxio.MXEIO_Init();
            // create a windows socket on port: "Port", with timeout: "timeout"
            // hConnection is a connection handle that identifies this connection
            Mxio.MXEIO_Connect(System.Text.Encoding.UTF8.GetBytes(IpAddress), Port, timeout, ref hConnection);
            IsConnected = true;
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
            try
            {
                // read values from all slots
                foreach (ModbusSlot slot in inputs.Values)
                    if (slot != null)
                        slot.Read(hConnection);
            }
            catch (Exception ex)
            {
                //Output.Log(ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// Write all outputs channels
        /// </summary>
        public void UpdateOutputs()
        {
            // write values to all output slots
            foreach (ModbusOutputSlot slot in outputs.Values)
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

        public IChannel GetChannelByName(string name)
        {
            ChannelBase channel;

            // all channels are in input slots
            foreach (ModbusSlot slot in inputs.Values)
                if ((channel = slot.GetChannelByName(name)) != null)
                    return channel;     // this slot contains channel with required name
            // channel with required name has not been found
            return null;
        }
        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        public bool IsConnected { get; private set; }

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
