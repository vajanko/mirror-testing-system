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

namespace MTS.Controls
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        #region UserControl Property

        public static readonly DependencyProperty UserControlProperty =
            DependencyProperty.Register("UserControl", typeof(UserControl), typeof(PopupWindow));

        public UserControl UserControl
        {
            get { return (UserControl)GetValue(UserControlProperty); }
            set { SetValue(UserControlProperty, value); }
        }

        #endregion

        public PopupWindow()
        {
            InitializeComponent();
        }
        public PopupWindow(UserControl content)
            : this()
        {
            UserControl = content;
        }
    }
}
