using System;
using System.IO;
using MTS.Base.Properties;

namespace MTS.Base
{
    public static class Logger
    {
        /// <summary>
        /// Value indicating if logging is enabled. If not Logger method has no effect.
        /// Logging is automatically disabled when some error with logging appears (f.e. 
        /// logging file is missing, ...)
        /// </summary>
        static private bool canLog;
        static private string logFile;

        /// <summary>
        /// Write one line et the end of log file. This method must not throw any exception
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An array of object to write using format</param>
        static public void Log(string format, params object[] args)
        {
            // only save logs if log file exists, this method should not throw exception
            if (canLog)
            {
                // if logging file doest not exists - this try-catch block will disable logging
                try
                {
                    DateTime date = DateTime.Now;
                    // save logs in format: dd.mm.yyyy hh:mm:ss :   message
                    System.IO.File.AppendAllText(logFile,
                        string.Format("{0} :\t{1}\n", 
                            string.Format("{0:dd/MM/yyyy HH:mm:ss}", date),
                            string.Format(format, args)));
                }
                catch
                {   // if anything other happens, switch logging of, so program will run normally
                    // if here an exception if thrown, it must be really something bad
                    canLog = false;
                }
            }
        }
        /// <summary>
        /// Write content of given exception to log file. This method must not throw any exception
        /// </summary>
        /// <param name="ex">Exception that should be wrote to log file</param>
        static public void Log(Exception ex)
        {
            Log("{0}\n{1}", ex.Message, ex.StackTrace);
        }

       
        //static private void settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    // when path to logging file in application settings change
        //    if (e.PropertyName == "LogFile")
        //    {   // if file does not exist logging will be disabled
        //        canLog = File.Exists(Settings.Default.GetLogFilePath());
        //    }
        //}

        #region Constructor

        /// <summary>
        /// Initialize static class for logging. This include checking for existence of logging file
        /// and registering handler that will check for file existence any time it change in application
        /// settings.
        /// </summary>
        static Logger()
        {
            // check if logging file exists
            // if not disable logging
            logFile = Settings.Default.GetLogFilePath();
            canLog = true;

            // register handler that will check if logging file exists any time logging file change
            //Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(settings_PropertyChanged);
        }

        #endregion
    }
}
