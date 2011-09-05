using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    sealed class SpiralTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Required duration of this test
        /// </summary>
        private int testingTime;

        #endregion

        public override void Initialize(TimeSpan time)
        {
            currentState = State.Initializing;
            base.Initialize(time);
        }
        public override void Update(TimeSpan time)
        {
            switch (currentState)
            {
                case State.Initializing:
                    minMeasuredCurrent = double.MaxValue;                   // initialize max and min
                    maxMeasuredCurrent = double.MinValue;                   // measured values
                    channels.HeatingFoilOn.Value = true;                    // switch on spiral
                    currentState = State.Measuring;                         // go to next state
                    break;
                case State.Measuring:
                    measureCurrent(time, channels.HeatingFoilCurrent);      // measure spiral current
                    if (Duration.TotalMilliseconds > testingTime)           // if testing time elapsed
                    {
                        channels.HeatingFoilOn.Value = false;               // switch off spiral
                        currentState = State.Finalizing;                    // go to next state
                    }
                    break;
                case State.Finalizing:
                    Finish(time, getTaskState());                           // set result state
                    currentState = State.None;                              // stop to update this test
                    break;
            }
        }

        #region State

        /// <summary>
        /// Current state of the task execution
        /// </summary>
        private State currentState = State.None;
        private enum State
        {
            /// <summary>
            /// Test is being initialized
            /// </summary>
            Initializing,
            /// <summary>
            /// Test is measuring current
            /// </summary>
            Measuring,
            /// <summary>
            /// Test is being finalized
            /// </summary>
            Finalizing,
            /// <summary>
            /// Test is being aborted
            /// </summary>
            Aborting,
            /// <summary>
            /// Unspecified state of test, nothig is executed
            /// </summary>
            None
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of test task that will execute spiral heating
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public SpiralTest(Channels channels, TestValue testParam)
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
