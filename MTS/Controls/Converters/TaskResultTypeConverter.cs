using System;
using System.Windows;
using System.Windows.Data;
using MTS.Data.Types;

namespace MTS.Controls
{
    public class TaskResultTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            if (targetType != typeof(string))
                throw new InvalidOperationException("Target type must be of type string");

            switch ((TaskResultType)(byte)value)
            {
                case TaskResultType.Completed: return "Completed";
                case TaskResultType.Failed: return "Failed";
                case TaskResultType.Aborted: return "Aborted";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(TaskResultType))
                throw new InvalidOperationException(
                    string.Format("Target type must be of type {0}", typeof(TaskResultType).Name));

            string res = (value as string).ToLower();

            switch (res)
            {
                case "completed": return TaskResultType.Completed;
                case "failed": return TaskResultType.Failed;
                case "aborted": return TaskResultType.Aborted;
                default: return null;
            }
        }

        #endregion
    }
}
