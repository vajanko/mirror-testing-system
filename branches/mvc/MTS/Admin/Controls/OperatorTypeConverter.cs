using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MTS.Base;


namespace MTS.Admin.Controls
{
    /// <summary>
    /// Provide localized conversion from <see cref="OperatorType"/> enumerator to string representation and vice versa
    /// </summary>
    public class OperatorTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Convert operator type to its localized string representation
        /// </summary>
        /// <param name="value">Operator type to convert</param>
        /// <param name="targetType">Type to convert given value to. Only <see cref="string"/> is accepted</param>
        /// <param name="parameter">Conversion parameter is unused</param>
        /// <param name="culture">Culture info to localized conversion</param>
        /// <returns>Operator type localized string representation</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // no value specified
            if (value == null) return null;

            // only to string conversion is allowed
            if (targetType != typeof(string))
                throw new InvalidOperationException("Target type must be of type string");

            return OperatorTypes.Instance[(OperatorEnum)value].Name;
        }
        /// <summary>
        /// Convert string representation of operator to <see cref="OperatorType"/> enumerator
        /// </summary>
        /// <param name="value">String value representation of operator type</param>
        /// <param name="targetType">Type to convert given value to. Only <see cref="OperatorType"/></param>
        /// <param name="parameter">Conversion parameter is unused</param>
        /// <param name="culture">Culture info to provide localized conversion</param>
        /// <returns>Operator type</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // no value specified
            if (value == null) return null;

            // only to OperatorType conversion is allowed
            if (targetType != typeof(OperatorEnum))
                throw new InvalidOperationException(
                    string.Format("Target type must be of type {0}", typeof(OperatorEnum).Name));

            // case is not important
            string res = value.ToString();

            // get operator type enum value with given name
            return OperatorTypes.Instance[res].Value;
        }

        #endregion
    }
}
