using System;

using MTS.AdminModule;
using MTS.EditorModule;

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

        public override void Initialize(TimeSpan time)
        {
            currentState = State.Initializing;
            base.Initialize(time);
        }
        public override void Update(TimeSpan time)
        {
            // In this case, if max time elapsed, task has to be aborted. If folding is running too long
            // it means that there is some problem with the acutator - Test will be aborted and folding
            // will not be finished

            // measure time - end if enought time has elapsed
            if (Duration.TotalMilliseconds > maxTestingTime)
                currentState = State.Aborting;

            switch (currentState)
            {
                case State.Initializing:
                    Output.WriteLine("{0}: Unfolding ... Time: {1}", Name, time);
                    channels.UnfoldPowerfold.Value = true;          // start to unfold
                    currentState = State.Unfolding;                 // switch to next state
                    break;
                case State.Unfolding:
                    // check current while unfolding
                    measureCurrent(time);
                    if ((channels.IsOldMirror.Value && channels.IsOldPowerfoldDown.Value) ||
                        (!channels.IsOldMirror.Value && channels.IsPowerfoldDown.Value))
                    {
                        channels.UnfoldPowerfold.Value = false;     // stop unfolding
                        channels.FoldPowerfold.Value = true;        // start to fold
                        currentState = State.Folding;               // switch to next state
                    }
                    break;
                case State.Folding:
                    // check current while folding
                    measureCurrent(time);
                    if ((channels.IsOldMirror.Value && channels.IsOldPowerfoldUp.Value) ||
                        (!channels.IsOldMirror.Value && channels.IsPowerfoldUp.Value))
                    {
                        channels.FoldPowerfold.Value = false;       // stop folding
                        currentState = State.Finalizing;            // switch to next state
                    }
                    break;
                case State.Finalizing:
                    // for sure - switch off actuator
                    channels.FoldPowerfold.Value = false;
                    channels.UnfoldPowerfold.Value = false;
                    Finish(time, getTaskState());                   // raise events, save state
                    currentState = State.None;                      // for sure - this test never get updated
                    break;
                case State.Aborting:
                    channels.FoldPowerfold.Value = false;
                    channels.UnfoldPowerfold.Value = false;
                    Finish(time, TaskState.Aborted);
                    currentState = State.None;
                    break;
            }
        }

        #region State

        private State currentState = State.None;
        private enum State
        {
            Initializing,
            Unfolding,
            Folding,
            Aborting,
            Finalizing,
            None
        }

        #endregion

        #region Constructors

        public PowerfoldTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            CurrentChannel = channels.PowerfoldCurrent;     // reading current values

            ParamCollection param = testParam.Parameters;
            IntParamValue iValue;
            // from test parameters get MAX_TESTING_TIME item
            if (param.ContainsKey(ParamDictionary.MAX_TESTING_TIME))
            {   // it must be double type value
                iValue = param[ParamDictionary.MAX_TESTING_TIME] as IntParamValue;
                if (iValue != null)     // param is of other type then double
                    maxTestingTime = iValue.Value;
            }
        }

        #endregion
    }
}
