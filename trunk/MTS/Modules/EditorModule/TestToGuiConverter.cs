using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace MTS.EditorModule
{
    [ValueConversion(typeof(Test), typeof(FrameworkElement))]
    class TestToGuiConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Test))
                return null;

            //StackPanel panel = new StackPanel();
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            int row = 0;

            //if (value is IPresence)
            //{
            //    grid.RowDefinitions.Add(new RowDefinition());
            //    TextBlock tb = new TextBlock { Text = "Presence" };
            //    tb.SetValue(Grid.RowProperty, row);
            //    CheckBox cb = new CheckBox { Content = "Should be present" };
            //    cb.SetValue(Grid.ColumnProperty, 1);
            //    cb.SetValue(Grid.RowProperty, row);
            //    grid.Children.Add(tb);
            //    grid.Children.Add(cb);

            //    row++;
            //}

            return grid;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
