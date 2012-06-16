using System;
using System.Globalization;
using System.Xml.Linq;

namespace MTS.IO.Address
{
    /// <summary>
    /// Address of a channel in Modbus communication module
    /// </summary>
    public class ModbusAddress : IAddress
    {
        /// <summary>
        /// (Get/Set) Slot number where this channel is placed
        /// </summary>
        public byte Slot { get; set; }
        /// <summary>
        /// (Get/Set) Address of this channel inside a particular slot
        /// </summary>
        public byte Channel { get; set; }

        #region IAddress Members

        /// <summary>
        /// (Get) Name of Modbus protocol
        /// </summary>
        public string ProtocolName
        {
            get { return ProtocolType.ToString(); }
        }
        /// <summary>
        /// (Get) Type of used protocol. Constant value of <see cref="ProtocolType.Modbus"/>
        /// </summary>
        public ProtocolType ProtocolType
        {
            get { return ProtocolType.Modbus; }
        }

        #endregion
    }
}
