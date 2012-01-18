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
            sendErrorButton.IsEnabled = false;      // if there is no exception these buttons can not be used
            viewDetailsButton.IsEnabled = false;
        }
        public ErrorWindow(string title, string message, Exception ex)
            : this(title, message)
        {
            exception = ex;
            sendErrorButton.IsEnabled = true;   // only exception could be send
            viewDetailsButton.IsEnabled = true;
        }
        

        #endregion

        /// <summary>
        /// This method is called when ViewLog button is clicked. Log text file is opened in current user text editor
        /// (for example notepad)
        /// </summary>
        /// <param name="sender">Instance of button that has been clicked</param>
        /// <param name="e">Click event arguments</param>
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
                if (sender is Button)
                    (sender as Button).IsEnabled = false;
            }
        }
        /// <summary>
        /// This method is called when SendError button is clicked. A report from given exception will be generated and
        /// sent to application administrator. If it is not possible to send this message, this functionality will be
        /// disabled.
        /// </summary>
        /// <param name="sender">Instance of button that has been clicked</param>
        /// <param name="e">Click event arguments</param>
        private void sendError_Click(object sender, RoutedEventArgs e)
        {
            // if exception is null following code will throw an exception and button will be disabled
            try
            {   // use application setting to define sender and receiver
                MailAddress to = new MailAddress(Settings.Default.AdminAddress, "MTS Admin");
                MailAddress from = new MailAddress(Settings.Default.AppSenderAddress, "MTS User");
                MailMessage message = new MailMessage(from, to);

                // generate report from given exception
                message.Body = generateMessage(exception);
                message.Subject = "MTS Application exception";

                // create client for sending emails and send messgage
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
                // show short message to user that mail has been sent
                shortMsgBlock.Foreground = Brushes.Green;
                shortMsgBlock.Text = "Error message sent successfully.";
            }
            catch
            {   // disable send button when any error occurs - user can not repeat this action any more (on this window)
                if (sender is Button)
                    (sender as Button).IsEnabled = false;
                // show short message to user that mail could not be sent
                shortMsgBlock.Foreground = Brushes.Red;
                shortMsgBlock.Text = "Error message could not be sent.";
            }
        }
        /// <summary>
        /// This method is called when ViewDetails button is clicked. 
        /// </summary>
        /// <param name="sender">Instance of button that has been clicked</param>
        /// <param name="e">Click event arguments</param>
        private void viewDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Generate a message from given exception that has been thrown in our application. Includes also some basic
        /// information about local computer. In this report will be included all inner exceptions and stack trace.
        /// </summary>
        /// <param name="ex">Exception that has been thrown in our application and that should be included to generated
        /// report</param>
        /// <returns>Report containing basic information about local computer and thrown exception</returns>
        private static string generateMessage(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            
            // include basic info about this computer
            msg.AppendLine("Dear support,\n\n an exception has been thrown in MTS application!\n");
            msg.AppendLine("Local computer info:");
            msg.AppendFormat("Computer name: {0}\n", Environment.MachineName);
            msg.AppendFormat("Operating system: {0}\n", Environment.OSVersion);
            msg.AppendFormat("User name: {0}\n", Environment.UserName);
            msg.AppendFormat("Current time: {0}\n\n", DateTime.Now);

            // append information from exception
            msg.AppendFormat("--------------- Begin Exception ---------------\n");
            writeException(msg, ex);    // append information about exception and all its inner exceptions
            // at the end add stack trace
            msg.AppendFormat("Stack trace:\n{0}\n", ex.StackTrace);
            msg.AppendFormat("---------------- End Exception -----------------\n");

            return msg.ToString();
        }
        /// <summary>
        /// Append to given StringBuilder information from given exception and all its inner exceptions
        /// </summary>
        /// <param name="str">Instance of <see cref="StringBuilder"/> to append string information to</param>
        /// <param name="ex">Instance of <see cref="Exception"/> to get information from</param>
        private static void writeException(StringBuilder str, Exception ex)
        {
            str.AppendFormat("--- Begin: {0} ---\n", ex.GetType());
            str.AppendFormat("Message: {0}\n", ex.Message);
            str.AppendFormat("Source: {0}\n", ex.Source);
            str.AppendFormat("TargetSite: {0}\n", ex.TargetSite);
            if (ex.InnerException != null)
                writeException(str, ex.InnerException);     // generate same report for inner exception
            str.AppendFormat("--- End: {0} ---\n", ex.GetType());
        }
    }
}
