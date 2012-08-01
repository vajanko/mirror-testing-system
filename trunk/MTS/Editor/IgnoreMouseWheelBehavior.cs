using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;


namespace MTS.Editor
{
    /// <summary>
    /// Captures and eats MouseWheel events so that a nested ListBox does not
    /// prevent an outer scrollable control from scrolling.
    /// Taken from: http://josheinstein.com/blog/index.php/2010/08/wpf-nested-scrollviewer-listbox-scrolling/
    /// Date: 1.8.2012
    /// </summary>
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;

            AssociatedObject.RaiseEvent(e2);

        }

    }
}
