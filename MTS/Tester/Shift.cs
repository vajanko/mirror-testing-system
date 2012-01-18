using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;
using System.Diagnostics;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Data;
using MTS.Data.Converters;
using MTS.Data.Types;
using MTS.Tester.Result;


namespace MTS.Tester
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
        private MTSContext context;
        
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

        private int mirrors = 1;
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
            private set
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
            private set
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
        /// <param name="tests">Collection of enabled tests</param>
        /// <returns>New instance of initialized TaskScheduler</returns>
        private TaskScheduler createScheduler(Channels channels, TestCollection tests)
        {
            TaskScheduler scheduler = new TaskScheduler(channels);

            // add for providing basic steps to start executing test
            // this contains tasks such as: open device, wait for mirror to be inserted, wait for start button,
            // close device
            scheduler.AddInitSequence();

            // wait for start
            //scheduler.AddWaitForStart();

            // rubber test
            TestValue rubber = tests.GetTest(TestCollection.Rubber);
            if (rubber != null)
                scheduler.AddRubberTest(rubber);

            // add tests of mirror movement
            scheduler.AddTravelTests(tests);

            // pull-off test
            TestValue pullOff = tests.GetTest(TestCollection.Pulloff);
            if (pullOff != null)
                scheduler.AddPulloffTest(pullOff);

            // test powerfold
            TestValue powerfold = tests.GetTest(TestCollection.Powerfold);
            if (powerfold != null)
                scheduler.AddTask(new PowerfoldTest(channels, powerfold));
            
            // test blinker
            TestValue blinker = tests.GetTest(TestCollection.DirectionLight);
            if (blinker != null)
                scheduler.AddTask(new BlinkerTest(channels, blinker));

            // test spiral
            TestValue spiral = tests.GetTest(TestCollection.Heating);
            if (spiral != null)
                scheduler.AddTask(new SpiralTest(channels, spiral));

            // open device
            scheduler.AddOpenDevice();

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
                    //time += watch.Elapsed;          // caluculate current time (DateTime.Now is not very exactly)
                    scheduler.Update(time + watch.Elapsed);         // execute tasks
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
            // first of all display result to user - saving may be spent some period of time
            string msg;
            TaskResultType resultCode = scheduler.GetResultCode();
            if (resultCode == TaskResultType.Completed)
            {
                msg = "Mirror passed!";
                channels.GreenLightOn.On();
                Passed++;
            }
            else if (resultCode == TaskResultType.Failed)
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

            Output.Write("Saving results do database ... ");
            List<TaskResult> results = scheduler.GetResultData();
            saveShiftResult(results, (Int16)Finished);
            Output.WriteLine("Saved!");

            if (Properties.Settings.Default.PrintLabels)
            {   // if printing of labels is required
                // print label for test mirror
                if (resultCode == TaskResultType.Completed)
                    Admin.Printing.PrintingManager.Print("mirror", "Passed");
                else if (resultCode == TaskResultType.Failed)
                    Admin.Printing.PrintingManager.Print("mirror", "Failed");
            }

            // wait a moment to display light
            Thread.Sleep(1000);
        }

        #region Database Methods

        /// <summary>
        /// Initialize test that are going to be used in this shift. Remove disabled test, save tests and their
        /// parameters that are going to be used to database. Use test or parameter database id as auxiliary property
        /// of test or parameter value. Those will be used when generating test or parameter results and saveing them
        /// to database
        /// </summary>
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
                    string unit = null;
                    if (param is DoubleParam) unit = (param as DoubleParam).Unit.Name;
                    else if (param is IntParam) unit = (param as IntParam).Unit.Name;

                    Param dbParam = context.Params.FirstOrDefault(p => p.Name == param.ValueId && p.Value == strValue);
                    if (dbParam == null)
                    {   // if it does not exists create a new one
                        byte valueType = (byte)param.ValueType();
                        dbParam = context.Params.Add(new Param { Name = param.ValueId, Type = valueType, Value = strValue, Unit = unit });
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
        /// Create a new instance of shift and save it to database. Current date and time is used for initializing
        /// start time of shift. Also all tests and parameters that are going to be used in this shift will be saved
        /// </summary>
        /// <param name="usedTests">Only tests that will be executed (not disabled)</param>
        private void createShift(TestCollection usedTests)
        {
            // 1) create a new instance of shift and save it to database
            mirrorId = context.Mirrors.Select(m => m.Id).First();   // depends on parameter settings
            operatorId = Admin.Operator.Instance.Id;                // loged in operator id
            dbShift = context.Shifts.Add(new Data.Shift
            {
                Start = DateTime.Now,       // date and time when shift has been started
                Finish = DateTime.Now,      // must be initialized otherwise exception is thrown
                MirrorId = mirrorId,        // mirror which is tested in this shift
                OperatorId = operatorId     // operator who has executed this shift
            });
            // by saveing shift new id will be generated - this is necessary for saveing test used in this shift
            context.SaveChanges();

            // 2) save information about used tests in current shift
            foreach (TestValue tv in usedTests)
            {
                context.TestShifts.Add(new TestShift { ShiftId = dbShift.Id, TestId = tv.DatabaseId });
            }
        }
        /// <summary>
        /// Save produced results from one sequence of tests. One shift contains many sequences. At this time
        /// shift is already saved in database and we reference it when saveing ouputs
        /// </summary>
        /// <param name="results">Collection of <see cref="TaskResult"/> containing data to be saved for
        /// each test and its parameters</param>
        /// <param name="sequence">Number of test sequnece within current shift</param>
        private void saveShiftResult(IEnumerable<TaskResult> results, Int16 sequence)
        {
            try
            {
                // this converter is used to convert object value to its string representation when
                // saveing to and retriving from database
                ParamTypeConverter converter = new ParamTypeConverter();

                foreach (TaskResult tRes in results.Where(t => t.HasData))
                {
                    // only save test outputs which have data to be saved
                    TestOutput dbTestOutput = context.TestOutputs.Add(new TestOutput
                    {
                        Result = (byte)tRes.ResultCode,     // result of test: Completed/Failed/Aborted
                        Start = tRes.Begin,                 // date and time then test has been started
                        Finish = tRes.End,                  // date and time when test has been finished
                        ShiftId = dbShift.Id,               // shift where this output was generated
                        TestId = tRes.DatabaseId,           // test used for this output
                        Sequence = sequence                 // number of test sequence within this shift
                    });
                    // generate new id of TestOutput, this is necessary for saveing parameters output
                    context.SaveChanges();

                    // only save parameter outputs which have data to be saved
                    foreach (ParamResult pRes in tRes.Params.Where(p => p.HasData))
                    {
                        // save param output (notice that if could be null also)
                        context.ParamOutputs.Add(new ParamOutput
                        {
                            ParamId = pRes.DatabaseId,      // parameter used for this output
                            TestOutpuId = dbTestOutput.Id,  // test output to which this parameter output belongs
                            Value = pRes.ResultStringValue  // value of parameter output (could be null)
                        });
                        // context should not be saved - will be saved with next test output or at the end of loop
                    }
                }

                // save data after last loop (param output is not saved)
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // handle error: 
                throw ex;
            }
        }
        /// <summary>
        /// Modify and save shift to database. This method shuld be called at the end of shift execution.
        /// Current data and time is used for end time of shift.
        /// </summary>
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

            // first update, so we do not use uninitialized values
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
           
            // create database layer
            context = new MTSContext();
            // prepare test collection for execution - remove disabled tests and the rest save to database
            // remember test and parameter database ids
            initTests();
            // create and save a new instance of this to database
            createShift(shiftTests);

            // only start execution loop if it is not started yet
            if (loop != null && !loop.IsAlive)
                loop.Start();
        }
        public void Finish()    // must be private method
        {
            // switch off everything dangerous
            setSafeStateOutputs(channels);

            // save date and time when shift has been finished - necessary for database
            saveShift();
            // release database layer
            context.Dispose();

            Output.WriteLine("Shift finished!");

            // raise notify event that shift has been finished
            RaiseExecuted();
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
        }

        #endregion
    }
}
