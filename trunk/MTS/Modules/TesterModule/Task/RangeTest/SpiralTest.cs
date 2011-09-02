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
        /// Duration of this test
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

        private State currentState = State.None;
        private enum State
        {
            Initializing,
            Measuring,
            Finalizing,
            Aborting,
            None
        }

        #endregion

        #region Constructors

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
