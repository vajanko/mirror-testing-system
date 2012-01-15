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

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginResult Result { get; private set; }

        /// <summary>
        /// This method is called when login button is clicked
        /// </summary>
        private void login_Click(object sender, RoutedEventArgs e)
        {   // try to log operator in
            this.DialogResult = Operator.TryLogin(loginBox.Text, passwordBox.Password);
            this.Close();   // close window - after that setup result of login operation (overwriter close result)
            if (DialogResult == true)
                Result = LoginResult.Success;
            else
                Result = LoginResult.Fail;
        }
        /// <summary>
        /// This method is called when window is closed manualy by used by clicking close button
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {   // login operation will be aborted
            Result = LoginResult.Abort;
        }

        #region Constructors

        private LoginWindow(Window owner)
            : base()
        {
            Owner = owner;
            InitializeComponent();
            loginBox.Focus();
        }
        /// <summary>
        /// Create a new instance of login window
        /// </summary>
        /// <param name="owner">Parent of this window</param>
        /// <param name="previousFailed">Value indicating if previous login was unsuccessfull. If so a message
        /// will be displayed to user</param>
        public LoginWindow(Window owner, bool previousFailed = false)
            : this(owner)
        {
            if (previousFailed)
                messageLabel.Visibility = System.Windows.Visibility.Visible;
            else
                messageLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion
    }
    /// <summary>
    /// Describes result of login operation
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// User was logged in successfully
        /// </summary>
        Success,
        /// <summary>
        /// User login failed
        /// </summary>
        Fail,
        /// <summary>
        /// User aborted login operation
        /// </summary>
        Abort
    }
}
