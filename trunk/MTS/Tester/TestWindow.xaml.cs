using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

using MTS.Base;
using MTS.Editor;
using MTS.Properties;
using MTS.IO;
using MTS.IO.Module;

using AvalonDock;

using Microsoft.Win32;

namespace MTS.Tester
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : DocumentContent
    {
        #region Private fields

        private Channels channels;
        private Timer timer;
        private Shift shift;

        #endregion

        #region Shift, Parameters and Device Properties

        #region Mirror Count

        private int total = 1;
        /// <summary>
        /// (Get/Set) Number of mirrors to test
        /// </summary>
        public int Total
        {
            get { return total; }
            set
            {
                total = value;
                RaisePropertyChanged("Total");
            }
        }

        /// <summary>
        /// (Get) Number of finished tests. When value is changed property changed event is raised
        /// </summary>
        public int Finished { get { return Passed + Failed; } }

        private int passed = 0;
        /// <summary>
        /// (Get/Set) Number of correctly finished tests. When value is changed property changed event is raised
        /// </summary>
        public int Passed
        {
            get { return passed; }
            set
            {
                passed = value;
                RaisePropertyChanged("Passed");
                RaisePropertyChanged("Finished");
            }
        }

        private int failed = 0;
        /// <summary>
        /// (Get/Set) Number of defective finished tests. When value is changed property changed event is raised
        /// </summary>
        public int Failed
        {
            get { return failed; }
            set
            {
                failed = value;
                RaisePropertyChanged("Failed");
                RaisePropertyChanged("Finished");
            }
        }

        #endregion

        #region State

        #region IsRunning Property

        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(TestWindow),
            new PropertyMetadata(false));

        /// <summary>
        /// (Get/Set) True if testing is running (shift is started). This is dependency property
        /// </summary>
        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }

        #endregion

        private bool isParamLoaded = false;
        /// <summary>
        /// (Get/Set) True if parameters are loaded
        /// </summary>
        public bool IsParamLoaded
        {
            get { return isParamLoaded; }
            set
            {
                isParamLoaded = value; 
                RaisePropertyChanged("IsParamLoaded");
            }
        }

        #endregion

        #endregion

        #region Properties

        public TestCollection Tests
        {
            get;
            protected set;
        }

        #endregion

        #region Overrided Methods

        /// <summary>
        /// Immediately close test window, but tell user if testing is running
        /// </summary>
        /// <returns>True if DocumentContent is should be removed</returns>
        public override bool Close()
        {
            if (IsRunning)  // do not close if testing is running
            {
                return false;
            }            
            return base.Close();
        }

        #endregion

        #region Events

        /// <summary>
        /// This method is called when Load Parameters ... button is clicked. Open file dialog will be opened and 
        /// parameters will be loaded to memory. They will be used for testing and some of them will be displayed
        /// in the testing window. This method will handle all exceptions.
        /// </summary>
        /// <param name="sender">Instance of button that has been clicked</param>
        /// <param name="e">Click event arguments</param>
        private void loadParameters(object sender, RoutedEventArgs e)
        {
            // create dialog to get file with parameters
            var file = FileManager.CreateOpenFileDialog();

            if (file.ShowDialog() != true)  // abort loading parameters
                return;

            try
            {   // load file to memory
                Tests = FileManager.ReadFile(file.FileName);
                IsParamLoaded = true;   // this will enable some buttons

                string filename = System.IO.Path.GetFileName(file.FileName);
                paramFile.Content = filename;
                Output.WriteLine(Resource.ParamLoadedMsg, filename);
            }
            catch (Exception ex)
            {   // display message to user if an error occurred
                IsParamLoaded = false;
                // if file manager does not know what kind of exception is this, it will be re-thrown
                ExceptionManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Start shift. At this time connection to device must be established
        /// </summary>
        private void startClick(object sender, RoutedEventArgs e)
        {
            if (shift != null && shift.IsRunning)    // prevent from starting shift multiple times
                return;

            try
            {
                // load channels configuration from application settings
                Output.Write(Resource.CreatingChannelsMsg);                
                channels = Settings.Default.GetChannelsInstance();
                channels.Connect();     // create connection to hardware
                channels.Initialize();  // initialize each channel and channels properties
                bindChannels(channels); // bind events to channel (when some value of channel change)
                Output.WriteLine(Resource.OKMsg);

                // create shift and register its handlers
                Output.Write(Resource.StartingShiftMsg);
                shift = new Shift(channels, Tests) { Total = this.Total };
                shift.SequenceExecuted += new ShiftExecutedHandler(sequenceExecuted);
                shift.ShiftExecuted += new ShiftExecutedHandler(shiftExecuted);
                shift.Initialize();     // prepare channels for execution - power supply must be on

                timer = new Timer(400);     // this timer will update user interface
                timer.Elapsed += new ElapsedEventHandler(timerElapsed);
                
                IsRunning = true;   // disable other buttons                
                shift.Start();      // start execution loop (new thread - return immediately)
                timer.Start();      // start to update user interface
                Output.WriteLine(Resource.OKMsg);
            }
            catch (Exception ex)
            {
                Output.WriteLine(Resource.StartingShiftFailedMsg);
                ExceptionManager.ShowError(ex);
            }
        }
        /// <summary>
        /// This method is called when one sequence of shift is executed
        /// </summary>
        /// <param name="sender">Instance of shift that has been executed</param>
        /// <param name="args">Sequence executed event arguments. Hold data about current shift state</param>
        private void sequenceExecuted(object sender, ShiftExecutedEventArgs args)
        {
            Passed = args.Passed;
            Failed = args.Failed;
        }
        /// <summary>
        /// This method is called when shift get executed
        /// </summary>
        /// <param name="sender">Instance of shift that has been executed</param>
        /// <param name="args">Shift executed event arguments. Hold data about final shift state</param>
        private void shiftExecuted(object sender, ShiftExecutedEventArgs args)
        {
            disconnect();
            Passed = args.Passed;
            Failed = args.Failed;
        }

        /// <summary>
        /// This method is called when stop button is clicked. Abort shift if it is running
        /// </summary>
        /// <param name="sender">Instance of button that has been pressed</param>
        /// <param name="e">Click event arguments</param>
        private void stopClick(object sender, RoutedEventArgs e)
        {
            if (shift != null)
                shift.Abort();
            disconnect();
        }
        /// <summary>
        /// This method is called cyclically to update user interface
        /// </summary>
        /// <param name="sender">Instance of timer which invoked this method</param>
        /// <param name="e"></param>
        void timerElapsed(object sender, ElapsedEventArgs e)
        {   // run on GUI thread
            spiralCurrent.Dispatcher.BeginInvoke(new Action(updateGui));
        }
        /// <summary>
        /// Close connection with tester hardware if it exists
        /// </summary>
        private void disconnect()
        {
            if (channels != null)
            {
                Output.Write(Resource.ClosingConnectionMsg);
                channels.Disconnect();
                Output.WriteLine(Resource.OKMsg);
            }
            if (timer != null)
                timer.Stop();
        }
        
        #endregion

        #region Channels Events

        /// <summary>
        /// Bind channels with events that will be raised when value of particular channel change
        /// </summary>
        /// <param name="channels"></param>
        private void bindChannels(Channels channels)
        {

        }

        #endregion

        #region Debug

        private void updateGui()
        {
            if (channels.IsDistanceSensorUp.Value)  // measuring is activated
                setRotation();

            spiralCurrent.AddValue(channels.HeatingFoilCurrent.RealValue);
            blinkerCurrent.AddValue(channels.DirectionLightCurrent.RealValue);
            actuatorACurrent.AddValue(channels.VerticalActuatorCurrent.RealValue);
            actuatorBCurrent.AddValue(channels.HorizontalActuatorCurrent.RealValue);
            powerfoldCurrent.AddValue(channels.PowerfoldCurrent.RealValue);
            powerSupplyVoltage1.AddValue(channels.PowerSupplyVoltage1.RealValue);
            powerSupplyVoltage2.AddValue(channels.PowerSupplyVoltage2.RealValue);
        }
        private void setRotation()
        {
            mirrorView.RotationAxis = channels.GetRotationAxis();
            mirrorView.RotationAngle = channels.GetRotationAngle();

            mirrorView.HorizontalAngle = channels.GetHorizontalAngle();
            mirrorView.VerticalAngle = channels.GetVerticalAngle();
        }

        #endregion

        #region Constructors

        public TestWindow()
        {
            InitializeComponent();
        }

        #endregion        
    }
}
