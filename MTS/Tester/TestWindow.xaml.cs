using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;

using AvalonDock;
using MTS.Base;
using MTS.Editor;
using MTS.IO;
using MTS.Properties;
using MTS.Data;

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

        private List<Controls.FlowControl> analogControls = new List<Controls.FlowControl>();

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
        /// This method is called when before new testing is started. Here variables (like number of tested mirrors)
        /// should be initialized
        /// </summary>
        private void initializeTesting()
        {
            Passed = 0;
            Failed = 0;

            IsRunning = true;       // disable other controls
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
                initializeTesting();    // initialize variables

                Mirror mirror = mirrorTypeBox.SelectedItem as Mirror;
                if (mirror == null)
                    throw new ApplicationException("No mirror is selected. Please select one mirror to be tested.");

                // load channels configuration from application settings
                Output.Write(Resource.CreatingChannelsMsg);                
                //channels = Settings.Default.GetChannelsInstance();
                channels.Connect();     // create connection to hardware                
                bindChannels(channels); // bind events to channel (when some value of channel change)
                Output.WriteLine(Resource.OKMsg);

                // create shift and register its handlers
                Output.Write(Resource.StartingShiftMsg);
                shift = new Shift(mirror.Id, channels, Tests) { Total = this.Total };
                shift.SequenceExecuted += new ShiftExecutedHandler(sequenceExecuted);
                shift.ShiftExecuted += new ShiftExecutedHandler(shiftExecuted);
                shift.Initialize();     // prepare channels for execution - power supply must be on

                timer = new Timer() { Interval = 400 };    // this timer will update user interface
                timer.Tick += new EventHandler(timer_Tick);
                             
                shift.Start();      // start execution loop (new thread - return immediately)
                timer.Start();      // start to update user interface
                Output.WriteLine(Resource.OKMsg);
            }
            catch (Exception ex)
            {
                IsRunning = false;
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
        {   // run disconnect method on GUI thread
            this.Dispatcher.Invoke(new Action(disconnect));
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
        /// <param name="e">Timer elapsed event argument</param>
        void timer_Tick(object sender, EventArgs e)
        {
            if (channels.IsDistanceSensorUp.Value)  // measuring is activated
            {
                mirrorView.RotationAxis = channels.GetRotationAxis();
                mirrorView.RotationAngle = channels.GetRotationAngle();

                mirrorView.HorizontalAngle = channels.GetHorizontalAngle();
                mirrorView.VerticalAngle = channels.GetVerticalAngle();
            }

            // update all analog channels controls
            foreach (var ctrl in analogControls)
                ctrl.Update();
        }
        /// <summary>
        /// Close connection with tester hardware if it exists. This method must be called only on same thread
        /// as graphical user interface is running
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
            IsRunning = false;
        }

        /// <summary>
        /// This method is called once when mirror type combo box is loaded. In this moment different types of mirror saved in
        /// database are loaded and displayed in combo box so operator may select one of them to test it
        /// </summary>
        /// <param name="sender">Instance of combo box that is loaded</param>
        /// <param name="e">Loaded event arguments</param>
        private void mirrorTypeBox_Loaded(object sender, RoutedEventArgs e)
        {
            var box = sender as System.Windows.Controls.ComboBox;
            if (box == null)
                return;

            try
            {   // load mirror types
                using (MTSContext context = new MTSContext())
                {
                    box.ItemsSource = context.Mirrors.ToList();
                    box.SelectedIndex = 0;    // select first mirror
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.ShowError(ex);
            }
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

        #region Constructors

        /// <summary>
        /// Create a new instance of tester window that allows user to manage and follow providing tests
        /// </summary>
        public TestWindow()
        {
            channels = Settings.Default.GetChannelsInstance();
            if (channels == null)
            {
                this.IsEnabled = false;
            }
            else
            {
                channels.Initialize();  // initialize each channel and channels properties
                foreach (IAnalogInput input in channels.GetChannels<IAnalogInput>())
                    analogControls.Add(new Controls.FlowControl()
                    {
                        Title = input.Name,
                        GetNewValue = new Func<double>(input.GetRealValue)  // this method will be called when control is updated
                    });
            }

            InitializeComponent();
            analogChannelsControls.DataContext = analogControls;
        }

        #endregion
    }
}
