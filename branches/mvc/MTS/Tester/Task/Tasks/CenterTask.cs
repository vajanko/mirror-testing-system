using System;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    public sealed class CenterTask : Task
    {
        #region Fields

        /// <summary>
        /// Direction in which we need to move the mirror to center it
        /// </summary>
        private MoveDirection centerDir;

        private Channels channels;
        
        #endregion

        private MoveDirection getCenterDir(double verAngle, double horAngle)
        {
            if (verAngle > 0) return MoveDirection.Up;
            else if (verAngle < 0) return MoveDirection.Down;
            else if (horAngle > 0) return MoveDirection.Right;
            else if (horAngle < 0) return MoveDirection.Left;
            else return MoveDirection.None;
        }
        private ExState getExecutionState(MoveDirection dir)
        {
            switch (dir)
            {
                case MoveDirection.Up: return ExState.Up;
                case MoveDirection.Down: return ExState.Down;
                case MoveDirection.Left: return ExState.Left;
                case MoveDirection.Right: return ExState.Right;
                default: return ExState.Finalizing;
            }
        }

        private void setupCenter(double verAngle, double horAngle)
        {
            centerDir = getCenterDir(verAngle, horAngle);
            channels.MoveMirror(centerDir);
            exState = getExecutionState(centerDir);
            Output.WriteLine("Center direction: {0}", centerDir);
        }

        public override void Update(DateTime time)
        {
            double ver = channels.GetVerticalAngle();
            double hor = channels.GetHorizontalAngle();

            switch (exState)
            {
                case ExState.Initializing:
                    setupCenter(ver, hor);
                    Output.WriteLine("Centering ... ");
                    break;
                case ExState.Up:
                    if (ver <= 0)
                        setupCenter(0, hor);
                    break;
                case ExState.Down:
                    if (ver >= 0)
                        setupCenter(0, hor);
                    break;
                case ExState.Left:
                    if (hor >= 0)
                        goTo(ExState.Finalizing);
                    break;
                case ExState.Right:
                    if (hor <= 0)
                        goTo(ExState.Finalizing);
                    break;
                case ExState.Finalizing:
                    Finish(time);
                    Output.WriteLine("Centered!");
                    break;
                case ExState.Aborting:
                    Finish(time);
                    Output.WriteLine("Centering aborted!");
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will center the mirror glass to zero plane as
        /// it is save in application settings
        /// </summary>
        /// <param name="channels"></param>
        public CenterTask(Channels channels)
        {
            this.channels = channels;
        }

        #endregion     
    }
}
