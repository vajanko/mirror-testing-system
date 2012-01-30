using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    class SetChannels : ChannelsTask<IDigitalOutput>
    {
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    foreach (var ch in channels)
                        Output.WriteLine("Setting {0} to\t{1}", ch.Channel.Name, ch.Value);
                    goTo(ExState.Finalizing);
                    break;
                case ExState.Finalizing:
                    foreach (var ch in channels)
                        ch.Channel.Value = ch.Value;
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
        public SetChannels()
        {
        }

        #endregion
    }
}
