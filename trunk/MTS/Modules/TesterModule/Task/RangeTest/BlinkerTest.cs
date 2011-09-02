using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    sealed class BlinkerTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Time of blinker switched on. This is a testing parameter, not a measured value
        /// </summary>
        private int lightingTime = 0;
        /// <summary>
        /// Time of blinker switched off. This is a testing parameter, not a measured value
        /// </summary>
        private int breakTime = 0;
        /// <summary>
        /// Number of blinks. Blinker is cyclically switched on ond off.
        /// </summary>
        private int blinksCount = 0;
        /// <summary>
        /// Time when blinker was switched on. This is measured value.
        /// </summary>
        private TimeSpan blinkerOn;
        /// <summary>
        /// Time when blinker was switched off. This is measured value.
        /// </summary>
        private TimeSpan blinkerOff;

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
                    maxMeasuredCurrent = double.MinValue;
                    minMeasuredCurrent = double.MaxValue;
                    channels.DirectionLightOn.Value = true;                 // switch on direction light
                    blinkerOn = time;                                       // save time of light on
                    currentState = State.BlinkerOn;                         // go to next state
                    break;
                case State.BlinkerOn:   // measure current
                    measureCurrent(time, channels.DirectionLightCurrent);   // measure current
                    if ((time - blinkerOn).TotalMilliseconds > lightingTime)// if lighting time elapsed
                    {
                        channels.DirectionLightOn.Value = false;            // switch of light
                        blinkerOff = time;                                  // save time of light off
                        blinksCount--;                                      // decrease one lighting period
                        if (blinksCount <= 0)                               // if no more periods should be executed
                            currentState = State.Finalizing;                // finish: go to next state
                        else
                            currentState = State.BlinkerOff;                // switch off: go to next state
                    }
                    break;
                case State.BlinkerOff:  // do not measure current
                    if ((time - blinkerOff).TotalMilliseconds > breakTime)  // if break time elapsed
                    {
                        channels.DirectionLightOn.Value = true;             // switch on light
                        blinkerOn = time;                                   // save time of lihgt on
                        currentState = State.BlinkerOn;                     // go to next state
                    }
                    break;
                case State.Finalizing:
                    Finish(time, getTaskState());                           // finish test with right result
                    currentState = State.None;                              // do not update this task any more
                    break;
            }
        }

        #region State

        private State currentState = State.None;
        private enum State
        {
            Initializing,
            BlinkerOn,
            BlinkerOff,
            Finalizing,
            Aborting,
            None
        }

        #endregion

        #region Constructors

        public BlinkerTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            ParamCollection param = testParam.Parameters;
            IntParamValue iValue;
            // from test parameters get LIGHTENING_TIME item
            if (param.ContainsKey(ParamDictionary.LIGHTENING_TIME))
            {   // it must be int type value
                iValue = param[ParamDictionary.LIGHTENING_TIME] as IntParamValue;
                if (iValue != null)     // param is of other type then int
                    lightingTime = iValue.Value;
            }
            // from test parameters get BREAK_TIME item
            if (param.ContainsKey(ParamDictionary.BREAK_TIME))
            {   // it must be int type value
                iValue = param[ParamDictionary.BREAK_TIME] as IntParamValue;
                if (iValue != null)     // param is of other type then int
                    breakTime = iValue.Value;
            }
            // from test parameters get BLINK_COUNT item
            if (param.ContainsKey(ParamDictionary.BLINK_COUNT))
            {   // it must be int type value
                iValue = param[ParamDictionary.BLINK_COUNT] as IntParamValue;
                if (iValue != null)     // param is of other type then int
                    blinksCount = iValue.Value;
            }
        }

        #endregion
    }
}
