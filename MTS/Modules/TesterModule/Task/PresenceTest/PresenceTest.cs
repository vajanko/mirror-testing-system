using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public class PresenceTest : TestTask
    {
        protected IDigitalInput PresenceChannel;
        private bool shouldBePresent;

        public override void Update(TimeSpan time)
        {
            TaskState state = TaskState.Passed;
            if (PresenceChannel.Value != shouldBePresent)
                state = TaskState.Failed;

            if (PresenceChannel.Value)
                Output.WriteLine("\"{0}\" test result: Is present", Name);
            else Output.WriteLine("\"{0}\" test result: Is not present", Name);

            Finish(time, state);

            base.Update(time);
        }

        #region Constructors

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
