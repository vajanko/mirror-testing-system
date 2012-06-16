using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO.Address
{
    /// <summary>
    /// Address of a channel in Dummy communication module
    /// </summary>
    public class DummyAddress : IAddress
    {
        #region IAddress Members

        /// <summary>
        /// (Get) Name of Dummy protocol
        /// </summary>
        public string ProtocolName { get { return ProtocolType.ToString(); } }
        /// <summary>
        /// (Get) Type of used protocol. Constant value of <see cref="ProtocolType.Dummy"/>
        /// </summary>
        public ProtocolType ProtocolType
        {
            get { return ProtocolType.Dummy; }
        }

        #endregion
    }
}
