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
        /// Direction in which we need to move the mirror to center it
        /// </summary>
        private MoveDirection CenterDir;

        #endregion

        public override void Initialize(TimeSpan time)
        {
            // define in which direction to center first
            double ver = channels.GetVerticalAngle();
            if (ver > 0)
            {
                channels.MoveUp();
                CenterDir = MoveDirection.Up;
            }
            else if (ver < 0)
            {
                channels.MoveDown();
                CenterDir = MoveDirection.Down;
            }
            else
            {
                double hor = channels.GetHorizontalAngle();
                if (hor > 0)
                {
                    channels.MoveRight();
                    CenterDir = MoveDirection.Right;
                }
                else if (hor < 0)
                {
                    channels.MoveLeft();
                    CenterDir = MoveDirection.Left;
                }
                else
                    CenterDir = MoveDirection.None;
            }
            Output.WriteLine("Centering ... Init direction: {0}, angle: {1}", CenterDir, ver);

            base.Initialize(time);
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            double ver = channels.GetVerticalAngle();
            double hor = channels.GetHorizontalAngle();

            if (CenterDir == MoveDirection.Up)    // vertialy - we need to center up
            {
                if (ver <= 0)        // verticaly is already centered
                {
                    // deside in which direction to center horizontaly
                    if (hor > 0)
                    {
                        channels.MoveRight();
                        CenterDir = MoveDirection.Right;
                    }
                    else if (hor < 0)
                    {
                        channels.MoveLeft();
                        CenterDir = MoveDirection.Left;
                    }
                    else
                    {   // this only happen when mirror is centered verticaly very exactly
                        Finish(time, TaskState.Completed);
                        return;
                    }
                }
            }
            else if (CenterDir == MoveDirection.Down) // verticaly - we need to center down
            {
                if (ver >= 0)        // verticaly is already centered
                {
                    // deside in which direction to center horizontaly
                    if (hor > 0)
                    {
                        channels.MoveRight();
                        CenterDir = MoveDirection.Right;
                    }
                    else if (hor < 0)
                    {
                        channels.MoveLeft();
                        CenterDir = MoveDirection.Left;
                    }
                    else
                    {   // this only happen when mirror is centered verticaly very exactly
                        Finish(time, TaskState.Completed);
                        return;
                    }
                }
            }
            else if (CenterDir == MoveDirection.Left) // horizontaly - we need to center left
            {
                if (hor >= 0)        // horizontaly is already centered
                    Finish(time, TaskState.Completed);  // centering finished
            }
            else if (CenterDir == MoveDirection.Right)// horizontaly - we need to center right
            {
                if (hor <= 0)        // horizontaly is already centered
                    Finish(time, TaskState.Completed);  // centering finished
            }
            else
                Finish(time, TaskState.Completed);  // in this case centerDir == CenterDirection.None

            base.UpdateOutputs(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            channels.Stop();

            Output.WriteLine("Centered! Duration: {0}", Duration);
            base.Finish(time, state);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will center the mirror glass to zero plane as
        /// it is save in application settings
        /// </summary>
        /// <param name="channels"></param>
        public CenterTask(Channels channels)
            : base(channels) { }

        #endregion     
    }
}
