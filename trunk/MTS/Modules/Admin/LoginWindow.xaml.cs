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
using MTS.Data;

namespace MTS.Admin
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private void login_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = Operator.TryLogin(loginBox.Text, passwordBox.Password);
            this.Close();
        }

        #region Constructors

        public LoginWindow(Window owner)
            : base()
        {
            Owner = owner;
        }
        public LoginWindow(Window owner, bool previousFailed = false)
            : this(owner)
        {
            InitializeComponent();
            if (previousFailed)
                messageLabel.Visibility = System.Windows.Visibility.Visible;
            else
                messageLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion
    }
}
