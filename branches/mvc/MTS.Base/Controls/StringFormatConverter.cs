using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MTS.Base
{
    public class StringFormatConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string format = parameter.ToString();
            if (values.Length == 1)
                return string.Format(culture, format, values[0]);
            else if (values.Length == 2)
                return string.Format(culture, format, values[0], values[1]);
            else if (values.Length == 3)
                return string.Format(culture, format, values[0], values[1], values[2]);

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
