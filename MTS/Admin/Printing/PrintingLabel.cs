﻿using System;
using System.Drawing;
using System.Drawing.Printing;

namespace MTS.Admin.Printing
{
    /// <summary>
    /// Represents label that will be printed for a particular mirror test.
    /// </summary>
    public class PrintLabel : PrintDocument
    {
        /// <summary>
        /// (Get/Set) Font that will be used for printing this label
        /// </summary>
        public Font LabelFont { get; set; }
        /// <summary>
        /// Format of date printed on the label
        /// </summary>
        private string dateFormat = "{0:dd/MM/yyyy}";
        /// <summary>
        /// (Get/Set) Width of label in hundredths of an inch
        /// </summary>
        public int PageWidth { get; set; }
        /// <summary>
        /// (Get/Set) Height of label is hundredths of an inch
        /// </summary>
        public int PageHeight { get; set; }
        
        /// <summary>
        /// (Get/Set) Name of tested mirror for which this label is printed
        /// </summary>
        public string MirrorName { get; set; }
        /// <summary>
        /// (Get/Set) Part number of tested mirror for which this label is printed
        /// </summary>
        public string PartNumber { get; set; }
        /// <summary>
        /// (Get/Set) Supplier name of tested mirror for which this label is printed
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// (Get/Set) Description of testing result. Will be printed on the label
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// (Get/Set) Date of mirror testing. Usually current date. Will be printed on the label
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// (Get) Entire text that will be printed on the label. This is a concatenation of <see cref="MirrorName"/>,
        /// <see cref="Result"/> and <see cref="Date"/>
        /// </summary>
        public string PrintText
        {
            get { return string.Join("\n", MirrorName, PartNumber, Supplier, Result, string.Format(dateFormat, Date)); }
        }

        /// <summary>
        /// This method is called before label is printed
        /// </summary>
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            // choose default font if nothing is selected
            if (LabelFont == null)
                LabelFont = new Font("Courier", 10);
            // initialize page size
            DefaultPageSettings.PaperSize = new PaperSize("Custom", PageWidth, PageHeight);
        }
        /// <summary>
        /// This method is called when label is printed
        /// </summary>
        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            
            // get left-top position of printed text
            int leftMargin = DefaultPageSettings.Margins.Left;
            int topMargin = DefaultPageSettings.Margins.Top;
            
            // print text
            e.Graphics.DrawString(PrintText, LabelFont, Brushes.Black, leftMargin, topMargin);
            // stop after this page
            e.HasMorePages = false;
        }


        #region Constructors

        /// <summary>
        /// Create a new instance of default label - without any text to print
        /// </summary>
        public PrintLabel() :base()
        {
            DefaultPageSettings.Color = false;
            DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
        }
        /// <summary>
        /// Create a new instance of label with specified text to be printed
        /// </summary>
        /// <param name="mirrorName">Name of mirror will be printed on the label</param>
        /// <param name="result">Result of mirror testing will be printed on the label</param>
        /// <param name="date">Date of mirror testing will be printed on the label</param>
        public PrintLabel(string mirrorName, string result, DateTime date)
            : this()
        {
            MirrorName = mirrorName;
            Result = result;
            Date = date;
        }

        #endregion
    }
}
