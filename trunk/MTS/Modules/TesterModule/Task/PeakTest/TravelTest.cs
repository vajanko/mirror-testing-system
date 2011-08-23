using System;
using System.Windows.Media.Media3D;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public sealed class TravelTest : PeakTest
    {
        #region Fields

        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde X
        /// </summary>
        protected Point3D PointX;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Y
        /// </summary>
        protected Point3D PointY;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Z
        /// </summary>
        protected Point3D PointZ;

        private double minAngle;
        private double angleAchieved;
        private int maxTestingTime;

        private MoveDirection travelDirection;

        #endregion

        #region Channels

        /// <summary>
        /// Channel that allows to move up the mirror
        /// </summary>
        private IDigitalOutput MoveUp;
        /// <summary>
        /// Channel that allows to move left the mirror
        /// </summary>
        private IDigitalOutput MoveLeft;
        /// <summary>
        /// Channel that allows to move mirror in reverse direction (moveing right when <paramref name="MoveLeft"/>
        /// is set, or moveing down when <paramref name="MoveUp"/> is set
        /// </summary>
        private IDigitalOutput MoveReverse;

        /// <summary>
        /// Input channel which contains distance measured by X-sonde
        /// </summary>
        private IAnalogInput DistanceX;
        /// <summary>
        /// Input channel which contains distance measured by X-sonde
        /// </summary>
        private IAnalogInput DistanceY;
        /// <summary>
        /// Input channel which contains distance measured by X-sonde
        /// </summary>
        private IAnalogInput DistanceZ;

        #endregion

        #region Movement

        private void moveUp()
        {
            MoveUp.Value = true;
            MoveReverse.Value = false;
            MoveLeft.Value = false;
        }
        private void moveDown()
        {
            MoveUp.Value = true;
            MoveReverse.Value = true;
            MoveLeft.Value = false;
        }
        private void moveLeft()
        {
            MoveLeft.Value = true;
            MoveReverse.Value = false;
            MoveUp.Value = false;
        }
        private void moveRight()
        {
            MoveLeft.Value = true;
            MoveReverse.Value = true;
            MoveUp.Value = false;
        }
        private void Stop()
        {
            MoveLeft.Value = false;
            MoveUp.Value = false;
            MoveReverse.Value = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// (Get) Normal vector of mirror plane in the zero position. This is the moment when mirror
        /// is not rotated
        /// </summary>
        protected Vector3D ZeroPlaneNormal { get; private set; }

        /// <summary>
        /// (Get) Angle between mirror surface and zero mirror position surface
        /// </summary>
        protected double Angle
        {
            get { return Vector3D.AngleBetween(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal); }
        }

        #endregion

        /// <summary>
        /// Any 3D plane is determined by 3 points. Calculate normal vector to plane determined by this three points
        /// </summary>
        /// <param name="x">X-coordinate of plane</param>
        /// <param name="y">Y-coordinate of plane</param>
        /// <param name="z">Z-coordinate of plane</param>
        private Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        {   // Get two vectors from tree points. Cross product gives us a pependicular vector to both of them
            return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        }

        public override void UpdateOutputs(TimeSpan time)
        {
            // decide which direction to move
            switch (travelDirection)
            {
                case MoveDirection.Up: moveUp(); break;
                case MoveDirection.Down: moveDown(); break;
                case MoveDirection.Left: moveLeft(); break;
                case MoveDirection.Right: moveRight(); break;
                default: Stop(); break;
            }

            // this test is running too long - it must be aborted
            if (Duration.TotalMilliseconds > maxTestingTime)
                Finish(time, TaskState.Aborted);

            base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {
            // read distance values
            PointX.Z = DistanceX.Value;
            PointY.Z = DistanceY.Value;
            PointZ.Z = DistanceZ.Value;

            angleAchieved = Angle;
            // final position has been reached - finish
            if (angleAchieved > minAngle)
                Finish(time, TaskState.Completed);

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            Stop();

            Output.WriteLine("{0}: Angle achieved: {1}, Time: {2}, Duration: {3}", Name, angleAchieved, time, Duration);

            base.Finish(time, state);
        }

        #region Constructors

        public TravelTest(Channels channels, TestValue testParam, Vector3D zeroPlaneNormal,
            Point3D pointX, Point3D pointY, Point3D pointZ, MoveDirection travelDirection)
            : base(channels, testParam)
        {
            // channels for controling mirror movement
            MoveUp = channels.MoveMirrorUp;
            MoveLeft = channels.MoveMirrorLeft;
            MoveReverse = channels.MoveReverse;
            // channels for getting mirror position
            DistanceX = channels.DistanceX;
            DistanceY = channels.DistanceY;
            DistanceZ = channels.DistanceZ;

            // find right current channel - we only measure current on actuator that is going to be used for
            // movement
            if (travelDirection.IsVertical())
                CurrentChannel = channels.VerticalActuatorCurrent;
            else CurrentChannel = channels.HorizontalActuatorCurrent;

            // X and Y coordinates of these points are positions of measuring sonds
            // Z cooridnates are distances of the mirror surface
            PointX = pointX;
            PointY = pointY;
            PointZ = pointZ;
            // normal of plane we are going to center to
            ZeroPlaneNormal = zeroPlaneNormal;
            // this test is going to move the mirror in this direction
            this.travelDirection = travelDirection;

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
