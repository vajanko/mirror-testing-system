using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;
using MTS.Editor;

namespace MTS.Tester
{
    abstract class ChannelTest<TChannel> : TestTask
    {
        /// <summary>
        /// Channels and their values we want to set or we expect it in this task
        /// </summary>
        protected List<ChannelData> myChannels = new List<ChannelData>();

        /// <summary>
        /// Add new channel to write particular value when this task get executed
        /// </summary>
        /// <param name="channel">Channel to write</param>
        /// <param name="value">Value to write</param>
        public void AddChannel(TChannel channel, BoolParam value)
        {
            myChannels.Add(new ChannelData(channel, value));
        }

        public ChannelTest(Channels channels, TestValue testParam)
            : base(channels, testParam) { }

        protected class ChannelData
        {
            public BoolParam Value;
            public TChannel Channel;

            public ChannelData(TChannel channel, BoolParam value)
            {
                Channel = channel;
                Value = value;
            }
        }
    }
}
