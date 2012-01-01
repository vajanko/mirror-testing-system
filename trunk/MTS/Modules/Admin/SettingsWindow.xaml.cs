﻿using System;
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

        private readonly string[] protocolTypes = new string[] { "EtherCAT", "Modbus", "Dummy" };

        #endregion

        #region Properties

        public string[] ProtocolTypes { get { return protocolTypes; } }

        //private bool saved; // use this property when do not want to raise PropertyChanged event
        ///// <summary>
        ///// (Get) When true document content is saved. Notice that when false, document content could be only
        ///// changed or the file does not exists yet.
        ///// </summary>
        //public bool Saved
        //{
        //    get { return saved; }
        //    protected set
        //    {
        //        saved = value;  // when document is unsaved a standard star is displayed at the top of document
        //        RaisePropertyChanged(DisplayTitleString);    // add or remove star in displayed string
        //    }
        //}
        ///// <summary>
        ///// (Get) Title of application settings tab. When some setting is changed an star (*) is
        ///// added at the end of title. 
        ///// </summary>
        //public string DisplayTitle
        //{
        //    get { return Saved ? "Settings" : "Settings*"; }
        //}

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

        #region Private Methods

        /// <summary>
        /// This method is called when any of Settings.Default property get changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {   // this will change DisplayTitle property - a star is added at the end
            //Saved = false;
        }
        /// <summary>
        /// This method is called after Settings.Default are saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Default_SettingsSaving(object sender, System.ComponentModel.CancelEventArgs e)
        {   // this will change DisplayTitle property - star at the end of title is removed 
            IsSaved = true;
        }

        void upDownButton_ValueChanged(object sender, RoutedEventArgs e)
        {   // this will change DisplayTitle property - a start is added at the end
            if (sender is Controls.UpDownButton)
                IsSaved = false;
        }
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {   // this will change DisplayTitle property - a start is added at the end
            if (sender is TextBox)
                IsSaved = false;
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   // this will change DisplayTitle property - a star is added at the end
            if (e.Source is ComboBox)
                IsSaved = false;
        }

        #endregion

        #region Settings

        private void settingsWindow_Initialized(object sender, EventArgs e)
        {
            // hide operators if loged user is not admin
            if (Operator.IsInRole(Data.Types.OperatorType.Admin))
                operatorsExpander.Visibility = System.Windows.Visibility.Visible;
            else
                operatorsExpander.Visibility = System.Windows.Visibility.Collapsed;

            // load application settings
            // PRINTER
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

            // PROTOCOL
            // select save protocol from settings
            if (!string.IsNullOrEmpty(Settings.Default.Protocol))
                protocols.SelectedValue = Settings.Default.Protocol;
            if (protocols.SelectedIndex < 0)
                protocols.SelectedIndex = 0;    // select first protocol
            // load setting for all protocol types
            loadProtocolSettings();
            // make visible settings part for currently selected protocol
            showProtocolSettings(protocols.SelectedItem.ToString().ToLower());

            // CALIBRETOR - remove this
            xPosition = HWSettings.Default.CalibretorX;
            yPosition = HWSettings.Default.CalibretorY;
            zPosition = HWSettings.Default.CalibretorZ;
            xyDistance.Value = (decimal)(int)(yPosition.Y - xPosition.Y);
            yzDistance.Value = (decimal)(int)(Math.Sqrt(Math.Pow(yPosition.Y - zPosition.Y, 2) + Math.Pow(zPosition.X, 2)));
            xzDistance.Value = (decimal)(int)(Math.Sqrt(Math.Pow(zPosition.Y, 2) + Math.Pow(zPosition.X, 2)));
            updateCalibretorsPositions();

            ChannelSettings = HWSettings.Default.ChannelSettings;            

            // register handler for application setting saving event
            Settings.Default.SettingsSaving += new System.Configuration.SettingsSavingEventHandler(Default_SettingsSaving);

            // remove start from title, when settings are just loaded
            IsSaved = true;
        }

        #endregion

        #region Button Clicks

        private void calibrate_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow form = new CalibrationWindow();

            if (form.ShowDialog() == true)      // calibration finished successfully
            {   // show calibrated values to user (do not save yet)
                IsSaved = false;      // calibration values has been changed
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {   // fire on close event
            this.Close();
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            // PRINTER
            // save selected printer name
            if (printers.SelectedItem != null)
                Settings.Default.PrinterName = printers.SelectedItem.ToString();
            Settings.Default.PrinterWidth = paperWidth.Value;
            Settings.Default.PrinterHeight = paperHeight.Value;

            // PROTOCOL
            // save selected protocol name
            if (protocols.SelectedItem != null)
            {
                string prot = protocols.SelectedItem.ToString();
                Settings.Default.Protocol = prot;
                // save settings for selected protocol
                saveProtocolSettings(prot.ToLower());
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

        private void ethercatSettings_BrowseClick(object sender, RoutedEventArgs e)
        {
            // open file dialog is created acording application settings in Settings.Default
            var dialog = Settings.Default.CreateOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {   // user has enter valid configuration file
                ethercatSettings.ConfigFilePath = dialog.FileName;                
            }
        }

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

        #region Protocol

        /// <summary>
        /// This method is called when another protocol from protocol type selection is selected.
        /// Afteer that settings panel for required protocol is shown.
        /// When applying settings - only selected setting of protocol are saved
        /// </summary>
        /// <param name="sender">Item in combobox which was selected</param>
        /// <param name="e"></param>
        private void protocols_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is ComboBox)
            {
                if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
                {
                    // currently selected combo box item
                    string selected = e.AddedItems[0].ToString().ToLower();
                    // just deselected combo box item
                    string deselected = e.RemovedItems[0].ToString().ToLower();
                    // show settings panel of selected protocol
                    showProtocolSettings(selected);
                    // hide settings panel of deselected protocol
                    hideProtocolSettings(deselected);
                }
                e.Handled = true;
            }
        }
        private void showProtocolSettings(string protocol)
        {
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
        private void hideProtocolSettings(string protocol)
        {
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
        private void saveProtocolSettings(string protocol)
        {
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
        private void loadProtocolSettings()
        {   // load settings for all protocol types
            // ETHERCAT
            ethercatSettings.ConfigFilePath = Settings.Default.EthercatConfigFile;
            ethercatSettings.TaskName = Settings.Default.EthercatTaskName;
            // MODBUS
            modbusSettings.ConfigFilePath = Settings.Default.ModbusConfigFile;
            modbusSettings.IPAddress = Settings.Default.ModbusIpAddress;
            modbusSettings.Port = Settings.Default.ModbusPort;
        }

        #endregion

        #region Sonds Caluculation

        private Point3D xPosition;
        private Point3D yPosition;
        private Point3D zPosition;

        private void caltulateCalibratorsPositions(double xy, double yz, double xz)
        {
            double cosRes = ((xy * xy) + (xz * xz) - (yz * yz)) / (2 * xy * xz);
            double beta = Math.Acos(cosRes);

            xPosition.X = 0;
            xPosition.Y = 0;

            yPosition.X = 0;
            yPosition.Y = xy;

            zPosition.Y = Math.Cos(beta) * xz;
            zPosition.X = Math.Sin(beta) * xz;
        }

        private void updateCalibretorsPositions()
        {
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
        /// calibretors positions in 2D space and display it to user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void distance_ValueChanged(object sender, RoutedEventArgs e)
        {
            updateCalibretorsPositions();
        }

        #endregion

        #region Operators (Database)

        private MTSContext context;

        private void operatorsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid != null)
            {
                context = new MTSContext();
                updateOperatorsGrid();
            }
        }

        private void addOperator_Click(object sender, RoutedEventArgs e)
        {
            EditOperatorWindow dialog = new Data.EditOperatorWindow();

            if (dialog.ShowDialog() == true)
            {
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
                catch
                {
                    MessageBox.Show("Could not be added");
                }
                updateOperatorsGrid();
            }
        }
        private void updateOperatorsGrid()
        {
            operatorsGrid.DataContext = context.Operators.ToList();
        }
       

        #endregion

        #region Constructors

        public SettingsWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}