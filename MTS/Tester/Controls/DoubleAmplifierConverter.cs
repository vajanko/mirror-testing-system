using System;
using System.Windows.Data;

namespace MTS.Tester.Controls
{
    /// <summary>
    /// This class is used to amplify some double value. It simply multiply converting value by Converter parameter
    /// </summary>
    class DoubleAmplifierConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Multiply value by Converter paramter
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            if (targetType != typeof(double))
                throw new InvalidOperationException("Target type must be a double");
            double angle = double.Parse(value.ToString());     // let Parse throw an exception if an error

            double param;
            if (!double.TryParse(parameter.ToString(), out param))
                param = 1;

            return angle * param;
        }
        /// <summary>
        /// This Method should be never called
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("There is no need to perform this conversion");
        }

        #endregion
    }
}
