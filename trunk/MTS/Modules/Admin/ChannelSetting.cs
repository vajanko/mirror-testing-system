using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Admin
{
    public class ChannelSetting
    {
        public string Id { get; private set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int RawLow { get; set; }
        public int RawHigh { get; set; }
        public double RealLow { get; set; }
        public double RealHigh { get; set; }

        #region Constructors

        public ChannelSetting(string id)
        {
            this.Id = id;
        }

        #endregion
    }
}
