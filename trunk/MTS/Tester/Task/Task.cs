using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Tester.Result;
using MTS.Data.Types;


namespace MTS.Tester
{
    /// <summary>
    /// Represents a method that will handle <paramref name="Task.TaskExecuted"/> event raised when
    /// task is finished
    /// </summary>
    /// <param name="sender">Task that has been executed</param>
    /// <param name="args">Holds result of this task</param>
    public delegate void TaskExecutedHandler(Task sender, TaskExecutedEventArgs args);

    /// <summary>
    /// Base class of each task.
    /// </summary>
    public abstract class Task
    {
        #region Fields

        /// <summary>
        /// Value describing result state of this task
        /// </summary>
        protected TaskResultType resultCode;

        #endregion

        #region Properties

        /// <summary>
        /// (Get/Set) Unique string identifier
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// (Get/Set) Name or short description of this task
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// (Get) Time when task has been started
        /// </summary>
        public DateTime Begin
        {
            get;
            protected set;
        }
        /// <summary>
        /// (Get) Time of last task's unfinished state. When task is not finished yet, value is the time of last
        /// Update() call
        /// </summary>
        public DateTime End
        {
            get;
            protected set;
        }
        /// <summary>
        /// (Get) Time of task duration
        /// </summary>
        public TimeSpan Duration { get { return End.Subtract(Begin); } }
        /// <summary>
        /// (Get) Value indicating if this test task is enabled. If true, task is executed,
        /// otherwise not.
        /// </summary>
        public bool Enabled { get; protected set; }

        public int ScheduleId { get; set; }

        #endregion

        

        /// <summary>
        /// Event that is raised when task get executed
        /// </summary>
        public event TaskExecutedHandler TaskExecuted;

        /// <summary>
        /// Raise task executed event
        /// </summary>
        protected void RaiseTaskExecuted(TaskResult result)
        {
            if (TaskExecuted != null)
                TaskExecuted(this, new TaskExecutedEventArgs(result));
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) - is {2}", Name, GetType().Name, Enabled ? "enabled" : "disabled");
        }

        #region Task Execution

        /// <summary>
        /// This method is called only once and initialize task execution. Any task can be reused just
        /// by calling this initialization method
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public void Initialize(DateTime time)
        {
            // any task can be reused just by calling this initialization method
            exState = ExState.Initializing;     // this makes the task reusable
            Begin = time;
        }

        /// <summary>
        /// Override this method to implement task execution, but also call this implementation. This method
        /// is cyclically called and performs task execution.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public virtual void Update(DateTime time)
        {
            End = time;     // last time updated
        }
        /// <summary>
        /// This method is called only once and initialize task result data.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public void Finish(DateTime time)
        {
            End = time;
            RaiseTaskExecuted(getResult()); // notify that task has been executed
            exState = ExState.None;         // prevent to do anything else
        }

        /// <summary>
        /// Abort this task. It will not be aborted immediately. But the aborting state will be saved and
        /// task will be finished at the next update (call of <see cref="Update"/> method)
        /// </summary>
        public void Abort()
        {   // task will be aborted at the next update
            goTo(ExState.Aborting);
        }

        protected virtual TaskResultType getResultCode()
        {
            return resultCode;
        }
        protected virtual TaskResult getResult()
        {
            // these value are common for all tasks
            // override this method is TestTask and add properties to TaskResult
            return new TaskResult()
            {
                ResultCode = getResultCode(),
                Begin = this.Begin,
                End = this.End,
                HasData = false
            };
        }

        #endregion

        #region Execution State

        /// <summary>
        /// Current state of the task execution. During execution task is passing through various
        /// states. In update method is decided what to do according the current state.
        /// </summary>
        protected ExState exState;
        /// <summary>
        /// Possible states of any task. This include possible states of all tasks.
        /// It is not necessary to handle all of this state in task implementation
        /// </summary>
        protected enum ExState
        {
            /// <summary>
            /// Test is being initialized
            /// </summary>
            Initializing,

            /// <summary>
            /// Test has been initialized and prerequisites has been finished. Testing may be started
            /// </summary>
            Starting,

            /// <summary>
            /// Test is measuring some kind of value (current, time, temperature, ...)
            /// </summary>
            Measuring,

            /// <summary>
            /// Direction light is switched on, current is being measured
            /// </summary>
            BlinkerOn,
            /// <summary>
            /// Direction light is switched off, nothing is being provided
            /// </summary>
            BlinkerOff,

            /// <summary>
            /// Whole mirror is being unfolded
            /// </summary>
            Unfolding,
            /// <summary>
            /// Whole mirror is being folded
            /// </summary>
            Folding,

            /// <summary>
            /// Sucker for pull-off test is down
            /// </summary>
            SuckerIsDown,
            /// <summary>
            /// Sucker for pull-off test is up
            /// </summary>
            SuckerIsUp,
            /// <summary>
            /// Air from the sucker disk is being sucked in. Vacuum is going to be created
            /// </summary>
            Sucking,
            /// <summary>
            /// Air is being blew in the sucker disk. This removes vacuum, so sucker disk may be moved
            /// down. This is usually done when finalizing pull-off test.
            /// </summary>
            Blowing,
            /// <summary>
            /// Some hardware component is being moved up (sucker disk, distance sensors, glass ...)
            /// </summary>
            MoveingUp,
            /// <summary>
            /// Some hardware component is being moved down (sucker disk, distance sensors, glass ...)
            /// </summary>
            MoveingDown,
            /// <summary>
            /// Some hardware component is being moved left
            /// </summary>
            MoveingLeft,
            /// <summary>
            /// Some hardware component is being moved right
            /// </summary>
            MoveingRight,

            StateA,
            StateB,

            /// <summary>
            /// Test is being finalized
            /// </summary>
            Finalizing,
            /// <summary>
            /// Test is being aborted
            /// </summary>
            Aborting,
            /// <summary>
            /// Unspecified state of test, nothing is executed
            /// </summary>
            None
        }

        /// <summary>
        /// Change execution state of this task to given one
        /// </summary>
        /// <param name="state"></param>
        protected void goTo(ExState state)
        {
            exState = state;
        }

        #endregion

        #region Time Measurement

        private DateTime start;
        /// <summary>
        /// Start to measure time from now. This method is usually used by some test that need to
        /// measure intervals between two states
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        protected void StartWatch(DateTime time) { start = time; }
        /// <summary>
        /// Calculate time (in milliseconds) elapsed since time measurement has been started
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        /// <returns>Time elapsed since StartWatch has been called</returns>
        protected double TimeElapsed(DateTime time) { return (time - start).TotalMilliseconds; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will be executed in cyclic loop. By default all tasks may be execute only
        /// sequentially - <see cref="BehaviourType"/> is set to AllDisallowed
        /// </summary>
        public Task()
        {
            Name = GetType().Name;
            Enabled = true;
        }

        #endregion
    }
}
