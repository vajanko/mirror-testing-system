using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MTS.Data.Converters;
using MTS.Data.Types;

namespace MTS.Controls
{
    public class ParamValueConverter : IMultiValueConverter
    {
        private ParamTypeConverter converter = new ParamTypeConverter();

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                throw new InvalidOperationException("Missing binding value. At least 2 must be specified!");
            else if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;
            string str = values[0] as string;
            if (str == null)
                return null;
                     
                //throw new InvalidOperationException("First binding value must be a string");
            byte type = (byte)values[1];

            object value = converter.ConvertFromString((ParamType)type, str, culture);
            if (value is double)
                return string.Format("{0:0.###}", value);
            else
                return value.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This type of conversion is unnecesary. Do not use it!");
        }

        #endregion
    }
}
