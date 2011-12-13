using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.TesterModule
{
    sealed class BlinkerTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Time (in seconds) of blinker switched on. This is a testing parameter, not a measured value
        /// </summary>
        private double lightingTime = 0;
        /// <summary>
        /// Time (in seconds) of blinker switched off. This is a testing parameter, not a measured value
        /// </summary>
        private double breakTime = 0;
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

        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    maxMeasuredCurrent = double.MinValue;                   // initialize max and min
                    minMeasuredCurrent = double.MaxValue;                   // measured values
                    channels.DirectionLightOn.SwitchOn();                   // switch on direction light
                    StartWatch(time);                                       // start measuring time of light on
                    goTo(ExState.BlinkerOn);                                // go to next state
                    Output.WriteLine("Switchig direction light on");
                    break;
                case ExState.BlinkerOn:   // measure current
                    measureCurrent(channels.DirectionLightCurrent);         // measure current
                    if (TimeElapsed(time) >= lightingTime)                   // if lighting time elapsed
                    {
                        channels.DirectionLightOn.SwitchOff();              // switch off light
                        StartWatch(time);                                   // start to measure time of light off
                        blinksCount--;                                      // decrease one lighting period
                        goTo(blinksCount <= 0 ? ExState.Finalizing : ExState.BlinkerOff);
                        if (exState == ExState.BlinkerOff)
                            Output.WriteLine("Switching direction light off");
                    }                                                       // off blinker
                    break;
                case ExState.BlinkerOff:  // do not measure current
                    if (TimeElapsed(time) >= breakTime)                     // if break time elapsed
                    {
                        channels.DirectionLightOn.SwitchOn();               // switch on light
                        StartWatch(time);                                   // start to measure time of light on
                        goTo(ExState.BlinkerOn);
                        Output.WriteLine("Switchig direction light on");
                    }   // after blinker is off always come a period when blinker is on
                    break;
                case ExState.Finalizing:
                    channels.DirectionLightOn.SwitchOff();                  // switch off light
                    Finish(time);                                           // finish and do not update any more
                    Output.WriteLine("Switching direction light off");
                    break;
                case ExState.Aborting:
                    channels.DirectionLightOn.SwitchOff();
                    Finish(time);
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
            DoubleParam dValue = testParam.GetParam<DoubleParam>(TestValue.LighteningTime);
            if (dValue != null)     // it must be of type int
                lightingTime = dValue.DoubleValue * 1000;   // convert to miliseconds
            // from test parameters get BreakTime item
            dValue = testParam.GetParam<DoubleParam>(TestValue.BreakTime);
            if (dValue != null)     // it must be of type int
                breakTime = dValue.DoubleValue * 1000;      // convert to miliseconds
            // from test parameters get BlinksTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.BlinkCount);
            if (dValue != null)     // it must be of type int
                blinksCount = iValue.IntValue;
        }

        #endregion
    }
}
