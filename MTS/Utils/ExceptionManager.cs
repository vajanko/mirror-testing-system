using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows;

namespace MTS.Utils
{
    public static class ExceptionManager
    {
        public static void ShowError(string caption, string format, params object[] args)
        {
            ErrorWindow wnd = new ErrorWindow(caption, string.Format(format, args));
            wnd.ShowDialog();
            //MessageBox.Show(string.Format(format, args), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowError(string caption, Image icon, string format, params object[] args)
        {
            ErrorWindow wnd = new ErrorWindow(caption, string.Format(format, args));
            wnd.ErrorIcon = icon;
            wnd.ShowDialog();
            //MessageBox.Show(string.Format(format, args), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void showError(System.SystemException ex)
        {
            if (ex is System.FormatException)
            {
                showError(ex as System.FormatException);
            }
            else if (ex is System.IO.IOException)
            {
                showError(ex as System.IO.IOException);
            }
        }
        private static void showError(System.IO.IOException ex)
        {
            if (ex is System.IO.FileNotFoundException)
            {
                var fe = (ex as System.IO.FileNotFoundException);                    
                ShowError(Errors.FileErrorTitle, Errors.FileErrorIcon, "File {0} not found", Path.GetFileName(fe.FileName));
            }
        }
        private static void showError(System.FormatException ex)
        {
            if (ex is System.IO.FileFormatException)
            {
                var fe = (ex as System.IO.FileFormatException);
                ShowError(Errors.FileErrorTitle, "File {0} may be corrupted", Path.GetFileName(fe.SourceUri.AbsolutePath));
            }
        }

        private static void showError(MTS.IO.IOException ex)
        {
            if (ex is MTS.IO.Module.ModuleException)
            {
                if (ex is MTS.IO.Module.ConnectionException)
                {
                    var ce = ex as MTS.IO.Module.ConnectionException;
                    ShowError(Errors.IOErrorTitle, "Connection to {0} module could not be established", ce.ProtocolName);
                }
                else
                {
                    var ce = ex as MTS.IO.Module.ConnectionException;
                    ShowError(Errors.IOErrorTitle, "An error occured while establishing connection to {0} module", ce.ProtocolName);
                }
            }
            else if (ex is MTS.IO.Channel.ChannelException)
            {
                var ce = ex as MTS.IO.Channel.ChannelException;
            }
            else if (ex is MTS.IO.Address.AddressException)
            {
                var ce = ex as MTS.IO.Address.AddressException;
            }
            else
            {   // when somethig very bad has happened and we do not know what
                ShowError(Errors.UnknownErrorTitle, "An unknown error occured on network layer. Check log file for more inforamtion");
            }
        }

        public static void ShowError(Exception ex)
        {
            if (ex is System.SystemException)
            {
                showError(ex as System.SystemException);
            }
            else if (ex is MTS.IO.IOException)  // hardware communication exception
            {
                showError(ex as MTS.IO.IOException);
            }
            else
            {   // when somethig very bad has happened and we do not know what
                ShowError(Errors.UnknownErrorTitle, "An unknown error occured. Check log file for more inforamtion");
            }

        }
        /// <summary>
        /// Logger thrown exception to logger file
        /// </summary>
        /// <param name="ex">Exception that has been thrown</param>
        public static void LogException(Exception ex)
        {
            if (ex is System.IO.FileFormatException)
            {
                var fe = (ex as System.IO.FileFormatException);
                Logger.Log("{0}: {1}, File: {2}\n\t{3}", ex.Source, ex.Message, fe.SourceUri, ex.StackTrace);
            }
            else if (ex is System.IO.FileNotFoundException)
            {
                var fe = (ex as System.IO.FileNotFoundException);
                Logger.Log("{0}: {1}, File name: {2}\n\t{3}", ex.Source, ex.Message, fe.FileName, ex.StackTrace);
            }
            else
                Logger.Log("{0}: {1}\n\t{2}", ex.Source, ex.Message, ex.StackTrace);
        }
    }
}
