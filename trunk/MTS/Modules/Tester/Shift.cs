using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Diagnostics;

using MTS.IO;
using MTS.Editor;
using MTS.Data;
using MTS.Data.Converters;
using MTS.Data.Types;
using MTS.Tester.Result;


namespace MTS.TesterModule
{
    public delegate void ShiftExecutedHandler(Shift sender, EventArgs args);

    public class Shift : INotifyPropertyChanged
    {
        #region Private Fields

        private Channels channels;
        private TestCollection shiftTests;
        private TaskScheduler scheduler;
        private Thread loop;
        private Data.Shift dbShift;
        MTSContext context;
        
        /// <summary>
        /// Id of mirror tested in this shift
        /// </summary>
        private int mirrorId;
        /// <summary>
        /// Id of operator who has executed this shift
        /// </summary>
        private int operatorId;

        #endregion

        public event ShiftExecutedHandler Executed;

        /// <summary>
        /// Raise task executed event
        /// </summary>
        protected void RaiseExecuted()
        {
            if (Executed != null)
                Executed(this, new EventArgs());
        }

        #region Properties

        private int mirrors = 0;
        /// <summary>
        /// (Get/Set) Number of mirrors to test
        /// </summary>
        public int Mirrors
        {
            get { return mirrors; }
            set
            {
                if (value < 1) value = 1;
                mirrors = value;
                NotifyPropretyChanged("Mirrors");
            }
        }
        /// <summary>
        /// (Get) Number of finised tests
        /// </summary>
        public int Finished
        {
            get { return Passed + Failed; }
        }
        private int passed = 0;
        /// <summary>
        /// (Get/Set) Number of correnct finished tests
        /// </summary>
        public int Passed
        {
            get { return passed; }
            set
            {
                passed = value;
                NotifyPropretyChanged("Passed");
            }
        }
        private int failed = 0;
        /// <summary>
        /// (Get/Set) Number of finished tests that have failed
        /// </summary>
        public int Failed
        {
            get { return failed; }
            set
            {
                failed = value;
                NotifyPropretyChanged("Failed");
            }
        }
        /// <summary>
        /// (Get) Number of mirror that remain to be tested
        /// </summary>
        public int Remained
        {
            get { return Mirrors - Finished; }
        }
        /// <summary>
        /// (Get) Value indicating if schift is running or not
        /// </summary>
        public bool IsRunning
        {
            get {  return (loop != null) ? loop.IsAlive : false; }  // true is thread is alive
        }
        /// <summary>
        /// (Get) Date and time when shift has been started
        /// </summary>
        public DateTime Begin { get; private set; }
        /// <summary>
        /// (Get) Date and time when shift has been finished
        /// </summary>
        public DateTime End { get; private set; }


        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise an PropertyChanged event that signalized that some property has been changed
        /// </summary>
        /// <param name="name">Name of the propety that has been changed</param>
        public void NotifyPropretyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Private Methods

        private void setSafeStateOutputs(Channels channels)
        {
            if (channels != null)
            {
                channels.StopMirror();
                channels.StopPowerfold();
                channels.StopAir();

                // stop direction light
                channels.DirectionLightOn.Value = false;
                // stop heating
                channels.HeatingFoilOn.Value = false;

                // move distance sensors down
                channels.MoveDistanceSensorUp.Value = false;
                channels.MoveDistanceSensorDown.Value = true;


                // switch off lights
                channels.RedLightOn.Value = false;
                channels.GreenLightOn.Value = false;

                // write to HW
                channels.UpdateOutputs();
            }
        }

        /// <summary>
        /// Create a new instance of scheduler and add task to it. At the time of calling channels must be created
        /// and connection must be established.
        /// No exception should be thrown!
        /// </summary>
        /// <param name="channels">Collection of hardware channels for task initialization</param>
        /// <returns>New instance of initialized TaskScheduler</returns>
        private TaskScheduler createScheduler(Channels channels, TestCollection tests)
        {
            TaskScheduler scheduler = new TaskScheduler(channels);

            // add for providing basic steps to start executing test
            // this contains tasks such as: open device, wait for mirror to be inserted, wait for start button,
            // close device
            //scheduler.AddInitSequence();

            // wait for start
            //scheduler.AddWaitForStart();

            // rubber test
            //scheduler.AddRubberTest(tests);

            //// wait for start
            //scheduler.AddWaitForStart();
            //// add tests of mirror movement
            //scheduler.AddTravelTests(tests);

            //// wait for start
            //scheduler.AddWaitForStart();
            //// pull-off test
            //scheduler.AddPulloffTest(tests);

            // wait for start
            //scheduler.AddWaitForStart();
            // test powerfold
            //scheduler.AddTask(new PowerfoldTest(channels, tests.GetTest(TestCollection.Powerfold)));

            // wait for start
            //scheduler.AddWaitForStart();
            // test blinker
            //scheduler.AddTask(new BlinkerTest(channels, tests.GetTest(TestCollection.DirectionLight)));

            // wait for start
            //scheduler.AddWaitForStart();
            // test spiral
            scheduler.AddTask(new SpiralTest(channels, tests.GetTest(TestCollection.Heating)));

            // open device
            //scheduler.AddOpenDevice();

            // add first task to be executed
            scheduler.Initialize();
            //// register handler to execute when scheduler finishes its work
            ////scheduler.Executed += new SchedulerExecutedHandler(schedulerExecuted);

            return scheduler;
        }

        private void executeLoop()
        {
            Stopwatch watch = new Stopwatch();  // time elapsed since last loop
            DateTime time = DateTime.Now;       // current time of updating scheduler

            Output.WriteLine("Execution loop started!");
            watch.Start();      // start measuring execution time

            // while there are some mirrors to be tested
            while (Remained > 0)
            {
                Output.WriteLine("Test sequence {0} started", Finished + 1);

                // create scheduler with tasks to be executed
                scheduler = createScheduler(channels, shiftTests);

                // this loop tests one mirror
                while (!scheduler.IsFinished)
                {
                    Thread.Sleep(200);              // for presentation purpose only
                    time += watch.Elapsed;          // caluculate current time (DateTime.Now is not very exactly)
                    scheduler.Update(time);         // execute tasks
                }

                Output.WriteLine("Test sequence {0} finished", Finished + 1);

                // write outputs to safe state
                setSafeStateOutputs(channels);

                // handle results
                handleResults(scheduler);
            }
            watch.Stop();       // stop measuring execution time

            // finalize shift execution
            Finish();
            Output.WriteLine("Execution loop finished!");
        }

        private void handleResults(TaskScheduler scheduler)
        {
            // first of all display result to user - saveing may be spent some period of time
            string msg;
            TaskResultCode resultCode = scheduler.GetResultCode();
            if (resultCode == TaskResultCode.Completed)
            {
                msg = "Mirror passed!";
                channels.GreenLightOn.On();
                Passed++;
            }
            else if (resultCode == TaskResultCode.Failed)
            {
                msg = "Mirror failed!";
                channels.RedLightOn.On();
                Failed++;
            }
            else
            {
                msg = "Aborted!";
            }

            Output.WriteLine("Result: " + msg);
            // switch light (green or red) on
            channels.UpdateOutputs();

            this.End = DateTime.Now;

            Output.Write("Saveing results do database ... ");
            List<TaskResult> results = scheduler.GetResultData();
            saveShiftResult(results);
            Output.WriteLine("Saved!");

            // wait a moment to display light
            Thread.Sleep(1000);
        }

        #region Database Methods

        private void initTests()
        {
            // 1) remove all disabled test - will not be executed
            List<TestValue> toRemove = new List<TestValue>();
            foreach (TestValue test in shiftTests)
                if (!test.Enabled)
                    toRemove.Add(test);
            foreach (TestValue test in toRemove)
                shiftTests.RemoveTest(test);

            // 2) save tests that will be used to database
            foreach (TestValue test in shiftTests)
            {
                // 2.1) save used test to database
                Test dbTest = context.Tests.FirstOrDefault(t => t.Name == test.ValueId);
                if (dbTest == null)
                {   // check if such a test already exists
                    dbTest = context.Tests.Add(new Test { Name = test.ValueId });
                    context.SaveChanges();  // this will generate a new id for this test
                }
                test.DatabaseId = dbTest.Id;

                // 2.2) save used test parameters to database
                foreach (ParamValue param in test)
                {
                    // 2.2.1) save used parameter to database
                    string strValue = param.ValueToString();
                    Param dbParam = context.Params.FirstOrDefault(p => p.Name == param.ValueId && p.Value == strValue);
                    if (dbParam == null)
                    {   // if it does not exists create a new one
                        byte valueType = (byte)param.ValueType();
                        dbParam = context.Params.Add(new Param { Name = param.ValueId, Type = valueType, Value = strValue });
                        context.SaveChanges();
                    }

                    // 2.2.2) save relationship between test and param if it does not exists yet
                    TestParam dbTestParam = context.TestParams
                        .FirstOrDefault(tp => tp.TestId == dbTest.Id && tp.ParamId == dbParam.Id);
                    if (dbTestParam == null)
                    {
                        dbTestParam = context.TestParams.Add(new TestParam { TestId = dbTest.Id, ParamId = dbParam.Id });
                    }

                    param.DatabaseId = dbParam.Id;
                }
            }
            context.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usedTests">Only tests that will be executed (not disabled)</param>
        private void createShift(TestCollection usedTests)
        {
            // 1) create a new instance of shift and save it to database
            mirrorId = context.Mirrors.Select(m => m.Id).First();   // depends on parameter settings
            operatorId = Admin.Operator.Instance.Id;                // loged in operator id
            dbShift = context.Shifts.Add(new Data.Shift
            {
                Start = DateTime.Now,
                Finish = DateTime.Now,      // must be initialized otherwise exception is thrown
                MirrorId = mirrorId,
                OperatorId = operatorId
            });
            context.SaveChanges();

            // 2) save information about used tests in current shift
            foreach (TestValue tv in usedTests)
            {
                context.TestShifts.Add(new TestShift { ShiftId = dbShift.Id, TestId = tv.DatabaseId });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        private void saveShiftResult(IEnumerable<TaskResult> results)
        {
            // this converter is used to convert object value to its string representation when
            // saveing to and retriving from database
            ParamTypeConverter converter = new ParamTypeConverter();
            Data.Shift dbShift = this.dbShift;

            foreach (TaskResult tRes in results)
            {
                if (tRes.HasData)
                {
                    // save test output
                    TestOutput dbTestOutput = context.TestOutputs.Add(new TestOutput
                    {
                        Result = (byte)tRes.ResultCode,
                        Start = tRes.Begin,
                        Finish = tRes.End,
                        ShiftId = dbShift.Id,
                        TestId = tRes.DatabaseId
                    });
                    context.SaveChanges();      // generate id

                    foreach (ParamResult pRes in tRes.Params)
                    {
                        // save param output (notice that if could be null also)
                        context.ParamOutputs.Add(new ParamOutput
                        {
                            ParamId = pRes.DatabaseId,
                            TestOutpuId = dbTestOutput.Id,
                            Value = pRes.ResultStringValue
                        });
                    }
                }
            }
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        private void saveShift()
        {
            dbShift.Finish = DateTime.Now;
            context.SaveChanges();
        }

        #endregion

        #endregion

        #region Public Methods

        public void Initialize()
        {
            Passed = 0;     // nothing is finished yet
            Failed = 0;

            // first update, so we do not use uninitialzed values
            channels.UpdateInputs();

            // setting value as this does not raise an event
            // an event is raised after that channels are updated
            channels.AllowPowerSupply.Value = true;

            // write to hardware value allowing power supply
            channels.UpdateOutputs();

            // while power supply is not switched on
            while (channels.IsPowerSupplyOff.Value == true)
            {
                // if power supply is not switch on, ask user to switch it on
                MessageBoxResult result = MessageBox.Show("Switch on power supply!", "Power supply",
                    MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK);
                if (result == MessageBoxResult.Cancel)  // abort if user does not switched on power supply
                {
                    Abort();
                    return;
                }
                // read value - check if user has switch on power supply
                channels.UpdateInputs();
            }
            // create a new thread for execution loop
            loop = new Thread(new ThreadStart(executeLoop));
        }
        public void Start()
        {   // save date and time when shift has been started - necessary for database
           
            // test
            context = new MTSContext();
            // prepare test collection for execution - remove disabled tests and the rest save to database
            // remember test and parameter id in database
            initTests();
            // create and save a new instance of this to database
            createShift(shiftTests);

            // only start execution loop if it is not started yet
            if (loop != null && !loop.IsAlive)
                loop.Start();
        }
        public void Finish()    // must be private method
        {
            // switch off everythig dangerous
            setSafeStateOutputs(channels);

            // save date and time when shift has been finished - necessary for database
            saveShift();

            context.Dispose();

            Output.WriteLine("Shift finished!");

            // raise notify event that shift has been finished
            RaiseExecuted();    // here data should be saved
        }
        public void Abort()
        {
            // abort executing thread
            // revision needed !!!
            if (loop != null)
                loop.Abort();

            setSafeStateOutputs(channels);

            Output.WriteLine("Shift aborted!");

            RaiseExecuted();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of shift. When shift is created connection already must be established
        /// and channels created and initialized
        /// </summary>
        /// <param name="channels">Communication layer for hardware</param>
        /// <param name="tests">Collection of testing parameters</param>
        public Shift(Channels channels, TestCollection tests)
        {
            this.channels = channels;
            this.shiftTests = tests;
            Mirrors = 1;        // default count
        }

        #endregion
    }
}
