using System;
using System.Collections.Generic;

using MTS.IO;

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

        /// <summary>
        /// Check if particular channel has required value and finish this task if so
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:  // start to check for a value
                    exState = ExState.Measuring;
                    break;
                case ExState.Measuring:     // wait for expected value on a particular channel
                    if (channel.Value == value)
                        exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    Finish(time, TaskState.Completed);
                    break;
                case ExState.Aborting:
                    Finish(time, TaskState.Aborted);
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
