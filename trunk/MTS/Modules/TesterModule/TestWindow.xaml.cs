using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MTS.EditorModule;
using MTS.AdminModule;

using AvalonDock;

using Microsoft.Win32;

namespace MTS.TesterModule
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : DocumentContent
    {
        #region Constatnts

        /// <summary>
        /// Constant string "IsRunning"
        /// </summary>
        public const string IsRunningString = "IsRunning";
        /// <summary>
        /// Constant string "IsLoaded"
        /// </summary>
        public const string ParamLoadedString = "ParamLoaded";
        /// <summary>
        /// Constant string "Mirrors"
        /// </summary>
        public const string MirrorsString = "Mirrors";
        /// <summary>
        /// Constant string "Finished"
        /// </summary>
        public const string FinishedString = "Finished";

        #endregion

        #region Shift Properties

        private int mirrors = 0;
        /// <summary>
        /// (Get/Set) Number of mirrors to test
        /// </summary>
        public int Mirrors
        {
            get { return mirrors; }
            set
            {
                mirrors = value;
                RaisePropertyChanged(MirrorsString);
            }
        }

        private int finished = 0;
        /// <summary>
        /// (Get/Set) Number of finised tests
        /// </summary>
        public int Finished
        {
            get { return finished; }
            set
            {
                finished = value;
                RaisePropertyChanged(FinishedString);
            }
        }

        #endregion

        #region Properties

        private bool isRunning = false;
        /// <summary>
        /// (Get/Set) True if testing is running (shift is started)
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;  // raise notify event - necessary for binding
                RaisePropertyChanged(IsRunningString);
            }
        }

        /// <summary>
        /// (Get) True if testing is not running (shift is not started)
        /// </summary>
        public bool NotRunning { get { return !IsRunning; } }

        private bool paramLoaded = false;
        /// <summary>
        /// (Get/Set) True if parameters are loaded
        /// </summary>
        public bool ParamLoaded
        {
            get { return paramLoaded; }
            set
            {
                paramLoaded = value;
                RaisePropertyChanged(ParamLoadedString);
                RaisePropertyChanged("ParamLoadedAndNotRunning");
            }
        }

        /// <summary>
        /// (Get) True if parameters are loaded and testing is not running
        /// </summary>
        public bool ParamLoadedAndNotRunning { get { return !IsRunning && ParamLoaded; } }

        public TestCollection Tests
        {
            get;
            protected set;
        }

        #endregion

        /// <summary>
        /// Imediatelly close test window, but tell user if testing is running
        /// </summary>
        /// <returns>True if DocumentContent is should be removed</returns>
        public override bool Close()
        {
            if (IsRunning)  // do not close if testing is runnig
            {
                return false;
            }            
            return base.Close();
        }

        #region Events

        /// <summary>
        /// Load parameter from file. These parameters will be used for testing and some of them will be displayed
        /// in the testing window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadParameters(object sender, RoutedEventArgs e)
        {
            // create dialog to get file with parameters
            var file = FileManager.CreateOpenFileDialog();

            if (file.ShowDialog() == true)  // user entered a file
            {
                try
                {   // load file to memory
                    Tests = FileManager.ReadFile(file.FileName);
                    ParamLoaded = true;    // this will enable some buttons
                    // copy some parameters to testing window
                    TestValue test = Tests[TestDictionary.INFO];

                    StringParamValue param = test.Parameters[ParamDictionary.PART_NUMBER] as StringParamValue;
                    partNumber.Content = param.Value;

                    param = test.Parameters[ParamDictionary.SUPPLIER_NAME] as StringParamValue;
                    supplierName.Content = param.Value;

                    param = test.Parameters[ParamDictionary.SUPPLIER_CODE] as StringParamValue;
                    description.Content = param.Value;
                }
                catch (Exception ex)
                {   // display message to user if an error occured
                    FileManager.HandleOpenException(ex);
                }
            }
        }

        // test area
        System.Timers.Timer timer = new System.Timers.Timer();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        TaskScheduler scheduler;

        public Channels Channels
        {
            get;
            set;
        }

        // test area
        private void startClick(object sender, RoutedEventArgs e)
        {
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 100;  // ms

            scheduler = new TaskScheduler();
            var module = new DummyModule();
            module.LoadConfiguration("task1.csv");
            module.Connect();

            Channels = new Channels(module);
            heatingFoil.DataContext = Channels;
            //scheduler.AddTask(new SpiralTest(Channels, Tests[TestDictionary.SPIRAL]));
            //scheduler.AddTask(new BlinkerTest(Channels, Tests[TestDictionary.BLINKER]));
            scheduler.AddTask(new PowerfoldTest(Channels, Tests[TestDictionary.POWERFOLD]));

            scheduler.Initialize();
            scheduler.Executed += new ExecutedHandler(scheduler_SchedulerExecuted);

            watch.Reset();
            timer.Start();
            watch.Start();
        }

        void scheduler_SchedulerExecuted(TaskScheduler sender, EventArgs args)
        {
            watch.Stop();
            timer.Stop();
        }
        private void stopClick(object sender, RoutedEventArgs e)
        {
            watch.Stop();
            timer.Stop();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Channels.Update(watch.Elapsed);
            scheduler.Update(watch.Elapsed);
        }

        #endregion

        #region Constructors

        public TestWindow()
        {
            InitializeComponent();
            IsRunning = false;
        }

        #endregion
    }
}
