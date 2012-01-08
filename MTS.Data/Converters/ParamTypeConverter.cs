using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MTS.Data.Types;

namespace MTS.Data.Converters
{
    /// <summary>
    /// Converter that converts parameter string representation to its strongly typed instance
    /// f.e.: "123.456" converts to 123.456 as double. Also conversion to string is supported
    /// </summary>
    public class ParamTypeConverter
    {
        public string ConvertToString(ParamType type, object value)
        {
            if (value == null)
                return null;
            switch (type)
            {
                default: return value.ToString();
            }
        }
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
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ConvertFromString(ParamType type, string value)
        {
            return ConvertFromString(type, value, CultureInfo.InvariantCulture);
        }
    }
}
