using System;
using System.Collections.Generic;

using MTS.AdminModule;


namespace MTS.TesterModule
{
    /// <summary>
    /// Represents a method that will handle <paramref name="Task.TaskExecuted"/> event raised when
    /// task is finished
    /// </summary>
    /// <param name="sender">Task that has been executed</param>
    /// <param name="args">Holds result of this task</param>
    public delegate void TaskExecutedHandler(Task sender, TaskExecutedEventArgs args);

    public abstract class Task
    {
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
        /// Override this method to implement task execution, but also call this implementation. This method
        /// is called only once and initialize task execution.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public virtual void Initialize(TimeSpan time)
        {
            BeginTime = time;
            state = TaskState.Running;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        public virtual void UpdateOutputs(TimeSpan time)
        {
            EndTime = time;
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
        /// Overrirde this method to implement task execution, but also call this implementation. This method
        /// is called only once and initialize task result data.
        /// </summary>
        /// <param name="time">Current time - time of system clock when this method is called</param>
        /// <param name="state">State of task at the end of execution</param>
        public virtual void Finish(TimeSpan time, TaskState state)
        {
            EndTime = time;
            this.state = state;
            result.State = this.state;
            result.Duration = this.Duration;
            RaiseTaskExecuted();
        }

        #endregion

        #region Constructors

        public Task(Channels channels)
        {
            state = TaskState.NotExecuted;
            this.channels = channels;
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
