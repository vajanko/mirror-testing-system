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

using MTS.Editor;
using MTS.Properties;
using MTS.IO;
using MTS.IO.Module;

using AvalonDock;

using Microsoft.Win32;

namespace MTS.TesterModule
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
        /// <summary>
        /// Constant string "Correct"
        /// </summary>
        public const string CorrectString = "Correct";
        /// <summary>
        /// Constant string "Defective"
        /// </summary>
        public const string DefectiveString = "Defective";

        #endregion

        #region Shift, Parameters and Device Properties

        #region Mirror Count

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

        private int correct = 0;
        /// <summary>
        /// (Get/Set) Number of correnct finished tests
        /// </summary>
        public int Correct
        {
            get { return correct; }
            set
            {
                correct = value;
                RaisePropertyChanged(CorrectString);
            }
        }

        private int defective = 0;
        /// <summary>
        /// (Get/Set) Number of defective finished tests
        /// </summary>
        public int Defective
        {
            get { return defective; }
            set
            {
                defective = value;
                RaisePropertyChanged(DefectiveString);
            }
        }

        #endregion

        #region State

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
                // this property and its negation has been changed
                RaisePropertyChanged(IsRunningString);
                RaisePropertyChanged("IsNotRunning");
            }
        }
        /// <summary>
        /// (Get) True if testing is not running (shift is not started) but it is being setup
        /// </summary>
        public bool IsNotRunning { get { return !IsRunning; } }

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
                // this property and its negation has been changed
                RaisePropertyChanged("IsParamLoaded");
                RaisePropertyChanged("IsNotParamLoaded");
            }
        }
        /// <summary>
        /// (Get) True if parameters are not loaded
        /// </summary>
        public bool IsNotParamLoaded { get { return !IsParamLoaded; } }

        private bool isDeviceConnected = false;
        /// <summary>
        /// (Get/Set) True if device is connected
        /// </summary>
        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                RaisePropertyChanged("IsDeviceConnected");
            }
        }

        #endregion

        private string shiftStatusMessage;
        /// <summary>
        /// (Get/Set) Message describing shift status
        /// </summary>
        public string ShiftStatusMessage
        {
            get { return shiftStatusMessage; }
            set
            {
                shiftStatusMessage = value;
                RaisePropertyChanged("ShiftStatusMessage");
            }
        }

        private string paramStatusMessage;
        /// <summary>
        /// (Get/Set) Message describing status of file with parameters
        /// </summary>
        public string ParamStatusMessage
        {
            get { return paramStatusMessage; }
            set
            {
                paramStatusMessage = value;
                RaisePropertyChanged("ParamStatusMessage");
            }
        }

        private string deviceStatusMessage;
        /// <summary>
        /// (Get/Set) Message describing status of device
        /// </summary>
        public string DeviceStatusMessage
        {
            get { return deviceStatusMessage; }
            set
            {
                deviceStatusMessage = value;
                RaisePropertyChanged("DeviceStatusMessage");
            }
        }

        #endregion

        #region Properties

        public TestCollection Tests
        {
            get;
            protected set;
        }

        #endregion

        #region Overrided Methods

        protected override void OnContentLoaded()
        {
            base.OnContentLoaded();
        }

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

        #endregion

        #region Events

        /// <summary>
        /// Load parameter from file. These parameters will be used for testing and some of them will be displayed
        /// in the testing window. Returns null if parameters could not be loaded
        /// This method will handle all exceptions!
        /// </summary>
        private void loadParameters(object sender, RoutedEventArgs e)
        {
            // create dialog to get file with parameters
            var file = FileManager.CreateOpenFileDialog();

            if (file.ShowDialog() == true)  // user entered a file
            {
                try
                {   // load file to memory
                    Tests = FileManager.ReadFile(file.FileName);
                    IsParamLoaded = true;   // this will enable some buttons

                    string filename = System.IO.Path.GetFileName(file.FileName);
                    Output.WriteLine("Parameters loaded from file: {0}", filename);
                    try
                    {   // copy some parameters to testing window
                        TestValue test = Tests.GetTest(TestCollection.Info);

                        StringParam param = test.GetParam<StringParam>(TestValue.PartNumber);
                        partNumber.Content = param.StringValue;

                        param = test.GetParam<StringParam>(TestValue.SupplierName);
                        supplierName.Content = param.StringValue;

                        param = test.GetParam<StringParam>(TestValue.DescriptionId);
                        description.Content = param.StringValue;

                        paramFile.Content = filename;
                    }
                    catch (Exception)
                    {   // this happens when some parameters are missing
                        MessageBox.Show("Some parameters are corrupted!", "Parameters warning", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {   // display message to user if an error occured
                    FileManager.HandleOpenException(ex);
                    IsParamLoaded = false;
                }

                if (IsParamLoaded)
                    ParamStatusMessage = "Loaded";
            }
        }

        /// <summary>
        /// Connect the device. This method initialize the connection and will cyclicaly watch for
        /// hardware changes
        /// </summary>
        private void connectClick(object sender, RoutedEventArgs e)
        {
            // try to initialize connection to HW
            if (initializeHardware(out channels))
            {   // initialization was successfull
                IsDeviceConnected = true;
                //DeviceStatusMessage = "Device connected";
                Output.WriteLine("Device connected successfully");
                DeviceStatusMessage = "Connected";
            }
        }
        /// <summary>
        /// Release connection resources and break the connection
        /// </summary>
        private void disconnectClick(object sender, RoutedEventArgs e)
        {
            if (channels != null)
            {
                channels.Disconnect();
                Output.WriteLine("Device disconnected successfully");
                IsDeviceConnected = false;
                DeviceStatusMessage = "Disconnected";
            }
        }
        /// <summary>
        /// Start shift. At this time connection to device must be established
        /// </summary>
        private void startClick(object sender, RoutedEventArgs e)
        {
            // prevent to start shift when device is not connected
            if (!IsDeviceConnected) return;

            // prevent from starting shift multiple times (too dangerous)
            if (shift != null && shift.IsRunning)
                return;

            Output.WriteLine("Starting shift ...");

            // this method should be called only when connection is established and parameters are loaded
            if (channels != null && channels.IsConnected && Tests != null)
            {
                shift = new Shift(channels, Tests) { Mirrors = this.Mirrors };
                shift.Executed += new ShiftExecutedHandler(shiftExecuted);
                shift.Initialize();

                timer = new Timer(400);     // this timer will update gui
                timer.Elapsed += new ElapsedEventHandler(timerElapsed);                

                // disable other buttons
                IsRunning = true;
                // start execution loop (new thread - return immediatelly)
                shift.Start();
                // start gui
                timer.Start();
            }
            else
                Output.WriteLine("Starting shift failed!");
            if (IsRunning)
                ShiftStatusMessage = "Running";
        }
        /// <summary>
        /// Stop shift. Abort if it is running
        /// </summary>
        private void stopClick(object sender, RoutedEventArgs e)
        {
            if (shift != null)
                shift.Abort();

            IsRunning = false;
            timer.Stop();   // this timer updates gui

            ShiftStatusMessage = "Stoped";
        }
        /// <summary>
        /// This method is called when shift get executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void shiftExecuted(Shift sender, EventArgs args)
        {
            IsRunning = false;
            timer.Stop();

            ShiftStatusMessage = "Stoped";
        }
        /// <summary>
        /// This method is called cyclically to update gui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerElapsed(object sender, ElapsedEventArgs e)
        {
            // run on gui thread
            spiralCurrent.Dispatcher.BeginInvoke(new Action(updateGui));
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize connection with hardware. This method will chreate channels, establisch a new connection
        /// and prepare device to execute tests.
        /// This method will handle all exceptions!
        /// <returns>Returns value indicating if initialization of hardware was sucsessfull</returns>
        /// </summary>
        private bool initializeHardware(out Channels channels)
        {
            bool success = true;
            // create communication layer
            channels = createChannels();
            // createConnection is called only if channels are not null
            if (channels == null || !createConnection(ref channels))
                return false;     // connection was not created

            // at this moment: channels are created and configuration is loaded, connection is established

            try
            {   
                // initialize handler that will be executed when some channel value change
                bindChannels(channels);
            }
            catch
            {
                success = false;
                MessageBox.Show("An error occured while configurating device connection. Check device configuration file:\n"
                    + Settings.Default.EthercatConfigFile, "Device error", MessageBoxButton.OK, MessageBoxImage.Error);
                // write log file
                Output.Log("Failed to initialize channels. Probably one of them is missing in configuration file: {0}",
                    Settings.Default.EthercatConfigFile);
            }
            // return value indicating if this method was successfull
            return success;
        }
        /// <summary>
        /// Create a new instance of channels collection <typeparamref name="Channels"/> acording
        /// application settings.
        /// This method will handle all exceptions!
        /// </summary>
        private Channels createChannels()
        {
            IModule module;

            // determine which module to use
            switch (Settings.Default.Protocol)
            {
                case "Beckhoff": module = new ECModule(Settings.Default.EthercatTaskName); break;
                case "Moxa": module = new ModbusModule(Settings.Default.ModbusIpAddress, Settings.Default.ModbusPort); break;
                case "Dummy": module = new DummyModule(); break;    // this is for debugging only
                default: module = new ECModule(Settings.Default.EthercatTaskName); break;
            }

            Channels channels = new Channels(module);
            // use configuration file from Settings
            // when loading file - an exception may be thrown
            try
            {   // get absolute path from relative one
                channels.LoadConfiguration(Settings.Default.EthercatConfigFile);
            }
            catch (System.IO.IOException ex)
            {   // file exception
                MessageBox.Show("Loading configuration file failed:\n" + ex.Message, "File error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                // even if channels were created blow them off, as they are unusefull
                channels = null;
            }
            catch (Exception)
            {   // somethig strange happened
                MessageBox.Show("An unknown error occured while loading configuration file", "Unkonwn error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                // even if channels were created blow them off, as they are unusefull
                channels = null;
            }

            return channels;
        }
        /// <summary>
        /// Create a new connection with given channels and return value indicating if it was successfull.
        /// This method will handle all exceptions!
        /// </summary>
        /// <param name="channels">Channels to connect. This value must not be null</param>
        /// <returns>True if connection was created</returns>
        private bool createConnection(ref Channels channels)
        {
            // if true - everything finished correctly
            bool success = true;

            try
            {
                // initialize each channel and channels properites
                channels.Initialize();
                // establish a new connection
                channels.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection to testing device could not be established!", "Connection failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                // debug
                Output.WriteLine("Connection exception: {0}, Type: {1}", ex.Message, ex.GetType());
                // may be connection was created, but initialization failed
                if (channels.IsConnected)
                    try
                    {
                        channels.Disconnect();
                    }
                    catch { }           // do not display nothing, this is not interesting    
                success = false;        // failed to start shift
            }
            // return value indicating whether the connection was or wasn't created
            return success;
        }

        /// <summary>
        /// Bind channels with events
        /// </summary>
        /// <param name="channels"></param>
        private void bindChannels(Channels channels)
        {
            channels.AllowPowerSupply.PropertyChanged += new PropertyChangedEventHandler(allowPowerSupplyChanged);
            channels.IsPowerSupplyOff.PropertyChanged += new PropertyChangedEventHandler(isPowerSupplyOnChanged);
        }

        #region Channels Events

        /// <summary>
        /// Constant string "Value"
        /// </summary>
        private const string ValueString = "Value";

        /// <summary>
        /// This method is called when AllowPowerSupply channel change
        /// </summary>
        void allowPowerSupplyChanged(object sender, PropertyChangedEventArgs e)
        {
            IDigitalOutput chan = (sender as IDigitalOutput);
            // check if it was digital output and Value property has been changed
            if (chan != null && e.PropertyName == ValueString)
            {
                if (chan.Value)
                {
                    Output.WriteLine("Switch on power supply");
                }
                else { }
            }
        }
        /// <summary>
        /// This method is called when IsPowerSupplyOn channel change
        /// </summary>
        void isPowerSupplyOnChanged(object sender, PropertyChangedEventArgs e)
        {
            IDigitalInput chan = (sender as IDigitalInput);
            // check if it was digital input and Value property has been changed
            if (chan != null && e.PropertyName == ValueString)
            {   // just describe status of power supply - ON or OFF
                if (chan.Value)
                {
                    Output.WriteLine("Power supply in ON");
                }
                else
                {
                    Output.WriteLine("Power supply is OFF");
                }
            }
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
            PointX.Z = channels.DistanceX.RealValue;
            PointY.Z = channels.DistanceY.RealValue;
            PointZ.Z = channels.DistanceZ.RealValue;
            Vector3D mirrorNormal = getPlaneNormal(PointX, PointY, PointZ);
            Vector3D axis = Vector3D.CrossProduct(ZeroPlaneNormal, mirrorNormal);

            mirrorView.RotationAxis = axis;
            mirrorView.RotationAngle = Vector3D.AngleBetween(mirrorNormal, ZeroPlaneNormal);

            double vertical = mirrorView.RotationAngle * Math.Cos(Vector3D.AngleBetween(axis, new Vector3D(1, 0, 0)) / 180 * Math.PI);
            double horizontal = mirrorView.RotationAngle * Math.Cos(Vector3D.AngleBetween(axis, new Vector3D(0, 1, 0)) / 180 * Math.PI);

            mirrorView.HorizontalAngle = horizontal;
            mirrorView.VerticalAngle = vertical;

        }

        // test

        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde X
        /// </summary>
        protected Point3D PointX;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Y
        /// </summary>
        protected Point3D PointY;
        /// <summary>
        /// Position in the 3D space of the surface which position is measured by sonde Z
        /// </summary>
        protected Point3D PointZ;

        /// <summary>
        /// (Get) Normal vector of mirror plane in the zero position. This is the moment when mirror
        /// is not rotated
        /// </summary>
        protected Vector3D ZeroPlaneNormal { get; private set; }
        private Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        {   // get two vectors from tree points. Cross product gives us a pependicular vector to both of them
            return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        }

        #endregion

        #endregion

        #region Constructors

        public TestWindow()
        {
            InitializeComponent();
            IsRunning = false;
            IsParamLoaded = false;

            // messages
            ParamStatusMessage = "Load parameters";
            ShiftStatusMessage = "Not running";
            DeviceStatusMessage = "Disconnected";

            // load hardware settings
            PointX = HWSettings.Default.SondeXPosition;
            PointY = HWSettings.Default.SondeYPosition;
            PointZ = HWSettings.Default.SondeZPosition;

            ZeroPlaneNormal = HWSettings.Default.ZeroPlaneNormal;
        }

        #endregion        

        // for presentation purpose only

        private void startButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (channels != null)
                channels.IsStartPressed.SetValue(true);
        }

        private void startButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (channels != null)
                channels.IsStartPressed.SetValue(false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
        }

        // for presentation purpose only
    }
}
