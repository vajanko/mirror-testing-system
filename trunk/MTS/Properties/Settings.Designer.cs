﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.431
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MTS.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Name of protocol used to communicate with tester hardware. Possible values are: Modbus, Ethercat or Dummy
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Name of protocol used to communicate with tester hardware. Possible values are: M" +
            "odbus, Ethercat or Dummy")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Dummy")]
        public string Protocol {
            get {
                return ((string)(this["Protocol"]));
            }
            set {
                this["Protocol"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Task1")]
        public string EthercatTaskName {
            get {
                return ((string)(this["EthercatTaskName"]));
            }
            set {
                this["EthercatTaskName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("127.0.0.1")]
        public string ModbusIpAddress {
            get {
                return ((string)(this["ModbusIpAddress"]));
            }
            set {
                this["ModbusIpAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("502")]
        public ushort ModbusPort {
            get {
                return ((ushort)(this["ModbusPort"]));
            }
            set {
                this["ModbusPort"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("logs.txt")]
        public string LogFile {
            get {
                return ((string)(this["LogFile"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("task1.csv")]
        public string EthercatConfigFile {
            get {
                return ((string)(this["EthercatConfigFile"]));
            }
            set {
                this["EthercatConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("template.xml")]
        public string TemplateFile {
            get {
                return ((string)(this["TemplateFile"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("config")]
        public string ConfigDir {
            get {
                return ((string)(this["ConfigDir"]));
            }
            set {
                this["ConfigDir"] = value;
            }
        }
        
        /// <summary>
        /// Absolute path to configuration file of channels for modbus protocol are stored
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Absolute path to configuration file of channels for modbus protocol are stored")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("moxaConfig.csv")]
        public string ModbusConfigFile {
            get {
                return ((string)(this["ModbusConfigFile"]));
            }
            set {
                this["ModbusConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal PrinterWidth {
            get {
                return ((decimal)(this["PrinterWidth"]));
            }
            set {
                this["PrinterWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public decimal PrinterHeight {
            get {
                return ((decimal)(this["PrinterHeight"]));
            }
            set {
                this["PrinterHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PrinterName {
            get {
                return ((string)(this["PrinterName"]));
            }
            set {
                this["PrinterName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("analog_channels.xml")]
        public string ChannelsConfigFile {
            get {
                return ((string)(this["ChannelsConfigFile"]));
            }
            set {
                this["ChannelsConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Setting {
            get {
                return ((string)(this["Setting"]));
            }
            set {
                this["Setting"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Program Files\\Microsoft SQL Server\\" +
            "MSSQL10.SQLEXPRESS\\MSSQL\\DATA\\mts.mdf\";Integrated Security=True;Connect Timeout=" +
            "30;User Instance=True")]
        public string MTSEntities {
            get {
                return ((string)(this["MTSEntities"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1234")]
        public ushort DummyPort {
            get {
                return ((ushort)(this["DummyPort"]));
            }
            set {
                this["DummyPort"] = value;
            }
        }
    }
}
