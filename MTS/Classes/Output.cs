using System;
using System.Windows;
using System.Windows.Controls;

using System.IO;
using MTS.Properties;

namespace MTS
{
    public static class Output
    {
        static private TextBox textBox;
        static public TextBox TextBox
        {
            get { return textBox; }
            set
            {
                textBox = value;
            }
        }

        /// <summary>
        /// Write one line at the end of output console.
        /// Thread safe.
        /// </summary>
        /// <param name="text">Text to write</param>
        static public void WriteLine(string text)
        {   // non blocking call
            textBox.Dispatcher.BeginInvoke(new Action<string>(writeLine), text);
        }
        /// <summary>
        /// Write one line at the end of output console
        /// </summary>
        /// <param name="format">A composit format string</param>
        /// <param name="args">An array of object to write using format</param>
        static public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
        /// <summary>
        /// Cleat output console
        /// </summary>
        static public void Clear()
        {
            textBox.Clear();
        }

        /// <summary>
        /// Write one line et the end of log file
        /// </summary>
        /// <param name="format">A composit format string</param>
        /// <param name="args">An array of object to write using format</param>
        static public void Log(string format, params object[] args)
        {
            // only save logs if log file exists, this mehtod should not throw exception
            if (canLog)
            {
                try
                {
                    DateTime date = DateTime.Now;
                    // save logs in format: Y.M.D H:M:S
                    System.IO.File.AppendAllText(Settings.Default.LogFile,
                        string.Format("{0}.{1}.{2} {3}:{4}:{5}\t{6}\n", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                        string.Format(format, args)));
                }
                catch
                {   // if anythig other happens, switch logging of, so program will run normally
                    // if here an exception if thrown, it must be really somethig bad
                    canLog = false;
                }
            }
        }

        /// <summary>
        /// This method could be called only from the thread where textBox has been created.
        /// Writes some text to ouptput console and breaks the line.
        /// </summary>
        /// <param name="text">Text to write</param>
        static private void writeLine(string text)
        {
            textBox.AppendText(text + "\n");
            textBox.ScrollToEnd();              // added lines are always visible
        }

        static Output()
        {
            canLog = File.Exists(Settings.Default.LogFile);
            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Default_PropertyChanged);
        }
        static private bool canLog = false;
        static void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LogFile")
            {
                canLog = File.Exists(Settings.Default.LogFile);
            }
        }
    }
}
