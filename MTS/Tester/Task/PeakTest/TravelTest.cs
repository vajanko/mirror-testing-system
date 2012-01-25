using System;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;
using MTS.Data.Types;

namespace MTS.Tester
{
    class TravelTest : PeakTest
    {
        #region Fields

        /// <summary>
        /// Currently achieved angle. This value should be initialized when test is being executed.
        /// </summary>
        private double angleMeasured;
        /// <summary>
        /// Currently time of traveling. This value should be initialized when test is being executed.
        /// </summary>
        private double testingTimeMeasured;

        /// <summary>
        /// Minimal angle to achieve
        /// </summary>
        private readonly DoubleParam minAngleParam;
        /// <summary>
        /// Maximal duration allowed for this test
        /// </summary>
        private readonly DoubleParam maxTestingTimeParam;

        /// <summary>
        /// Maximal duration allowed for this test in miliseconds
        /// </summary>
        private readonly double maxTestingTime;
        /// <summary>
        /// Minimal angle to achieve in degrees
        /// </summary>
        private readonly double minAngle;

        private MoveDirection travelDirection;
        private IAnalogInput actuatorChannel;

        #endregion

        public override void Update(DateTime time)
        {
            // In this case, if max time elapsed, task has to be finished. The final position has not been reached,
            // but we already know that this is a bed peace
            if (testingTimeMeasured > maxTestingTime)
                goTo(ExState.Finalizing);

            switch (exState)
            {
                case ExState.Initializing:
                    angleMeasured = 0;                              // initialize variables
                    testingTimeMeasured = 0;
                    StartWatch(time);                               // start to measure time
                    channels.MoveMirror(travelDirection);           // start to move mirror glass
                    actuatorChannel = travelDirection.IsHorizontal() ?
                        channels.HorizontalActuatorCurrent :        // decide on which channel to measure current
                        channels.VerticalActuatorCurrent;           // depends on which direction we are moving in
                    goTo(ExState.Measuring);                        // switch to next state
                    Output.WriteLine("Moving in direction: {0}", travelDirection);
                    break;
                case ExState.Measuring:
                    testingTimeMeasured = TimeElapsed(time);        // measure time
                    measureCurrent(time, actuatorChannel);          // measure current
                    angleMeasured = channels.GetRotationAngle(travelDirection);                        
                        //channels.GetRotationAngle();    // measure angle
                    if (angleMeasured >= minAngle)                  // final position reached
                        goTo(ExState.Finalizing);                   // finish
                    break;
                case ExState.Finalizing:
                    channels.StopMirror();                          // stop moving mirror glass
                    Finish(time);                                   //
                    Output.WriteLine("Stop moving");
                    break;
                case ExState.Aborting:
                    channels.StopMirror();                          // stop moving mirror glass
                    Finish(time);                                   //
                    break;
            }
        }

        protected override TaskResult getResult()
        {
            TaskResult result = base.getResult();

            // we have been measuring angle in degrees, now convert it back to parameter unit
            // in this state will be saved to database
            double angle = convertBack(minAngleParam, Units.Degrees, angleMeasured);
            result.Params.Add(new ParamResult(minAngleParam, angle));
            // we have been measuring time in miliseconds, now convert it back to parameter unit
            // in this state will be saved to database
            double time = convertBack(maxTestingTimeParam, Units.Miliseconds, testingTimeMeasured);
            result.Params.Add(new ParamResult(maxTestingTimeParam, time));

            return result;
        }
        protected override TaskResultType getResultCode()
        {
            if (angleMeasured >= minAngle)
                return base.getResultCode();
            else
                return Data.Types.TaskResultType.Failed;    // angle was not achieved
        }

        #region Constructors

        public TravelTest(Channels channels, TestValue testParam, MoveDirection travelDirection)
            : base(channels, testParam)
        {
            // this test is going to move the mirror in this direction
            this.travelDirection = travelDirection;

            // initialization of testing parameters
            // from test parameters get MinAngle item
            minAngleParam = testParam.GetParam<DoubleParam>(TestValue.MinAngle);
            // from test parameters get MaxTestingTime item
            maxTestingTimeParam= testParam.GetParam<DoubleParam>(TestValue.MaxTestingTime);

            // for measuring angle only use degrees
            minAngle = convert(minAngleParam, Units.Degrees);
            // for measuring time only use miliseconds
            maxTestingTime = convert(maxTestingTimeParam, Units.Miliseconds);
        }

        #endregion
    }
}
