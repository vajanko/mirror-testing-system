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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Diagnostics;

using MTS.IO;
using MTS.IO.Module;
using MTS.Properties;
using MTS.Tester;

using MTS.Tester.Result;

namespace MTS.Admin
{
    /// <summary>
    /// Interaction logic for CalibrationWindow.xaml
    /// </summary>
    public partial class CalibrationWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        private string status = string.Empty;
        /// <summary>
        /// (Get) Desription of calibration status. When changed property changed event is raised
        /// Thread safe
        /// </summary>
        public string Status
        {
            get { lock (this.status) { return status; } }
            private set
            {
                lock (this.status)      // this setter can be called from multiple threads
                {
                    status = value;
                }
                OnPropertyChanged("Status");
            }
        }

        private bool isRunning;
        /// <summary>
        /// (Get) Value indicating wheter calibration is being executed just now. When changed property
        /// changed event is raised.
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
            private set
            {
                this.isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private bool isExecuted;
        public bool IsExecuted
        {
            get { return isExecuted; }
            set { isExecuted = value; }
        }

        private double calibrationState;
        /// <summary>
        /// (Get) Percentage of finished calibration task. When changed property changed event is raised.
        /// Thread safe
        /// </summary>
        public double CalibrationState
        {
            get { return calibrationState; }
            set
            {
                calibrationState = value;
                OnPropertyChanged("CalibrationState");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Calibration

        /// <summary>
        /// Thread that will handle calibration (executed in a loop)
        /// </summary>
        private Thread calibrationThread;
        /// <summary>
        /// Normal of the mirror after calibration
        /// </summary>
        private Vector3D mirrorNormal;

        private TaskScheduler scheduler;

        private void beginCalibration()
        {
            // create a new thread that will execute calibration loop
            ThreadStart start = new ThreadStart(calibrate);
            calibrationThread = new Thread(start);
            calibrationThread.Start();
        }
        private void calibrate()
        {
            // create a module based on current application settings (could be of different procotol type
            // and loaded from different configuration files
            IModule module = null;

            IsExecuted = false;

            try
            {
                // connect to hw
                Status = "Initializing ... ";
                module = Settings.Default.GetModuleInstance();
                Channels channels = Settings.Default.GetChannelsInstance();
                channels.Connect();
                channels.Initialize();
                CalibrationState++;

                channels.UpdateInputs();    // read channel values
                // while power supply is not switched on
                while (channels.IsPowerSupplyOff.Value == true)
                {
                    // if power supply is not switch on, ask user to switch it on
                    MessageBoxResult result = MessageBox.Show("Switch on power supply!", "Power supply",
                        MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK);
                    if (result == MessageBoxResult.Cancel)  // abort if user does not switched on power supply
                    {
                        Status += "Aborted";
                        return;
                    }
                    // read value - check if user has switch on power supply
                    channels.UpdateInputs();
                }

                // create scheduler
                scheduler = new TaskScheduler(channels);
                scheduler.AddInitSequence();        // open device and wait for mirror to be inserted, then close it
                scheduler.AddDistanceSensorsUp();   // move up distance sensors for measuring
                scheduler.AddTask(new Calibrate(channels));     // read distance sensor values
                scheduler.AddDistanceSensorsDown(); // move down distance sensors - so device could be opened
                scheduler.AddOpenDevice();          // open device - after that calibration has finished
                CalibrationState++;
                Stopwatch watch = new Stopwatch();  // time elapsed since last loop
                DateTime time = DateTime.Now;       // current time of updating scheduler

                // enter calibration loop
                IsRunning = true;

                Status = "Calibrating ... ";
                scheduler.Initialize();
                watch.Start();
                while (!scheduler.IsFinished)
                {   // in this loop execute all tasks - open device, wait for mirror to be inserted, close device,
                    // and move sensors up measure mirror normal, move sensors down open device
                    time += watch.Elapsed;
                    scheduler.Update(time);
                    if (CalibrationState < 100)
                        CalibrationState++;
                }

                // calibration has been executed successfully without throwing any exception
                IsExecuted = handleResult(scheduler);

                Status += "Finished";
            }
            catch (Exception ex)
            {
                Status += "Failed!";
                ExceptionManager.ShowError(ex);
            }
            finally
            {   // release connection allocated resources
                if (module != null)
                    module.Disconnect();
                IsRunning = false;
            }
        }
        private bool handleResult(TaskScheduler scheduler)
        {
            // this sould contain only one result from calibration task
            var results = scheduler.GetResultData();
            bool executed = false;

            try
            {   // if any of this data is missing as exception will be thrown
                TaskResult res = results.Where(r => r.Value != null && r.ValueId == "Calibration").First();
                ParamResult disX = res.Params.Where(p => p.ValueId == "DistanceX").First();
                ParamResult disY = res.Params.Where(p => p.ValueId == "DistanceY").First();
                ParamResult disZ = res.Params.Where(p => p.ValueId == "DistanceZ").First();

                // save mirror normal - when ok button is clicked, also will be save to hardware settings file
                mirrorNormal = new Vector3D((double)disX.ResultValue, (double)disY.ResultValue, (double)disZ.ResultValue);
                executed = true;
            }
            catch
            {   // result of the calibration taks is corrupted
                Status = "Missing calibration data!";
            }

            return executed;
        }
        private void abortCalibration()
        {

        }

        #endregion

        #region Events

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // operation was cancled - should not be saved calibration values
            this.DialogResult = false;

            // stop calibration if it is running
            if (calibrationThread != null && calibrationThread.IsAlive)
                calibrationThread.Abort();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // operation finished successfully - calibration may be saved to settings file
            // check if there are some errors with calibration (this could be loose of connection, ...)
            this.DialogResult = false;
            // save calibrated value
            if (IsExecuted)
            {
                HWSettings.Default.ZeroPlaneNormal = mirrorNormal;
                this.DialogResult = true;       // calibration was executed successfully
            }
        }
        /// <summary>
        /// This method is called when winwod get initialized. Imadiatelly after that calibration will start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Initialized(object sender, EventArgs e)
        {
            beginCalibration();
        }

        #endregion

        #region Constructors

        public CalibrationWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
