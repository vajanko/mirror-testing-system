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
using MTS.Base;
using MTS.Properties;
using MTS.Tester;

using MTS.Tester.Result;

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Allows user to execute mirror calibration.
    /// </summary>
    partial class CalibrationWindow : Window, INotifyPropertyChanged
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

        /// <summary>
        /// Task scheduler used for executing task needed for calibration process
        /// </summary>
        private TaskScheduler scheduler;
        /// <summary>
        /// Value indication whether executing of scheduler should be aborted. Use this value when abortion is
        /// needed rather than calling Abort method on <see cref="scheduler"/>
        /// </summary>
        private bool isAborting;

        /// <summary>
        /// Start calibration process on a new thread
        /// </summary>
        private void beginCalibration()
        {
            isAborting = false;
            // create a new thread that will execute calibration loop
            ThreadStart start = new ThreadStart(calibrate);
            calibrationThread = new Thread(start);
            calibrationThread.Start();
        }
        /// <summary>
        /// Run all the process of calibration. This method should be executed on the calibration thread.
        /// When abortion is needed use <see cref="isAborting"/> field
        /// </summary>
        private void calibrate()
        {
            // create a module based on current application settings (could be of different protocol type
            // and loaded from different configuration files
            IModule module = null;

            // when executed is true - means: calibration was successful and can by saved
            IsExecuted = false;

            // catch connection exceptions
            try
            {   // connect to hw
                Status = "Initializing ... ";
                //module = Settings.Default.GetModuleInstance();
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

                // create scheduler and add tasks needed for calibration process
                scheduler = new TaskScheduler(channels);
                scheduler.Load(Settings.Default.GetCalibConfigPath());

                CalibrationState++;
                Stopwatch watch = new Stopwatch();  // time elapsed since last loop
                DateTime time = DateTime.Now;       // current time of updating scheduler
                double state = CalibrationState;

                // enter calibration loop
                IsRunning = true;

                Status = "Calibrating ... ";
                scheduler.Initialize();
                watch.Start();
                while (!scheduler.IsFinished)
                {   // in this loop execute all tasks - open device, wait for mirror to be inserted, close device,
                    // and move sensors up measure mirror normal, move sensors down open device
                    //time += watch.Elapsed;
                    scheduler.Update(time + watch.Elapsed);
                    CalibrationState = state + scheduler.ExecutedTasks;
                    if (isAborting)
                        scheduler.Abort(time + watch.Elapsed);  // after calling this method scheduler.IsFinished is true
                }

                if (isAborting)
                {
                    IsExecuted = false;
                    Status += "Aborted";
                }
                else
                {
                    CalibrationState++;
                    // calibration has been executed successfully without throwing any exception
                    IsExecuted = handleResult(scheduler);
                    CalibrationState++;
                    Status += "Finished";
                    CalibrationState = 100;
                }
            }
            catch (Exception ex)
            {
                Status += "Failed!";
                Output.WriteLine("An error occurred while calibrating mirror: {0}", ex.Message);
            }
            finally
            {   // release connection allocated resources
                if (module != null)
                    module.Disconnect();
                IsRunning = false;
            }
        }
        /// <summary>
        /// Get result data from scheduler used for calibration and save them to application settings
        /// </summary>
        /// <param name="scheduler">Task scheduler used for calibration process</param>
        /// <returns>Value indicating whether saving calibration results was successful</returns>
        private bool handleResult(TaskScheduler scheduler)
        {
            // this should contain only one result from calibration task
            var results = scheduler.GetResultData();
            bool executed = false;

            try
            {   // if any of this data is missing as exception will be thrown
                TaskResult res = results.Where(r => r.Description == "Calibration").First();

                ParamResult disX = res.Params.Where(p => p.ValueId == "DistanceX").First();
                ParamResult disY = res.Params.Where(p => p.ValueId == "DistanceY").First();
                ParamResult disZ = res.Params.Where(p => p.ValueId == "DistanceZ").First();

                // save mirror normal - when ok button is clicked, also will be save to hardware settings file
                mirrorNormal = new Vector3D((double)disX.ResultParam.Value, (double)disY.ResultParam.Value, (double)disZ.ResultParam.Value);
                executed = true;
            }
            catch
            {   // result of the calibration tasks is corrupted
                Status = "Missing calibration data!";
            }

            return executed;
        }

        #endregion

        #region Events

        /// <summary>
        /// This method is called when cancel button is clicked on the calibration window. If calibration is running
        /// will be aborted.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // operation was cancled - should not be saved calibration values
            this.DialogResult = false;

            // stop calibration if it is running
            isAborting = true;
        }
        /// <summary>
        /// This method is called when ok button is clicked on the calibration window. If calibration was successfull
        /// results will be save to application settings
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // operation finished successfully - calibration may be saved to settings file
            // check if there are some errors with calibration (this could be loose of connection, ...)
            
            // save calibrated value
            if (IsExecuted)
            {
                mirrorNormal.Normalize();
                HWSettings.Default.ZeroPlaneNormal = mirrorNormal;
                this.DialogResult = true;       // calibration was executed successfully
            }
            else
            {   // do not save calibration results
                this.DialogResult = false;
            }
        }
        /// <summary>
        /// This method is called when winwod get initialized. Imediatelly after that calibration will start
        /// </summary>
        private void window_Initialized(object sender, EventArgs e)
        {
            beginCalibration(); // start calibration on new thread
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instace of calibration window and initialize its components
        /// </summary>
        public CalibrationWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
