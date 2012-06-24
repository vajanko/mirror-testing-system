using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTS.Base
{
    /// <summary>
    /// Interface for an object that can be displayed in a dialog window
    /// </summary>
    public interface IDialogSettings
    {
        /// <summary>
        /// (Get) Dialog window title
        /// </summary>
        string Title { get; }
        /// <summary>
        /// (Get) Visibility setting for first button
        /// </summary>
        Visibility Button1Visibility { get; }
        /// <summary>
        /// (Get) Content (usually) text for first button in the dialog window
        /// </summary>
        object Button1Content { get; }
        /// <summary>
        /// (Get) Handler that will be called when first button is clicked. Returns
        /// true if window can be closed after handler has been called.
        /// </summary>
        Func<bool> Button1Click { get; }

        /// <summary>
        /// (Get) Visibility setting for second button
        /// </summary>
        Visibility Button2Visibility { get; }
        /// <summary>
        /// (Get) Content (usually) text for second button in the dialog window
        /// </summary>
        object Button2Content { get; }
        /// <summary>
        /// (Get) Handler that will be called when second button is clicked. Returns
        /// true if window can be closed after handler has been called.
        /// </summary>
        Func<bool> Button2Click { get; }
        /// <summary>
        /// (Get) Instance of control to be displayed in the dialog window
        /// </summary>
        //UserControl Control { get; }

        /// <summary>
        /// (Get) Enumerator value specifying which button is the default one
        /// </summary>
        ButtonType DefaultButton { get; }
    }
}
