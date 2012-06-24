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
        private OperatorViewModel viewModel;
        MTSContext dbContext;

        #region Events

        /// <summary>
        /// This method is called when OK Button is clicked. Result of this dialog will be true.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            bool valid = isValid(this);

            if (!valid)
            {
                string error = getError(this);
                ExceptionManager.ShowError(Errors.ErrorTitle, Errors.ErrorIcon, error);
            }
            else
            {
                this.DialogResult = valid;
            }
        }

        private bool isValid(DependencyObject obj)
        {
            bool hasErrors = Validation.GetHasError(obj);
            hasErrors = hasErrors || LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().All(ch => isValid(ch));

            return !hasErrors;
        }

        private string getError(DependencyObject obj)
        {
            var errors = Validation.GetErrors(obj);
            if (errors.Count > 0)
                return errors[0].ErrorContent.ToString();

            string error = LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>()
                .Select(ch => getError(ch)).FirstOrDefault(e => e != null);

            return error;
        }

        #endregion

        #region Constructors

        public EditOperatorWindow()
        {
            InitializeComponent();
            typeBox.ItemsSource = OperatorTypes.Instance.DataTypes;
            dbContext = new MTSContext();
        }
        public EditOperatorWindow(OperatorViewModel viewModel)
            : this()
        {
            this.viewModel = viewModel;
            this.DataContext = viewModel;
            loginValidator.MyLogin = viewModel.Login;
        }

        #endregion
    }
}
