﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using TwinCAT.Ads;

using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    public sealed class ECModule : IModule
    {
        #region Channels alocated memory

        // connection object
        private TcAdsClient client = new TcAdsClient();

        // all channels
        private List<ChannelBase> inputs = new List<ChannelBase>();
        private BinaryWriter iWriter;      // input writer
        private AdsStream iReadStream;     // input read stream
        private int iReadStreamOffset;
        private const int readCommand = 0xF080;     // constant that is entered to method call when reading

        // only output channels
        private List<ChannelBase> outputs = new List<ChannelBase>();
        private BinaryWriter oWriter;   // output writer
        private AdsStream oReadStream;  // output read stream
        private int oWriterOffset;      // 
        private const int writeCommand = 0xF081;    // constant that is entered to method call when writing

        #endregion

        /// <summary>
        /// (Get/Set) Name of task in TwinCAT IO Server. This name is necassary for variable handles
        /// </summary>
        public string TaskName { get; set; }

        #region IModule Members

        private readonly char[] whiteSpaces = { ' ', '\t', '\r' };
        private readonly char[] csvSep = { ';' };
        private readonly char[] floatSep = { '.' };
        private const string boolString = "bool";

        private const string inputString = "Input";
        private const string outputString = "Output";
        /// <summary>
        /// Load configuration of channels form file. In case of EtherCAT (Beckhoff) implementation
        /// this file should be .CSV file generated by TwinCAT System Manager. When configuration is being
        /// loaded, must not be Listening yet.
        /// </summary>
        /// <param name="filename">Path to file where configuration of channels is stored</param>
        public void LoadConfiguration(string filename)
        {
            // When loading channels: all of them are added to inputs and they are added to this collection
            // Some of these channels are also outputs - to write them when channels are update, add them to
            // outputs collection
            // CSV format:
            // Name; ;Type;Size;>Address;In/Out;User ID;Linked to
            // Example: HeatingFoilCurrent;X;INT;2.0;0.0;Input;0;Data In . Channel 1 . Term 2 (KL3152) . Box 1 (BK1120) . Device 1 (EtherCAT) . I/O Devices

            // parse TwinCAT configuration file
            ChannelBase channel;
            string tmp;
            int size;
            string[] items;
            StreamReader reader;

            reader = new StreamReader(filename);

            reader.ReadLine();  // skip first line (.csv file format)

            while (!reader.EndOfStream)
            {
                items = reader.ReadLine().Split(csvSep, StringSplitOptions.None);
                if (items.Length < 6) continue; // not enought items per line - skip it                

                tmp = items[2].ToLower();   // type of variable
                if (tmp == boolString)      // bool is a digital channel
                {
                    if (items[5] == inputString)
                        channel = new DigitalInput();
                    else if (items[5] == outputString)
                    {
                        channel = new DigitalOutput();
                        outputs.Add(channel);           // outputs channels are added also to inputs   
                    }
                    else continue;          // skip this line
                    size = sizeof(bool);    // even if size is one bit - in program we use it as one byte (bool)
                }
                else                        // not bool (some kind of int) is an analog channel
                {
                    // parse channel size
                    // items[3] is a channel size - split number by '.' and take the first item
                    if (int.TryParse(items[3].Split(floatSep, StringSplitOptions.RemoveEmptyEntries)[0]
                        , System.Globalization.NumberStyles.AllowDecimalPoint, null, out size))
                    {
                        // read channel type: input/output
                        if (items[5] == inputString)
                            channel = new AnalogInput();
                        else if (items[5] == outputString)
                        {
                            channel = new AnalogOutput();
                            outputs.Add(channel);           // outputs channels are added also to inputs
                        }
                        else continue;  // skip this line
                    }
                    else continue;      // skip this line
                }
                // create a particular type of address fot this king of channel
                ECAddress addr = new ECAddress();
                channel.Name = items[0];
                channel.Size = size;
                // Full name in format: TaskName.Inputs.VariableName
                // notice the "s" at the end of "Input" or "Output" string
                addr.FullName = TaskName + "." + items[5] + "s." + items[0];
                // initialize variable "address"
                addr.IndexGroup = (int)AdsReservedIndexGroups.SymbolValueByHandle;
                // notice that we do not initialize channel.IndexOffset, because for that a connection is necessary
                //addr.IndexOffset = client.CreateVariableHandle(channel.FullName);

                // set channel address
                channel.Address = addr;

                // all channel are added to inputs (also outputs)
                inputs.Add(channel);
            }

            reader.Close();
        }

        /// <summary>
        /// Establish a connection to ADS device
        /// </summary>
        public void Connect()
        {
            // notice that we are connecting to local server which by the way handle any communication 
            // with remote side
            if (!client.IsConnected)
            {
                try
                {
                    client.Connect(AmsNetId.Local, 301);    // ??? fuck - write me an email
                }
                catch (Exception ex)
                {   // establishing connection failed
                    throw new ConnectionException("Connection to the module could not be established",
                        ex) { ProtocolName = ChannelException.EtherCatString };
                }
            }
        }

        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        /// <exception cref="AddressException">Address of some channel does not exists or it variable
        /// handle could not be createds</exception>
        public void Initialize()
        {
            // see that we are calling a method of the client - connection must be established already
            foreach (ChannelBase channel in inputs)
            {
                ECAddress addr = channel.Address as ECAddress;
                if (addr != null)   // specifiy address of each channel - it must be ethercat address
                {
                    try
                    {
                        addr.IndexOffset = client.CreateVariableHandle(addr.FullName);
                    }
                    catch (Exception ex)
                    {   // variable handle of some address could not be created
                        throw new AddressException(string.Format(@"Variable handle of EtherCAT address
                            {0} could not be found", addr.FullName), ex) { ChannelName = channel.Name };
                    }
                }
                else
                {
                    // each channel must have an address
                    throw new AddressException("EtherCAT address on channel not found") { ChannelName = channel.Name };
                }
            }

            alocateChannels();  // alocat memory for reading a writing channels
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
        /// <exception cref="ChannelException">An error occured while reading some channel</exception>
        public void UpdateInputs()
        {
            // do not read if there are no inputs
            if (inputs.Count == 0) return;
            // jump at the beginning of the read stream - readed data are going to be written here
            iReadStream.Seek(0, SeekOrigin.Begin);
            // read data from hardware to read stream
            client.ReadWrite(readCommand, inputs.Count, iReadStream, (AdsStream)iWriter.BaseStream);
            // jump at the beginng of the data (skip error codes)
            iReadStream.Seek(iReadStreamOffset, SeekOrigin.Begin);

            // remove this
            BinaryReader reader = new BinaryReader(iReadStream);

            // read values from stream and write to input channels
            for (int i = 0; i < inputs.Count; i++)
            {
                // check error codes and throw exception if some error occures
                // ...
                inputs[i].ValueBytes = reader.ReadBytes(inputs[i].Size);
            }
        }
        /// <summary>
        /// Write all outputs channels
        /// </summary>
        /// <exception cref="ChannelException">An error occured while writing some channel</exception>
        public void UpdateOutputs()
        {
            // do not write if there are no outputs
            if (outputs.Count == 0) return;
            // seek to position behid info data (IndexGroup, IndexOffset and Size), bytes before never change
            oWriter.Seek(oWriterOffset, SeekOrigin.Begin);
            // write channels values
            for (int i = 0; i < outputs.Count; i++)
                oWriter.Write(outputs[i].ValueBytes);
            // return at previous position where outputs start
            oWriter.Seek(oWriterOffset, SeekOrigin.Begin);
            // write values from stream to hardware
            client.ReadWrite(writeCommand, outputs.Count, oReadStream, (AdsStream)oWriter.BaseStream);

            // check error codes and throw exception if some error occures
        }

        /// <summary>
        /// Close connection between local computer and some hardware component
        /// </summary>
        public void Disconnect()
        {
            // delete variable handles from TwinCAT IO Server
            for (int i = 0; i < inputs.Count; i++)
            {
                ECAddress addr = inputs[i].Address as ECAddress;
                if (addr != null)
                    client.DeleteVariableHandle(addr.IndexOffset);
            }
            // release resources allocated by TwinCAT IO Server
            client.Dispose();
        }

        /// <summary>
        /// Get an instance of paricular channel identified by its name. Return null if ther is no such a channel.
        /// In case of Beckhoff (EtherCAT) IModule implementation this is TwinCAT IO Server variable name.
        /// </summary>
        /// <param name="name">Unic name (identifier) of required channel</param>
        /// <exception cref="ChannelException">Channel identified by its name does not exists in current
        /// module</exception>
        public IChannel GetChannelByName(string name)
        {
            // this is very simple impelementation. It looks through all channels and tries to find
            // that one with given name
            for (int i = 0; i < inputs.Count; i++)
                if (inputs[i].Name == name)     // look for channel with paricular name
                    return inputs[i];
            // channel with name "name" was not found
            throw new ChannelException("Channel not found") { ChannelName = name, ProtocolName = ChannelException.EtherCatString };
        }

        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        public bool IsConnected
        {
            get { return (client != null) ? client.IsConnected : false; }
        }

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return inputs.GetEnumerator();
        }

        #endregion

        #endregion

        #region Channel Handlig

        /// <summary>
        /// Read all input and output channels
        /// </summary>
        /// <exception cref="ChannelException">An error occured while reading some channel</exception>
        private void readChannels()
        {
            // do not read if there are no inputs
            if (inputs.Count == 0) return;
            // jump at the beginning of the read stream - readed data are going to be written here
            iReadStream.Seek(0, SeekOrigin.Begin);
            // read data from hardware to read stream
            client.ReadWrite(readCommand, inputs.Count, iReadStream, (AdsStream)iWriter.BaseStream);
            // jump at the beginng of the data (skip error codes)
            iReadStream.Seek(iReadStreamOffset, SeekOrigin.Begin);

            // remove this
            BinaryReader reader = new BinaryReader(iReadStream);

            // read values from stream and write to input channels
            for (int i = 0; i < inputs.Count; i++)
            {
                // check error codes and throw exception if some error occures
                // ...
                inputs[i].ValueBytes = reader.ReadBytes(inputs[i].Size);
            }
        }
        /// <summary>
        /// Write all output channels
        /// </summary>
        /// <exception cref="ChannelException">An error occured while writing some channel</exception>
        private void writeChannels()
        {
            // do not write if there are no outputs
            if (outputs.Count == 0) return;
            // seek to position behid info data (IndexGroup, IndexOffset and Size), bytes before never change
            oWriter.Seek(oWriterOffset, SeekOrigin.Begin);
            // write channels values
            for (int i = 0; i < outputs.Count; i++)
                oWriter.Write(outputs[i].ValueBytes);
            // return at previous position where outputs start
            oWriter.Seek(oWriterOffset, SeekOrigin.Begin);
            // write values from stream to hardware
            client.ReadWrite(writeCommand, outputs.Count, oReadStream, (AdsStream)oWriter.BaseStream);

            // check error codes and throw exception if some error occures
            
        }
        /// <summary>
        /// Allocate and initialize memory necessary for channel handlig. This method is called only once
        /// and then initialized memory is reused.
        /// </summary>
        private void alocateChannels()
        {
            // 1. allocate memory for input channels
            int count = inputs.Count;  // number of items to read
            // for each variable readed from hardware 4 more bytes for error status are necessary
            int readLength = count * 4;     // we are going to skip these bytes when reading inputs
            // position in the stream where value for reading begins, before are only error codes
            iReadStreamOffset = readLength;
            // Information about reading variables are stored in a stream. We put this values to stream
            // throught BinaryWriter. This is space necessary for additional info about reading variable:
            // IndexGroup, IndexOffset, Size - 4B for each item
            int writeLength = count * 12;

            iWriter = new BinaryWriter(new AdsStream(writeLength));
            for (int i = 0; i < count; i++)
            {
                ECAddress addr = inputs[i].Address as ECAddress;
                if (addr == null) continue;     // skip this channel if no (or not correct) address is present
                iWriter.Write(addr.IndexGroup);
                iWriter.Write(addr.IndexOffset);
                iWriter.Write(inputs[i].Size);
                readLength += inputs[i].Size;       // count size of readed memory (+error codes)
            }
            // to this stream data are going to be read - size is sum of all variable sizes + error codes
            iReadStream = new AdsStream(readLength);


            // 2. allocate memory for writing channels
            count = outputs.Count;  // number of items to write
            // For each variable writed an error code (4B size) is returned. This is size of memory for all error codes
            readLength = count * 4;
            // Information about writing variables are stored in a stream. We put this values to stream
            // throught BinaryWriter. This is space necessary for additional info about writing variable:
            // IndexGroup, IndexOffset, Size - 4B for each item
            writeLength = count * 12;       // but that is not all - add space for values of writing channels
            oWriterOffset = writeLength;    // position in the stream where values for writing begins, before are only info data
            for (int i = 0; i < count; i++) // add memory for each variable that is going to be written
                writeLength += outputs[i].Size;            

            // create BinaryWriter and write info data about every variable (channel)
            oWriter = new BinaryWriter(new AdsStream(writeLength));
            for (int i = 0; i < count; i++)
            {
                ECAddress addr = outputs[i].Address as ECAddress;
                if (addr == null) continue;     // skip this channel if no (or not correct) address is present
                oWriter.Write(addr.IndexGroup);    // notice that data are writed at the end
                oWriter.Write(addr.IndexOffset);
                oWriter.Write(outputs[i].Size);
            }
            // when writing add data at the end of this writer - oWriterOffset points here

            // to this stream error codes will be written
            oReadStream = new AdsStream(readLength);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of EtherCAT module
        /// </summary>
        /// <param name="taskName">Name of task in TwinCAT IO Server. This is necessary for variables
        /// handles</param>
        public ECModule(string taskName)
        {
            TaskName = taskName;    // this is necessary for variable handles
        }

        #endregion
    }
}
