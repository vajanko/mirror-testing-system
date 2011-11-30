using System;
using System.Collections.Generic;

using MTS.IO;

namespace MTS.TesterModule
{
    class ExecuteIf : Task
    {
        /// <summary>
        /// Channel on which we are observing the value
        /// </summary>
        private IDigitalInput conditionChannel;
        /// <summary>
        /// Value that we are comparing to value on observed channel
        /// </summary>
        private bool conditionValue;
        /// <summary>
        /// Task that will be executed if observed channel has required value
        /// </summary>
        private Task thenTask;
        /// <summary>
        /// Task that will be executed if observed channel has not required value.
        /// If this is null. Nothig will be executed
        /// </summary>
        private Task elseTask;

        /// <summary>
        /// Task scheduler that is executing this task. This is necesary for re-planig task
        /// at runtime
        /// </summary>
        private TaskScheduler scheduler;

        /// <summary>
        /// Check for value on condition channel and executed apropriate task
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    Output.WriteLine("Condition: if ({0} == {1})", conditionChannel.Name,
                        conditionValue);
                    if (conditionChannel.Value == conditionValue)
                        exState = ExState.StateA;
                    else 
                        exState = ExState.StateB;
                    break;
                case ExState.StateA:
                    Output.WriteLine("Changing to task {0}", thenTask.Name);
                    scheduler.ChangeTask(this, thenTask);
                    break;
                case ExState.StateB:
                    if (elseTask != null)
                    {
                        Output.WriteLine("Changing to task {0}", elseTask.Name);
                        scheduler.ChangeTask(this, elseTask);
                    }
                    else
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
        /// Create an instance of task that will execute another task depending on value of particular channel
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="scheduler">Task scheduler that is executing this task. This is necessary for re-planning
        /// task at runtime</param>
        /// <param name="condition">Channel on which we are observing the value</param>
        /// <param name="value">Value that we are comparing to value on "condition" channel</param>
        /// <param name="thenTask">Task that will be executed if "condition" channel has value "value"</param>
        /// <param name="elseTask">Task that will be executed if "condition" channel has not value "value"</param>
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
        /// <summary>
        /// Create an instance of task that will execute another task depending on value of particular channel
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="scheduler">Task scheduler that is executing this task. This is necessary for re-planning
        /// task at runtime</param>
        /// <param name="condition">Channel on which we are observing the value</param>
        /// <param name="value">Value that we are comparing to value on "condition" channel</param>
        /// <param name="thenTask">Task that will be executed if "condition" channel has value "value"</param>
        public ExecuteIf(Channels channels, TaskScheduler scheduler, IDigitalInput condition, bool value, Task thenTask)
            : this(channels, scheduler, condition, value, thenTask, null) { }

        #endregion
    }
}
