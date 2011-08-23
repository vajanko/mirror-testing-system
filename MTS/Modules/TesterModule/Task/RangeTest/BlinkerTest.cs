using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    sealed class BlinkerTest : RangeCurrentTest
    {
        #region Fields

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
            blinkerOn = time;   // test is starting with blinker on
            blinksCount--;      // first blink has started just now
            base.Initialize(time);    // this will switch on blinker
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            if (ControlChannel.Value)
            {   // blinker is on
                if ((time - blinkerOn).TotalMilliseconds > lightingTime)
                {   // its time has elapsed
                    if (blinksCount == 0)   // no other blinks should be executed
                        Finish(time, TaskState.Completed);  // then finish this task
                    else
                    {   // more blinks should be executed
                        ControlChannel.Value = false;    // switch off light
                        blinksCount--;
                        blinkerOff = time;
                    }
                }
            }
            else
            {   // blinker is off
                if ((time - blinkerOff).TotalMilliseconds > breakTime)
                {   // its time has elapsed
                    ControlChannel.Value = true;      // switch on light
                    blinkerOn = time;
                }
            }
            base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {
            // only measure current when blinker is on
            if (ControlChannel.Value)
                base.Update(time);
        }

        #region Constructors

        public BlinkerTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            ControlChannel = channels.DirectionLightOn;
            CurrentChannel = channels.DirectionLightCurrent;

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
