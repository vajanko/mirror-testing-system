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

namespace MTS.Utils
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        #region Dependency Properties

        #region ErrorTitle Property

        public static readonly DependencyProperty ErrorTitleProperty =
            DependencyProperty.Register("ErrorTitle", typeof(string), typeof(ErrorWindow),
            new PropertyMetadata(string.Empty));

        public string ErrorTitle
        {
            get { return (string)GetValue(ErrorTitleProperty); }
            set { SetValue(ErrorTitleProperty, value); }
        }

        #endregion

        #region Message Property

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ErrorWindow),
            new PropertyMetadata(string.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        #endregion

        public System.Drawing.Image ErrorIcon
        {
            set
            {
                BitmapImage img = new BitmapImage();
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                value.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
                errorIcon.Source = img;
            }
        }

        #region Constructors

        public ErrorWindow()
        {
            InitializeComponent();            
        }
        public ErrorWindow(string title, string message)
            : this()
        {
            ErrorTitle = title;
            Message = message;
        }

        #endregion

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
