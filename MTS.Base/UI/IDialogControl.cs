using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MTS.Base
{
    /// <summary>
    /// Interface to UI control that can be displayed in a dialog window
    /// </summary>
    public interface IDialogControl
    {
        /// <summary>
        /// (Get) Dialog window settings.
        /// </summary>
        IDialogSettings Settings { get; }
        /// <summary>
        /// (Get) Control displayed in the dialog window.
        /// </summary>
        Control Control { get; }
    }
}
