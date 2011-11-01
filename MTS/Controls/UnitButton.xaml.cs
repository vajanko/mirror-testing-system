using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTS.Controls
{
    /// <summary>
    /// Interaction logic for UnitButton.xaml
    /// </summary>
    public partial class UnitButton : UserControl
    {
        //#region Unit Property

        //public static readonly DependencyProperty UnitProperty =
        //    DependencyProperty.Register("Unit", typeof(Units), typeof(UpDownButton),
        //    new PropertyMetadata(Units.None));
        ///// <summary>
        ///// (Get/Set) Unit type of the numwric value
        ///// </summary>
        //public Units Unit
        //{
        //    get { return (Units)GetValue(UnitProperty); }
        //    set { SetValue(UnitProperty, value); }
        //}

        //#endregion


        //public UnitButton()
        //{
        //    InitializeComponent();
        //}
    }

    //[ValueConversion(typeof(Units), typeof(string))]
    //public class UnitToStringConverter : IValueConverter
    //{
    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (targetType != typeof(string)) return null;

    //        Units unit = (Units)value;
    //        switch (unit)
    //        {
    //            case Units.Degrees: return "˚";
    //            case Units.Grams: return "g";
    //            case Units.MiliAmpheres: return "mA";
    //            case Units.MiliMeters: return "mm";
    //            case Units.MiliSeconds: return "ms";
    //            case Units.Percets: return "%";
    //            default: return "";
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    //public enum Units
    //{
    //    None,
    //    Degrees,
    //    MiliAmpheres,
    //    MiliMeters,
    //    Grams,
    //    MiliSeconds,
    //    Percets
    //}

    //static public class ExtensionUnits
    //{
    //    /// <summary>
    //    /// Get the symbol identifying a particular unit types
    //    /// </summary>
    //    /// <param name="unit">Unit type</param>
    //    /// <returns>Symbol indentifying a particular unit</returns>
    //    static public string GetSymbol(this Units unit)
    //    {
    //        switch (unit)
    //        {
    //            case Units.Degrees: return "˚";
    //            case Units.Grams: return "g";
    //            case Units.MiliAmpheres: return "mA";
    //            case Units.MiliMeters: return "mm";
    //            case Units.MiliSeconds: return "ms";
    //            case Units.Percets: return "%";
    //            default: return "";
    //        }
    //    }
    //}
}
