using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;

namespace MTS.IO
{
    public class ChannelSetting
    {
        [Category("Channel")]
        [DisplayName("Id")]
        public string Id { get; set; }

        [Category("Channel")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Category("Channel")]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Category("Channel")]
        [DisplayName("RawLow")]
        public int RawLow { get; set; }

        [Category("Channel")]
        [DisplayName("RawHigh")]
        public int RawHigh { get; set; }

        [Category("Channel")]
        [DisplayName("RealLow")]
        public double RealLow { get; set; }

        [Category("Channel")]
        [DisplayName("RealHigh")]
        public double RealHigh { get; set; }

        #region Constructors

        public ChannelSetting() { }
        public ChannelSetting(string id)
        {
            this.Id = id;
        }

        #endregion
    }

    public class ChannelSettings : CollectionBase
    {
        public ChannelSetting this[int index]
        {
            get { return (ChannelSetting)List[index]; }
        }
        public void Add(ChannelSetting channel)
        {
            List.Add(channel);
        }
        public void Remove(ChannelSetting channel)
        {
            List.Remove(channel);
        }

        public ChannelSetting GetSetting(string channelName)
        {
            channelName = channelName.ToLower();
            foreach (ChannelSetting channel in List)
                if (channel.Id.ToLower() == channelName)
                    return channel;
            return null;
        }

        public ChannelSettings() { }
    }
}

