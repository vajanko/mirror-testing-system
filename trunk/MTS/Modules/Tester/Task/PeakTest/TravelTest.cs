using System;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Editor;

namespace MTS.TesterModule
{
    public sealed class TravelTest : PeakTest
    {
        #region Fields

        private double minAngle;
        private double angleAchieved;
        private int maxTestingTime;

        private MoveDirection travelDirection;
        private IAnalogInput actuatorChannel;

        #endregion

        public override void Update(TimeSpan time)
        {
            // In this case, if max time elapsed, task has to be aborted. The final position has not been reached,
            // but we already know that this is a bed pieace
            if (Duration.TotalMilliseconds > maxTestingTime)
                exState = ExState.Aborting;

            switch (exState)
            {
                case ExState.Initializing:
                    angleAchieved = 0;                              // initialize variables - default values
                    channels.MoveMirror(travelDirection);           // start to move mirror glass
                    actuatorChannel = travelDirection.IsHorizontal() ?
                        channels.HorizontalActuatorCurrent :        // decide on which channel to measure current
                        channels.VerticalActuatorCurrent;           // depends on which direction we are moveing in
                    exState = ExState.Measuring;                    // swtich to next state
                    break;
                case ExState.Measuring:
                    measureCurrent(time, actuatorChannel);          // measure current
                    angleAchieved = channels.GetRotationAngle();    // measure angle
                    if (angleAchieved > minAngle)                   // final position reached
                        exState = ExState.Finalizing;               // finish
                    break;
                case ExState.Finalizing:
                    channels.StopMirror();                          // stop moveing mirror glass
                    exState = ExState.None;                         // stop to update this test
                    Finish(time, getTaskState());                   // finish with apropriate state
                    break;
                case ExState.Aborting:
                    channels.StopMirror();                          // stop moveing mirror glass
                    exState = ExState.None;                         // stop to update this test
                    Finish(time, TaskState.Aborted);
                    break;
            }
        }

        #region Constructors

        public TravelTest(Channels channels, TestValue testParam, MoveDirection travelDirection)
            : base(channels, testParam)
        {
            // this test is going to move the mirror in this direction
            this.travelDirection = travelDirection;

            // initialization of testing parameters
            // from test parameters get MinAngle item
            DoubleParam dValue = testParam.GetParam<DoubleParam>(TestValue.MinAngle);
            if (dValue != null)     // it must be of type double
                minAngle = dValue.DoubleValue;
            // from test parameters get MaxTestingTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.MaxTestingTime);
            if (iValue != null)     // it must be of type int
                maxTestingTime = iValue.IntValue;
        }

        #endregion
    }
}
