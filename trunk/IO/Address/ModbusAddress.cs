using System;
using System.Globalization;
using System.Xml.Linq;

namespace MTS.IO.Address
{
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

        #region IXmlFormatable Members

        /// <summary>
        /// Save Modbus address to xml format
        /// </summary>
        /// <returns>Instance of element in xml format containing address data</returns>
        public XElement ToXml()
        {
            XElement addr = new XElement("address");
            addr.Add(new XElement("slot", Slot.ToString(CultureInfo.InvariantCulture)));
            addr.Add(new XElement("channel", Channel.ToString(CultureInfo.InvariantCulture)));
            return addr;
        }
        /// <summary>
        /// Initialize Modbus address from given xml format
        /// </summary>
        /// <param name="data">Xml data to be initialized address from</param>
        public void FromXml(XElement data)
        {
            try
            {
                XElement slot = data.Element("group");
                XElement channel = data.Element("offset");
                Slot = byte.Parse(slot.Value, CultureInfo.InvariantCulture);
                Channel = byte.Parse(channel.Value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new AddressException(string.Format(Resource.AddrParsingFailedMsg, ProtocolName), ex) { ProtocolName = this.ProtocolName };
            }
        }

        #endregion
    }
}
