using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    /// <summary>
    /// Pull-off test is designed to check if mirror glass has been installed enough properly
    /// and if it can resists some pull-off force.
    /// This test is completely independent and there is no need to do any preparation steps before
    /// it is executed. Execution sequence is following: Sucker disk is moved up, close to the mirror
    /// glass. Sucking from the disk is started until vacuum between sucker disk and glass is created.
    /// Then sucker disk is moved down to pull the glass off. If it is possible, test is finalized
    /// with status: Failed. Otherwise, moving sucker disk down is stopped, vacuum is removed by blowing
    /// air from the sucker disk, which is then moved down to make space for other tests.
    /// It is automatically checked at the beginning, if sucker disk is up. If not, it is moved up.
    /// Otherwise sucking to create vacuum is started immediately.
    /// </summary>
    class PulloffTest : TestTask
    {
        /// <summary>
        /// Required duration in milliseconds of pulling off the mirror glass. 
        /// </summary>
        private int testingTime = 0;

        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    testingTime = 0;                // initialize variables                    

                    channels.SuckerUp();            // start to move sucker disk up
                    goTo(ExState.Up);
                    break;
                case ExState.Up:
                    if (channels.IsSuckerUp.Value)
                    {                               // sucker disk is already up
                        channels.StartSucking();    // suck air to create vacuum
                        goTo(ExState.StateA);
                    }                               // otherwise sucker disk is not yet up
                    break;
                case ExState.StateA:
                    if (channels.IsVacuum.Value)
                    {                               // there is already vacuum inside the sucker disk
                        channels.StopAir();         // stop sucking air
                        channels.SuckerDown();      // move sucker disk down
                        StartWatch(time);           // measure time
                        goTo(ExState.Down);
                    }                               // otherwise there is no vacuum yet
                    break;
                case ExState.Down:           // this is actually required testing
                    // measure time and finish if time has elapsed

                    if (channels.IsSuckerDown.Value)
                    {                               // glass has been pulled off
                        goTo(ExState.Finalizing);
                    }
                    else if (TimeElapsed(time) > testingTime)
                    {   // enough time has elapsed, remove sucker disk from mirror
                        channels.StartBlowing();    // first vacuum must be removed
                        channels.SuckerUp();        // stop to pull-off and wait for vacuum to be removed
                        goTo(ExState.StateB);       // wait for vacuum to be removed
                    }                               // otherwise pull-off test is being executed
                    break;
                case ExState.StateB:
                    if (!channels.IsVacuum.Value)
                    {                               // vacuum is already removed
                        channels.StopAir();         // stop blowing air
                        channels.SuckerDown();      // remove sucker disk from mirror glass
                        goTo(ExState.Finalizing);
                    }                               // otherwise vacuum is not removed yet
                    break;

                case ExState.Finalizing:            // just wait for sucker disk to be down
                    if (channels.IsSuckerDown.Value)
                        Finish(time);
                    break;
                case ExState.Aborting:              // something bad has happened - aborting test
                    // without resolving current state of hardware, may be sucker disk is still on the mirror
                    Finish(time);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of pull-off test which will prove the quality of mirror
        /// glass installation by trying to pull it off.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public PulloffTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            IntParam iValue = testParam.GetParam<IntParam>(ParamIds.TestingTime);
            if (iValue != null)     // it must be of type integer
                testingTime = iValue.NumericValue;
        }

        #endregion
    }

}
