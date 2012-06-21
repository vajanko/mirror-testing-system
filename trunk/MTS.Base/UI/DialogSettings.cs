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

        public string Title { get; set; }

        public Visibility Buttton1Visibility { get; set; }

        public object Button1Content { get; set; }

        public Func<bool> Button1Click { get; set; }

        public Visibility Button2Visibility { get; set; }

        public object Button2Content { get; set; }

        public Func<bool> Button2Click { get; set; }

        public UserControl Control { get; set; }

        public ButtonType DefaultButton { get; set; }

        #endregion
    }

    public enum ButtonType
    {
        None,
        Button1,
        Button2
    }
}
