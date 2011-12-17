using System;

namespace MTS.IO.Module
{
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
            base(slot, startChannel, channelsCount) { }

        #endregion
    }
}
