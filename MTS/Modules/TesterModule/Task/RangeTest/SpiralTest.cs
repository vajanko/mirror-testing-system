using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;

namespace MTS.TesterModule
{
    sealed class SpiralTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Required duration in miliseconds of spiral to be swtiched on during this test
        /// </summary>
        private int testingTime;

        #endregion

        /// <summary>
        /// This method is called repeatidly in a loop to execute this test. At this moment
        /// logic of the test excution is provided.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    minMeasuredCurrent = double.MaxValue;                   // initialize max and min
                    maxMeasuredCurrent = double.MinValue;                   // measured values
                    channels.HeatingFoilOn.SwitchOn();                      // switch on spiral
                    exState = ExState.Measuring;                            // go to next state
                    break;
                case ExState.Measuring:
                    measureCurrent(time, channels.HeatingFoilCurrent);      // measure spiral current
                    if (Duration.TotalMilliseconds > testingTime)           // if testing time elapsed
                        exState = ExState.Finalizing;                       // go to next state
                    break;
                case ExState.Finalizing:
                    channels.HeatingFoilOn.SwitchOff();                     // swtich off spiral
                    Finish(time, getTaskState());                           // set result state
                    exState = ExState.None;                                 // stop to update this test
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of test task that will execute spiral heating by switchig it on
        /// and measuring the current.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public SpiralTest(Channels channels, TestValue testParam)
            : base(channels, testParam) 
        {
            // from test parameters get TestingTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.TestingTime);
            if (iValue != null)     // it must be of type int
                testingTime = iValue.IntValue;
        }

        #endregion
    }
}
