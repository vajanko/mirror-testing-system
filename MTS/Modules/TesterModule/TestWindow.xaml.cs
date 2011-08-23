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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MTS.EditorModule;
using MTS.AdminModule;
using MTS.Properties;

using AvalonDock;

using Microsoft.Win32;

namespace MTS.TesterModule
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : DocumentContent
    {
        #region Fields

        private Channels channels;
        private TaskScheduler scheduler;

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

        #region Shift and Parameters Properties

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
                    try
                    {   // copy some parameters to testing window                        
                        TestValue test = Tests[TestDictionary.INFO];

                        StringParamValue param = test.Parameters[ParamDictionary.PART_NUMBER] as StringParamValue;
                        partNumber.Content = param.Value;

                        param = test.Parameters[ParamDictionary.SUPPLIER_NAME] as StringParamValue;
                        supplierName.Content = param.Value;

                        param = test.Parameters[ParamDictionary.SUPPLIER_CODE] as StringParamValue;
                        description.Content = param.Value;
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
            }
        }

        /// <summary>
        /// Start shift. This means: established a new connection and execute as much tests as user has requested
        /// </summary>
        private void startClick(object sender, RoutedEventArgs e)
        {
            // disable other buttons
            IsRunning = true;

            // create communication layer
            channels = createChannels();
            // createConnection is called only if channels are not null
            if (channels == null || !createConnection(ref channels))
            {
                // shift can not start - channels were not created, or if were
                // connection was not created
                IsRunning = false;
                return;
            }
            // at this time: channels are created and configuration is loaded, connection is established
            // we may start to execute tasks
            startExecutingLoop(channels);
        }
        private void stopClick(object sender, RoutedEventArgs e)
        {
            // dispose created connection and related resources
            if (channels != null)
                channels.Disconnect();

            IsRunning = false;
        }
        void schedulerExecuted(TaskScheduler sender, EventArgs args)
        {
            if (timer != null)
                timer.Stop();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a new instance of channels collection <typeparamref name="Channels"/> acording
        /// application settings.
        /// This method will handle all exceptions!
        /// </summary>
        private Channels createChannels()
        {
            IModule module;

            // determine which module to use
            switch (Settings.Default.HW_Type)
            {
                case "Beckhoff": module = new ECModule(Settings.Default.TaskName); break;
                case "Moxa": module = new ModbusModule(Settings.Default.IpAddress, Settings.Default.Port); break;
                case "Dummy": module = new DummyModule(); break;    // this is for debugging only
                default: module = new ECModule(Settings.Default.TaskName); break;
            }

            Channels channels = new Channels(module);
            // use configuration file from Settings
            // when loading file - an exception may be thrown
            try
            {
                channels.LoadConfiguration(Settings.Default.ChannelsConfigFile);
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
            {   // establish a new connection
                channels.Connect();
                // initialize each channel and channels properites
                channels.Initialize();
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
        /// Create a new instance of scheduler and add task to it. At the time of calling channels must be created
        /// and connection must be established.
        /// No exception should be thrown!
        /// </summary>
        /// <param name="channels">Collection of hardware channels for task initialization</param>
        /// <returns>New instance of initialized TaskScheduler</returns>
        private TaskScheduler createScheduler(Channels channels)
        {
            TaskScheduler scheduler = new TaskScheduler(channels);

            // add tasks to scheduler
            scheduler.AddTask(new CenterTask(channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });
            scheduler.AddTask(new TravelTest(channels, Tests[TestDictionary.TRAVEL_NORTH],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Up));
            scheduler.AddTask(new CenterTask(channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(channels, Tests[TestDictionary.TRAVEL_SOUTH],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Down));
            scheduler.AddTask(new CenterTask(channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(channels, Tests[TestDictionary.TRAVEL_EAST],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Right));
            scheduler.AddTask(new CenterTask(channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(channels, Tests[TestDictionary.TRAVEL_WEST],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Left));
            scheduler.AddTask(new CenterTask(channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new SpiralTest(channels, Tests[TestDictionary.SPIRAL]));
            //scheduler.AddTask(new HeatingSignPresence(channels, Tests[TestDictionary.SPIRAL]));
            scheduler.AddTask(new BlinkerTest(channels, Tests[TestDictionary.BLINKER]));
            scheduler.AddTask(new PowerfoldTest(channels, Tests[TestDictionary.POWERFOLD]));

            scheduler.Initialize();
            // register handler to execute when scheduler finishes its work
            scheduler.Executed += new ExecutedHandler(schedulerExecuted);

            return scheduler;
        }

        System.Timers.Timer timer;

        private void startExecutingLoop(Channels channels)
        {
            scheduler = createScheduler(channels);

            timer = new System.Timers.Timer();
            timer.Interval = 400;   // ms
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
            IsRunning = true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            scheduler.UpdateOutputs(e.SignalTime.TimeOfDay);        // this need to be merged to one method
            scheduler.Update(e.SignalTime.TimeOfDay);

            spiralCurrent.Dispatcher.BeginInvoke(
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

        #region Constructors

        public TestWindow()
        {
            InitializeComponent();
            IsRunning = false;
            IsParamLoaded = false;

            PointX = new Point3D(0, 0, 100);
            PointY = new Point3D(200, 0, 100);
            PointZ = new Point3D(200, 100, 100);
            ZeroPlaneNormal = getPlaneNormal(PointX, PointY, PointZ);
        }

        #endregion

        //#region Debuging

        //private void updateGui()
        //{
        //    setRotation();
        //    spiralCurrent.AddValue(channels.HeatingFoilCurrent.RealValue);
        //    actuatorACurrent.AddValue(channels.VerticalActuatorCurrent.RealValue);
        //    actuatorBCurrent.AddValue(channels.HorizontalActuatorCurrent.RealValue);
        //}
        //private void setRotation()
        //{
        //    PointX.Z = channels.DistanceX.Value;
        //    PointY.Z = channels.DistanceY.Value;
        //    PointZ.Z = channels.DistanceZ.Value;
        //    Vector3D mirrorNormal = getPlaneNormal(PointX, PointY, PointZ);
        //    Vector3D axis = Vector3D.CrossProduct(ZeroPlaneNormal, mirrorNormal);
        //    mirrorView.RotationAxis = axis;
        //    mirrorView.RotationAngle = Vector3D.AngleBetween(mirrorNormal, ZeroPlaneNormal);
        //}

        //void sonde_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    //Channels.DistanceX.SetValue((uint)xSonde.Value);
        //    //Channels.DistanceY.SetValue((uint)ySonde.Value);
        //    //Channels.DistanceZ.SetValue((uint)zSonde.Value);

        //    PointX.Z = xSonde.Value;
        //    PointY.Z = ySonde.Value;
        //    PointZ.Z = zSonde.Value;
        //    Vector3D mirrorNormal = getPlaneNormal(PointX, PointY, PointZ);
        //    Vector3D axis = Vector3D.CrossProduct(ZeroPlaneNormal, mirrorNormal);
        //    mirrorView.RotationAxis = axis;
        //    mirrorView.RotationAngle = Vector3D.AngleBetween(mirrorNormal, ZeroPlaneNormal);
        //}

        //// test area
        //private Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        //{   // get two vectors from tree points. Cross product gives us a pependicular vector to both of them
        //    return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        //}

        ///// <summary>
        ///// (Get) Angle between mirror surface and zero mirror position surface
        ///// </summary>
        //protected double Angle
        //{
        //    get { return Vector3D.AngleBetween(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal); }
        //}

        ///// <summary>
        ///// Position in the 3D space of the surface which position is measured by sonde X
        ///// </summary>
        //protected Point3D PointX;
        ///// <summary>
        ///// Position in the 3D space of the surface which position is measured by sonde Y
        ///// </summary>
        //protected Point3D PointY;
        ///// <summary>
        ///// Position in the 3D space of the surface which position is measured by sonde Z
        ///// </summary>
        //protected Point3D PointZ;

        ///// <summary>
        ///// (Get) Normal vector of mirror plane in the zero position. This is the moment when mirror
        ///// is not rotated
        ///// </summary>
        //protected Vector3D ZeroPlaneNormal { get; private set; }

        //private void resetClick(object sender, RoutedEventArgs e)
        //{
        //    xSonde.Value = 100;
        //    ySonde.Value = 100;
        //    zSonde.Value = 100;
        //}

        //#endregion
    }
}
