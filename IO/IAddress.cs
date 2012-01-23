using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MTS.IO
{
    public interface IAddress
    {
        /// <summary>
        /// (Get) Name of the protocol that is using this address
        /// </summary>
        string ProtocolName { get; }

        /// <summary>
        /// (Get) Type of protocol that is using this address
        /// </summary>
        ProtocolType ProtocolType { get; }
    }
}
