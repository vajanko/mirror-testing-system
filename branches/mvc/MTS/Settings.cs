using System;
using System.IO;
using MTS.IO;
using MTS.Base;
using MTS.Base.Properties;

namespace MTS.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            //// To add event handlers for saving and changing settings, uncomment the lines below:

            //this.SettingChanging += this.SettingChangingEventHandler;

            //this.SettingsSaving += this.SettingsSavingEventHandler;
            
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
        /// <summary>
        /// Get absolute system path to the directory from which this application was executed
        /// </summary>
        /// <returns></returns>
        public string GetExecutingDirectory()
        {   // get directory part from executing assembly path
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        /// <summary>
        /// Get absolute path to directory where configuration files for this application are stored
        /// In configuration directory are stored, for example: template for editor, configuration of
        /// channels, ...
        /// </summary>
        /// <returns></returns>
        public string GetConfigDirectory()
        {
            // if in application settings absolute path to configuration directory is defined, just return
            // otherwise get path to executing directory and combine it with relative configuration 
            // directory from application settings
            if (Path.IsPathRooted(Default.ConfigDir))
                return Default.ConfigDir;
            else
                return Path.Combine(GetExecutingDirectory(), Default.ConfigDir);
        }
        /// <summary>
        /// Get absolute path to configuration directory of this application
        /// </summary>
        /// <returns></returns>
        //public string GetChannelsConfigPath()
        //{
        //    return Path.Combine(GetConfigDirectory(), this.ChannelsConfigFile);
        //}
        /// <summary>
        /// Get path to configuration file where channels for current protocol are saved. Current protocol is a user setting
        /// describing type of module which will be used for communication with tester hardware. If protocol is not set, 
        /// EtherCAT configuration file will be used as default value
        /// </summary>
        /// <returns>Absolute path to configuration file containing all channels used in this application</returns>
        public string GetProtocolConfigPath()
        {
            string protocolConfig;
            switch (this.Protocol.ToLower())
            {
                case "ethercat": protocolConfig = this.EthercatConfigFile; break;
                case "modbus": protocolConfig = this.ModbusConfigFile; break;
                case "dummy": protocolConfig = this.DummyConfigFile; break;
                default: protocolConfig = this.EthercatConfigFile; break;
            }
            //return protocolConfig;
            return Path.Combine(GetExecutingDirectory(), protocolConfig);
        }
        /// <summary>
        /// Get absolute path to configuration file where configuration of all tasks is stored
        /// </summary>
        /// <returns>Absolute path to tasks configuration file</returns>
        public string GetTasksConfigPath()
        {
            return Path.Combine(GetExecutingDirectory(), TasksConfigFile);
        }
        /// <summary>
        /// Get absolute path to configuration file where configuration of all tasks is stored
        /// </summary>
        /// <returns>Absolute path to calibration configuration file</returns>
        public string GetCalibConfigPath()
        {
            return Path.Combine(GetExecutingDirectory(), CalibConfigFile);
        }

        /// <summary>
        /// Create an open file dialog that will handle opening of configuration file
        /// </summary>
        /// <returns>Instance of open file dialog initialized for opening configuration files</returns>
        public Microsoft.Win32.OpenFileDialog CreateOpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".csv";
            dialog.Filter = "Configuration file (.csv)|*.csv";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.InitialDirectory = GetConfigDirectory();
            dialog.Multiselect = false;
            dialog.Title = "Open hardware module configuration file";

            return dialog;
        }

        /// <summary>
        /// Create an instance of module for communication with tester hardware based on current protocol settings.
        /// For example if current protocol is EtherCAT, this method will create an instance of
        /// <see cref="ECModule"/> and load its configuration from file which is defined in this
        /// settings as configuration file for EtherCAT protocol.
        /// </summary>
        /// <returns>A loaded an initialized instance of module prepared for communication with tester hardware</returns>
        public IModule GetModuleInstance()
        {
            // make a decision based on current protocol settings
            string protocol = this.Protocol;
            protocol.ToLower();
            MTS.IO.IModule module;

            switch (protocol)
            {
                case "ethercat": 
                    module = new MTS.IO.Module.ECModule("EtherCAT", this.EthercatTaskName);
                    break;
                case "modbus":
                    module = new MTS.IO.Module.ModbusModule(this.ModbusIpAddress, this.ModbusPort);
                    break;
                default:
                    module = new MTS.IO.Module.DummyModule(this.DummyIpAddress, this.DummyPort);
                    break;
            }

            return module;
        }
        /// <summary>
        /// Create an instance of channels for communication with tester hardware based on current protocol settings.
        /// For example if current protocol is EtherCAT, this method will create an instance of channels using
        /// <see cref="ECModule"/> and load its configuration from file which is defined in this settings as configuration file
        /// for EtherCAT protocol. This method will handle all exception and show error window to user if necessary.
        /// At the moment of calling this method no connection is established with the tester hardware.
        /// </summary>
        /// <returns>Instance of <see cref="Channels"/> with loaded channels or null if channels couldn't be created</returns>
        public Channels GetChannelsInstance()
        {
            // make a decision based on current protocol settings which module will be created for channels communication
            IModule module = this.GetModuleInstance();
            // path to configuration file where channels used for current module are stored
            string configPath = Settings.Default.GetProtocolConfigPath();
            // load channel settings from hardware settings file
            ChannelSettings settings = HWSettings.Default.ChannelSettings;

            // create just instance of channel collection, without any initialization or connection
            Channels channels = new Channels(module, settings);
            // initialize calibrators positions
            channels.InitializeCalibratorsSettings(HWSettings.Default.ZeroPlaneNormal,
                HWSettings.Default.CalibretorX, HWSettings.Default.CalibretorY, HWSettings.Default.CalibretorZ);
            try
            {   // load configuration and create channels for this module
                channels.LoadConfiguration(configPath);
            }
            catch (FileNotFoundException ex)
            {   // exception was thrown because configuration file was not found
                ExceptionManager.ShowError(ex, Errors.ErrorTitle, Errors.ErrorIcon, Errors.ConfigFileNotFoundMsg, ex.FileName);
                channels = null;
            }
            catch (Exception ex)
            {   // other error raised when configuration file was read
                ExceptionManager.ShowError(ex);
                channels = null;    // return null indicating that channels were not created
            }

            return channels;
        }
    }
}
