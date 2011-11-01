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

namespace MTS.Admin
{
    /// <summary>
    /// Interaction logic for ModbusSettings.xaml
    /// </summary>
    public partial class ModbusSettings : UserControl
    {
        /// <summary>
        /// (Get/Set) IP address of remote modbus component
        /// </summary>
        public string IPAddress
        {
            get { return ipAddress.Text; }
            set { ipAddress.Text = value; }
        }
        /// <summary>
        /// (Get/Set) Port on which is listenting remote modbus component
        /// </summary>
        public ushort Port
        {
            get
            {
                ushort p;
                return ushort.TryParse(port.Text, out p) ? p : (ushort)0;
            }
            set { port.Text = value.ToString(); }
        }
        /// <summary>
        /// (Get/Set) Absolute path to ethercat module configuration file
        /// </summary>
        public string ConfigFilePath
        {
            get { return configFile.Text; }
            set { configFile.Text = value; }
        }

        /// <summary>
        /// Occures when browse button is clicked
        /// </summary>
        public event RoutedEventHandler BrowseClick
        {
            add { browseButton.Click += value; }
            remove { browseButton.Click -= value; }
        }

        #region Constructors

        public ModbusSettings()
        {
            InitializeComponent();
        }

        #endregion
    }
}
