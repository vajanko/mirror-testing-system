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
    /// Interaction logic for ChangePasswordControl.xaml
    /// </summary>
    public partial class ChangePasswordControl : UserControl
    {
        #region Fields

        private string login;
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

        public void Init(string login, Func<string, string, bool> passValidator)
        {
            this.login = login;
            this.validateOldPassword = passValidator;
        }

        #region Constructors

        public ChangePasswordControl()
        {
            InitializeComponent();
        }

        #endregion
    }

    public class ChangedPasswordEventArgs : EventArgs
    {

        public string OldPassword { get; private set; }
        /// <summary>
        /// (Get) New password successfully changed
        /// </summary>
        public string NewPassword { get; private set; }
        /// <summary>
        /// (Get) Login of user for which password was changed
        /// </summary>
        public string Login { get; private set; }


        public ChangedPasswordEventArgs(string login, string oldPassword, string newPassword)
        {
            Login = login;
            NewPassword = newPassword;
            OldPassword = oldPassword;
        }
    }
}
