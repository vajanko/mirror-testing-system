using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.TesterModule
{
    public delegate void SchedulerExecutedHandler(TaskScheduler sender, EventArgs args);

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
        // result of all executed tasks
        private List<TaskResult> results = new List<TaskResult>();       

        #endregion

        #region Properties

        /// <summary>
        /// (Get) Value indicating if there are no more task to be executed
        /// </summary>
        public bool IsFinished { get; private set; }
        /// <summary>
        /// (Get) Value indicating if executing of scheduler is being aborted by some external force
        /// </summary>
        public bool IsAborting { get; private set; }


        #endregion

        public event SchedulerExecutedHandler Executed;

        /// <summary>
        /// Raise task executed event
        /// </summary>
        protected void RaiseExecuted()
        {
            IsFinished = true;
            if (Executed != null)
                Executed(this, new EventArgs());
        }

        #region Task Sequences

        public void AddInitSequence()
        {
            // allow power supply
            this.AddTask(new SetValue(channels, channels.AllowPowerSupply, true));

            // check if power supply is on - we are waiting for false value
            this.AddTask(new WaitForValue(channels, channels.IsPowerSupplyOff, false));

            // open device if not opened
            this.AddOpenDevice();

            // missing checking of mirror presence
            // ...

            // wait for start button to be pressed and released
            this.AddWaitForStart();

            // switch off lights from previous sequence
            SetMultipleValues multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.GreenLightOn, false);
            multi.AddChannel(channels.RedLightOn, false);
            this.AddTask(multi);

            // close device
            this.AddCloseDevice();

            // after this initialization task, tester is prepared to run testing sequence
        }

        public void AddWaitForStart()
        {
            // wait for start button to be pressed
            this.AddTask(new WaitForValue(channels, channels.IsStartPressed, true));
            // and released
            this.AddTask(new WaitForValue(channels, channels.IsStartPressed, false));
        }

        public void AddOpenDevice()
        {
            // open device: multiple values must be set
            // for sure set all channels
            // device must be closed strongly so that it may be opened slowly after

            // this (multi) task will open device
            SetMultipleValues multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.LockStrong, false);   // release strong lock
            multi.AddChannel(channels.LockWeak, false);     // release weak lock
            multi.AddChannel(channels.UnlockWeak, true);    // unlock weak
            multi.AddChannel(channels.UnlockStrong, true);  // unlock strong

            // only open device if it is closed
            // if is old mirror
            //   open if old is closed
            // else
            //   open if new is closed
            this.AddTask(new ExecuteIf(channels, this, channels.IsOldMirror, true,
                new ExecuteIf(channels, this, channels.IsOldLocked, true, multi),
                new ExecuteIf(channels, this, channels.IsLocked, true, multi)));

            // wait for (depending if it is new or old mirror) device to be opened
            this.AddTask(new ExecuteIf(channels, this, channels.IsOldMirror, true,
                new WaitForValue(channels, channels.IsOldLocked, false),
                new WaitForValue(channels, channels.IsLocked, false)));

            // after this tasks has been executed - device is opened
        }

        public void AddCloseDevice()
        {
            // close device weakly (slowly)
            SetMultipleValues multiTask = new SetMultipleValues(channels);
            // for sure set all channels
            multiTask.AddChannel(channels.LockStrong, false);   // release strong lock
            multiTask.AddChannel(channels.UnlockWeak, false);   // release weak unlock
            multiTask.AddChannel(channels.UnlockStrong, false); // release strong unlock
            multiTask.AddChannel(channels.LockWeak, true);      // lock weak
            this.AddTask(multiTask);

            // wait for (depending if it is new or old mirror) device to be closed
            this.AddTask(new ExecuteIf(channels, this, channels.IsOldMirror, true,
                new WaitForValue(channels, channels.IsOldLocked, true),
                new WaitForValue(channels, channels.IsLocked, true)));

            // lock strongly
            this.AddTask(new SetValue(channels, channels.LockStrong, true));
        }

        public void AddDistanceSensorsUp()
        {
            // move sensors up
            SetMultipleValues multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.MoveDistanceSensorDown, false);
            multi.AddChannel(channels.MoveDistanceSensorUp, true);
            this.AddTask(multi);

            // wait for sensors to be up
            this.AddTask(new WaitForValue(channels, channels.IsDistanceSensorUp, true));

            // wait for sensors to be really up
            this.AddTask(new Wait(channels, 1000));
        }

        public void AddDistanceSensorsDown()
        {
            // move sensors down
            SetMultipleValues multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.MoveDistanceSensorDown, true);
            multi.AddChannel(channels.MoveDistanceSensorUp, false);
            this.AddTask(multi);

            // wait for sensors to be down
            this.AddTask(new WaitForValue(channels, channels.IsDistanceSensorDown, true));
        }

        public void AddTravelTests(TestCollection tests)
        {
            // move distance sensors up for measuring
            this.AddDistanceSensorsUp();

            // allow mirror movement
            this.AddTask(new SetValue(channels, channels.AllowMirrorMovement, true));

            // center mirror glass to zero plane saved in HWSettings
            this.AddTask(new CenterTask(channels));
            // move north
            this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelNorth) , MoveDirection.Up));

            // center mirror glass to zero plane saved in HWSettings
            this.AddTask(new CenterTask(channels));
            // move north
            this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelSouth), MoveDirection.Down));

            // center mirror glass to zero plane saved in HWSettings
            this.AddTask(new CenterTask(channels));
            // move north
            this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelWest), MoveDirection.Left));

            // center mirror glass to zero plane saved in HWSettings
            this.AddTask(new CenterTask(channels));
            // move north
            this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelEast), MoveDirection.Right));

            // after travel test has been executed - center mirror back to its zero position
            this.AddTask(new CenterTask(channels));

            // disable mirror movement
            this.AddTask(new SetValue(channels, channels.AllowMirrorMovement, false));

            // move distance sensors down - not necesary at all
            this.AddDistanceSensorsDown();
        }

        public void AddRubberTest(TestCollection tests)
        {
            TestValue test = tests.GetTest(TestCollection.Rubber);
            // add this test if it is enabled
            if (test.Enabled)
                this.AddTask(new ExecuteIf(channels, this, channels.IsLeftMirror, true,
                    new PresenceTest(channels, test, channels.IsLeftRubberPresent),
                    new PresenceTest(channels, test, channels.IsRightRubberPresent)));
        }

        public void AddPulloffTest(TestCollection tests)
        {
            // move sucker up
            SetMultipleValues multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.MoveSuckerUp, true);
            multi.AddChannel(channels.MoveSuckerDown, false);
            this.AddTask(multi);

            // wait for sucker to be up
            this.AddTask(new WaitForValue(channels, channels.IsSuckerUp, true));

            // start suction
            this.AddTask(new SetValue(channels, channels.SuckOn, true));

            // wait for vacuum
            this.AddTask(new WaitForValue(channels, channels.IsVacuum, true));

            // provide test
            this.AddTask(new PulloffTest(channels, tests.GetTest(TestCollection.Pulloff)));

            // stop suction
            this.AddTask(new SetValue(channels, channels.SuckOn, false));
            // start blowing
            this.AddTask(new SetValue(channels, channels.BlowOn, true));

            // move sucker down
            multi = new SetMultipleValues(channels);
            multi.AddChannel(channels.MoveSuckerUp, false);
            multi.AddChannel(channels.MoveSuckerDown, true);
            this.AddTask(multi);

            // wait for sucker to be down
            this.AddTask(new WaitForValue(channels, channels.IsSuckerDown, true));
            // stop blowing
            this.AddTask(new SetValue(channels, channels.BlowOn, false));
        }

        #endregion

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

            // save result
            results.Add(args.Result);

            // check if there are more tasks to be executed
            if (executing.Count == 0 && toExecute.Count == 0)
                RaiseExecuted();

            // one task finished - we may add new one or more
            Task task;
            do
            {   // add all tasks that may be executed
                task = getNextTask();
                if (task != null)
                    prepared.AddFirst(task);    // Initialize will be called on this task at next Update()
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
        /// Change currently executing taks for another one
        /// </summary>
        /// <param name="executingTask">Task that is currently being executed</param>
        /// <param name="newTask">Task that shall be executed instead of this one</param>
        public void ChangeTask(Task executingTask, Task newTask)
        {
            executingTask.TaskExecuted -= new TaskExecutedHandler(taskExecuted);
            // remove executing task from executing collection
            executing.Remove(executingTask);
            // move it to executed collection
            executed.AddFirst(executingTask);
            // add new task to collection of prepared tasks at the first position
            // - this will execute it in the next update
            newTask.TaskExecuted += new TaskExecutedHandler(taskExecuted);  // this will remove it when finished
            prepared.AddFirst(newTask);
        }

        #region Lifecycle

        /// <summary>
        /// Prepare scheduler for tasks executing. This method must be called before first Update()
        /// </summary>
        public void Initialize()
        {
            IsFinished = false;
            IsAborting = false;

            // nothing to execute
            if (toExecute.Count <= 0)
            {   // raise event that execution has finished
                RaiseExecuted();
                return;
            }
            // initialize tasks if necessary
            // add first task to prepared collection
            prepared.AddFirst(toExecute.First.Value);
            toExecute.RemoveFirst();
            // initialize channels
        }
        /// <summary>
        /// Update all executing tasks
        /// </summary>
        /// <param name="time">Time at moment of calling this method</param>
        public void Update(DateTime time)
        {
            // begin execute all prepared tasks and add them to executing colletion
            foreach (Task task in prepared)
            {
                task.Initialize(time);
                executing.AddFirst(task);
            }
            prepared.Clear();           // prepared tasks became executing

            // write all outputs and read inputs (in this order)
            channels.Update();

            // update all executing tasks
            LinkedListNode<Task> node1 = executing.First, node2;
            while (node1 != null)
            {
                node2 = node1.Next;
                node1.Value.Update(time);   // notice that when updating node1 - it could be removed
                node1 = node2;              // because of that we hold next node = node2
            }
        }

        #endregion

        public TaskResultCode GetResultCode()
        {
            // check if scheduler has been aborted by some external force
            if (IsAborting)
                return TaskResultCode.Aborted;

            // check if there is some failed task - then result is Failed
            foreach (TaskResult result in results)
                if (result.ResultCode == TaskResultCode.Failed)
                    return TaskResultCode.Failed;
            // otherwise everythig finished correctly
            return TaskResultCode.Completed;
        }
        public bool AreAllPassed()
        {
            bool ret = true;
            foreach (TaskResult result in results)
                if (result != null && result.ResultCode == TaskResultCode.Completed)
                    ret = false;
            return ret;
        }

        public List<TaskResult> GetResultData()
        {
            return results;
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
