using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    class WaitForValue : Task
    {
        /// <summary>
        /// Channel whose value we are observing
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
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:  // start to check for a value
                    goTo(ExState.Measuring);
                    Output.WriteLine("Waiting for {0} to be {1}", Name, value);
                    break;
                case ExState.Measuring:     // wait for expected value on a particular channel
                    if (channel.Value == value)
                        exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    Output.WriteLine("Aborting waiting for {0} to be {1}", Name, value);
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
