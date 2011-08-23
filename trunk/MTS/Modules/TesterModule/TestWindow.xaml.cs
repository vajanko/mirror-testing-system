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

using System.Windows.Media.Media3D;

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
            //Channels = new Channels(new ECModule("Task1"));
            Channels = new Channels(new DummyModule());

            Channels.LoadConfiguration("C:/task1.csv");
            Channels.Connect();

            Channels.Initialize();
            
            scheduler = new TaskScheduler(Channels);

            scheduler.AddTask(new CenterTask(Channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });
            scheduler.AddTask(new TravelTest(Channels, Tests[TestDictionary.TRAVEL_NORTH],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Up));
            scheduler.AddTask(new CenterTask(Channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(Channels, Tests[TestDictionary.TRAVEL_SOUTH],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Down));
            scheduler.AddTask(new CenterTask(Channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(Channels, Tests[TestDictionary.TRAVEL_EAST],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Right));
            scheduler.AddTask(new CenterTask(Channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            scheduler.AddTask(new TravelTest(Channels, Tests[TestDictionary.TRAVEL_WEST],
                ZeroPlaneNormal, PointX, PointY, PointZ, MoveDirection.Left));
            scheduler.AddTask(new CenterTask(Channels, ZeroPlaneNormal, PointX, PointY, PointZ) { Name = "Center" });

            //scheduler.AddTask(new SpiralTest(Channels, Tests[TestDictionary.SPIRAL]));
            //scheduler.AddTask(new HeatingSignPresence(Channels, Tests[TestDictionary.SPIRAL]));
            //scheduler.AddTask(new BlinkerTest(Channels, Tests[TestDictionary.BLINKER]));
            //scheduler.AddTask(new PowerfoldTest(Channels, Tests[TestDictionary.POWERFOLD]));

            Channels.DistanceX.SetValue((uint)PointX.Z);
            Channels.DistanceY.SetValue((uint)PointY.Z);
            Channels.DistanceZ.SetValue((uint)PointZ.Z);

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
            scheduler.UpdateOutputs(watch.Elapsed);
            scheduler.Update(watch.Elapsed);

            spiralCurrent.Dispatcher.BeginInvoke(new Action(updateGui));
        }

        #endregion

        private void updateGui()
        {
            setRotation();
            spiralCurrent.AddValue(Channels.HeatingFoilCurrent.RealValue);
            actuatorACurrent.AddValue(Channels.VerticalActuatorCurrent.RealValue);
            actuatorBCurrent.AddValue(Channels.HorizontalActuatorCurrent.RealValue);
        }
        private void setRotation()
        {
            PointX.Z = Channels.DistanceX.Value;
            PointY.Z = Channels.DistanceY.Value;
            PointZ.Z = Channels.DistanceZ.Value;
            Vector3D mirrorNormal = getPlaneNormal(PointX, PointY, PointZ);
            Vector3D axis = Vector3D.CrossProduct(ZeroPlaneNormal, mirrorNormal);
            mirrorView.RotationAxis = axis;
            mirrorView.RotationAngle = Vector3D.AngleBetween(mirrorNormal, ZeroPlaneNormal);
        }

        #region Constructors

        public TestWindow()
        {
            InitializeComponent();
            IsRunning = false;
            // test
            xSonde.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sonde_ValueChanged);
            ySonde.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sonde_ValueChanged);
            zSonde.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sonde_ValueChanged);

            PointX = new Point3D(0, 0, 100);          // zero plane
            PointY = new Point3D(200, 0, 100);
            PointZ = new Point3D(200, 100, 100);
            ZeroPlaneNormal = getPlaneNormal(PointX, PointY, PointZ);

            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 400;  // ms
        }

        #endregion

        void sonde_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Channels.DistanceX.SetValue((uint)xSonde.Value);
            //Channels.DistanceY.SetValue((uint)ySonde.Value);
            //Channels.DistanceZ.SetValue((uint)zSonde.Value);

            PointX.Z = xSonde.Value;
            PointY.Z = ySonde.Value;
            PointZ.Z = zSonde.Value;
            Vector3D mirrorNormal = getPlaneNormal(PointX, PointY, PointZ);
            Vector3D axis = Vector3D.CrossProduct(ZeroPlaneNormal, mirrorNormal);
            mirrorView.RotationAxis = axis;
            mirrorView.RotationAngle = Vector3D.AngleBetween(mirrorNormal, ZeroPlaneNormal);
        }

        // test area
        private Vector3D getPlaneNormal(Point3D x, Point3D y, Point3D z)
        {   // get two vectors from tree points. Cross product gives us a pependicular vector to both of them
            return Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
        }

        /// <summary>
        /// (Get) Angle between mirror surface and zero mirror position surface
        /// </summary>
        protected double Angle
        {
            get { return Vector3D.AngleBetween(getPlaneNormal(PointX, PointY, PointZ), ZeroPlaneNormal); }
        }

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

        private void resetClick(object sender, RoutedEventArgs e)
        {
            xSonde.Value = 100;
            ySonde.Value = 100;
            zSonde.Value = 100;
        }
    }
}
