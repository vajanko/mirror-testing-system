using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTS.Base
{
    /// <summary>
    /// Default implementation of <see cref="IDialogSettings"/> interface.
    /// Holds information about control displayed in a dialog window.
    /// </summary>
    public class DialogSettings : IDialogSettings
    {
        #region IDialogSettings Members

        /// <summary>
        /// (Get) Dialog window title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// (Get) Visibility setting for first button
        /// </summary>
        public Visibility Button1Visibility { get; set; }
        /// <summary>
        /// (Get) Content (usually) text for first button in the dialog window
        /// </summary>
        public object Button1Content { get; set; }
        /// <summary>
        /// (Get) Handler that will be called when first button is clicked. Returns
        /// true if window can be closed after handler has been called.
        /// </summary>
        public Func<bool> Button1Click { get; set; }
        /// <summary>
        /// (Get) Visibility setting for second button
        /// </summary>
        public Visibility Button2Visibility { get; set; }
        /// <summary>
        /// (Get) Content (usually) text for second button in the dialog window
        /// </summary>
        public object Button2Content { get; set; }
        /// <summary>
        /// (Get) Handler that will be called when second button is clicked. Returns
        /// true if window can be closed after handler has been called.
        /// </summary>
        public Func<bool> Button2Click { get; set; }
        /// <summary>
        /// (Get) Instance of control to be displayed in the dialog window
        /// </summary>
        //public UserControl Control { get; set; }
        /// <summary>
        /// (Get) Enumerator value specifying which button is the default one
        /// </summary>
        public ButtonType DefaultButton { get; set; }

        #endregion

        #region Constructors

        public DialogSettings()
        {
            Title = "Dialog Window";
            DefaultButton = ButtonType.None;
        }

        #endregion
    }

    public enum ButtonType
    {
        None,
        Button1,
        Button2
    }
}
