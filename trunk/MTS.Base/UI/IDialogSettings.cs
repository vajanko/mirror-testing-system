using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MTS.Base
{
    public interface IDialogSettings
    {
        /// <summary>
        /// (Get) Dialog window title
        /// </summary>
        string Title { get; }
        
        Visibility Buttton1Visibility { get; }
        object Button1Content { get; }
        Func<bool> Button1Click { get; }

        Visibility Button2Visibility { get; }
        object Button2Content { get; }
        Func<bool> Button2Click { get; }

        UserControl Control { get; }

        ButtonType DefaultButton { get; }
    }
}
