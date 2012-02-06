using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Data
{
    public partial class Mirror
    {
        /// <summary>
        /// Get string representation of mirror type
        /// </summary>
        /// <returns>String representation of mirror type</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, (Types.MirrorType)Type);
        }
    }
}
