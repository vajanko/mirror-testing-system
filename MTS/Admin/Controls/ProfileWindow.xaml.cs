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

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {


        private void changePassword_PasswordFailed(object sender, EventArgs e)
        {
            errorLabel.Content = "Your current password is either incorrect or new and confirmation password do not match";
        }

        private void changePassword_PasswordChanged(object sender, ChangedPasswordEventArgs e)
        {
            Operator.ChangePassword(e.Login, e.OldPassword, e.NewPassword);
        }

        private bool validatePassword(string login, string password)
        {   // ask database if this credentials are valid to change this user password
            return Operator.CanLogin(login, password);
        }


        #region Constructors

        public ProfileWindow()
        {
            InitializeComponent();
            changePassword.Init(Operator.Instance.Login, validatePassword);
            nameLabel.Content = Operator.Instance.Name;
            surnameLabel.Content = Operator.Instance.Surname;
            groupLabel.Content = Operator.Instance.Type;
        }

        #endregion
    }
}
