using System;
using System.Collections.Generic;

using MTS.IO;

namespace MTS.TesterModule
{
    class SetMultipleValues : Task
    {
        /// <summary>
        /// Channels whos values we want to set
        /// </summary>
        private List<IDigitalOutput> outChannels = new List<IDigitalOutput>();
        /// <summary>
        /// Values that we want to write to channels
        /// </summary>
        private List<bool> outValues = new List<bool>();
        /// <summary>
        /// Add new channel to write particular value when this taks get executed
        /// </summary>
        /// <param name="channel">Channel to write</param>
        /// <param name="value">Value to write</param>
        public void AddChannel(IDigitalOutput channel, bool value)
        {
            outChannels.Add(channel);
            outValues.Add(value);
        }

        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    for (int i = 0; i < outChannels.Count; i++)
                        outChannels[i].Value = outValues[i];
                    Finish(time, TaskState.Completed);
                    break;
                case ExState.Aborting:
                    Finish(time, TaskState.Aborted);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will write multiple values to channels
        /// </summary>
        public SetMultipleValues(Channels channels)
            : base(channels) { }

        #endregion
    }
}
