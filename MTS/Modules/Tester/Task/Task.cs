using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Tester.Result;


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
        /// Collection of channels from communication with remote hardware. This colleciton is regulary
        /// updated in a loop. New values are writed to remote hardware memory and values from hardware
        /// are writed to this collection
        /// </summary>
        protected Channels channels;

        /// <summary>
        /// Value describing result state of this task
        /// </summary>
        protected TaskResultCode resultCode;

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
            exState = ExState.None;         // prevent to do anythig else
        }

        protected void chCode(TaskResultCode resultCode)
        {
            this.resultCode = resultCode;
        }
        protected virtual TaskResultCode getResultCode()
        {
            return resultCode;
        }
        protected virtual TaskResult getResult()
        {
            // these value are common for all tasks
            // overrirde this method is TestTask and add properties to TaskResult
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
        /// Current state of the task execution. During execution task is passing throught various
        /// states. In update method is desided what to do according the current state.
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
            /// Whole mirror is being unfoled
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
            /// Air from the sudker disk is being sucked in. Vacuum is going to be created
            /// </summary>
            Sucking,
            /// <summary>
            /// Air is being blowed in the sucker disk. This removes vacuum, so sucker disk may be moved
            /// down. This is usually done when finilizing pull-off test.
            /// </summary>
            Blowing,
            /// <summary>
            /// Some hardware component is being moved up (sucker disk, distance sensors, galss ...)
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
            /// Unspecified state of test, nothig is executed
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
        /// Calculate time elapsed since time measurement has been started
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        /// <returns>Time elapsed since StartWatch has been called</returns>
        protected double TimeElapsed(DateTime time) { return (time - start).TotalMilliseconds; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will be executed in cyclic loop
        /// </summary>
        /// <param name="channels">Instance of channels collection for communication with hardware</param>
        public Task(Channels channels)
        {            
            this.channels = channels;       // channels are used for communication with HW
        }

        #endregion
    }
}
