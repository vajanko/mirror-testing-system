using System;
using System.Windows;
using System.Windows.Data;

namespace MTS.Controls
{
    public class TaskResultCodeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            if (targetType != typeof(string))
                throw new InvalidOperationException("Target type must be of type string");

            switch ((byte)value)
            {
                case 0: return "Completed";
                case 1: return "Failed";
                case 2: return "Aborted";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("Target type must be of type string");

            string res = (value as string).ToLower();

            switch (res)
            {
                case "completed": return 0;
                case "failed": return 1;
                case "aborted": return 2;
                default: return null;
            }
        }

        #endregion
    }
}
