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

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Interaction logic for EthercatSettings.xaml
    /// </summary>
    public partial class EthercatSettings : UserControl
    {
        /// <summary>
        /// (Get/Set) Name of ethercat task
        /// </summary>
        public string TaskName
        { 
            get { return taskName.Text; }
            set { taskName.Text = value; }
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

        public EthercatSettings()
        {
            InitializeComponent();
        }

        #endregion
    }
}
