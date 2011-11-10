using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace MTS.IO.Settings
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

        public ChannelSetting() { }
        public ChannelSetting(string id)
        {
            this.Id = id;
        }

        #endregion
    }

    public class ChannelSettingsCollection : IEnumerable<ChannelSetting>
    {
        private Dictionary<string, ChannelSetting> settings = new Dictionary<string, ChannelSetting>();

        public ChannelSetting GetSetting(string channelName)
        {
            return settings[channelName];
        }
        public void AddSetting(ChannelSetting setting)
        {
            settings[setting.Name] = setting;
        }

        #region IEnumerable<ChannelSetting> Members

        public IEnumerator<ChannelSetting> GetEnumerator()
        {
            return settings.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return settings.Values.GetEnumerator();
        }

        #endregion

        public ChannelSettingsCollection() { }
    }
}

