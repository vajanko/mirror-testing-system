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
using MTS.Base;
using MTS.Base.Properties;

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Display basic information about given operator and allows to change his or her password.
    /// </summary>
    public partial class ProfileWindow : Window
    {

        /// <summary>
        /// This method is called when password of operator failed to change. A message about failure is displayed
        /// to user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePassword_PasswordFailed(object sender, EventArgs e)
        {
            ExceptionManager.ShowError(Errors.ErrorTitle, 
                "Your current password is either incorrect or new and confirmation password do not match");
        }
        /// <summary>
        /// This method is called when password of operator is changed. A message about success is displayed to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePassword_PasswordChanged(object sender, ChangedPasswordEventArgs e)
        {
            Operator.ChangePassword(e.Login, e.OldPassword, e.NewPassword);
            MessageBox.Show("Password changed successfully", "Password", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// This method is called when user try to change this operator password. It is checked if password can be
        /// changed
        /// </summary>
        /// <param name="login">Login of operator who's password you want to change</param>
        /// <param name="password">Password of operator who's password you want to change</param>
        /// <returns></returns>
        private bool validatePassword(string login, string password)
        {   // ask database if this credentials are valid to change this user password
            return Operator.CanLogin(login, password);
        }
        /// <summary>
        /// This method is called when keyboard key is released. If <see cref="Key.Escape"/> was pressed window
        /// is closed.
        /// </summary>
        /// <param name="sender">Instance of window where key was released</param>
        /// <param name="e">Key event argument holding value of released key</param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && sender is Window)
                (sender as Window).Close();
        }


        #region Constructors

        /// <summary>
        /// Create a new instance of operator profile window containing his or her basic information and
        /// possibility to change password
        /// </summary>
        public ProfileWindow()
        {
            InitializeComponent();
            // initialize profile data - operator info
            changePassword.Init(Operator.Instance.Login, validatePassword);
            nameBlock.Text = Operator.Instance.FullName;
            loginBlock.Text = Operator.Instance.Login;
            groupBlock.Text = Operator.Instance.Type.ToString();
        }

        #endregion
    }
}
