using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
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

        public override void Initialize(TimeSpan time)
        {
            base.Initialize(time);
            Output.WriteLine("Setting channel \"{0}\" to \"{1}\"", Name, value);
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            // write value and finish
            channel.Value = value;
            Finish(time, TaskState.Completed);
            base.UpdateOutputs(time);
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
