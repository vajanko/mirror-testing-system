﻿using System;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class PowerfoldTest : PeakTest
    {
        #region Private fields

        /// <summary>
        /// Max duration allowed fot this test in miliseconds
        /// </summary>
        private readonly double maxTestingTime;
        /// <summary>
        /// Maximal duration allowed for this test
        /// </summary>
        private readonly DoubleParam maxTestingTimeParam;

        #endregion

        public override void Update(DateTime time)
        {
            // In this case, if max time elapsed, task has to be aborted. If folding is running too long
            // it means that there is some problem with the acutator - Test will be aborted and folding
            // will not be finished

            // measure time - end if enought time has elapsed
            if (Duration.TotalMilliseconds > maxTestingTime)
                exState = ExState.Aborting;

            switch (exState)
            {
                case ExState.Initializing:
                    maxMeasuredOverloadTime = 0;                 // initialize variables
                    isOverloaded = false;

                    channels.StartUnfolding();                   // start to unfold
                    goTo(ExState.Unfolding);                     // switch to next state
                    break;
                case ExState.Unfolding:                          // check current while unfolding
                    measureCurrent(time, channels.PowerfoldCurrent);
                    if (channels.IsUnfolded())
                    {
                        channels.StartFolding();                 // stop unfolding and start to fold
                        goTo(ExState.Folding);                   // switch to next state
                    }
                    break;
                case ExState.Folding:                            // check current while folding
                    measureCurrent(time, channels.PowerfoldCurrent);
                    if (channels.IsFolded())
                        goTo(ExState.Finalizing);                // switch to next state
                    break;
                case ExState.Finalizing:
                    channels.StopPowerfold();                    // for sure - switch off actuator
                    Finish(time);
                    break;
                case ExState.Aborting:
                    channels.StopPowerfold();                    // for sure - swtitch off actuator
                    Finish(time);
                    break;
            }
        }

        #region Constructors

        public PowerfoldTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MAX_TESTING_TIME item
            maxTestingTimeParam = testParam.GetParam<DoubleParam>(TestValue.MaxTestingTime);
            // for measuring time only use miliseconds
            maxTestingTime = convert(maxTestingTimeParam, Units.Miliseconds);
        }

        #endregion
    }
}