using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;

namespace MTS
{
    public static class ExceptionManager
    {
        public static void ShowError(string caption, string format, params object[] args)
        {
            MessageBox.Show(string.Format(format, args), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowError(Exception ex)
        {
            if (ex is System.IO.IOException)
            {
                if (ex is System.IO.FileNotFoundException)
                {
                    var fe = (ex as System.IO.FileNotFoundException);
                    ShowError("File Error", "File {0} not found", Path.GetFileName(fe.FileName));
                }
                else if (ex is System.IO.FileFormatException)
                {
                    var fe = (ex as System.IO.FileFormatException);
                    ShowError("File Error", "File {0} may be corrupted", Path.GetFileName(fe.SourceUri.AbsolutePath));
                }
            }
            else if (ex is MTS.IO.IOException)  // hardware communication exception
            {
                if (ex is MTS.IO.Module.ModuleException)
                {
                    if (ex is MTS.IO.Module.ConnectionException)
                    {
                        var ce = ex as MTS.IO.Module.ConnectionException;
                        ShowError("Connection Error", "Connection to {0} module could not be established", ce.ProtocolName);
                    }
                    else
                    {
                        var ce = ex as MTS.IO.Module.ConnectionException;
                        ShowError("Module Error", "An error occured while establishing connection to {0} module", ce.ProtocolName);
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
                    ShowError("Unkown Error", "An unknown error occured on network layer. Check log file for more inforamtion");
                }
            }
            else
            {   // when somethig very bad has happened and we do not know what
                ShowError("Unkown Error", "An unknown error occured. Check log file for more inforamtion");
            }

        }
        /// <summary>
        /// Log thrown exception to logger file
        /// </summary>
        /// <param name="ex">Exception that has been thrown</param>
        public static void LogException(Exception ex)
        {
            if (ex is System.IO.FileFormatException)
            {
                var fe = (ex as System.IO.FileFormatException);
                Output.Log("{0}: {1}, File: {2}\n\t{3}", ex.Source, ex.Message, fe.SourceUri, ex.StackTrace);
            }
            else if (ex is System.IO.FileNotFoundException)
            {
                var fe = (ex as System.IO.FileNotFoundException);
                Output.Log("{0}: {1}, File name: {2}\n\t{3}", ex.Source, ex.Message, fe.FileName, ex.StackTrace);
            }
            else
                Output.Log("{0}: {1}\n\t{2}", ex.Source, ex.Message, ex.StackTrace);
        }
    }
}
