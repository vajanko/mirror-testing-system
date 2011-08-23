using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    sealed class SpiralTest : RangeCurrentTest
    {
        /// <summary>
        /// Duration of this test
        /// </summary>
        private int testingTime;

        public override void Update(TimeSpan time)
        {
            // measure time - end if enought time has elapsed
            if (Duration.TotalMilliseconds > testingTime)
                Finish(time, TaskState.Completed);  // time of this test has elapsed - finish it
            else
                base.Update(time);                  // measure current
        }

        #region Constructors

        public SpiralTest(Channels channels, TestValue testParam)
            : base(channels, testParam) 
        {
            ControlChannel = channels.HeatingFoilOn;
            CurrentChannel = channels.HeatingFoilCurrent;

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
