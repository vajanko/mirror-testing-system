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

using MTS.Base.Properties;
using MTS.Base;
using MTS.Data.Types;

namespace MTS.Data
{
    /// <summary>
    /// Interaction logic for EditOperatorWindow.xaml
    /// </summary>
    public partial class EditOperatorWindow : Window
    {
        #region Properties

        public string OperatorName { get; set; }
        public string OperatorSurname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public OperatorType Type { get; set; }

        public string ErorrMessage
        {
            get;
            set;
        }

        #endregion

        #region Events

        /// <summary>
        /// This method is called when OK Button is clicked. Result of this dialog will be true.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // check whether password provided by user are equal
            if (passwordBox.SecurePassword.Length > 0 && passwordBox.Password == confirmPasswordBox.Password)
            {
                this.DialogResult = true;
            }
            else
            {
                ExceptionManager.ShowError(Errors.ErrorTitle, Errors.ErrorIcon, "Passwords you have entered are different");
            }
        }

        /// <summary>
        /// This method is called when value of password box change. Update value of <see cref="Password"/>
        /// property.
        /// </summary>
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //Controls.Validators.PasswordEqualityValidator val = new Controls.Validators.PasswordEqualityValidator();
            //val.ConfirmPassword = confirmPasswordBox;
            //val.Password = passwordBox;
            Password = passwordBox.Password;
        }

        #endregion

        private bool finalValidation()
        {
            bool result = passwordBox.SecurePassword.Length > 0 &&
                passwordBox.SecurePassword.ToString() == confirmPasswordBox.SecurePassword.ToString();

            result = result && isValid(loginBox.GetBindingExpression(TextBox.TextProperty), loginBox.Text);
            result = result && isValid(nameBox.GetBindingExpression(TextBox.TextProperty), nameBox.Text);
            result = result && isValid(surnameBox.GetBindingExpression(TextBox.TextProperty), surnameBox.Text);
            
            return result;
        }
        private bool isValid(BindingExpression exp, object value)
        {
            return exp != null &&
                exp.ValidationError.RuleInError.Validate(value, System.Globalization.CultureInfo.CurrentCulture).IsValid;
        }

        #region Constructors

        public EditOperatorWindow()
        {
            InitializeComponent();
            Password = string.Empty;
        }
        public EditOperatorWindow(string name, string surname, string login, OperatorType type)
            : this()
        {
            nameBox.Text = name;
            surnameBox.Text = surname;
            loginBox.Text = login;
            typeBox.SelectedIndex = (int)type;
        }

        #endregion
    }
}
