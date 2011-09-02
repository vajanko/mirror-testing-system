using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    class ExecuteIf : Task
    {
        private IDigitalInput conditionChannel;
        private bool conditionValue;

        private Task thenTask;
        private Task elseTask;

        private TaskScheduler scheduler;

        public override void Update(TimeSpan time)
        {
            if (conditionChannel.Value == conditionValue)
                scheduler.ChangeTask(this, thenTask);
            else
            {   // check if else condition is specified
                // if not just finish this task
                if (elseTask != null)
                    scheduler.ChangeTask(this, elseTask);
                else Finish(time, TaskState.Completed);
            }
            // after this method is called it never returns to be called            
            base.Update(time);
        }
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ExecuteIf(Channels channels, TaskScheduler scheduler, IDigitalInput condition, bool value,
            Task thenTask, Task elseTask)
            : base(channels)
        {
            this.conditionChannel = condition;
            this.conditionValue = value;
            this.thenTask = thenTask;
            this.elseTask = elseTask;
            this.scheduler = scheduler;
        }

        public ExecuteIf(Channels channels, TaskScheduler scheduler, IDigitalInput condition, bool value, Task thenTask)
            : this(channels, scheduler, condition, value, thenTask, null) { }

        #endregion
    }
}
