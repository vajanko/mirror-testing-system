using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    class Wait : Task
    {
        /// <summary>
        /// Time that we are going to wait
        /// </summary>
        private int miliseconds;
        /// <summary>
        /// Show how long we are going to wait
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Initialize(TimeSpan time)
        {
            Output.WriteLine("Waiting for {0} ms", miliseconds);
            base.Initialize(time);
        }
        /// <summary>
        /// Check if enought time has elapsed and finish this task fi so
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(TimeSpan time)
        {
            EndTime = time;
            if (Duration.TotalMilliseconds > miliseconds)
                Finish(time, TaskState.Completed);
            base.Update(time);
        }
        /// <summary>
        /// Show how long we have been waiting
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="state">Final state of this task</param>
        public override void Finish(TimeSpan time, TaskState state)
        {
            Output.WriteLine("Waiting for {0} ms finished!", Duration.Milliseconds);
            base.Finish(time, state);
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
