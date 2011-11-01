using System.IO;

namespace MTS.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
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
            // if in application settings absolute path to configuration direcotry is defined, just return
            // otherwise get path to executiong directory and combine it with relative configuration 
            // directory from application settings
            if (Path.IsPathRooted(Default.ConfigDir))
                return Default.ConfigDir;
            else
                return Path.Combine(GetExecutingDirectory(), Default.ConfigDir);
        }

        /// <summary>
        /// Get absolute system path to file where template for test collection file is stored
        /// </summary>
        /// <returns>Absolute path to template file</returns>
        public string GetTemplatePath()
        {
            return Path.Combine(GetConfigDirectory(), Settings.Default.TemplateFile);
        }

        public string GetChannelsConfigPath()
        {
            return Path.Combine(GetConfigDirectory(), this.ChannelsConfigFile);
        }

        /// <summary>
        /// Create an open file dialog that will handle opening of caonfiguration file
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
        /// <paramref name="MTS.IO.Module.ECModule"/> and load its configuration from file which is defined in this
        /// settings as configuration file for EtherCAT protocol.
        /// </summary>
        /// <returns>A loaded an initialized instance of module prepared for communication with tester hardware</returns>
        public MTS.IO.IModule GetModuleInstance()
        {
            // make a decision based on current protocol settings
            string protocol = this.Protocol;
            protocol.ToLower();
            MTS.IO.IModule module;

            switch (protocol)
            {
                case "ethercat": 
                    module = new MTS.IO.Module.ECModule(this.EthercatTaskName);
                    module.LoadConfiguration(this.EthercatConfigFile);
                    break;
                case "modbus":
                    module = new MTS.IO.Module.ModbusModule(this.ModbusIpAddress, this.ModbusPort);
                    module.LoadConfiguration(this.ModbusConfigFile);
                    break;
                default:
                    module = new MTS.IO.Module.DummyModule();
                    break;
            }
            module.Initialize();

            return module;
        }
    }
}
