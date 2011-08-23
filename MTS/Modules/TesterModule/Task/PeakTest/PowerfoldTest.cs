using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    sealed class PowerfoldTest : PeakTest
    {
        #region Properties

        /// <summary>
        /// Maximal duration allowed for this test
        /// </summary>
        private int maxTestingTime;
        /// <summary>
        /// True if powerfold is folded
        /// </summary>
        private bool isFolded = false;

        private IDigitalOutput FoldChannel;
        private IDigitalOutput UnfoldChannel;

        private IDigitalInput FoldedSensor;
        private IDigitalInput UnfoldedSensor1;
        private IDigitalInput UnfoldedSensor2;

        #endregion

        public override void Initialize(TimeSpan time)
        {
            //FoldChannel.Value = true;   // fold powerfold
            isFolded = false;

            base.Initialize(time);
            Output.WriteLine("{0}: Folding ... Time: {1}", Name, time);
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            if (!isFolded && FoldedSensor.Value)
            {   // powerfold was not folded, but right now get folded
                isFolded = true;
                FoldChannel.Value = false;  // now lets go back - unfold ???
                UnfoldChannel.Value = true;
                Output.WriteLine("{0}: Unfolding ... Time: {1}", Name, time);
            }
            else if (isFolded && UnfoldedSensor1.Value && UnfoldedSensor2.Value)
            {   // powerfold was folded, but right now get unfolded
                Output.WriteLine("{0}: Unfolded! Time: {1}", Name, time);
                Finish(time, TaskState.Completed);
            }
            else
                base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {
            // In this case, if max time elapses, task has to be aborted. But also folding must be finished
            // because mirror is not in right position

            // measure time - end if enought time has elapsed
            if (Duration.TotalMilliseconds > maxTestingTime)
            {   // time of this test has elapsed - finish it
                Output.WriteLine("{0}: running too long. Aborting... Time: {1}", Name, time);
                Finish(time, TaskState.Aborted);        // test has been aborted by itself
                return;     // no other measures shall be done
            }
            
            base.Update(time);  // measure current
        }

        #region Constructors

        public PowerfoldTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            CurrentChannel = channels.PowerFoldCurrent;     // reading current values
            FoldChannel = channels.Fold;        // control folding
            UnfoldChannel = channels.Unfold;    // control unfolding

            // checking foled/unfolded state
            FoldedSensor = channels.PowerFoldFoldedPositionSensor;
            UnfoldedSensor1 = channels.PowerFoldUnfoldedPositionSensor1;
            UnfoldedSensor2 = channels.PowerFoldUnfoldedPositionSensor2;


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
