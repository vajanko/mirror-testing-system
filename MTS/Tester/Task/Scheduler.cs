using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using OK.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;
using MTS.Data.Types;
using System.Runtime;

namespace MTS.Tester
{
    public delegate void SchedulerExecutedHandler(TaskScheduler sender, EventArgs args);

    public class TaskScheduler
    {
        #region Private Fields

        /// <summary>
        /// Communication layer - connection with hw module
        /// </summary>
        private Channels channels;

        private IModule module;

        private IOrientedGraph requiredGraph;
        private IGraph disallowedGraph;
        private List<Task> tasks;

        /// <summary>
        /// These tasks should be executed
        /// </summary>
        private Queue<Task> toExecute = new Queue<Task>();
        /// <summary>
        /// Tasks that can be executed right now, but its method Initialize has not called yet
        /// </summary>
        private LinkedList<Task> prepared = new LinkedList<Task>();
        /// <summary>
        /// These tasks are executing at this time
        /// </summary>
        private LinkedList<Task> executing = new LinkedList<Task>();
        /// <summary>
        /// These tasks are already executed - necessary to collect data
        /// </summary>
        private LinkedList<Task> executed = new LinkedList<Task>();
        /// <summary>
        /// Result of all executed tasks
        /// </summary>
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

        /// <summary>
        /// (Get) Number of executed tasks
        /// </summary>
        public int ExecutedTasks { get { return executed.Count; } }
        /// <summary>
        /// (Get) Number of currently executing tasks
        /// </summary>
        public int ExecutingTasks { get { return executing.Count; } }

        #endregion

        #region Task Sequences

        public void AddInitSequence()
        {
            //// allow power supply
            //this.AddTask(new SetChannels(channels.AllowPowerSupply, true));

            //// check if power supply is on - we are waiting for false value
            //this.AddTask(new WaitForChannels(channels.IsPowerSupplyOff, false));

            //// open device if not opened
            //this.AddOpenDevice();

            //// missing checking of mirror presence
            //// ...

            //// wait for start button to be pressed and released
            //this.AddWaitForStart();

            //// switch off lights from previous sequence
            //SetMultipleValues multi = new SetMultipleValues();
            //multi.AddChannel(channels.GreenLightOn, false);
            //multi.AddChannel(channels.RedLightOn, false);
            //this.AddTask(multi);

            //// close device
            //this.AddCloseDevice();

            //// after this initialization task, tester is prepared to run testing sequence
        }

        public void AddWaitForStart()
        {
            //// wait for start button to be pressed
            //this.AddTask(new WaitForChannels(channels.IsStartPressed, true));
            //// and released
            //this.AddTask(new WaitForChannels(channels.IsStartPressed, false));
        }

        public void AddOpenDevice()
        {
            //// open device: multiple values must be set
            //// for sure set all channels
            //// device must be closed strongly so that it may be opened slowly after

            //// this (multi) task will open device
            //SetMultipleValues multi = new SetMultipleValues();
            //multi.AddChannel(channels.LockStrong, false);   // release strong lock
            //multi.AddChannel(channels.LockWeak, false);     // release weak lock
            //multi.AddChannel(channels.UnlockWeak, true);    // unlock weak
            //multi.AddChannel(channels.UnlockStrong, true);  // unlock strong

            //// only open device if it is closed
            //// if is old mirror
            ////   open if old is closed
            //// else
            ////   open if new is closed
            //this.AddTask(new IfChannels(this, channels.IsOldMirror, true,
            //    new IfChannels(this, channels.IsOldLocked, true, multi),
            //    new IfChannels(this, channels.IsLocked, true, multi)));

            //// wait for (depending if it is new or old mirror) device to be opened
            //this.AddTask(new IfChannels(this, channels.IsOldMirror, true,
            //    new WaitForChannels(channels.IsOldLocked, false),
            //    new WaitForChannels(channels.IsLocked, false)));

            //// after this tasks has been executed - device is opened
        }

        public void AddCloseDevice()
        {
            //// close device weakly (slowly)
            //SetMultipleValues multiTask = new SetMultipleValues();
            //// for sure set all channels
            //multiTask.AddChannel(channels.LockStrong, false);   // release strong lock
            //multiTask.AddChannel(channels.UnlockWeak, false);   // release weak unlock
            //multiTask.AddChannel(channels.UnlockStrong, false); // release strong unlock
            //multiTask.AddChannel(channels.LockWeak, true);      // lock weak
            //this.AddTask(multiTask);

            //// wait for (depending if it is new or old mirror) device to be closed
            //this.AddTask(new IfChannels(this, channels.IsOldMirror, true,
            //    new WaitForChannels(channels.IsOldLocked, true),
            //    new WaitForChannels(channels.IsLocked, true)));

            //// lock strongly
            //this.AddTask(new SetChannels(channels.LockStrong, true));
        }

        public void AddDistanceSensorsUp()
        {
            //// move sensors up
            //SetMultipleValues multi = new SetMultipleValues();
            //multi.AddChannel(channels.MoveDistanceSensorDown, false);
            //multi.AddChannel(channels.MoveDistanceSensorUp, true);
            //this.AddTask(multi);

            //// wait for sensors to be up
            //this.AddTask(new WaitForChannels(channels.IsDistanceSensorUp, true));

            //// wait for sensors to be really up
            //this.AddTask(new Wait(1000));
        }

        public void AddDistanceSensorsDown()
        {
            //// move sensors down
            //SetMultipleValues multi = new SetMultipleValues();
            //multi.AddChannel(channels.MoveDistanceSensorDown, true);
            //multi.AddChannel(channels.MoveDistanceSensorUp, false);
            //this.AddTask(multi);

            //// wait for sensors to be down
            //this.AddTask(new WaitForChannels(channels.IsDistanceSensorDown, true));
        }

        public void AddTravelTests(TestCollection tests)
        {
            //TestValue north = tests.GetTest(TestCollection.TravelNorth);
            //TestValue south = tests.GetTest(TestCollection.TravelSouth);
            //TestValue west = tests.GetTest(TestCollection.TravelWest);
            //TestValue east = tests.GetTest(TestCollection.TravelEast);

            //// if all travel tests are disabled, no prerequisites are necessary to be done
            //if (north == null && south == null && west == null && east == null)
            //    return;

            //// prerequisites:
            //// move distance sensors up for measuring
            //this.AddDistanceSensorsUp();
            //// allow mirror movement
            //this.AddTask(new SetChannels(channels.AllowMirrorMovement, true));

            //if (north != null)
            //{
            //    // center mirror glass to zero plane saved in HWSettings
            //    this.AddTask(new CenterTask(channels));
            //    // move north
            //    this.AddTask(new TravelTest(channels, north, MoveDirection.Up));
            //}

            //if (south != null)
            //{
            //    // center mirror glass to zero plane saved in HWSettings
            //    this.AddTask(new CenterTask(channels));
            //    // move north
            //    this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelSouth), MoveDirection.Down));
            //}

            //if (west != null)
            //{
            //    // center mirror glass to zero plane saved in HWSettings
            //    this.AddTask(new CenterTask(channels));
            //    // move north
            //    this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelWest), MoveDirection.Left));
            //}

            //if (east != null)
            //{
            //    // center mirror glass to zero plane saved in HWSettings
            //    this.AddTask(new CenterTask(channels));
            //    // move north
            //    this.AddTask(new TravelTest(channels, tests.GetTest(TestCollection.TravelEast), MoveDirection.Right));
            //}

            //// mirror must not be centered at the end - operator will do this manually

            //// disable mirror movement
            //this.AddTask(new SetChannels(channels.AllowMirrorMovement, false));

            //// move distance sensors down - not necessary at all
            //this.AddDistanceSensorsDown();
        }

        public void AddRubberTest(TestValue test)
        {
            //Task task =new IfChannels(this, channels.IsLeftMirror, true,
            //    new PresenceTest(channels, test, channels.IsLeftRubberPresent),
            //    new PresenceTest(channels, test, channels.IsRightRubberPresent));
            //AddTask(task);
        }

        public void AddPulloffTest(TestValue test)
        {
            //// move sucker up
            //SetMultipleValues multi = new SetMultipleValues();
            //multi.AddChannel(channels.MoveSuckerUp, true);
            //multi.AddChannel(channels.MoveSuckerDown, false);
            //this.AddTask(multi);

            //// wait for sucker to be up
            //this.AddTask(new WaitForChannels(channels.IsSuckerUp, true));

            //// start suction
            //this.AddTask(new SetChannels(channels.SuckOn, true));

            //// wait for vacuum
            //this.AddTask(new WaitForChannels(channels.IsVacuum, true));

            //// provide test
            //this.AddTask(new PulloffTest(channels, test));

            //// stop suction
            //this.AddTask(new SetChannels(channels.SuckOn, false));
            //// start blowing
            //this.AddTask(new SetChannels(channels.BlowOn, true));

            //// move sucker down
            //multi = new SetMultipleValues();
            //multi.AddChannel(channels.MoveSuckerUp, false);
            //multi.AddChannel(channels.MoveSuckerDown, true);
            //this.AddTask(multi);

            //// wait for sucker to be down
            //this.AddTask(new WaitForChannels(channels.IsSuckerDown, true));
            //// stop blowing
            //this.AddTask(new SetChannels(channels.BlowOn, false));
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

            // one task finished - we may add new one or more
            Task task;
            Output.Write("scheduling: ");
            do
            {   // add all tasks that may be executed
                task = getNextTask();
                if (task != null)
                {
                    prepared.AddFirst(task);    // Initialize will be called on this task at next Update()
                    Output.Write("{0} - ", task.Name);
                }
            } while (task != null);
            Output.WriteLine("");
            // 
            if (prepared.Count == 0 && executing.Count == 0)
                IsFinished = true;
        }
        /// <summary>
        /// Find next task that can be executed now. If there is no such a task returns null
        /// </summary>
        private Task getNextTask()
        {   // task selector

            Task task = null;
            //if (executing.Count > 0 || prepared.Count > 0)    // for now only allow sequential task scheduling
            //    return null;                                  // if there is some task being executed - no other could be added

            //if (toExecute.Count > 0)    // otherwise there are no more tasks
            //{   // this is very simple and could be change in future
            //    task = toExecute.First.Value;
            //    toExecute.RemoveFirst();
            //}

            if (toExecute.Count > 0)
            {
                Task t = toExecute.Peek();
                
                if (t.Enabled &&
                    executing.Concat(prepared).All(e => !requiredGraph.Contains(e.ScheduleId, t.ScheduleId) &&
                    !disallowedGraph.Contains(e.DisallowId, t.DisallowId)))
                    task = toExecute.Dequeue();
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
            toExecute.Enqueue(task);    // only task that will be executed are added
            // register handler for each task that will be called when task is executed
            task.TaskExecuted += new TaskExecutedHandler(taskExecuted);
        }
        /// <summary>
        /// Change currently executing tasks for another one
        /// </summary>
        /// <param name="executingTask">Task that is currently being executed</param>
        /// <param name="newTask">Task that shall be executed instead of this one</param>
        public void ChangeTask(Task executingTask, Task newTask)
        {
            // overlay executing task with new task
            newTask.ScheduleId = executingTask.ScheduleId;
            newTask.DisallowId = executingTask.DisallowId;

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

        public void Load(string filename, TestCollection tests)
        {
            try
            {
                // load configuration file
                XDocument doc = XDocument.Load(filename);
                TaskParser parser = new TaskParser(doc.Element("timeline"), tests, channels, this);

                parser.ParseTasks(out requiredGraph, out disallowedGraph, out tasks);

                // all tasks in topological order that are enabled
                List<int> order = requiredGraph.TopologicalSort().ToList();
                foreach (var taskId in order.Where(i => i < tasks.Count && tasks[i].Enabled))
                {
                    Task task = tasks[taskId];
                    task.ScheduleId = taskId;
                    AddTask(tasks[taskId]);
                }
                foreach (var taskId in disallowedGraph.GetVertices())
                {
                    Task task = tasks[taskId];
                    task.DisallowId = taskId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Load(string filename)
        {   // sometimes tests are unnecessary
            Load(filename, null);
        }

        /// <summary>
        /// Prepare scheduler for tasks executing. This method must be called before first Update()
        /// </summary>
        public void Initialize()
        {
            IsFinished = false;
            IsAborting = false;

            // nothing to execute
            if (toExecute.Count <= 0)
                return;
            // initialize tasks if necessary
            // add first task to prepared collection
            prepared.AddFirst(toExecute.Dequeue());
            // initialize channels
        }
        /// <summary>
        /// Update all executing tasks
        /// </summary>
        /// <param name="time">Current system time</param>
        public void Update(DateTime time)
        {
            // begin execute all prepared tasks and add them to executing collection
            foreach (Task task in prepared)
            {
                task.Initialize(time);
                executing.AddFirst(task);
            }
            prepared.Clear();           // prepared tasks became executing

            // "disable" garbage collection for a moment
            var lastMode = GCSettings.LatencyMode;
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            // write all outputs and read inputs (in this order)
            channels.Update();
            GCSettings.LatencyMode = lastMode;

            // update all executing tasks
            LinkedListNode<Task> node1 = executing.First, node2;
            while (node1 != null)
            {
                node2 = node1.Next;
                node1.Value.Update(time);   // notice that when updating node1 - it could be removed
                node1 = node2;              // because of that we hold next node = node2
            }
        }
        /// <summary>
        /// Abort all executing tasks and set safe state on the channels
        /// </summary>
        /// <param name="time">Current system time</param>
        public void Abort(DateTime time)
        {   // change state of scheduler to aborting
            IsAborting = true;

            // switch the module to save state - switch off all dangerous channels
            channels.SetupSafeState();
            // remove all tasks that have not been initialized yet
            toExecute.Clear();
            prepared.Clear();

            // change states of all executing tasks to aborting
            foreach (Task task in executing)
                task.Abort();
            // update all executing tasks to abort themselves
            Update(time);

            // scheduler has finished its execution
            IsFinished = true;
        }

        #endregion

        /// <summary>
        /// Calculate value indicating whether all task have been executed correctly and finished with
        /// <see cref="TaskResultType.Completed"/> state
        /// </summary>
        /// <returns><see cref="TaskResultType.Completed"/> if all task have been executed correctly
        /// or <see cref="TaskResultType.Failed"/> if at least one task failed or 
        /// <see cref="TaskResultType.Aborted"/> if scheduler was aborted by calling <see cref="Abort"/> method</returns>
        public TaskResultType GetResultCode()
        {
            // check if scheduler has been aborted by some external force
            if (IsAborting)
                return TaskResultType.Aborted;

            // check if there is some failed task - then result is Failed
            foreach (TaskResult result in results)
                if (result.ResultCode == TaskResultType.Failed)
                    return TaskResultType.Failed;
            // otherwise everything finished correctly
            return TaskResultType.Completed;
        }
        /// <summary>
        /// Calculate value indicating whether all task have been executed correctly and finished with
        /// <see cref="TaskResultType.Completed"/> state
        /// </summary>
        /// <returns>True if all task have been executed correctly</returns>
        public bool AreAllPassed()
        {
            bool ret = true;
            foreach (TaskResult result in results)
                if (result != null && result.ResultCode == TaskResultType.Completed)
                    ret = false;
            return ret;
        }
        /// <summary>
        /// Get result data of all task that have been executed or at least aborted during the execution.
        /// This data does not include task that have been scheduled to be executed but weren't
        /// </summary>
        /// <returns>Collection of result data for all executed (finished or aborted) tasks</returns>
        public List<TaskResult> GetResultData()
        {
            return results;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of task scheduler passing it a communication layer for hardare module
        /// </summary>
        /// <param name="channels">Communication layer of hardware module. Collection of all channels used to 
        /// read or write value from or to hardware module</param>
        public TaskScheduler(Channels channels)
        {
            this.channels = channels;
            this.module = channels;
        }

        #endregion
    }
}
