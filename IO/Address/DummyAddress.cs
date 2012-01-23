using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO.Address
{
    public class DummyAddress : IAddress
    {
        #region IAddress Members

        public string ProtocolName { get { return ProtocolType.ToString(); } }

        public ProtocolType ProtocolType
        {
            get { return ProtocolType.Dummy; }
        }

        #endregion
    }
}
