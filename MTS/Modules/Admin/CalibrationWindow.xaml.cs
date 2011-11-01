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

using System.Threading;

using MTS.IO;
using MTS.IO.Module;
using MTS.Properties;

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
            double step = 1;

            IsExecuted = false;

            try
            {
                //module = Settings.Default.GetModuleInstance();
                // enter calibration loop
                IsRunning = true;

                while (CalibrationState < 100)
                {
                    Thread.Sleep(3);
                    CalibrationState += step;
                }

                // calibration has been executed successfully without throwing any exception
                IsExecuted = true;
            }
            catch
            {
                Status = "Connection could not be established!";
            }
            finally
            {
                if (module != null)
                    module.Disconnect();
                IsRunning = false;
            }

            // 
            
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

            this.DialogResult = true;       // calibration was executed successfully
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
