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
        /// This method could be called only from the thread where textBox has been created.
        /// Writes some text to ouptput console and breaks the line.
        /// </summary>
        /// <param name="text">Text to write</param>
        static private void writeLine(string text)
        {
            textBox.AppendText(text + "\n");
            textBox.ScrollToEnd();              // added lines are always visible
        }

        #region Constructors

        #endregion
    }
}
