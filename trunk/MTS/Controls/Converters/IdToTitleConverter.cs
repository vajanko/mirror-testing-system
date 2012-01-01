using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace MTS.Controls
{
    public class IdToTitleConverter : IMultiValueConverter
    {
        private Func<string, bool, string> converter;

        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(string) && values != null && values.Length == 2
                && values[0] is string && values[1] is bool)
            {
                if (converter != null)
                    return converter(values[0] as string, (bool)values[1]);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion

        /// <summary>
        /// Create a new instance of converter used to for conversion from Id and IsSaved value
        /// to Title. For this conversion a given method is used
        /// </summary>
        /// <param name="convertMethod">Method that takes one string and one bool parameter and returns
        /// string. This method provide conversion from Id and IsSaved values to Title</param>
        public IdToTitleConverter(Func<string, bool, string> convertMethod)
        {
            this.converter = convertMethod;
        }
    }
}
