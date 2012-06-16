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
    /// Allows user to change password
    /// </summary>
    public partial class ChangePasswordControl : UserControl
    {
        #region Fields

        /// <summary>
        /// Login of operator we want to change
        /// </summary>
        private string login;
        /// <summary>
        /// Delegate gets login and password and return value indicating whether these credentials are valid
        /// </summary>
        Func<string, string, bool> validateOldPassword;

        #endregion

        public delegate void PasswordChangedHandler(object sender, ChangedPasswordEventArgs e);
        public delegate void PasswordFailedHandler(object sender, EventArgs e);

        private event PasswordChangedHandler passwordChanged;
        /// <summary>
        /// Occurs when password is changed successfully
        /// </summary>
        public event PasswordChangedHandler PasswordChanged
        {
            add { passwordChanged += value; }
            remove { passwordChanged -= value; }
        }

        private event PasswordFailedHandler passwordFailed;
        /// <summary>
        /// Occurs when user tries to change password, but the change fails
        /// </summary>
        public event PasswordFailedHandler PasswordFailed
        {
            add { passwordFailed += value; }
            remove { passwordFailed -= value; }
        }

        /// <summary>
        /// Raise <see cref="PasswordChanged"/> event.
        /// </summary>
        /// <param name="oldPassword">Old password that is going to be changed</param>
        /// <param name="newPassword">New password that has been currently changed</param>
        protected void OnPasswordChanged(string oldPassword, string newPassword)
        {
            if (passwordChanged != null)
                passwordChanged(this, new ChangedPasswordEventArgs(login, oldPassword, newPassword));
        }
        /// <summary>
        /// Raise <see cref="PasswordFailed"/> event.
        /// </summary>
        protected void OnPasswordFailed()
        {
            if (passwordFailed != null)
                passwordFailed(this, EventArgs.Empty);
        }

        /// <summary>
        /// This method is called when change button is clicked. This event will try to changed user password
        /// and display appropriate message to user.
        /// </summary>
        /// <param name="sender">Instance of change button which has been clicked</param>
        /// <param name="e">Event argument of button clicked</param>
        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            // check if given password is correct - exists in database
            // and if the change two password are the same - if so fire changed event
            if (validateOldPassword(login, passwordBox.Password) &&
                newPasswordBox.Password == confirmPasswordBox.Password)
                OnPasswordChanged(passwordBox.Password, newPasswordBox.Password);
            else
                OnPasswordFailed();
        }

        /// <summary>
        /// Initialize <see cref="ChangePasswordControl"/> by passing it user login which password could be changed
        /// with this control and function which will validate old used password. This method should be only used
        /// when instance of <see cref="ChangePasswordControl"/> is created in XAML. Otherwise use constructor.
        /// </summary>
        /// <param name="login">Login of user which password could be changed with this control</param>
        /// <param name="passValidator">Function that will validate old used password</param>
        public void Init(string login, Func<string, string, bool> passValidator)
        {
            this.login = login;
            this.validateOldPassword = passValidator;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ChangePasswordControl"/> which allows user to change his or her password
        /// by validating current password and confirming twice entered new password.
        /// </summary>
        public ChangePasswordControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Create a new instance of <see cref="ChangePasswordControl"/> which allows user to change his or her password
        /// by validating current password and confirming twice entered new password.
        /// </summary>
        /// <param name="login">Login of user which password could be changed with this control</param>
        /// <param name="passValidator">Function that will validate old used password</param>
        public ChangePasswordControl(string login, Func<string, string, bool> passValidator)
            : this()
        {
            Init(login, passValidator);
        }

        #endregion
    }

    public class ChangedPasswordEventArgs : EventArgs
    {
        /// <summary>
        /// (Get) Previous user password
        /// </summary>
        public string OldPassword { get; private set; }
        /// <summary>
        /// (Get) New password successfully changed
        /// </summary>
        public string NewPassword { get; private set; }
        /// <summary>
        /// (Get) Login of user for which password was changed
        /// </summary>
        public string Login { get; private set; }

        /// <summary>
        /// Create a new instance of <see cref="ChangedPasswordEventArgs"/> containing event data for password
        /// changed event
        /// </summary>
        /// <param name="login">Login of used who's password has been changed</param>
        /// <param name="oldPassword">Previous user password</param>
        /// <param name="newPassword">New password successfully changed</param>
        public ChangedPasswordEventArgs(string login, string oldPassword, string newPassword)
        {
            Login = login;
            NewPassword = newPassword;
            OldPassword = oldPassword;
        }
    }
}
