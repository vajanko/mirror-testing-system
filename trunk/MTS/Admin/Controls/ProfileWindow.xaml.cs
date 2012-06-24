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
        public ProfileWindow(ProfileWindowViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }

        #endregion
    }
}
