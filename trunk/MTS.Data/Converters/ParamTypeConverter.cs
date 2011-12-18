using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MTS.Data.Types;

namespace MTS.Data.Converters
{
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
        public object ConvertFromString(ParamType type, string value)
        {
            switch (type)
            {
                case ParamType.Int: return int.Parse(value, CultureInfo.InvariantCulture);
                case ParamType.Double: return double.Parse(value, CultureInfo.InvariantCulture);
                case ParamType.Bool: return bool.Parse(value);
                case ParamType.String: return value;
                default: return null;
            }
        }
    }
}
