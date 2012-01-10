using System;
using System.Windows;
using System.Windows.Data;

namespace MTS.Controls
{
    public class ParamIdToNameConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2)
                throw new InvalidOperationException("Missing binding value. At least 2 must be specified!");
            else if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return null;

            string testId = values[0] as string;
            if (testId == null)
                return null;
                //throw new InvalidOperationException("First binding value must be a string");

            string paramId = values[1] as string;
            if (paramId == null)
                throw new InvalidOperationException("Second binding value must be a string");

            string name = MTS.Editor.FileManager.GetParamName(testId, paramId);

            return name == null ? paramId : name;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
