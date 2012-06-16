using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

using MTS.IO;
using MTS.IO.Module;
using MTS.IO.Protocol;
using MTS.Properties;

namespace MTS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Startup

        /// <summary>
        /// Register open command for this application so any testing file may be opened by double clicking
        /// on testing parameter file defined by this application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private void registerOpenCommand(string[] args)
        {
            if (args != null && args.Count() > 0)
            {   // save the absolute path to opening file - this will be handled after user interface is loaded
                this.Properties["open"] = args[0];
            }
        }


        private IModule createEthercatModule(string protocolName)
        {
            IModule module = new ECModule(protocolName, Settings.Default.EthercatTaskName);

            module.LoadConfiguration(Settings.Default.EthercatConfigFile);

            return module;
        }
        private IModule createModbusModule(string protocolName)
        {
            IModule module = new ModbusModule(Settings.Default.ModbusIpAddress, Settings.Default.ModbusPort);

            module.LoadConfiguration(Settings.Default.ModbusConfigFile);

            return module;
        }
        private IModule createDummyModule(string protcolName)
        {
            IModule module = new DummyModule(Settings.Default.DummyIpAddress, Settings.Default.DummyPort);

            module.LoadConfiguration(Settings.Default.DummyConfigFile);

            return module;
        }

        /// <summary>
        /// Register used protocols used by this application
        /// </summary>
        private void registerProtocols()
        {
            ProtocolManager.Instance.RegisterProtocol("EtherCAT", @"EtherCAT communication protocol developed by 
Beckhoff and used in automation technology considered to be the most efficeint on the world", createEthercatModule);

            ProtocolManager.Instance.RegisterProtocol("Modbus", "TCP modification of Modbus protocol", createModbusModule);

            ProtocolManager.Instance.RegisterProtocol("Dummy", "Experimental communication protocol used by simulator program",
                createDummyModule);
        }

        

        /// <summary>
        /// This method is called when application is starting up
        /// </summary>
        /// <param name="e">Application startup command line arguments</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            registerOpenCommand(e.Args);

            registerProtocols();

            base.OnStartup(e);
        }

        #endregion
    }
}
