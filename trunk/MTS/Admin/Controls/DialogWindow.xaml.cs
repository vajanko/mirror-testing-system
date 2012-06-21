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

namespace MTS.Admin.Controls
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        IDialogSettings dialogSettings;

        void button1_Click(object sender, RoutedEventArgs e)
        {
            if (dialogSettings.Button1Click == null)
                return;

            if (dialogSettings.Button1Click())
            {
                this.Close();
            }
        }

        void button2_Click(object sender, RoutedEventArgs e)
        {
            if (dialogSettings.Button2Click == null)
                return;

            if (dialogSettings.Button2Click())
            {
                this.Close();
            }
        }

        #region Constructors

        public DialogWindow()
        {
            InitializeComponent();
        }

        public DialogWindow(IDialogControl ctrl)
            : this()
        {
            this.dialogSettings = ctrl.Settings;

            button1.Visibility = dialogSettings.Buttton1Visibility;
            button1.Content = dialogSettings.Button1Content;
            button1.Click += new RoutedEventHandler(button1_Click);

            button2.Visibility = dialogSettings.Button2Visibility;
            button2.Content = dialogSettings.Button2Content;
            button2.Click += new RoutedEventHandler(button2_Click);

            contentCtrl.Content = ctrl.Control;

            if (dialogSettings.DefaultButton == ButtonType.Button1)
                button1.IsDefault = true;
            else if (dialogSettings.DefaultButton == ButtonType.Button2)
                button1.IsDefault = true;
                
        }

        #endregion
    }
}
