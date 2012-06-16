using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Base.Properties;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace MTS.Base
{
    public class ErrorWindowViewModel : ViewModelBase
    {
        private Exception exception;

        #region Properties

        private string _shortMessage;
        /// <summary>
        /// (Get/Set) Localized string of short error message. <see cref="PropertyChanged"/> event is raised
        /// when this property change.
        /// </summary>
        public string ShortMessage
        {
            get { return _shortMessage; }
            set
            {
                _shortMessage = value;
                OnPropertyChanged("ShortMessage");
            }
        }

        private Brush _shortMessageBrush;
        /// <summary>
        /// (Get/Set) Brush of short message text brush. <see cref="PropertyChanged"/> event is raised
        /// when this property change.
        /// </summary>
        public Brush ShortMessageBrush
        {
            get { return _shortMessageBrush; }
            set
            {
                _shortMessageBrush = value;
                OnPropertyChanged("ShortMessageBrush");
            }
        }

        private string _message;
        /// <summary>
        /// (Get/Set) Localize string containing error message. <see cref="PropertyChanged"/> event is raised
        /// when this property change.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        //private Image _errorIcon;
        /// <summary>
        /// (Get/Set) Error icon displayed in the error window. <see cref="PropertyChanged"/> event is raised
        /// when this property change.
        /// </summary>
        //public Image ErrorIcon
        //{
        //    get { return _errorIcon; }
        //    set
        //    {
        //        _errorIcon = value;
        //        OnPropertyChanged("ErrorIcon");
        //    }
        //}

        public ImageSource ErrorIcon { get; private set; }

        private bool _canSendError;
        /// <summary>
        /// (Get/Set) Value indicating whether error can be sent through email. <see cref="PropertyChanged"/> event 
        /// is raised when this property change.
        /// </summary>
        public bool CanSendError
        {
            get { return _canSendError; }
            set
            {
                _canSendError = value;
                OnPropertyChanged("CanSendError");
            }
        }

        private bool _canViewDetails;
        /// <summary>
        /// (Get/Set) Value indicating whether details of error message can be seen. <see cref="PropertyChanged"/> 
        /// event is raised when this property change.
        /// </summary>
        public bool CanViewDetails
        {
            get { return _canViewDetails; }
            set
            {
                _canViewDetails = value;
                OnPropertyChanged("CanViewDetails");
            }
        }

        private bool _canViewLog;
        /// <summary>
        /// (Get/Set) Value indicating whether log file can be opened for viewing. <see cref="PropertyChanged"/> 
        /// event is raised when this property change.
        /// </summary>
        public bool CanViewLog
        {
            get { return _canViewLog; }
            set
            {
                _canViewLog = value;
                OnPropertyChanged("CanViewLog");
            }
        }

        public ICommand SendErrorCommand { get; private set; }
        public ICommand ViewDetailsCommand { get; private set; }
        public ICommand ViewLogCommand { get; private set; }

        /// <summary>
        /// (Get) ViewModel of inner exception if it exists
        /// </summary>
        public ErrorWindowViewModel InnerViewModel
        {
            get
            {
                if (!CanViewDetails)
                    return null;
                return new ErrorWindowViewModel(DisplayName, exception.InnerException);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Display log file to user
        /// </summary>
        private void viewLog(object parameter)
        {
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
                CanViewLog = false;
            }
        }
        /// <summary>
        /// Send displayed error to application administrator
        /// </summary>
        private void sendError(object parameter)
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

                // create client for sending emails and send message
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
                ShortMessage = "Error message sent successfully.";
                ShortMessageBrush = Brushes.Green;
            }
            catch
            {   // disable send button when any error occurs - user can not repeat this action any more (on this window)
                CanSendError = false;
                // show short message to user that mail could not be sent
                ShortMessage = "Error message could not be sent.";
                ShortMessageBrush = Brushes.Red;
            }
        }
        /// <summary>
        /// View error details if possible
        /// </summary>
        private void viewDetails(object parameter)
        {
            ErrorWindow wnd = new ErrorWindow(InnerViewModel);
            wnd.ShowDialog();
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

        #endregion

        #region Constructors

        public ErrorWindowViewModel(string displayName = "Error", Exception exception = null, System.Drawing.Bitmap errorIcon = null)
            : base(displayName)
        {
            this.exception = exception;

            if (errorIcon != null)
            {
                MemoryStream stream = new MemoryStream();
                errorIcon.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
                ErrorIcon = bi;
            }

            if (exception != null)
            {
                Message = exception.Message;
                CanSendError = true;
                SendErrorCommand = new DelegateCommand(sendError);

                CanViewDetails = exception.InnerException != null;
                ViewDetailsCommand = new DelegateCommand(viewDetails);
                
                CanViewLog = true;
                ViewLogCommand = new DelegateCommand(viewLog);
            }
            else
            {
                CanSendError = false;
                CanViewDetails = false;
                CanViewLog = true;
            }

        }

        #endregion
    }
}
