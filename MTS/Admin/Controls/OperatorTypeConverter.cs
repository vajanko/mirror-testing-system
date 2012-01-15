using System;
using System.Windows;
using System.Windows.Data;
using MTS.Data.Types;

namespace MTS.Admin.Controls
{
    public class OperatorTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            if (targetType != typeof(string))
                throw new InvalidOperationException("Target type must be of type string");

            switch ((OperatorType)(byte)value)
            {
                case OperatorType.Admin: return "Admin";
                case OperatorType.User: return "User";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            if (targetType != typeof(OperatorType))
                throw new InvalidOperationException(
                    string.Format("Target type must be of type {0}", typeof(OperatorType).Name));

            string res = value.ToString().ToLower();

            switch (res)
            {
                case "admin": return OperatorType.Admin;
                case "user": return OperatorType.User;
                default: return null;
            }
        }

        #endregion
    }
}
