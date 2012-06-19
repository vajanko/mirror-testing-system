using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class DirectionLightTest : RangeCurrentTest
    {
        #region Private fields

        /// <summary>
        /// Number of blinks that are not executed yet. This value should be initialized when test is being executed.
        /// </summary>
        private int blinksCountMeasured;

        /// <summary>
        /// Time of blinker switched on
        /// </summary>
        private readonly DoubleParam lighteningTimeParam;
        /// <summary>
        /// Time of blinker switched off
        /// </summary>
        private readonly DoubleParam breakTimeParam;
        /// <summary>
        /// Number of blinks. Blinker is cyclically switched on and off.
        /// </summary>
        private readonly IntParam blinksCountParam;
        /// <summary>
        /// Time of blinker switched on in milliseconds
        /// </summary>
        private readonly double lightingTime;
        /// <summary>
        /// Time of blinker switched off in milliseconds
        /// </summary>
        private readonly double breakTime;
        /// <summary>
        /// Number of blinks. Blinker is cyclically switched on and off.
        /// </summary>
        private readonly int blinksCount;

        #endregion

        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    maxCurrentMeasured = double.MinValue;                   // initialize measured variable
                    minCurrentMeasured = double.MaxValue;
                    blinksCountMeasured = 0;

                    channels.DirectionLightOn.On();                         // switch on direction light
                    StartWatch(time);                                       // start measuring time of light on
                    goTo(ExState.On);                                // go to next state
                    Output.WriteLine("Switching direction light on");
                    break;
                case ExState.On:   // measure current
                    measureCurrent(channels.DirectionLightCurrent);         // measure current
                    if (TimeElapsed(time) >= lightingTime)                  // if lighting time elapsed
                    {
                        channels.DirectionLightOn.Off();              // switch off light
                        StartWatch(time);                                   // start to measure time of light off
                        ++blinksCountMeasured;                              // increase one lighting period
                        goTo(blinksCountMeasured < blinksCount ? 
                            ExState.Off : ExState.Finalizing);
                        if (exState == ExState.Off)
                            Output.WriteLine("Switching direction light off");
                    }                                                       // off blinker
                    break;
                case ExState.Off:  // do not measure current
                    if (TimeElapsed(time) >= breakTime)                     // if break time elapsed
                    {
                        channels.DirectionLightOn.On();               // switch on light
                        StartWatch(time);                                   // start to measure time of light on
                        goTo(ExState.On);
                        Output.WriteLine("Switching direction light on");
                    }   // after blinker is off always come a period when blinker is on
                    break;
                case ExState.Finalizing:
                    channels.DirectionLightOn.Off();                  // switch off light
                    Finish(time);                                           // finish and do not update any more
                    Output.WriteLine("Switching direction light off");
                    break;
                case ExState.Aborting:
                    channels.DirectionLightOn.Off();
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

            // these values have been used but not output has been generated
            result.Params.Add(new ParamResult(lighteningTimeParam));
            result.Params.Add(new ParamResult(breakTimeParam));
            result.Params.Add(new ParamResult(blinksCountParam));

            return result;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of test that will execute direction light test
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public DirectionLightTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get LighteningTime item
            lighteningTimeParam = testParam.GetParam<DoubleParam>(ParamIds.LighteningTime);
            // from test parameters get BreakTime item
            breakTimeParam = testParam.GetParam<DoubleParam>(ParamIds.BreakTime);
            // from test parameters get BlinksTime item
            blinksCountParam = testParam.GetParam<IntParam>(ParamIds.BlinkCount);

            // for measuring time we only use milliseconds
            lightingTime = convert(lighteningTimeParam, Units.Miliseconds);
            breakTime = convert(breakTimeParam, Units.Miliseconds);
            // this value does not need to be converted
            blinksCount = blinksCountParam.NumericValue;
        }

        #endregion
    }
}
