using System;

namespace MTS.AdminModule
{
    class ModbusAddress
    {
        /// <summary>
        /// (Get/Set) Slot number where this channel is placed
        /// </summary>
        public byte Slot { get; set; }
        /// <summary>
        /// (Get/Set) Address of this channel inside a particular slot
        /// </summary>
        public byte Channel { get; set; }
    }
}
