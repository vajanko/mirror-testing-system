using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public class PresenceTest : TestTask
    {
        /// <summary>
        /// Channel on which presence of particular mirror component is signalized
        /// </summary>
        protected IDigitalInput PresenceChannel;
        /// <summary>
        /// Value indicating if mirror component should be present
        /// </summary>
        private bool shouldBePresent;

        /// <summary>
        /// Check for presence of particular mirror component
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(TimeSpan time)
        {
            // decide if test passed of failed
            TaskState state = TaskState.Passed;
            if (PresenceChannel.Value != shouldBePresent)
                state = TaskState.Failed;
            // show result to user
            if (PresenceChannel.Value)
                Output.WriteLine("\"{0}\" test result: Is present", Name);
            else Output.WriteLine("\"{0}\" test result: Is not present", Name);

            Finish(time, state);

            base.Update(time);
        }

        #region Constructors

        /// <summary>
        /// Create a new instace of test that will check for presence of particular mirror component
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        /// <param name="channel">Channel on which the presnece is sinalized</param>
        public PresenceTest(Channels channels, TestValue testParam, IDigitalInput channel)
            : base(channels, testParam)
        {
            PresenceChannel = channel;

            ParamCollection param = testParam.Parameters;
            BoolParamValue bValue;
            // from test parameters get TestingTime item
            if (param.ContainsKey(ParamDictionary.TEST_PRESENCE))
            {  // it must be bool type value
                bValue = param[ParamDictionary.TEST_PRESENCE] as BoolParamValue;
                if (bValue != null)     // otherwise param is of other type then bool
                    shouldBePresent = bValue.Value;
            }
        }

        #endregion
    }
}
