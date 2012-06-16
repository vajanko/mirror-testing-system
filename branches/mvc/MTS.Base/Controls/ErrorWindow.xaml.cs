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
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net.Mail;

using MTS.Base.Properties;
using System.Net;
using System.Drawing;

namespace MTS.Base
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #region Constructors

        public ErrorWindow()
        {
            DataContext = new ErrorWindowViewModel();
            InitializeComponent();
        }
        public ErrorWindow(ErrorWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        #endregion
    }
}
