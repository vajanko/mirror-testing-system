using System;

using MTS.IO;
using MTS.Editor;

namespace MTS.TesterModule
{
    sealed class PowerfoldTest : PeakTest
    {
        #region Private fields

        /// <summary>
        /// Maximal duration allowed for this test
        /// </summary>
        private int maxTestingTime;

        #endregion

        public override void Update(TimeSpan time)
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
                    channels.StartUnfolding();                   // start to unfold
                    exState = ExState.Unfolding;                 // switch to next state
                    break;
                case ExState.Unfolding:                          // check current while unfolding
                    measureCurrent(time, channels.PowerfoldCurrent);
                    if (channels.IsUnfolded())
                    {
                        channels.StartFolding();                 // stop unfolding and start to fold
                        exState = ExState.Folding;               // switch to next state
                    }
                    break;
                case ExState.Folding:                            // check current while folding
                    measureCurrent(time, channels.PowerfoldCurrent);
                    if (channels.IsFolded())
                        exState = ExState.Finalizing;            // switch to next state
                    break;
                case ExState.Finalizing:
                    channels.StopPowerfold();                    // for sure - switch off actuator
                    Finish(time, getTaskState());                // raise events, save state
                    exState = ExState.None;                      // stop to update this test
                    break;
                case ExState.Aborting:
                    channels.StopPowerfold();                    // for sure - swtitch off actuator
                    Finish(time, TaskState.Aborted);             // raise events, save state
                    exState = ExState.None;                      // stop to update this test
                    break;
            }
        }

        #region Constructors

        public PowerfoldTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MAX_TESTING_TIME item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.MaxTestingTime);
            if (iValue != null)     // it must be of type int
                maxTestingTime = iValue.IntValue;
        }

        #endregion
    }
}
