using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Tester.Result;

namespace MTS.Tester
{
    class SetValue : Task
    {
        /// <summary>
        /// Channel whos value we want to set
        /// </summary>
        private IDigitalOutput channel;
        /// <summary>
        /// Value that we want to write to channel
        /// </summary>
        private bool value;

        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    exState = ExState.Finalizing;
                    Output.WriteLine("Setting {0} to {1}", Name, value);
                    break;
                case ExState.Finalizing:
                    channel.Value = value;
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of a task that will wait for a specific value on a particular
        /// channel for ever
        /// </summary>
        /// <param name="channel">Channel to wait for specific value on</param>
        /// <param name="value">Value we are waiting for</param>
        public SetValue(Channels channels, IDigitalOutput channel, bool value)
            : base(channels) 
        {
            this.channel = channel;
            this.value = value;
            Name = channel.Name;
        }

        #endregion
    }
}
