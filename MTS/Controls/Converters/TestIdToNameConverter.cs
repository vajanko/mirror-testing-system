using System;
using System.Xml.Linq;
using System.Globalization;
using System.Windows.Data;
using MTS.Editor;

namespace MTS.Controls
{
    public class TestIdToNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            string name = FileManager.GetTestName(value.ToString());

            return name == null ? value.ToString() : name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
