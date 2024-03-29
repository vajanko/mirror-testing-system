﻿using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class HeatingFoilTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Time of measuring current on the spiral. This value should be initialized when test is being executed.
        /// </summary>
        private double testingTimeMeasured;

        /// <summary>
        /// Required duration of this task in milliseconds
        /// </summary>
        private readonly double testingTime;
        /// <summary>
        /// Required duration of this task
        /// </summary>
        private readonly DoubleParam testingTimeParam;

        #endregion

        /// <summary>
        /// This method is called repeatedly in a loop to execute this test. At this moment
        /// logic of the test execution is provided.
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
                    channels.HeatingFoilOn.Off();               // switch off spiral
                    Finish(time);
                    break;
                case ExState.Aborting:
                    channels.HeatingFoilOn.Off();               // switch off spiral
                    Finish(time);
                    break;
            }
        }

        /// <summary>
        /// Generate object holding result data for this task such as time of execution and results of 
        /// used parameters.
        /// </summary>
        /// <returns>Object describing all results of this task</returns>
        protected override TestResult getTestResult()
        {
            TestResult result = base.getTestResult();

            // we have been measuring time in milliseconds, now convert it back to parameter unit
            // in this state will be saved to database
            double time = convertBack(testingTimeParam, Units.Miliseconds, testingTimeMeasured);
            result.Params.Add(new ParamResult(testingTimeParam, time));

            return result;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of test task that will execute spiral heating by switching it on
        /// and measuring the current.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public HeatingFoilTest(Channels channels, TestValue testParam)
            : base(channels, testParam) 
        {
            // from test parameters get TestingTime item
            testingTimeParam = testParam.GetParam<DoubleParam>(ParamIds.TestingTime);

            // for measuring time we only use milliseconds
            testingTime = convert(testingTimeParam, Units.Miliseconds);
        }

        #endregion
    }
}
