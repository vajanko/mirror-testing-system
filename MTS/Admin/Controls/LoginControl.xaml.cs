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

using MTS.Base;
using System.Drawing;

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl, IDialogControl
    {
        private bool login()
        {
            bool success = Operator.TryLogin(loginBox.Text, passwordBox.Password);

            if (!success)
            {
                Bitmap image = Resources.FindName("loginFailed") as Bitmap;
                MessageControl ctrl = new MessageControl(string.Format("Login failed for user \"{0}\"", loginBox.Text), "Error", image);
                DialogWindow dialog = new DialogWindow(ctrl, this.Parent as Window);

                dialog.ShowDialog();
            }

            return success;
        }

        public LoginControl()
        {
            InitializeComponent();
            loginBox.Focus();
        }

        #region IDialogControl Members

        private DialogSettings settings;
        public IDialogSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new DialogSettings();
                    settings.Title = "Login";
                    settings.Button2Content = "Login";
                    settings.Button2Click = login;
                    settings.Button1Visibility = Visibility.Collapsed;

                    settings.DefaultButton = ButtonType.Button2;
                }

                return settings;
            }
        }

        public Control Control
        {
            get { return this; }
        }

        #endregion
    }
}
