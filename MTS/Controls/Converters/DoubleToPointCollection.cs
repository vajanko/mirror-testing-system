using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MTS.Controls
{
    class DoubleToPointCollection : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Universal method, bet it is only used for converting DoubleQueue to PointCollection
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;     // no double collection -> no Point collection

            if (targetType != typeof(PointCollection))
                throw new InvalidOperationException("Target type must be a PointCollection");
            DoubleQueue queue = value as DoubleQueue;
            if (queue == null)
                throw new InvalidOperationException("Converted value must a DoubleQueue");

            var points = new PointCollection();

            // skip values that are not added yet
            double x = (queue.Capacity - queue.Count - 1) * queue.Step;
            foreach (double val in queue)
                points.Add(new Point(x += queue.Step, (queue.Zero - val)));

            return points;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("There is no need to perform this conversion");
        }

        #endregion
    }
}
