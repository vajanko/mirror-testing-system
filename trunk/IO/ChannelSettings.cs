using System;
using System.Collections;
using System.Collections.Generic;

namespace MTS.IO
{
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
        public ChannelSettings(int capacity) : base(capacity) { }
    }
}
