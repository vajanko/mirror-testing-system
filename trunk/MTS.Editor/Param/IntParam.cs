using System;
using System.Globalization;
using MTS.Base;

namespace MTS.Editor
{
    /// <summary>
    /// Test parameter holding integer value
    /// </summary>
    public sealed class IntParam : NumericParam<int>
    {
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String to convert to integer value</param>
        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = int.Parse(value, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Get enumerable type of this parameter: <see cref="ParamType.Int"/>
        /// </summary>
        /// <returns><see cref="ParamType.Int"/></returns>
        public override ParamType ValueType()
        {
            return ParamType.Int;
        }

        /// <summary>
        /// Convert integer parameter value to specified unit value. Returns unchanged parameter value if conversion
        /// is not possible (Incompatible unit etc.)
        /// </summary>
        /// <param name="unit">Unit to convert to</param>
        /// <returns>Converted parameter value to specified unit</returns>
        public override int ConvertTo(Unit unit)
        {   // convert to integer value
            return this.Unit.ConvertTo(unit, NumericValue);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of integer parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public IntParam(string id) : base(id) { }

        #endregion
    }
}
