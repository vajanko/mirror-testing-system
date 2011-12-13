using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.TesterModule
{
    sealed class SpiralTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Required duration in seconds of spiral to be swtiched on during this test
        /// </summary>
        private double testingTime;
        //private TimeSpan start;

        #endregion

        /// <summary>
        /// This method is called repeatidly in a loop to execute this test. At this moment
        /// logic of the test excution is provided.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    minMeasuredCurrent = double.MaxValue;             // initialize max and min
                    maxMeasuredCurrent = double.MinValue;             // measured values
                    channels.HeatingFoilOn.SwitchOn();                // switch on spiral
                    StartWatch(time);                                 // start measuring time                                      
                    goTo(ExState.Measuring);                          // start measuring
                    break;
                case ExState.Measuring:
                    measureCurrent(channels.HeatingFoilCurrent);      // measure spiral current
                    if (TimeElapsed(time) > testingTime)              // if testing time elapsed
                        goTo(ExState.Finalizing);                     // go to next state                     
                    break;
                case ExState.Finalizing:
                    channels.HeatingFoilOn.SwitchOff();               // swtich off spiral
                    Finish(time);                                     //
                    break;
                case ExState.Aborting:
                    channels.HeatingFoilOn.SwitchOff();               // swtich off spiral
                    Finish(time);
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
            DoubleParam dValue = testParam.GetParam<DoubleParam>(TestValue.TestingTime);
            if (dValue != null)     // it must be of type int
                testingTime = dValue.DoubleValue;
        }

        #endregion
    }
}
