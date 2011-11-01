using System;
using System.Windows;

namespace MTS.Modules.Parts
{
    /// <summary>
    /// Use to add some new panel to main window
    /// </summary>
    interface IPanel
    {
        /// <summary>
        /// Get the graphical user interface represented as any framework content element
        /// </summary>
        FrameworkContentElement Gui { get; }
    }
}
