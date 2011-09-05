using System;
using System.Collections.Generic;

using MTS.AdminModule;

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
        /// <summary>
        /// Show values we are going to write
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Initialize(System.TimeSpan time)
        {
            base.Initialize(time);
            string msg = "Setting multiple values:";
            for (int i = 0; i < outChannels.Count; i++)
                msg += string.Format("\n\t{0} <- {1}", outChannels[i].Name, outValues[i]);
            Output.WriteLine(msg);
        }
        /// <summary>
        /// Write values to channels
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void UpdateOutputs(TimeSpan time)
        {
            for (int i = 0; i < outChannels.Count; i++)
                outChannels[i].Value = outValues[i];
            Finish(time, TaskState.Completed);
            base.UpdateOutputs(time);
        }
        /// <summary>
        /// Show what we have written
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="state">Final state of this task</param>
        public override void Finish(TimeSpan time, TaskState state)
        {
            string msg = "Multiple values are set:";
            for (int i = 0; i < outChannels.Count; i++)
                msg += string.Format("\n\t{0} == {1}", outChannels[i].Name, outChannels[i].Value);
            Output.WriteLine(msg);

            base.Finish(time, state);
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
