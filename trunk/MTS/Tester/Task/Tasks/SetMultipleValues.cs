using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    class SetMultipleValues : Task
    {
        /// <summary>
        /// Channels whose values we want to set
        /// </summary>
        private List<IDigitalOutput> outChannels = new List<IDigitalOutput>();
        /// <summary>
        /// Values that we want to write to channels
        /// </summary>
        private List<bool> outValues = new List<bool>();
        /// <summary>
        /// Add new channel to write particular value when this task get executed
        /// </summary>
        /// <param name="channel">Channel to write</param>
        /// <param name="value">Value to write</param>
        public void AddChannel(IDigitalOutput channel, bool value)
        {
            outChannels.Add(channel);
            outValues.Add(value);
        }

        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    goTo(ExState.Finalizing);
                    for (int i = 0; i < outChannels.Count; i++)
                        Output.WriteLine("Setting {0} to\t{1}", outChannels[i].Name, outValues[i]);
                    break;
                case ExState.Finalizing:
                    for (int i = 0; i < outChannels.Count; i++)
                        outChannels[i].Value = outValues[i];
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will write multiple values to channels
        /// </summary>
        public SetMultipleValues() { }

        #endregion
    }
}
