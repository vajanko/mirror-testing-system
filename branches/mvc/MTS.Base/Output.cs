using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace MTS.Base
{
    public static class Output
    {
        static private TextBox textBox;
        /// <summary>
        /// (Get/Set) Instance of <see cref="TextBox"/> where any output is written
        /// </summary>
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
        /// <param name="format">A composite format string</param>
        /// <param name="args">An array of object to write using format</param>
        static public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
        /// <summary>
        /// Write a peace of text at the end of output console without breaking new line
        /// Thread safe.
        /// </summary>
        /// <param name="text">Text to write</param>
        static public void Write(string text)
        {
            textBox.Dispatcher.BeginInvoke(new Action<string>(write), text);
        }
        /// <summary>
        /// Write a peace of text at the end of output console without breaking new line
        /// Thread safe.
        /// </summary>
        /// <param name="format">A composite format string</param>
        /// <param name="args">An array of object to write using format</param>
        static public void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
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
        /// Writes some text to output console and breaks the line.
        /// </summary>
        /// <param name="text">Text to write</param>
        static private void writeLine(string text)
        {
            write(text + "\n");
        }
        /// <summary>
        /// This method could be called only from the thread where textBox has been created.
        /// Writes some text to output console without breaking new line
        /// </summary>
        /// <param name="text">Text to write</param>
        static private void write(string text)
        {
            textBox.AppendText(text);
            textBox.ScrollToEnd();              // added lines are always visible
        }

        #region Constructors

        #endregion
    }
}
