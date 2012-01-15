using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTS.Base
{
    [TypeConverter(typeof(UnitConverter))]
    public class Unit
    {
        public Unit LargerUnit { get; set; }
        public Unit SmallerUnit { get; set; }

        /// <summary>
        /// (Get/Set) Short name of unit f.e.: mm, ms, s, mA, ...
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Get/Set) Long name of unit f.e.: millimeters, amperes, ...
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Convert unit to string representation using short name - see <see cref="Name"/>
        /// </summary>
        /// <returns><see cref="Name"/> of unit</returns>
        public override string ToString() { return Name; }

        /// <summary>
        /// Convert given value to particular unit
        /// </summary>
        /// <param name="unit">Unit to convert given value to</param>
        /// <param name="value">Value to be converted</param>
        /// <returns>Value converted to particular unit or given value if conversion is not possible</returns>
        public double ConvertTo(Unit unit, double value)
        {
            Unit current = this;
            double result = value;
            while (current != null)
            {
                if (current.Name == unit.Name)
                    return result;
                current = current.LargerUnit;
                result /= 1000;
            }

            current = this;
            result = value;
            while (current != null)
            {
                if (current.Name == unit.Name)
                    return result;
                current = current.SmallerUnit;
                result *= 1000;
            }
            return value;
        }
        /// <summary>
        /// Convert given value to particular unit
        /// </summary>
        /// <param name="unit">Unit to convert given value to</param>
        /// <param name="value">Value to be converted</param>
        /// <returns>Value converted to particular unit or given value if conversion is not possible</returns>
        public int ConvertTo(Unit unit, int value)
        {
            Unit current = this;
            int result = value;
            while (current != null)
            {
                if (current.Name == unit.Name)
                    return result;
                current = current.LargerUnit;
                result /= 1000;
            }

            current = this;
            result = value;
            while (current != null)
            {
                if (current.Name == unit.Name)
                    return result;
                current = current.SmallerUnit;
                result *= 1000;
            }
            return result;
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
                    case "V": return Units.Volts;
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

        public static Unit UnitFromString(string unit)
        {
            unit = unit.ToLower();
            switch (unit)
            {
                case "miliampheres": return Miliampheres;
                case "ampheres": return Ampheres;
                case "seconds": return Seconds;
                case "miliseconds": return Miliseconds;
                case "milimeters": return Milimeters;
                case "grams": return Grams;
                case "volts": return Volts;
                case "degrees": return Degrees;
                default: return None;
            }
        }

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
