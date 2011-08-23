using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTS
{
    public delegate void WriteHandler(string text);

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
        /// Write one line at the end of output console
        /// </summary>
        /// <param name="text">Text to write</param>
        static public void WriteLine(string text)
        {
            textBox.Dispatcher.Invoke(new WriteHandler(writeLine), text);
        }
        /// <summary>
        /// Write one line at the end of output console
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        static public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
        static private void writeLine(string text)
        {
            textBox.AppendText(text + "\n");
            textBox.ScrollToEnd();              // added lines are always visible
        }
        /// <summary>
        /// Cleat output console
        /// </summary>
        static public void Clear()
        {
            textBox.Clear();
        }
    }
}
