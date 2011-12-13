using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Threading;

using MTS.IO;
using MTS.Editor;
using System.Diagnostics;

namespace MTS.TesterModule
{
    public delegate void ShiftExecutedHandler(Shift sender, EventArgs args);

    public class Shift : INotifyPropertyChanged
    {
        #region Private Fields

        private Channels channels;
        private TestCollection tests;
        private TaskScheduler scheduler;
        private Thread loop;

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
                // stop powerfold
                channels.FoldPowerfold.Value = false;
                channels.UnfoldPowerfold.Value = false;
                // stop mirror movement
                channels.MoveMirrorHorizontal.Value = false;
                channels.MoveMirrorVertical.Value = false;
                channels.MoveMirrorReverse.Value = false;
                channels.AllowMirrorMovement.Value = false;

                // stop direction light
                channels.DirectionLightOn.Value = false;
                // stop heating
                channels.HeatingFoilOn.Value = false;

                // move distance sensors down
                channels.MoveDistanceSensorUp.Value = false;
                channels.MoveDistanceSensorDown.Value = true;

                // 
                channels.SuckOn.Value = false;

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
            scheduler.AddInitSequence();

            // wait for start
            //scheduler.AddWaitForStart();

            // rubber test
            scheduler.AddRubberTest(tests);

            //// wait for start
            //scheduler.AddWaitForStart();
            //// add tests of mirror movement
            scheduler.AddTravelTests(tests);

            //// wait for start
            //scheduler.AddWaitForStart();
            //// pull-off test
            //scheduler.AddPulloffTest(tests);

            // wait for start
            //scheduler.AddWaitForStart();
            // test powerfold
            scheduler.AddTask(new PowerfoldTest(channels, tests.GetTest(TestCollection.Powerfold)));

            // wait for start
            //scheduler.AddWaitForStart();
            // test blinker
            scheduler.AddTask(new BlinkerTest(channels, tests.GetTest(TestCollection.DirectionLight)));

            // wait for start
            //scheduler.AddWaitForStart();
            // test spiral
            scheduler.AddTask(new SpiralTest(channels, tests.GetTest(TestCollection.Heating)));

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
            Stopwatch watch = new Stopwatch();
            DateTime time = DateTime.Now;       // time of updating scheduler

            Output.WriteLine("Execution loop started!");
            watch.Start();
            while (Remained > 0)
            {
                Output.WriteLine("Test sequence {0} started", Finished + 1);

                // create scheduler with tasks to be executed
                scheduler = createScheduler(channels, tests);

                while (!scheduler.IsFinished)
                {
                    Thread.Sleep(50);   // for presentation purpose only
                    time += watch.Elapsed;  // current time
                    scheduler.UpdateOutputs(time);
                    scheduler.Update(time);
                }
                Output.WriteLine("Test sequence {0} finished", Finished + 1);

                // write outputs to safe state
                setSafeStateOutputs(channels);

                // handle results
                schedulerExecuted(scheduler, new EventArgs());
            }
            watch.Stop();
            Output.WriteLine("Execution loop finished!");
            Finish();
        }

        private void schedulerExecuted(TaskScheduler sender, EventArgs args)
        {
            bool result = sender.AreAllPassed();
            string msg;
            if (result)
            {
                msg = "Mirror passed!";
                channels.GreenLightOn.Value = true;
                Passed++;
            }
            else
            {
                msg = "Mirror failed!";
                channels.RedLightOn.Value = true;
                Failed++;
            }
            Output.WriteLine("Result: " + msg);
            // switch light (green or red) on
            channels.UpdateOutputs();
            Thread.Sleep(1000);
        }

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
        {
            // only start execution loop if it is not started yet
            if (loop != null && !loop.IsAlive)
                loop.Start();
        }
        public void Finish()    // must be private method
        {
            // switch off everythig dangerous
            setSafeStateOutputs(channels);

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
            this.tests = tests;
            Mirrors = 1;
        }

        #endregion
    }
}
