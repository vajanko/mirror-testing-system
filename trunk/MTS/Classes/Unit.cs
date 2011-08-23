using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTS
{
    [TypeConverter(typeof(UnitConverter))]
    public class Unit
    {
        public Unit LargerUnit { get; set; }
        public Unit SmallerUnit { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class UnitConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }
        /// <summary>
        /// Realize conversion from string to Unit
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = value as string;
                switch (str)
                {
                    case "mA": return Units.Miliampheres;
                    case "A": return Units.Ampheres;
                    case "s": return Units.Seconds;
                    case "ms": return Units.Miliseconds;
                    default: return Units.None;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// Realize conversion form Unit to string
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public static class Units
    {
        public static Unit None { get; set; }

        public static Unit Miliampheres { get; set; }
        public static Unit Ampheres { get; set; }

        public static Unit Seconds { get; set; }
        public static Unit Miliseconds { get; set; }

        public static Unit Milimeters { get; set; }

        public static Unit Grams { get; set; }

        public static Unit Volts { get; set; }

        public static Unit Degrees { get; set; }

        static Units()
        {
            None = new Unit() { Name = "", FullName = "None" };

            Miliampheres = new Unit() { Name = "mA", FullName = "Miliampheres" };
            Ampheres = new Unit { Name = "A", FullName = "Ampheres", SmallerUnit = Miliampheres };
            Miliampheres.LargerUnit = Ampheres;

            Seconds = new Unit() { Name = "s", FullName = "Seconds" };
            Miliseconds = new Unit() { Name = "ms", FullName = "Miliseconds", LargerUnit = Seconds };
            Seconds.SmallerUnit = Miliseconds;

            Milimeters = new Unit() { Name = "mm", FullName = "Milimeters" };

            Grams = new Unit() { Name = "g", FullName = "Grams" };

            Volts = new Unit() { Name = "V", FullName = "Volts" };

            Degrees = new Unit() { Name = "°", FullName = "Degrees" };
        }
    }
}
