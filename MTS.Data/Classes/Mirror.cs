using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Data
{
    public partial class Mirror
    {
        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, (Types.MirrorType)Type);
        }
    }
}
