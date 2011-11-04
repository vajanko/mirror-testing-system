using System;
using System.Collections.Generic;
using MTS.IO.Channel;
using MTS.IO.Address;

namespace MTS.IO.Module
{
    /// <summary>
    /// Base class for all Modbus slots
    /// </summary>
    abstract class ModbusSlot
    {
        #region Properties

        /// <summary>
        /// (Get) Address of this slot inside Modbus module
        /// </summary>
        public byte Slot { get; protected set; }
        /// <summary>
        /// (Get) Address of first channel inside this slot. (Default zero is usually enought)
        /// </summary>
        public byte StartChannel { get; protected set; }
        /// <summary>
        /// (Get) Number of channels inside this slot
        /// </summary>
        public byte ChannelsCount { get; protected set; }

        #endregion

        public ChannelBase[] Channels { get; protected set; }

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public abstract void Read(int hConnection);

        /// <summary>
        /// Insert a channel to this slot
        /// </summary>
        /// <param name="channel">Modbus channel to insert</param>
        public void AddChannel(ChannelBase channel)
        {
            // this is the only place where modbus address is used
            ModbusAddress addr = channel.Address as ModbusAddress;
            if (addr != null)
                Channels[addr.Channel - StartChannel] = channel;
        }
        /// <summary>
        /// Get an instance of paricular channel identified by its name. Return null if ther is no such a channel
        /// </summary>
        /// <param name="name">Unic name (identifier) of required channel</param>
        public ChannelBase GetChannelByName(string name)
        {
            for (int i = 0; i < Channels.Length; i++)
                if (Channels[i] != null && Channels[i].Name == name)       
                    return Channels[i];     // find channel with particular name
            // if this method did not returned in the cycle - there is no such a channel
            return null;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus slot
        /// </summary>
        /// <param name="slot">Address of this slot insede Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusSlot(byte slot, byte startChannel, byte channelsCount)
        {
            this.Slot = slot;
            this.StartChannel = startChannel;
            this.ChannelsCount = channelsCount;
        }

        #endregion
    }
    /// <summary>
    /// Base class for all Modbus output slots
    /// </summary>
    abstract class ModbusOutputSlot : ModbusSlot
    {
        /// <summary>
        /// Write values of all channels
        /// </summary>
        /// <param name="hConnection">Handle of connection to which to write</param>
        public abstract void Write(int hConnection);

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus output slot
        /// </summary>
        /// <param name="slot">Address of this slot insede Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusOutputSlot(byte slot, byte startChannel, byte channelsCount) :
            base(slot, startChannel, channelsCount)
        {
        }

        #endregion
    }

    class ModbusDISlot : ModbusSlot
    {
        /// <summary>
        /// 32 bits for reading binary values. Zero bit in this number is value of StartChannel and so on ...
        /// </summary>
        private uint inputs;

        /// <summary>
        /// Return logical value of bit on a paritcular position inside integer vector
        /// </summary>
        /// <param name="vector">Integer vector (as binary)</param>
        /// <param name="position">Position inside vector of required logical value</param>
        protected bool getValue(uint vector, int position)
        {
            // shift required postion at the beginnning of the vector
            // delete all positions (set to 0) except of the first one (using number 1)
            // compare to number 1 (true if at the required position is 1)
            return ((vector >> position) & 1) == 1;
        }

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public override void Read(int hConnection)
        {
            DigitalInput channel;

            // read value from channels to integer vector
            Mxio.DI_Reads(hConnection, Slot, StartChannel, ChannelsCount, ref inputs);
            // copy values to channels
            for (int i = 0; i < ChannelsCount; i++)
            {   // some of channels may be unused
                channel = Channels[i] as DigitalInput;
                if (channel != null)                        // get logical values at particular position in
                    channel.SetValue(getValue(inputs, i));  // readed input values
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus digital input slot
        /// </summary>
        /// <param name="slot">Address of this slot insede Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusDISlot(byte slot, byte startChannel, byte channelsCount)
            : base(slot, startChannel, channelsCount)
        {
            // allocate as much memory as necessary
            Channels = new DigitalInput[channelsCount];
        }

        #endregion
    }

    class ModbusDOSlot : ModbusOutputSlot
    {
        /// <summary>
        /// 32 bits for writing binary values. Zero bit in this number is value of StartChannel and so on ...
        /// </summary>
        private uint outputs;

        /// <summary>
        /// Return logical value of bit on a paritcular position inside integer vector
        /// </summary>
        /// <param name="vector">Integer vector (as binary)</param>
        /// <param name="position">Position inside vector of required logical value</param>
        protected bool getValue(uint vector, int position)
        {
            // shift required postion at the beginnning of the vector
            // delete all positions (set to 0) except of the first one (using number 1)
            // compare to number 1 (true if at the required position is 1)
            return ((vector >> position) & 1) == 1;
        }

        private void setValue(ref uint vector, int position, bool value)
        {
            if (value)  // write 1
                // create a vector of 0's with 1 at required position: ...00100...
                // OR: all bits stay unchaged, bit with 1 will change to 1 (zero or one)
                vector |= ((uint)1 << position);
            else        // write 0
                // create vector of 0's with 1 at required position: ...00100...
                // NOT: create vector of 1's with 0 at required position: ...11011...
                // AND: all bits stay unchaged, bit with 0 will change to 0 (zero or one)
                vector &= ~((uint)1 << position);            
        }

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public override void Read(int hConnection)
        {
            DigitalOutput channel;

            // read value from hardware channels to integer vector
            Mxio.DO_Reads(hConnection, Slot, StartChannel, ChannelsCount, ref outputs);
            // copy values to channels
            for (int i = 0; i < ChannelsCount; i++)         // get logical values at particular position in
            {   // some of channels may be unused
                channel = Channels[i] as DigitalOutput;
                if (channel != null)
                    channel.SetValue(getValue(outputs, i)); // readed input values
            }
        }
        /// <summary>
        /// Write values of all channels
        /// </summary>
        /// <param name="hConnection">Handle of connection to which to write</param>
        public override void Write(int hConnection)
        {
            DigitalOutput channel;

            // copy value from channels to outputs vector
            for (int i = 0; i < ChannelsCount; i++)
            {   // some of channels may be unused
                channel = Channels[i] as DigitalOutput;
                if (channel != null)                        // set zero/one values at particular position in
                    setValue(ref outputs, i, channel.Value);// output vector
            }
            // write values in integer vector to hardware channels
            Mxio.DO_Writes(hConnection, Slot, StartChannel, ChannelsCount, outputs);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus digital output slot
        /// </summary>
        /// <param name="slot">Address of this slot insede Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusDOSlot(byte slot, byte startChannel, byte channelsCount)
            : base(slot, startChannel, channelsCount)
        {
            // allocate as much memory as necessary
            Channels = new DigitalOutput[channelsCount];
        }

        #endregion
    }

    class ModbusAISlot : ModbusSlot
    {
        /// <summary>
        /// Array of integer values for reading analog inputs. Item with zero index is value of
        /// StartChannel and so on ...
        /// </summary>
        ushort[] inputs;
        double[] dInputs;

        /// <summary>
        /// Read values of all channels
        /// <param name="hConnnection">Handle of connection from which to read</param>
        /// </summary>
        public override void Read(int hConnection)
        {
            AnalogInput channel;
            // read values from hardware channels to integer array

            // maximum 4 values can read
            // !!! NOT VERY EFFECTIVE !!!

            //Mxio.AI_ReadRaws(hConnection, Slot, StartChannel, 4, inputs);            
            for (int i = 0; i < ChannelsCount; i++)
                Mxio.AI_ReadRaw(hConnection, Slot, (byte)(i + StartChannel), ref inputs[i]);

            // copy values to channels
            for (int i = 0; i < ChannelsCount; i++)
            {   // some of channels may be unused
                channel = Channels[i] as AnalogInput;
                if (channel != null)
                    channel.SetValue(inputs[i]);    // for unused channel value of inputs[i] is unspecified
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus digital input slot
        /// </summary>
        /// <param name="slot">Address of this slot insede Modbus module</param>
        /// <param name="startChannel">Address of first channel inside this slot</param>
        /// <param name="channelsCount">Number of channels inside this slot</param>
        public ModbusAISlot(byte slot, byte startChannel, byte channelsCount)
            : base(slot, startChannel, channelsCount)
        {
            // allocate as much memory as necessary
            Channels = new AnalogInput[channelsCount];
            inputs = new ushort[channelsCount];
            dInputs = new double[channelsCount];
        }

        #endregion
    }

    // ModbusAOSlot is not implemented
}
