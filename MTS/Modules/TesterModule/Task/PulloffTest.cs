using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    class PulloffTest : TestTask
    {
        private int testingTime;

        public override void Initialize(System.TimeSpan time)
        {
            // start to pull-off
            channels.MoveSuckerUp.Value = false;
            channels.MoveSuckerDown.Value = true;

            base.Initialize(time);
        }
        public override void Update(TimeSpan time)
        {
            if (Duration.TotalMilliseconds > testingTime)
                Finish(time, TaskState.Completed);

            if (channels.IsSuckerDown.Value)
            {
                Output.WriteLine("Glass was pulled off");
                Finish(time, TaskState.Failed);
            }
            base.Update(time);
        }

        #region Constructors

        public PulloffTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {

            ParamCollection param = testParam.Parameters;
            IntParamValue iValue;
            // from test parameters get TestingTime item
            if (param.ContainsKey(ParamDictionary.TESTING_TIME))
            {  // it must be int type value
                iValue = param[ParamDictionary.TESTING_TIME] as IntParamValue;
                if (iValue != null)     // param is of other type then int
                    testingTime = iValue.Value;
            }
        }

        #endregion
    }

}
