using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    public delegate void ExecutedHandler(TaskScheduler sender, EventArgs args);

    public class TaskScheduler
    {
        #region Private Fields

        private Channels channels;

        // these tasks should be executed
        private LinkedList<Task> toExecute = new LinkedList<Task>();
        // tasks that can be executed right now, but its method BeginExecute was not called yet
        private LinkedList<Task> prepared = new LinkedList<Task>();
        // these tasks are executing at this time
        private LinkedList<Task> executing = new LinkedList<Task>();
        // these tasks are already executed - necessary to collect data
        private LinkedList<Task> executed = new LinkedList<Task>();

        #endregion

        public event ExecutedHandler Executed;

        /// <summary>
        /// Raise task executed event
        /// </summary>
        protected void RaiseExecuted()
        {
            if (Executed != null)
                Executed(this, new EventArgs());
        }

        #region Private Methods

        /// <summary>
        /// Handler that is called when some task finishes its execution
        /// </summary>
        /// <param name="sender">Task that has been executed</param>
        /// <param name="args">Event argument that holds task result and status data</param>
        private void taskExecuted(Task sender, TaskExecutedEventArgs args)
        {
            executing.Remove(sender);   // move from executing to executed
            executed.AddFirst(sender);

            if (executing.Count == 0 && toExecute.Count == 0)
                RaiseExecuted();

            // one task finished - we may add new one or more
            Task task;
            do
            {   // add all tasks that may be executed
                task = getNextTask();
                if (task != null)
                    prepared.AddFirst(task);    // BeginExecute will be called on this tasks
            } while (task != null);
        }
        /// <summary>
        /// Find next task that can be executed now. If there is no such a task returns null
        /// </summary>
        private Task getNextTask()
        {
            Task task = null;
            if (executing.Count > 0 || prepared.Count > 0)    // for now only allow sequencial task scheduling
                return null;                                  // if there is some task being executed - no other could be added

            if (toExecute.Count > 0)    // otherwise there are no more tasks
            {   // this is very simple and could be change in future
                task = toExecute.First.Value;
                toExecute.RemoveFirst();
            }
            return task;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add new task to scheduler that is going to be executed
        /// </summary>
        /// <param name="task">An instance of tasks that is going to be executed</param>
        public void AddTask(Task task)
        {
            // register handler for each task that will be called when task is executed
            task.TaskExecuted += new TaskExecutedHandler(taskExecuted);
            toExecute.AddLast(task);    // only task that will be executed are added
        }

        /// <summary>
        /// Prepare scheduler for tasks executing. This method must be called before first Update()
        /// </summary>
        public void Initialize()
        {
            // initialize tasks if necessary
            // add first task to prepared collection
            prepared.AddFirst(toExecute.First.Value);
            toExecute.RemoveFirst();
            // initialize channels
        }
        public void UpdateOutputs(TimeSpan time)
        {
            // begin execute all prepared tasks and add them to executing colletion
            foreach (Task task in prepared)
            {
                task.Initialize(time);
                executing.AddFirst(task);
            }
            prepared.Clear();

            // update all executing tasks
            LinkedListNode<Task> node1 = executing.First, node2;
            while (node1 != null)
            {
                node2 = node1.Next;
                node1.Value.UpdateOutputs(time);    // notice that when updating node1 - it could be removed
                node1 = node2;                      // because of that we hold next node = node2
            }
            // update output channels - values that has been just writed by executing tasks
            channels.UpdateOutputs();
        }
        /// <summary>
        /// Update all executing tasks
        /// </summary>
        /// <param name="time">Time at moment of calling this method</param>
        public void Update(TimeSpan time)
        {
            // update input channels - values that are going to be read by executing tasks
            channels.UpdateInputs();
            // update all executing tasks
            LinkedListNode<Task> node1 = executing.First, node2;
            while (node1 != null)
            {
                node2 = node1.Next;
                node1.Value.Update(time);   // notice that when updating node1 - it could be removed
                node1 = node2;              // because of that we hold next node = node2
            }
        }

        public List<TaskResult> GetResultData()
        {
            throw new NotImplementedException("Return collection of TaskResult for each task");
        }

        #endregion

        #region Constructors

        public TaskScheduler(Channels channels)
        {
            this.channels = channels;
        }

        #endregion
    }
}
