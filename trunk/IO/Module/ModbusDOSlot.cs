using System;
using System.Collections.Generic;
using MTS.IO.Channel;

namespace MTS.IO.Module
{
    class ModbusDOSlot : ModbusOutputSlot
    {
        /// <summary>
        /// 32 bits for writing binary values. Zero bit in this number is value of StartChannel and so on ...
        /// </summary>
        private uint outputs;

        /// <summary>
        /// Return logical value of bit on a particular position inside integer vector
        /// </summary>
        /// <param name="vector">Integer vector (as binary)</param>
        /// <param name="position">Position inside vector of required logical value</param>
        protected static bool getValue(uint vector, int position)
        {
            // shift required position at the beginning of the vector
            // delete all positions (set to 0) except of the first one (using number 1)
            // compare to number 1 (true if at the required position is 1)
            return ((vector >> position) & 1) == 1;
        }

        private static void setValue(ref uint vector, int position, bool value)
        {
            if (value)  // write 1
                // create a vector of 0's with 1 at required position: ...00100...
                // OR: all bits stay unchanged, bit with 1 will change to 1 (zero or one)
                vector |= ((uint)1 << position);
            else        // write 0
                // create vector of 0's with 1 at required position: ...00100...
                // NOT: create vector of 1's with 0 at required position: ...11011...
                // AND: all bits stay unchanged, bit with 0 will change to 0 (zero or one)
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
                    channel.SetValue(getValue(outputs, i)); // read input values
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
        /// <param name="slot">Address of this slot inside Modbus module</param>
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
}
