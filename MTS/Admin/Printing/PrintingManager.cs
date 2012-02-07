using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Properties;

namespace MTS.Admin.Printing
{
    /// <summary>
    /// Static helper providing printing service of labels
    /// </summary>
    public static class PrintingManager
    {
        /// <summary>
        /// Print label for given mirror, testing result and date of testing
        /// </summary>
        /// <param name="mirrorName">Name of the mirror to be printed on the label</param>
        /// <param name="result">Result message of mirror testing process that will be printed on the label</param>
        /// <param name="date">Date when mirror testing was executed</param>
        public static void Print(string mirrorName, string result, DateTime date)
        {
            // create a new label
            PrintLabel label = new PrintLabel(mirrorName, result, date);
            // initialize its size
            label.PageWidth = (int)((double)Settings.Default.PrinterWidth * 3.93700787);
            label.PageHeight = (int)((double)Settings.Default.PrinterHeight * 3.93700787);
            // select printer to use
            label.PrinterSettings.PrinterName = Settings.Default.PrinterName;
            // send label to printer
            label.Print();
        }
        /// <summary>
        /// Print label for given mirror and testing result using current date
        /// </summary>
        /// <param name="mirrorName">Name of the mirror to be printed on the label</param>
        /// <param name="result">Result message of mirror testing process that will be printed on the label</param>
        public static void Print(string mirrorName, string result)
        {
            Print(mirrorName, result, DateTime.Now);
        }
    }
}
