using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;

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

        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    maxMeasuredCurrent = double.MinValue;                   // initialize max and min
                    minMeasuredCurrent = double.MaxValue;                   // measured values
                    channels.DirectionLightOn.SwitchOn();                   // switch on direction light
                    blinkerOn = time;                                       // save time of light on
                    exState = ExState.BlinkerOn;                            // go to next state
                    break;
                case ExState.BlinkerOn:   // measure current
                    measureCurrent(time, channels.DirectionLightCurrent);   // measure current
                    if ((time - blinkerOn).TotalMilliseconds > lightingTime)// if lighting time elapsed
                    {
                        channels.DirectionLightOn.SwitchOff();              // switch off light
                        blinkerOff = time;                                  // save time of light off
                        blinksCount--;                                      // decrease one lighting period
                        exState = (blinksCount <= 0) ?                      // if no more periods should be executed
                            ExState.Finalizing : ExState.BlinkerOff;        // finish this test. otherwise switch
                    }                                                       // off blinker
                    break;
                case ExState.BlinkerOff:  // do not measure current
                    if ((time - blinkerOff).TotalMilliseconds > breakTime)  // if break time elapsed
                    {
                        channels.DirectionLightOn.SwitchOn();               // switch on light
                        blinkerOn = time;                                   // save time of lihgt on
                        exState = ExState.BlinkerOn;                        // go to next state
                    }   // after blinker is off olways come a period when blinker is on
                    break;
                case ExState.Finalizing:
                    channels.DirectionLightOn.SwitchOff();                  // switch off light
                    Finish(time, getTaskState());                           // finish test with right result
                    exState = ExState.None;                                 // do not update this task any more
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of test that will execute direction light test
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public BlinkerTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get LighteningTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.LighteningTime);
            if (iValue != null)     // it must be of type int
                lightingTime = iValue.IntValue * 1000;
            // from test parameters get BreakTime item
            iValue = testParam.GetParam<IntParam>(TestValue.BreakTime);
            if (iValue != null)     // it must be of type int
                breakTime = iValue.IntValue * 1000;
            // from test parameters get BLINK_COUNT item
            iValue = testParam.GetParam<IntParam>(TestValue.BlinkCount);
            if (iValue != null)     // it must be of type int
                blinksCount = iValue.IntValue;
        }

        #endregion
    }
}
