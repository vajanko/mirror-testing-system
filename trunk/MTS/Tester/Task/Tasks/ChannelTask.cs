using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;

namespace MTS.Tester
{
    abstract class ChannelsTask<TChannel> : Task where TChannel : IChannel
    {
        /// <summary>
        /// Channels and their values we want to set or we expect it in this task
        /// </summary>
        protected List<ChannelData> channels = new List<ChannelData>();

        /// <summary>
        /// Add new channel to write particular value when this task get executed
        /// </summary>
        /// <param name="channel">Channel to write</param>
        /// <param name="value">Value to write</param>
        public void AddChannel(TChannel channel, bool value)
        {
            channels.Add(new ChannelData(channel, value));
        }

        protected class ChannelData
        {
            public bool Value;
            public TChannel Channel;

            public ChannelData(TChannel channel, bool value)
            {
                Channel = channel;
                Value = value;
            }
        }
    }
}
