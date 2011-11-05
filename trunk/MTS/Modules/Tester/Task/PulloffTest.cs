using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;

namespace MTS.TesterModule
{
    /// <summary>
    /// Pull-off test is designed to check if mirror glass has been installed enought properly
    /// and if it can resists some pull-off force.
    /// This test is completly independent and there is no need to do any preparation steps before
    /// it is executed. Execution sequnece is following: Sucker disk is moved up, close to the mirror
    /// glass. Sucking from the disk is started until vacuum between sucker disk and glass is created.
    /// Then sucker disk is moved down to pull the glass off. If it is possible, test is finalized
    /// with status: Failed. Otherwise, moveing sucker disk down is stoped, vacuum is removed by blowing
    /// air from the sucker disk, which is then moved down to make space for other tests.
    /// It is automatically checked at the beginning, if sucker disk is up. If not, it is moved up.
    /// Otherwise sucking to create vacuum is started imediatelly.
    /// </summary>
    class PulloffTest : TestTask
    {
        /// <summary>
        /// Required duration in miliseconds of pulling off the mirror glass. 
        /// </summary>
        private int testingTime = 0;

        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    testingTime = 0;                // initialize variables
                    finalState = TaskState.Passed;  // change if glass is pulled-off

                    if (channels.IsSuckerUp.Value)
                    {                               // if sucker disk is already up
                        channels.StartSucking();    // suck air to create vacuum
                        exState = ExState.Sucking;  // this state will wait for vacuum
                    }
                    else
                    {                               // sucker disk is not up yet
                        channels.SuckerUp();        // start to move it up first
                        exState = ExState.MoveingUp;
                    }
                    break;
                case ExState.MoveingUp:
                    if (channels.IsSuckerUp.Value)
                    {                               // sucker disk is already up
                        channels.StartSucking();    // suck air to create vacuum
                        exState = ExState.Sucking;
                    }                               // otherwise sucker disk is not yet up
                    break;
                case ExState.Sucking:
                    if (channels.IsVacuum.Value)
                    {                               // there is already vacuum inside the sucker disk
                        channels.StopAir();         // stop sucking air
                        channels.SuckerDown();      // move sucker disk down
                        StartWatch(time);           // measure time
                    }                               // otherwise there is no vacuum yet
                    break;
                case ExState.MoveingDown:           // this is actually required testing
                    // measure time and finish if time has elapsed

                    if (channels.IsSuckerDown.Value)
                    {                               // glass has been pulled off
                        finalState = TaskState.Failed;
                        exState = ExState.Finalizing;
                    }
                    else if (TimeElapsed(time) > testingTime)
                    {   // enought time has elapsed, remove sucker disk from mirror
                        channels.StartBlowing();    // first vacuum must be removed
                        channels.SuckerUp();        // stop to pull-off and wait for vacuum to be removed
                        exState = ExState.Blowing;  // wait for vacuum to be removed
                    }                               // otherwise pull-off test is being executed
                    break;
                case ExState.Blowing:
                    if (!channels.IsVacuum.Value)
                    {                               // vacuum is already removed
                        channels.StopAir();         // stop blowing air
                        channels.SuckerDown();      // remove sucker disk from mirror glass
                        exState = ExState.Finalizing;
                    }                               // otherwise vacuum is not removed yet
                    break;

                case ExState.Finalizing:            // just wait for sucker disk to be down
                    if (channels.IsSuckerDown.Value)
                        Finish(time, getFinalState());
                    break;
                case ExState.Aborting:              // something bad has happened - aborting test
                    Finish(time, TaskState.Aborted);// without resolving current state of hardware
                    break;                          // may be sucker disk is still on the mirror
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of pull-off test which will prove the quality of mirror
        /// glass instalation by trying to pull it off.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        public PulloffTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.TestingTime);
            if (iValue != null)     // it must be of type int
                testingTime = iValue.IntValue;
        }

        #endregion
    }

}
