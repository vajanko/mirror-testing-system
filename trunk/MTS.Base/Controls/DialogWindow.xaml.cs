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
using System.Drawing;
using System.IO;

namespace MTS.Base
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml. Window for displaying dialog controls.
    /// </summary>
    public partial class DialogWindow : Window
    {
        /// <summary>
        /// Settings of current dialog windows. Holds information about dialog buttons and
        /// their actions.
        /// </summary>
        IDialogSettings dialogSettings;

        /// <summary>
        /// This method is called when button1 is clicked
        /// </summary>
        void button1_Click(object sender, RoutedEventArgs e)
        {
            if (dialogSettings.Button1Click == null)
                return;

            if (dialogSettings.Button1Click())
            {
                this.Close();
            }
        }
        /// <summary>
        /// This method is called when button2 is clicked
        /// </summary>
        void button2_Click(object sender, RoutedEventArgs e)
        {
            if (dialogSettings.Button2Click == null)
                return;

            if (dialogSettings.Button2Click())
            {
                this.Close();
            }
        }
        /// <summary>
        /// Initialize dialog window icon - load from bitmap
        /// </summary>
        /// <param name="icon">Bitmap image to be used as the icon</param>
        private void initIcon(Bitmap icon)
        {
            // get image stream
            MemoryStream stream = new MemoryStream();
            icon.Save(stream, icon.RawFormat);
            stream.Seek(0, SeekOrigin.Begin);

            // create bitmap source
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            this.Icon = bitmap;
        }

        #region Constructors

        /// <summary>
        /// Default generated constructor
        /// </summary>
        private DialogWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize a new instance of dialog window with given control
        /// </summary>
        /// <param name="ctrl">Dialog control and its settings for current window</param>
        /// <param name="owner">Window that owns this window. Current window will be displayed relatively
        /// to the owner.</param>
        public DialogWindow(IDialogControl ctrl, Window owner = null, Bitmap icon = null)
            : this()
        {
            this.Owner = owner;

            this.dialogSettings = ctrl.Settings;

            this.Title = dialogSettings.Title;

            button1.Visibility = dialogSettings.Button1Visibility;
            button1.Content = dialogSettings.Button1Content;
            button1.Click += new RoutedEventHandler(button1_Click);

            button2.Visibility = dialogSettings.Button2Visibility;
            button2.Content = dialogSettings.Button2Content;
            button2.Click += new RoutedEventHandler(button2_Click);

            dockPanel.Children.Add(ctrl.Control);
            ctrl.Control.SetValue(DockPanel.DockProperty, Dock.Top);

            if (dialogSettings.DefaultButton == ButtonType.Button1)
                button1.IsDefault = true;
            else if (dialogSettings.DefaultButton == ButtonType.Button2)
                button2.IsDefault = true;

            initIcon(icon);
        }

        #endregion

        /// <summary>
        /// This method is called when a key is released. It is checking for Escape key which will close
        /// the dialog window.
        /// </summary>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && sender is Window)
                (sender as Window).Close();
        }
    }

}
