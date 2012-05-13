using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MTS.Data.Types;

namespace MTS.Data.Converters
{
    /// <summary>
    /// Converter of parameter type and parameter value in string representation to its strongly typed instance.
    /// Also conversion to string is supported.
    /// </summary>
    public class ParamTypeConverter
    {
        /// <summary>
        /// Converts given strongly typed parameter value to its string representation
        /// </summary>
        /// <param name="type">Type of parameter value</param>
        /// <param name="value">Strongly typed instance of parameter value</param>
        /// <returns>String representation of parameter value</returns>
        public string ConvertToString(ParamType type, object value)
        {
            if (value == null)
                return null;
            switch (type)
            {
                default: return value.ToString();
            }
        }
        /// <summary>
        /// Converts given parameter value in string representation to corresponding type according to
        /// <paramref name="type"/> value
        /// </summary>
        /// <param name="type">Type of parameter value</param>
        /// <param name="value">String representation of parameter value</param>
        /// <param name="cultureInfo"></param>
        /// <returns>Strongly typed instance of parameter value</returns>
        public object ConvertFromString(ParamType type, string value, CultureInfo cultureInfo)
        {
            switch (type)
            {
                case ParamType.Int: return int.Parse(value, cultureInfo);
                case ParamType.Double: return double.Parse(value, cultureInfo);
                case ParamType.Bool: return bool.Parse(value);
                case ParamType.String: return value;
                default: return null;
            }
        }
        /// <summary>
        /// Convert given value using invariant culture (see <see cref="CultureInfo.InvariantCulture"/>
        /// </summary>
        /// <param name="type">Type of parameter value</param>
        /// <param name="value">String representation of parameter value</param>
        /// <returns>Strongly typed instance of parameter value</returns>
        public object ConvertFromString(ParamType type, string value)
        {
            return ConvertFromString(type, value, CultureInfo.InvariantCulture);
        }
    }
}
