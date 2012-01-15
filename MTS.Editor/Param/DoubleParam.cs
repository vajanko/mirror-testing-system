using System;
using System.Globalization;
using MTS.Base;

namespace MTS.Editor
{
    /// <summary>
    /// Test parameter holding double value. 
    /// </summary>
    public sealed class DoubleParam : NumericParam<double>
    {
        /// <summary>
        /// (Get/Set) Number of allowed decimals for this parameter value
        /// </summary>
        public int Decimals { get; set; }
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String to convert to double value</param>
        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = double.Parse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }
        /// <summary>
        /// Get enumerable type of this parameter: <see cref="ParamType.Double"/>
        /// </summary>
        /// <returns><see cref="ParamType.Double"/></returns>
        public override ParamType ValueType()
        {
            return ParamType.Double;
        }

        /// <summary>
        /// Convert double parameter value to specified unit value. Returns unchanged parameter value if conversion
        /// is not possible (Incompatible unit etc.)
        /// </summary>
        /// <param name="unit">Unit to convert to</param>
        /// <returns>Converted parameter value to specified unit</returns>
        public override double ConvertTo(Unit unit)
        {   // convert to double value
            return this.Unit.ConvertTo(unit, NumericValue);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of double parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public DoubleParam(string id) : base(id) { }

        #endregion
    }
}
