using System;
using System.Collections.Generic;

using MTS.IO;


namespace MTS.TesterModule
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
        /// <summary>
        /// Collection of channels from communication with remote hardware. This colleciton is regulary
        /// updated in a loop. New values are writed to remote hardware memory and values from hardware
        /// are writed to this collection
        /// </summary>
        protected Channels channels;
        protected TaskState state;
        protected TaskResult result = new TaskResult();

        #region Properties

        /// <summary>
        /// (Get/Set) Name or short description of this task
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// (Get) True if task has been finished corectly - is completed
        /// </summary>
        public bool IsCompleted { get { return state == TaskState.Completed; } }
        /// <summary>
        /// (Get) True if task is running (BeginExecute method was called)
        /// </summary>
        public bool IsRunning { get { return state == TaskState.Running; } }
        /// <summary>
        /// (Get) Time when task has been started
        /// </summary>
        public TimeSpan BeginTime
        {
            get;
            protected set;
        }
        /// <summary>
        /// (Get) Time of last task's unfinished state. When task is not finished yet, value is the time of last
        /// Update() call
        /// </summary>
        public TimeSpan EndTime
        {
            get;
            protected set;
        }
        /// <summary>
        /// (Get) Time of task duration
        /// </summary>
        public TimeSpan Duration { get { return EndTime.Subtract(BeginTime); } }

        #endregion

        /// <summary>
        /// Event that is raised when task get executed
        /// </summary>
        public event TaskExecutedHandler TaskExecuted;

        /// <summary>
        /// Raise task executed event
        /// </summary>
        protected void RaiseTaskExecuted()
        {
            if (TaskExecuted != null)
                TaskExecuted(this, new TaskExecutedEventArgs(result, EndTime));
        }

        #region Task Execution

        /// <summary>
        /// This method is called only once and initialize task execution. Any task can be reused just
        /// by calling this initialization method
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public void Initialize(TimeSpan time)
        {
            // any task can be reused just by calling this initialization method
            exState = ExState.Initializing;     // this makes the task reusable
            BeginTime = time;
            state = TaskState.Running;
        }

        /// <summary>
        /// Override this method to implement task execution, but also call this implementation. This method
        /// is cyclically called and performs task execution.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public virtual void Update(TimeSpan time)
        {
            EndTime = time;     // last time updated
        }
        /// <summary>
        /// This method is called only once and initialize task result data.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        /// <param name="state">State of task at the end of execution</param>
        public void Finish(TimeSpan time, TaskState state)
        {
            EndTime = time;
            this.state = state;
            result.State = this.state;
            result.Duration = this.Duration;
            RaiseTaskExecuted();

            exState = ExState.None;     // prevnet to do anithig else
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

        #endregion

        #region Time Measurement

        private TimeSpan start;
        /// <summary>
        /// Start to measure time from now. This method is usually used by some test that need to
        /// measure intervals between two states
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        protected void StartWatch(TimeSpan time) { start = time; }
        /// <summary>
        /// Calculate time elapsed since time measurement has been started
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        /// <returns>Time elapsed since StartWatch has been called</returns>
        protected double TimeElapsed(TimeSpan time) { return (time - start).TotalMilliseconds; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will be executed in cyclic loop
        /// </summary>
        /// <param name="channels">Instance of channels collection for communication with hardware</param>
        public Task(Channels channels)
        {
            state = TaskState.NotExecuted;  // task has not been executed yet
            this.channels = channels;       // channels are used for communication with HW
        }

        #endregion
    }

    public class TaskExecutedEventArgs : EventArgs
    {
        public TaskState State
        {
            get { return Result.State; }
        }

        public TaskResult Result
        {
            get;
            protected set;
        }

        public TimeSpan EndTime
        {
            get;
            protected set;
        }

        public TaskExecutedEventArgs(TaskResult result, TimeSpan endTime)
        {
            Result = result;
            EndTime = endTime;
        }
    }

    /// <summary>
    /// Describes current state of any task
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Task has not been executed yet
        /// </summary>
        NotExecuted,
        /// <summary>
        /// Task is running at this time
        /// </summary>
        Running,
        /// <summary>
        /// Task has been aborted by some external force
        /// </summary>
        Aborted,
        /// <summary>
        /// Task has been executed and is completed
        /// </summary>
        Completed,
        /// <summary>
        /// Task has been executed correctly. This value is used only for test tasks
        /// </summary>
        Passed,
        /// <summary>
        /// Task has not been executed correctly and error has been found. This value is used only for test tasks
        /// </summary>
        Failed
    }

    /// <summary>
    /// Describes result of any task. Result is generated when task has been finished (corectly
    /// or aborted)
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// (Get/Set) State of task.
        /// </summary>
        public TaskState State { get; set; }

        /// <summary>
        /// (Get/Set) Duration of executed task
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
    /// <summary>
    /// Describes result of test task. This result also contains testing parameters used for this test task
    /// </summary>
    public class TestTaskResult : TaskResult
    {
        
    }
}
