using System;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    public sealed class TravelTest : PeakTest
    {
        #region Fields

        private double angleAchieved;
        private DoubleParam minAngle;
        private DoubleParam maxTestingTime;

        private MoveDirection travelDirection;
        private IAnalogInput actuatorChannel;

        #endregion

        public override void Update(DateTime time)
        {
            // In this case, if max time elapsed, task has to be aborted. The final position has not been reached,
            // but we already know that this is a bed pieace
            if (Duration.TotalMilliseconds > maxTestingTime.DoubleValue)
                exState = ExState.Aborting;

            switch (exState)
            {
                case ExState.Initializing:
                    angleAchieved = 0;                              // initialize variables - default values
                    channels.MoveMirror(travelDirection);           // start to move mirror glass
                    actuatorChannel = travelDirection.IsHorizontal() ?
                        channels.HorizontalActuatorCurrent :        // decide on which channel to measure current
                        channels.VerticalActuatorCurrent;           // depends on which direction we are moveing in
                    goTo(ExState.Measuring);                        // swtich to next state
                    Output.WriteLine("Moveing in direction: {0}", travelDirection);
                    break;
                case ExState.Measuring:
                    measureCurrent(time, actuatorChannel);          // measure current
                    angleAchieved = channels.GetRotationAngle();    // measure angle
                    if (angleAchieved > minAngle.DoubleValue)       // final position reached
                        goTo(ExState.Finalizing);                   // finish
                    break;
                case ExState.Finalizing:
                    channels.StopMirror();                          // stop moveing mirror glass
                    Finish(time);                                   //
                    Output.WriteLine("Stop moveing");
                    break;
                case ExState.Aborting:
                    channels.StopMirror();                          // stop moveing mirror glass
                    Finish(time);                                   //
                    break;
            }
        }

        protected override TaskResult getResult()
        {
            TaskResult result = base.getResult();

            result.Params.Add(new ParamResult(minAngle, angleAchieved));
            result.Params.Add(new ParamResult(maxTestingTime, Duration.TotalMilliseconds));

            return result;
        }

        #region Constructors

        public TravelTest(Channels channels, TestValue testParam, MoveDirection travelDirection)
            : base(channels, testParam)
        {
            // this test is going to move the mirror in this direction
            this.travelDirection = travelDirection;

            // initialization of testing parameters
            // from test parameters get MinAngle item
            minAngle = testParam.GetParam<DoubleParam>(TestValue.MinAngle);
            if (minAngle == null)
                throw new ParamNotFoundException(TestValue.MinAngle);
            // from test parameters get MaxTestingTime item
            maxTestingTime= testParam.GetParam<DoubleParam>(TestValue.MaxTestingTime);
            if (maxTestingTime == null)
                throw new ParamNotFoundException(TestValue.MaxTestingTime);
        }

        #endregion
    }
}
