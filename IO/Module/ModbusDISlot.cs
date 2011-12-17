using System;
using MTS.IO.Channel;

namespace MTS.IO.Module
{
    class ModbusDISlot : ModbusSlot
    {
        /// <summary>
        /// 32 bits for reading binary values. Zero bit in this number is value of StartChannel and so on ...
        /// </summary>
        private uint inputs;

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
                    channel.SetValue(getValue(inputs, i));  // read input values
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Modbus digital input slot
        /// </summary>
        /// <param name="slot">Address of this slot inside Modbus module</param>
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
}
