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
using System.Diagnostics;
using System.Net.Mail;

using MTS.Base.Properties;
using System.Net;

namespace MTS.Base
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private Exception exception;

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

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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
            sendErrorButton.IsEnabled = false;
        }
        public ErrorWindow(string title, string message, Exception ex)
            : this(title, message)
        {
            exception = ex;
            sendErrorButton.IsEnabled = true;   // only exception could be send
        }
        

        #endregion

        private void viewLog_Click(object sender, RoutedEventArgs e)
        {   // get log file name from application settings
            try
            {
                string logFile = Settings.Default.GetLogFilePath();
                if (System.IO.File.Exists(logFile))
                {   // if this file exists open it in current user text editor
                    Process.Start(logFile);
                }
            }
            catch
            {   // disable view log button if any error occurs
                viewLogButton.IsEnabled = false;
            }
        }

        private void sendError_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (exception == null)
                    sendErrorButton.IsEnabled = false;
                MailAddress to = new MailAddress(Settings.Default.AdminAddress, "MTS Admin");
                MailAddress from = new MailAddress(Settings.Default.AppSenderAddress, "MTS User");
                MailMessage message = new MailMessage(from, to);

                message.Body = generateExceptionMsg(exception);
                message.Subject = "MTS Application exception";

                SmtpClient client = new SmtpClient
                {
                    Host = Settings.Default.SmtpHost,
                    Port = Settings.Default.SmtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Settings.Default.AppSenderAddress, Settings.Default.AppSenderPassword)
                };
                client.Send(message);
                Message = "Error message has been successfully sent to application support.";
            }
            catch
            {   // disable send button when any error occurs
                sendErrorButton.IsEnabled = false;
                Message = "Message could not be sent. Check your internet connection.";
            }
        }

        private static string generateExceptionMsg(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            
            msg.AppendLine("Dear support,\n\n an exception has been thrown in MTS application!\n");
            msg.AppendLine("Local computer info:");
            msg.AppendFormat("Computer name: {0}\n", Environment.MachineName);
            msg.AppendFormat("Operating system: {0}\n", Environment.OSVersion);
            msg.AppendFormat("User name: {0}\n", Environment.UserName);
            msg.AppendFormat("Current time: {0}\n\n", DateTime.Now);

            msg.AppendFormat("--------------- Begin Exception ---------------\n");
            writeException(msg, ex);
            msg.AppendFormat("Stack trace:\n{0}\n", ex.StackTrace);
            msg.AppendFormat("---------------- End Exception -----------------\n");

            return msg.ToString();
        }
        private static void writeException(StringBuilder str, Exception ex)
        {
            str.AppendFormat("--- Begin: {0} ---\n", ex.GetType());
            str.AppendFormat("Message: {0}\n", ex.Message);
            str.AppendFormat("Source: {0}\n", ex.Source);
            str.AppendFormat("TargetSite: {0}\n", ex.TargetSite);
            if (ex.InnerException != null)
                writeException(str, ex.InnerException);
            str.AppendFormat("--- End: {0} ---\n", ex.GetType());
        }
    }
}
