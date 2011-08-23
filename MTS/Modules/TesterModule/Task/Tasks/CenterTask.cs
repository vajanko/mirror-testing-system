using System;
using System.Windows.Media.Media3D;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public sealed class CenterTask : Task
    {
        #region Fields

        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde X
        /// </summary>
        private Point3D PointX;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Y
        /// </summary>
        private Point3D PointY;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Z
        /// </summary>
        private Point3D PointZ;
        /// <summary>
        /// Direction in which we need to move the mirror to center it
        /// </summary>
        private MoveDirection CenterDir;

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

        #region Properties

        /// <summary>
        /// (Get) Normal vector of mirror plane in the zero position. This is the moment when mirror
        /// is not rotated
        /// </summary>
        public Vector3D ZeroPlaneNormal { get; set; }

        /// <summary>
        /// (Get) Angle between mirror surface and zero mirror position surface
        /// </summary>
        //protected double Angle
        //{
        //    get { return Vector3D.AngleBetween(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal); }
        //}

        /// <summary>
        /// (Get) Vector laying on the intersection of current mirror surface and zero position mirror surface
        /// This is the vector around which mirror is rotated
        /// </summary>
        private Vector3D Axis
        {
            get { return Vector3D.CrossProduct(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal); }
        }

        #endregion

        /// <summary>
        /// Read channels connected to sonds which are measuring mirror distance
        /// </summary>
        private void readDistances()
        {
            PointX.Z = DistanceX.Value;
            PointY.Z = DistanceY.Value;
            PointZ.Z = DistanceZ.Value;
        }
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

        public override void Initialize(TimeSpan time)
        {
            // first we need to know the initial position of the mirror
            readDistances();
            // define in which direction to center first
            Vector3D axis = Axis;   // this is a line around which is mirror rotated
            if (axis.X > 0)
                CenterDir = MoveDirection.Up;
            else if (axis.X < 0)
                CenterDir = MoveDirection.Down;
            else if (axis.Y > 0)                    // this only happen when mirror is centered verticaly
                CenterDir = MoveDirection.Left;   // very exactly
            else if (axis.Y < 0)
                CenterDir = MoveDirection.Right;
            else
                CenterDir = MoveDirection.None;   // this only happen when mirror is centered verticaly and
                                                    // horizontaly very exactly
            Output.WriteLine("{0}: center direction: {1}", Name, CenterDir);

            base.Initialize(time);
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            Vector3D axis = Axis;

        Center:
            if (CenterDir == MoveDirection.Up)    // vertialy - we need to center up
                if (axis.X <= 0)        // verticaly is already centered
                {
                    // deside in which direction to center horizontaly
                    if (axis.Y > 0)
                        CenterDir = MoveDirection.Left;
                    else if (axis.Y < 0)
                        CenterDir = MoveDirection.Right;
                    else
                    {   // this only happen when mirror is centered verticaly very exactly
                        Finish(time, TaskState.Completed);
                        return;
                    }
                    goto Center;        // re-deside what to center
                }
                else moveUp();          // center verticaly
            else if (CenterDir == MoveDirection.Down) // verticaly - we need to center down
                if (axis.X >= 0)        // verticaly is already centered
                {
                    // deside in which direction to center horizontaly
                    if (axis.Y > 0)
                        CenterDir = MoveDirection.Left;
                    else if (axis.Y < 0)
                        CenterDir = MoveDirection.Right;
                    else
                    {   // this only happen when mirror is centered verticaly very exactly
                        Finish(time, TaskState.Completed);
                        return;
                    }
                    goto Center;        // re-deside what to center
                }
                else moveDown();        // center verticaly
            else if (CenterDir == MoveDirection.Left) // horizontaly - we need to center left
                if (axis.Y <= 0)        // horizontaly is already centered
                    Finish(time, TaskState.Completed);  // centering finished
                else moveLeft();        // center horizontaly
            else if (CenterDir == MoveDirection.Right)// horizontaly - we need to center right
                if (axis.Y >= 0)        // horizontaly is already centered
                    Finish(time, TaskState.Completed);  // centering finished
                else moveRight();       // center horizontaly
            else
                Finish(time, TaskState.Completed);  // in this case centerDir == CenterDirection.None

            base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {
            readDistances();        // read mirror position

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            Stop();
            Output.WriteLine("{0}: Mirror centered in time: {1}", Name, Duration);
            base.Finish(time, state);
        }

        #region Constructors

        public CenterTask(Channels channels, Vector3D zeroPlaneNormal, Point3D pointX, Point3D pointY, Point3D pointZ)
            : base(channels)
        {
            // channels for controling mirror movement
            MoveUp = channels.MoveMirrorUp;
            MoveLeft = channels.MoveMirrorLeft;
            MoveReverse = channels.MoveReverse;
            // channels for getting mirror position
            DistanceX = channels.DistanceX;
            DistanceY = channels.DistanceY;
            DistanceZ = channels.DistanceZ;

            // X and Y coordinates of these points are positions of measuring sonds
            // Z cooridnates are distances of the mirror surface
            PointX = pointX;
            PointY = pointY;
            PointZ = pointZ;

            // normal of plane we are going to center to
            ZeroPlaneNormal = zeroPlaneNormal;
        }

        #endregion     
    }
}
