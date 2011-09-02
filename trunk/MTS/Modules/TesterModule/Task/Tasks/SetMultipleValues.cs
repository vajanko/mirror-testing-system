using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    class SetMultipleValues : Task
    {
        private List<IDigitalOutput> outChannels = new List<IDigitalOutput>();
        private List<bool> outValues = new List<bool>();

        public void AddChannel(IDigitalOutput channel, bool value)
        {
            outChannels.Add(channel);
            outValues.Add(value);
        }

        public override void Initialize(System.TimeSpan time)
        {
            base.Initialize(time);
            string msg = "Setting multiple values:";
            for (int i = 0; i < outChannels.Count; i++)
                msg += string.Format("\n\t{0} <- {1}", outChannels[i].Name, outValues[i]);
            Output.WriteLine(msg);
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            for (int i = 0; i < outChannels.Count; i++)
                outChannels[i].Value = outValues[i];
            Finish(time, TaskState.Completed);
            base.UpdateOutputs(time);
        }
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
        /// 
        /// </summary>
        public SetMultipleValues(Channels channels)
            : base(channels) 
        {
        }

        #endregion
    }
}
