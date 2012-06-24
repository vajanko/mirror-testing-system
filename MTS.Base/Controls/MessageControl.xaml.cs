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
using System.Drawing;

namespace MTS.Base
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl, IDialogControl
    {
        public string Message { get; private set; }

        public string Title { get; private set; }

        public MessageControl()
        {
            InitializeComponent();
        }
        public MessageControl(string message, string title = "Dialog", Bitmap image = null)
            : this()
        {
            this.Message = message;
            this.Title = title;

            textCtrl.Text = Message;
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
                    settings.Title = Title;
                    settings.Button2Content = "OK";
                    settings.Button2Click = new Func<bool>(() => true);
                    settings.DefaultButton = ButtonType.Button2;
                    settings.Button1Visibility = Visibility.Collapsed;
                }

                return settings;
            }
        }

        public Control Control
        {
            get { return this; }
        }

        #endregion

        #region Static Members

        public static void Show(string message, string title = "Dialog", Bitmap icon = null, Window owner = null)
        {
            MessageControl ctrl = new MessageControl(message, title);
            DialogWindow wnd = new DialogWindow(ctrl, owner, icon);

            wnd.ShowDialog();
        }

        #endregion
    }
}
