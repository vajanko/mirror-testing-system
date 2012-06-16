using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MTS.Base
{
    internal class FormatValueToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts given value using to string representation using <see cref="string.Format"/> method
        /// </summary>
        /// <param name="values">First item is a value to convert, second one is string format for 
        /// <see cref="string.Format"/> method</param>
        /// <param name="targetType">Type to convert given values to</param>
        /// <param name="parameter">Converter parameter - ignored value</param>
        /// <param name="culture">Culture to use when converting given value to its string representation</param>
        /// <returns>String representation of given value formatted by given string format</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3)
                return string.Join(" ", string.Format(culture, values[1].ToString(), values[0]),
                    string.Format(culture, "{0}", values[2])).Trim();
            else
                return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] tmp = value.ToString().Split();
            List<object> values = new List<object>();
            for (int i = 0; i < tmp.Length; i++)
            {
                decimal val;
                if (targetTypes[i] == typeof(decimal) &&
                    decimal.TryParse(tmp[i], System.Globalization.NumberStyles.Number, culture, out val))
                    values.Add(val);
                else
                    values.Add(tmp[i]);
            }
            return values.ToArray();
        }

        #endregion
    }
}
