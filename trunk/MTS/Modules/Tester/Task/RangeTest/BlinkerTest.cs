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
                    Output.WriteLine("Switchig direction light on");
                    break;
                case ExState.BlinkerOn:   // measure current
                    measureCurrent(time, channels.DirectionLightCurrent);   // measure current
                    if ((time - blinkerOn).TotalSeconds > lightingTime)// if lighting time elapsed
                    {
                        channels.DirectionLightOn.SwitchOff();              // switch off light
                        blinkerOff = time;                                  // save time of light off
                        blinksCount--;                                      // decrease one lighting period
                        exState = (blinksCount <= 0) ?                      // if no more periods should be executed
                            ExState.Finalizing : ExState.BlinkerOff;        // finish this test. otherwise switch
                        if (exState == ExState.BlinkerOff)
                            Output.WriteLine("Switching direction light off");
                    }                                                       // off blinker
                    break;
                case ExState.BlinkerOff:  // do not measure current
                    if ((time - blinkerOff).TotalSeconds > breakTime)  // if break time elapsed
                    {
                        channels.DirectionLightOn.SwitchOn();               // switch on light
                        blinkerOn = time;                                   // save time of lihgt on
                        exState = ExState.BlinkerOn;                        // go to next state
                        Output.WriteLine("Switchig direction light on");
                    }   // after blinker is off always come a period when blinker is on
                    break;
                case ExState.Finalizing:
                    channels.DirectionLightOn.SwitchOff();                  // switch off light
                    Finish(time, getTaskState());                           // finish test with right result
                    exState = ExState.None;                                 // do not update this task any more
                    Output.WriteLine("Switching direction light off");
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
                lightingTime = dValue.DoubleValue;
            // from test parameters get BreakTime item
            dValue = testParam.GetParam<DoubleParam>(TestValue.BreakTime);
            if (dValue != null)     // it must be of type int
                breakTime = dValue.DoubleValue;
            // from test parameters get BlinksTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.BlinkCount);
            if (dValue != null)     // it must be of type int
                blinksCount = iValue.IntValue;
        }

        #endregion
    }
}
