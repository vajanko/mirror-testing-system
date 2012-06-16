using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class WaitForChannels : ChannelsTask<IDigitalInput>
    {
        /// <summary>
        /// Check if particular channels have required value and finish this task if so
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:  // start to check for a value
                    goTo(ExState.Measuring);
                    Output.WriteLine("Waiting for");
                    foreach (var ch in channels)
                        Output.WriteLine("{0} to be {1}", ch.Channel.Name, ch.Value);
                    break;
                case ExState.Measuring:     // wait for expected value on a all channels channel
                    if (channels.TrueForAll(ch => ch.Channel.Value == ch.Value))
                        exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }

            
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of a task that will wait for a specific values on a particular
        /// channels for ever
        /// </summary>
        public WaitForChannels() 
        {
        }

        #endregion
    }
}
