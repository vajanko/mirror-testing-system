using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using System.Drawing.Printing;
using System.Security.Cryptography;
using AvalonDock;
using Microsoft.Win32;

using MTS.Properties;
using MTS.Controls;
using MTS.IO;
using MTS.Data;
using MTS.Utils;
using System.Collections.Specialized;

namespace MTS.Admin
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class SettingsWindow : DocumentItem
    {
        #region Constants

        /// <summary>
        /// Constant string "DisplayTitle"
        /// </summary>
        public const string ChannelSettingsString = "ChannelSettings";
        /// <summary>
        /// Supported types of protocol
        /// </summary>
        private readonly string[] protocolTypes = new string[] { "EtherCAT", "Modbus", "Dummy" };

        #endregion

        #region Properties

        /// <summary>
        /// (Get) Supported types of protocol
        /// </summary>
        public string[] ProtocolTypes { get { return protocolTypes; } }

        private ChannelSettings _channelSettings;
        /// <summary>
        /// (Get) List of <paramref name="ChannelSetting"/> describing settings of a particular analog
        /// channel. This list is saved in application settings. 
        /// </summary>
        public ChannelSettings ChannelSettings
        {
            get { return _channelSettings; }
            private set
            {
                _channelSettings = value;
                RaisePropertyChanged(ChannelSettingsString);
            }
        }

        #endregion

        #region Control Events

        /// <summary>
        /// This method is called when any of Settings.Default property get changed
        /// </summary>
        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {   // this will change DisplayTitle property - a star is added at the end
            //Saved = false;
        }
        /// <summary>
        /// This method is called after Settings.Default are saved
        /// </summary>
        private void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {   // this will change DisplayTitle property - star at the end of title is removed 
            IsSaved = true;
        }
        /// <summary>
        /// This method is called when any of <see cref="Controls.UpDownButton"/> in this window change its value.
        /// State of settings window will be changed to usaved.
        /// </summary>
        void upDownButton_ValueChanged(object sender, RoutedEventArgs e)
        {   // this will change DisplayTitle property - a start is added at the end
            if (sender is Controls.UpDownButton)
                IsSaved = false;
        }
        /// <summary>
        /// This method is called when any of <see cref="TextBox"/> in this window change its value.
        /// State of settings window will be changed to usaved.
        /// </summary>
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {   // this will change DisplayTitle property - a start is added at the end
            if (sender is TextBox)
                IsSaved = false;
        }
        /// <summary>
        /// This method is called when any of <see cref="ComboBox"/> in this window change its selected value.
        /// State of settings window will be changed to usaved.
        /// </summary>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   // this will change DisplayTitle property - a star is added at the end
            if (e.Source is ComboBox)
                IsSaved = false;
        }
        /// <summary>
        /// This method is called when some property of any item in <see cref="ChannelSettings"/> collecion change.
        /// State of settings window will be changed to usaved.
        /// </summary>
        private void ChannelSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            IsSaved = false;
        }

        /// <summary>
        /// This method is called when once settings window is initilized
        /// </summary>
        private void settingsWindow_Initialized(object sender, EventArgs e)
        {
            // initialize OPERATORS section
            initializeOperators();

            // load application settings
            // initialize PRINTER section
            // get names of all installed printers on this conputer
            foreach (var printer in PrinterSettings.InstalledPrinters)
                this.printers.Items.Add(printer.ToString());
            
            // select saved printer from settings
            if (!string.IsNullOrEmpty(Settings.Default.PrinterName))
                // if saved printer is already not installed - selected index will be -1
                printers.SelectedItem = Settings.Default.PrinterName;
            else
                printers.SelectedIndex = 0;     // select first printer
            // load paper size setting
            paperWidth.Value = Settings.Default.PrinterWidth;
            paperHeight.Value = Settings.Default.PrinterHeight;
            printLabels.IsChecked = Settings.Default.PrintLabels;

            // initialize PROTOCOL section
            initializeProtocol();

            // initialize CALIBRETOR section
            initializeCalibrators();

            // initialize CHANNELS section
            ChannelSettings = HWSettings.Default.ChannelSettings;
            ChannelSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ChannelSettings_PropertyChanged);

            // register handler for application setting saving event
            Settings.Default.SettingsSaving += new System.Configuration.SettingsSavingEventHandler(Default_SettingsSaving);

            // remove star from title, when settings are just loaded
            IsSaved = true;
        }

        /// <summary>
        /// This method is called when close button in settings window is clicked. Window will be close imediatelly
        /// if all settings are saved. Otherwise user will be asked about discarding or saveing settings
        /// </summary>
        private void close_Click(object sender, RoutedEventArgs e)
        {   // fire on close event
            this.Close();   // this could be also canceled when settings are not saved
        }
        /// <summary>
        /// This method is called when apply button in settings window is clicked. All changed settings will be save
        /// </summary>
        private void apply_Click(object sender, RoutedEventArgs e)
        {
            // PRINTER
            // save selected printer name
            if (printers.SelectedItem != null)
                Settings.Default.PrinterName = printers.SelectedItem.ToString();
            Settings.Default.PrinterWidth = (int)paperWidth.Value;
            Settings.Default.PrinterHeight = (int)paperHeight.Value;
            Settings.Default.PrintLabels = (bool)printLabels.IsChecked;

            // PROTOCOL
            // save selected protocol name
            if (protocols.SelectedItem != null)
            {
                string prot = protocols.SelectedItem.ToString();
                Settings.Default.Protocol = prot;
                // save settings for selected protocol
                saveProtocolSettings(prot);
            }

            // CALIBRETOR
            HWSettings.Default.XYDistance = (double)xyDistance.Value;
            HWSettings.Default.YZDistance = (double)yzDistance.Value;
            HWSettings.Default.XZDistance = (double)xzDistance.Value;
            caltulateCalibratorsPositions((double)xyDistance.Value, (double)yzDistance.Value, (double)xzDistance.Value);

            HWSettings.Default.Save();            
            Settings.Default.Save();

            IsSaved = true;
        }

        #endregion

        #region Protocol

        /// <summary>
        /// This method is called once when settings window is initialized. It will load protocol settings from
        /// application settings permanent storage - <see cref="Settings"/>
        /// </summary>
        private void initializeProtocol()
        {
            // select saved protocol from settings
            if (!string.IsNullOrEmpty(Settings.Default.Protocol))
                protocols.SelectedValue = Settings.Default.Protocol;
            if (protocols.SelectedIndex < 0)
                protocols.SelectedIndex = 0;    // select first protocol if non was selected
            // load setting for all protocol types
            // ETHERCAT
            ethercatSettings.ConfigFilePath = Settings.Default.EthercatConfigFile;
            ethercatSettings.TaskName = Settings.Default.EthercatTaskName;
            // MODBUS
            modbusSettings.ConfigFilePath = Settings.Default.ModbusConfigFile;
            modbusSettings.IPAddress = Settings.Default.ModbusIpAddress;
            modbusSettings.Port = Settings.Default.ModbusPort;
            // DUMMY

            // make visible settings part for currently selected protocol
            showProtocolSettings(protocols.SelectedItem.ToString());
        }
        /// <summary>
        /// This method is called when another protocol from protocol type selection is selected.
        /// Afteer that settings panel for required protocol is shown.
        /// When applying settings - only selected setting of protocol are saved
        /// </summary>
        /// <param name="sender">Item in combobox which was selected</param>
        private void protocols_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
                {
                    // currently selected combo box item
                    string selected = e.AddedItems[0].ToString();
                    // just deselected combo box item
                    string deselected = e.RemovedItems[0].ToString();
                    // show settings panel of selected protocol
                    showProtocolSettings(selected);
                    // hide settings panel of deselected protocol
                    hideProtocolSettings(deselected);
                    // settings have been changed - another protocol is used now
                    IsSaved = false;
                }
                e.Handled = true;
            }
        }
        /// <summary>
        /// Show settings panel for given protocol
        /// </summary>
        /// <param name="protocol">Name of protocol to show panel for</param>
        private void showProtocolSettings(string protocol)
        {
            protocol = protocol.ToLower();
            switch (protocol)
            {
                case "ethercat": ethercatSettings.Visibility = Visibility.Visible;
                    break;
                case "modbus": modbusSettings.Visibility = Visibility.Visible;
                    break;
                case "dummy":
                    break;
            }
        }
        /// <summary>
        /// Hide settings panel for given protocol
        /// </summary>
        /// <param name="protocol">Name of protocol to hide panel for</param>
        private void hideProtocolSettings(string protocol)
        {
            protocol = protocol.ToLower();
            switch (protocol)
            {
                case "ethercat": ethercatSettings.Visibility = Visibility.Collapsed;
                    break;
                case "modbus": modbusSettings.Visibility = Visibility.Collapsed;
                    break;
                case "dummy":
                    break;
            }
        }
        /// <summary>
        /// Save settings for given protocol
        /// </summary>
        /// <param name="protocol">Name of protocol to save settings for</param>
        private void saveProtocolSettings(string protocol)
        {
            protocol = protocol.ToLower();
            switch (protocol)
            {
                case "ethercat":
                    Settings.Default.EthercatTaskName = ethercatSettings.TaskName;
                    Settings.Default.EthercatConfigFile = ethercatSettings.ConfigFilePath;
                    break;
                case "modbus":
                    Settings.Default.ModbusIpAddress = modbusSettings.IPAddress;
                    Settings.Default.ModbusPort = modbusSettings.Port;
                    Settings.Default.ModbusConfigFile = modbusSettings.ConfigFilePath;
                    break;
                case "dummy":
                    break;
            }
        }

        /// <summary>
        /// This method is called when browse button on EtherCAT settings panel is clicked
        /// </summary>
        private void ethercatSettings_BrowseClick(object sender, RoutedEventArgs e)
        {
            // open file dialog is created acording application settings in Settings.Default
            var dialog = Settings.Default.CreateOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {   // user has enter valid configuration file
                ethercatSettings.ConfigFilePath = dialog.FileName;
            }
        }
        /// <summary>
        /// This method is called when browse button on Modbus settings panel is clicked
        /// </summary>
        private void modbusSettings_BrowseClick(object sender, RoutedEventArgs e)
        {
            // open file dialog is created acording application settings in Settings.Default
            var dialog = Settings.Default.CreateOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {   // user has enter valid configuration file
                modbusSettings.ConfigFilePath = dialog.FileName;
            }
        }

        #endregion

        #region Calibrators

        /// <summary>
        /// Position of X-calibrator in 3D space (Z-coordinate is unused)
        /// </summary>
        private Point3D xPosition;
        /// <summary>
        /// Position of Y-calibrator in 3D space (Z-coordinate is unused)
        /// </summary>
        private Point3D yPosition;
        /// <summary>
        /// Position of Z-calibrator in 3D space (Z-coordinate is unused)
        /// </summary>
        private Point3D zPosition;

        /// <summary>
        /// This method is called once when settings window is initialized. It will load calibrator settings from
        /// application settings permanent storage - <see cref="HWSettings"/>
        /// </summary>
        private void initializeCalibrators()
        {   // initialize values of UpDownButtons
            // convert from double to int (number is rounded) and then to decimal
            xyDistance.Value = (decimal)(int)HWSettings.Default.XYDistance;
            yzDistance.Value = (decimal)(int)HWSettings.Default.YZDistance;
            xzDistance.Value = (decimal)(int)HWSettings.Default.XZDistance;

            updateCalibretorsPositions();
        }
        /// <summary>
        /// From given distances of between three calibrators calculate their positions in 2D space where 
        /// X calibrator will be constant [0,0] and Y will be placed always on y-axis
        /// </summary>
        /// <param name="xy">Distance between X and Y calibrator</param>
        /// <param name="yz">Distance between Y and Z calibrator</param>
        /// <param name="xz">Distance between X and Z calibrator</param>
        private void caltulateCalibratorsPositions(double xy, double yz, double xz)
        {
            double cosRes = ((xy * xy) + (xz * xz) - (yz * yz)) / (2 * xy * xz);
            double beta = Math.Acos(cosRes);

            xPosition.X = 0;
            xPosition.Y = 0;

            // check position if they are inside the calibratorsCanvas
            yPosition.X = 0;
            yPosition.Y = xy;


            zPosition.Y = Math.Cos(beta) * xz;
            zPosition.X = Math.Sin(beta) * xz;
        }
        /// <summary>
        /// Recalculate positions of calibrators in 2D space and update user interface
        /// </summary>
        private void updateCalibretorsPositions()
        {
            // recalculate calibrators positions so that thay can not be placed outside the area
            double a2 = (double)(xyDistance.Value * xyDistance.Value);
            double b2 = (double)(yzDistance.Value * yzDistance.Value);
            double c2 = (double)(xzDistance.Value * xzDistance.Value);

            xyDistance.MinValue = 0;
            xyDistance.MaxValue = yzDistance.Value + xzDistance.Value;

            yzDistance.MaxValue = Math.Min((decimal)(int)Math.Sqrt(a2 + c2), 200);
            yzDistance.MinValue = Math.Max((decimal)(int)Math.Sqrt(c2 - a2), 0);

            xzDistance.MaxValue = Math.Min((decimal)(int)Math.Sqrt(a2 + b2), 200);
            xzDistance.MinValue = Math.Max((decimal)(int)Math.Sqrt(b2 - a2), 0);


            caltulateCalibratorsPositions((double)xyDistance.Value, (double)yzDistance.Value, (double)xzDistance.Value);

            calibretorX.SetValue(Canvas.LeftProperty, xPosition.X);
            calibretorX.SetValue(Canvas.BottomProperty, xPosition.Y);

            calibretorY.SetValue(Canvas.LeftProperty, yPosition.X);
            calibretorY.SetValue(Canvas.BottomProperty, yPosition.Y);

            calibretorZ.SetValue(Canvas.LeftProperty, zPosition.X);
            calibretorZ.SetValue(Canvas.BottomProperty, zPosition.Y);
        }

        /// <summary>
        /// This method is called when any distace between two calibretors change. We will recalculate the
        /// calibretors positions in 2D space and display it to user. After calling this mehtod sate of settings
        /// window will change to unsaved.
        /// </summary>
        private void distance_ValueChanged(object sender, RoutedEventArgs e)
        {
            updateCalibretorsPositions();
            IsSaved = false;    // setting changed
        }
        /// <summary>
        /// This method is called when calibrate button in Calibrators section is clicked
        /// </summary>
        private void calibrate_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow form = new CalibrationWindow();

            if (form.ShowDialog() == true)      // calibration finished successfully
            {   // show calibrated values to user (do not save yet)
                IsSaved = false;      // calibration values has been changed
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Database layer (EntityFramework)
        /// </summary>
        private MTSContext context;

        /// <summary>
        /// This method is called once when settings window is initialized. At this place we will decide if
        /// operators grid will be displayed
        /// </summary>
        private void initializeOperators()
        {
            // hide operators if loged user is not admin
            if (Operator.IsInRole(Data.Types.OperatorType.Admin))
            {
                operatorsExpander.Visibility = System.Windows.Visibility.Visible;
                // this method will load operators from database
                operatorsGrid.Loaded += new RoutedEventHandler(operatorsGrid_Loaded);
            }
            else
            {   // in this case operators will not be loaded
                operatorsExpander.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// This method is called once when operators grid get loaded
        /// </summary>
        private void operatorsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid)   // if sender is grid load it
            {
                context = new MTSContext(); // connect to database
                updateOperatorsGrid();      // load operators
            }
        }
        /// <summary>
        /// This method is called when add button in operators settings section is clicked. A window for adding new 
        /// operator will be opened
        /// </summary>
        private void addOperator_Click(object sender, RoutedEventArgs e)
        {
            // create dialog for creating a new operator
            EditOperatorWindow dialog = new EditOperatorWindow();

            if (dialog.ShowDialog() == true)    // otherwise no operator will be added
            {
                // from password compute hash - only that will be saved to database
                string password = Operator.ComputeHash(dialog.Password);

                try
                {   // create and add new operator
                    context.Operators.Add(new Data.Operator()
                    {
                        Login = dialog.Login,
                        Name = dialog.OperatorName,
                        Surname = dialog.OperatorSurname,
                        Password = password,
                        Type = (byte)dialog.Type
                    });
                    // commit changes - here an exception could be thrown
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowError(ex);
                }
                updateOperatorsGrid();
            }
        }
        /// <summary>
        /// This method is called when delete button in operators settings section is clicked. Selected operator
        /// will be deleted and all its data
        /// </summary>
        private void deleteOperator_Click(object sender, RoutedEventArgs e)
        {   // get selected operator from the grid
            Data.Operator op = operatorsGrid.SelectedValue as Data.Operator;
            if (op == null)
            {   // no operator is selected
                ExceptionManager.ShowError(Errors.ErrorTitle, Errors.ErrorIcon, "No operator is selected. Select an operator and then click \"Delete\"");
            }
            else
            {
                try
                {
                    // delete operator (and its data)
                }
                catch (Exception ex)
                {
                    ExceptionManager.ShowError(ex);
                }
            }
        }

        private void editOperator_Click(object sender, RoutedEventArgs e)
        {
            Data.Operator op = operatorsGrid.SelectedValue as Data.Operator;
            if (op == null)
                ExceptionManager.ShowError(Errors.ErrorTitle, Errors.ErrorIcon, "No operator is selected. Select an operator and then click \"Edit\"");
            else
            {
                EditOperatorWindow dialog = new EditOperatorWindow(op.Name, op.Surname, op.Login, (Data.Types.OperatorType)op.Type);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        op.Name = dialog.OperatorName;
                        op.Surname = dialog.OperatorSurname;
                        op.Login = dialog.Login;
                        op.Type = (byte)dialog.Type;
                        context.SaveChanges();
                        updateOperatorsGrid();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.ShowError(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Load operators from database and set them as data context to operators grid
        /// </summary>
        private void updateOperatorsGrid()
        {
            operatorsGrid.DataContext = context.Operators.ToList();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="SettingsWindow"/> and initialize its componens
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
