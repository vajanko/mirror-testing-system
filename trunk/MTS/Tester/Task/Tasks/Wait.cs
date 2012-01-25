using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Tester.Result;

namespace MTS.Tester
{
    class Wait : Task
    {
        /// <summary>
        /// Time that we are going to wait
        /// </summary>
        private int milliseconds;
        /// <summary>
        /// Check if enough time has elapsed and finish this task if so
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:  // start to measure time
                    StartWatch(time);
                    goTo(ExState.Measuring);
                    Output.Write("Waiting for {0} ms ... ", milliseconds);
                    break;
                case ExState.Measuring:     // just wait for required time and finish
                    double elapsed = TimeElapsed(time);
                    if (elapsed > milliseconds)
                        goTo(ExState.Finalizing);
                    break;
                case ExState.Finalizing:
                    Finish(time);
                    Output.WriteLine("Finished");
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will wait for a particular period of time
        /// </summary>
        /// <param name="milliseconds">Time in milliseconds to wait</param>
        public Wait(int milliseconds)
        {
            this.milliseconds = milliseconds;
        }

        #endregion
    }
}
