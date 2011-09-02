using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MTS.AdminModule
{
    class ModbusModule : IModule
    {
        public ushort Port { get; protected set; }
        public string IpAddress { get; protected set; }

        private uint timeout = 5000;
        private int hConnection;

        private Dictionary<int, ModbusSlot> inputs = new Dictionary<int, ModbusSlot>();
        private Dictionary<int, ModbusSlot> outputs = new Dictionary<int, ModbusSlot>();

        #region IModule Members

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
            ModbusChannel channel;
            // at the beginning all channels are created and inserted to this collection
            // after that slots are created and channels are inserted to them
            List<ModbusChannel> channels = new List<ModbusChannel>();

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
                        channel = new ModbusDigitalInput();
                    else if (items[3] == outputString)
                        channel = new ModbusDigitalOutput();
                    else continue;// I/O type of channel is wrong - skip this line
                else              // analog channel (usually 16 length)
                    // check for I/O type of channel
                    if (items[3] == inputString)
                        channel = new ModbusAnalogInput();
                    else if (items[3] == outputString)
                        channel = new ModbusAnalogOutput();
                    else continue;// I/O type of channel is wrong - skip this line

                // now instance of channel is created - save parsed size
                channel.Size = value;
                // "parsing" channel name
                channel.Name = items[0];

                // parsing channel number
                if (!int.TryParse(items[2], out value))
                    continue;   // parsing channel number failed - skip this line
                channel.Channel = (byte)value;

                // parsing slot number
                if (!int.TryParse(items[1], out value))
                    continue;   // parsing slot number failed - skip this line
                channel.Slot = (byte)value;

                // add all channels to this collection
                channels.Add(channel);
            }
            // close file - release resources
            reader.Close();

            // now all channels are created - add them to slots
            while (channels.Count > 0)
            {
                byte slot = channels[0].Slot;   // this item must exist, otherwise channels.Count == 0
                ModbusSlot mSlot = null;
                
                int startChannel = (from c in channels where c.Slot == slot select c.Channel).Min();
                // get maximum number of channel in the same slot
                int channelsCount = (from c in channels where c.Slot == slot select c.Channel).Max();
                // this is the number of reserved channels - there may be some free slots between
                // startChannel and channel with max number, but we must leave them free
                // add 1 because the numbers of channel starts with 0
                channelsCount = channelsCount - startChannel + 1;

                Type type = channels[0].GetType();
                if (type == typeof(ModbusDigitalInput))
                    mSlot = new ModbusDISlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(ModbusDigitalOutput))
                    mSlot = new ModbusDOSlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(ModbusAnalogInput))
                    mSlot = new ModbusAISlot(slot, (byte)startChannel, (byte)channelsCount);
                else if (type == typeof(ModbusAnalogOutput))
                    mSlot = null;       // ModbusAOSlot not implemented yet

                if (mSlot == null)    // slot could not be created
                {
                    // skip this channels, but before remove all channels that belongs to this slot
                    channels.RemoveAll(new Predicate<ModbusChannel>(c => c.Slot == slot));
                    continue;
                }

                // add all channels with same slot number and same type to created slot
                for (int i = 0; i < channels.Count; i++)
                    if (channels[i].Slot == slot && channels[i].GetType() == type)
                        mSlot.AddChannel(channels[i]);

                // add created slot to inputs - all slots are inputs also
                inputs.Add(slot, mSlot);
                // if it is also an output slot - add it to outputs
                if (mSlot is ModbusOutputSlot)
                    outputs.Add(slot, mSlot);

                // channels are added to slot - remove them from temporary collection
                channels.RemoveAll(new Predicate<ModbusChannel>(c => c.Slot == slot));
            }
        }

        public void Connect()
        {
            // initialize Modbus connection - alocate Modbus resources
            Mxio.MXEIO_Init();
            // create a windows socket on port: "Port", with timeout: "timeout"
            // hConnection is a connnection handle that identifies this connection
            Mxio.MXEIO_Connect(Encoding.UTF8.GetBytes(IpAddress), Port, timeout, ref hConnection);
            isConnected = true;
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
                Output.Log(ex.Message);
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
            isConnected = false;
        }

        public IChannel GetChannelByName(string name)
        {
            ModbusChannel channel;

            // all channels are in input slots
            foreach (ModbusSlot slot in inputs.Values)
                if ((channel = slot.GetChannelByName(name)) != null)
                    return channel;     // this slot contains channel with required name
            // channel with required name has not been found
            return null;
        }

        private bool isConnected;
        /// <summary>
        /// (Get) Value indicating that this module is connected to remote hardware
        /// </summary>
        public bool IsConnected
        {
            get { return isConnected; }
            private set { isConnected = value; }
        }

        public void SwitchOffDigitalOutputs()
        {
            if (!IsConnected) return;

            ModbusDOSlot s;
            ModbusDigitalOutput ch;
            foreach (ModbusOutputSlot slot in outputs.Values)
            {
                s = slot as ModbusDOSlot;
                if (s != null)
                {
                    for (int i = 0; i < s.ChannelsCount; i++)
                    {
                        ch = s.Channels[i] as ModbusDigitalOutput;
                        if (ch != null)
                            ch.Value = false;
                    }
                    s.Write(hConnection);
                }
            }
        }

        #endregion

        #region Constructors

        public ModbusModule(string ipAddress, ushort port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        #endregion
    }
}
