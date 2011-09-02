using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    class WaitForValue : Task
    {
        /// <summary>
        /// Channel whos value we are observing
        /// </summary>
        private IDigitalInput channel;
        /// <summary>
        /// Value that we expect on channel
        /// </summary>
        private bool value;

        public override void Initialize(System.TimeSpan time)
        {
            base.Initialize(time);
            Output.WriteLine("Waiting for \"{0}\" to be {1}", Name, value);
        }
        public override void Update(TimeSpan time)
        {
            // wait for expected value on a particular channel
            if (channel.Value == value)
                Finish(time, TaskState.Completed);
            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            Output.WriteLine("Channel \"{0}\" is {1}", Name, channel.Value);
            base.Finish(time, state);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of a task that will wait for a specific value on a particular
        /// channel for ever
        /// </summary>
        /// <param name="channel">Channel to wait for specific value on</param>
        /// <param name="value">Value we are waiting for</param>
        public WaitForValue(Channels channels, IDigitalInput channel, bool value)
            : base(channels) 
        {
            this.channel = channel;
            this.value = value;
            Name = channel.Name;
        }

        #endregion
    }
}
