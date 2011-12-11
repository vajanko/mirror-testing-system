﻿using System;
using System.Collections.Generic;

using MTS.IO;

namespace MTS.TesterModule
{
    class Wait : Task
    {
        /// <summary>
        /// Time that we are going to wait
        /// </summary>
        private int miliseconds;
        /// <summary>
        /// Check if enought time has elapsed and finish this task fi so
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:  // start to measure time
                    StartWatch(time);
                    exState = ExState.Measuring;
                    break;
                case ExState.Measuring:     // just wait for required time and finish
                    if (TimeElapsed(time) > miliseconds)
                        exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    Finish(time, TaskState.Completed);
                    break;
                case ExState.Aborting:
                    Finish(time, TaskState.Aborted);
                    break;
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instnce of task that will wait for a particular period of time
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="miliseconds">Time in miliseconds to wait</param>
        public Wait(Channels channels, int miliseconds)
            : base(channels) 
        {
            this.miliseconds = miliseconds;
        }

        #endregion
    }
}