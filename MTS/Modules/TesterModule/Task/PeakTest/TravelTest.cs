using System;
using System.Windows.Media.Media3D;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public sealed class TravelTest : PeakTest
    {
        #region Fields

        private double minAngle;
        private double angleAchieved;
        private int maxTestingTime;

        private MoveDirection travelDirection;

        #endregion

        public override void UpdateOutputs(TimeSpan time)
        {
            // decide which direction to move
            switch (travelDirection)
            {
                case MoveDirection.Up: channels.MoveUp(); break;
                case MoveDirection.Down: channels.MoveDown(); break;
                case MoveDirection.Left: channels.MoveLeft(); break;
                case MoveDirection.Right: channels.MoveRight(); break;
                default: channels.Stop(); break;
            }

            // this test is running too long - it must be aborted
            if (Duration.TotalMilliseconds > maxTestingTime)
                Finish(time, TaskState.Aborted);

            base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {
            angleAchieved = channels.GetRotationAngle();
            // final position has been reached - finish
            if (angleAchieved > minAngle)
                Finish(time, TaskState.Completed);

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            channels.Stop();

            Output.WriteLine("{0}: Angle achieved: {1}, Time: {2}, Duration: {3}", Name, angleAchieved, time, Duration);

            base.Finish(time, state);
        }

        #region Constructors

        public TravelTest(Channels channels, TestValue testParam, MoveDirection travelDirection)
            : base(channels, testParam)
        {
            // find right current channel - we only measure current on actuator that is going to be used for
            // movement
            if (travelDirection.IsVertical())
                CurrentChannel = channels.VerticalActuatorCurrent;
            else CurrentChannel = channels.HorizontalActuatorCurrent;
            // this test is going to move the mirror in this direction
            this.travelDirection = travelDirection;

            // initialization of testing parameters
            ParamCollection param = testParam.Parameters;
            DoubleParamValue dValue;
            // from test parameters get MIN_ANGLE item
            if (param.ContainsKey(ParamDictionary.MIN_ANGLE))
            {   // it must be double type value
                dValue = param[ParamDictionary.MIN_ANGLE] as DoubleParamValue;
                if (dValue != null)     // param is of other type then double
                    minAngle = dValue.Value;
            }
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
