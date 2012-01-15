using System;
using System.Windows;
using System.Windows.Controls;

using MTS.Data.Types;


namespace MTS.Data
{
    public class ResultStyleSelector : StyleSelector
    {
        public Style CompletedStyle { get; set; }
        public Style FailedStyle { get; set; }
        public Style AbortedStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            DbTestResult result = item as DbTestResult;
            if (result == null)
                return base.SelectStyle(item, container);

            Style style = null;
            TaskResultType code = (TaskResultType)result.Result;
            if (code == TaskResultType.Completed)
                style = CompletedStyle;
            else if (code == TaskResultType.Failed)
                style = FailedStyle;
            else if (code == TaskResultType.Aborted)
                style = AbortedStyle;

            if (style == null)
                return base.SelectStyle(item, container);
            else
                return style;
        }
    }
}
