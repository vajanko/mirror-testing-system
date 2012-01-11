using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class SpiralTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Time of measuring current on the spiral. This value should be initialized when test is beging executed.
        /// </summary>
        private double testingTimeMeasured;

        /// <summary>
        /// Required duration of this task in miliseconds
        /// </summary>
        private readonly double testingTime;
        /// <summary>
        /// Required duration of this task
        /// </summary>
        private readonly DoubleParam testingTimeParam;

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
                    minCurrentMeasured = double.MaxValue;             // initialize max and min
                    maxCurrentMeasured = double.MinValue;             // measured values
                    testingTimeMeasured = 0;

                    channels.HeatingFoilOn.On();                      // switch on spiral
                    StartWatch(time);                                 // start measuring time                                      
                    goTo(ExState.Measuring);                          // start measuring
                    break;
                case ExState.Measuring:
                    measureCurrent(channels.HeatingFoilCurrent);      // measure spiral current
                    testingTimeMeasured = TimeElapsed(time);          // measure time
                    if (testingTimeMeasured >= testingTime)           // if testing time elapsed
                        goTo(ExState.Finalizing);                     // go to next state                     
                    break;
                case ExState.Finalizing:
                    channels.HeatingFoilOn.SwitchOff();               // swtich off spiral
                    Finish(time);
                    break;
                case ExState.Aborting:
                    channels.HeatingFoilOn.SwitchOff();               // swtich off spiral
                    Finish(time);
                    break;
            }
        }

        /// <summary>
        /// Generate object holding result data for this task such as time of execution and results of 
        /// used parameters.
        /// </summary>
        /// <returns>Object describing all results of this task</returns>
        protected override TaskResult getResult()
        {
            TaskResult result = base.getResult();

            // we have been measuring time in miliseconds, now convert it back to parameter unit
            // in this state will be saved to database
            double time = convertBack(testingTimeParam, Units.Miliseconds, testingTimeMeasured);
            result.Params.Add(new ParamResult(testingTimeParam, time));

            return result;
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
            testingTimeParam = testParam.GetParam<DoubleParam>(TestValue.TestingTime);

            // for measuring time we only use miliseconds
            testingTime = convert(testingTimeParam, Units.Miliseconds);
        }

        #endregion
    }
}
