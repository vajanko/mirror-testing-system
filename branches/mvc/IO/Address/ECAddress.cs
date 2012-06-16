using System;
using System.Globalization;
using System.Xml.Linq;
using MTS.IO;
using TwinCAT.Ads;

namespace MTS.IO.Address
{
    /// <summary>
    /// Address of a channel in EtherCAT communication module
    /// </summary>
    public class ECAddress : IAddress
    {
        /// <summary>
        /// (Get/Set) Identifier of group of channels this one belongs to. Group is a view on some collection of
        /// channels. For example one group may be Inputs or Output. Default value is 
        /// <see cref="AdsReservedIndexGroups.SymbolValueByHandle"/>
        /// </summary>
        public int IndexGroup { get; set; }
        /// <summary>
        /// (Get/Set) Address of this channel inside group identified by <see cref="IndexGroup"/>
        /// </summary>
        public int IndexOffset { get; set; }

        /// <summary>
        /// (Get/Set) Full name of this channel. By this identifier access to TwinCAT IO server to this channel
        /// if possible. In TwinCAT system manager also called - Server Symbol Name
        /// </summary>
        public string FullName { get; set; }

        #region IAddress Members

        /// <summary>
        /// (Get) Name of EtherCAT protocol
        /// </summary>
        public string ProtocolName
        {
            get { return ProtocolType.ToString();  }
        }
        /// <summary>
        /// (Get) Type of used protocol. Constant value of <see cref="ProtocolType.EtherCAT"/>
        /// </summary>
        public ProtocolType ProtocolType
        {
            get { return ProtocolType.EtherCAT; }
        }

        #endregion

        /// <summary>
        /// Get string representation of EtherCAT address containing <see cref="IndexGroup"/> and <see cref="IndexOffset"/>
        /// </summary>
        /// <returns>String representation of EtherCAT address</returns>
        public override string ToString()
        {
            return string.Format(Resource.EtherCATAddrStr, IndexGroup, IndexOffset);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ECAddress"/> referencing value of EtherCAT communication module
        /// </summary>
        public ECAddress()
        {
            IndexGroup = (int)AdsReservedIndexGroups.SymbolValueByHandle;
        }

        #endregion
    }
}
