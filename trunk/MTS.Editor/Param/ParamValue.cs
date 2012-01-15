using System;
using System.Globalization;

namespace MTS.Editor
{
    /// <summary>
    /// Base class for all test parameters
    /// </summary>
    public abstract class ParamValue : ValueBase
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";
        /// <summary>
        /// (Get) Value of parameter
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Convert parameter <see cref="Value"/> to string representation using invariant culture info
        /// </summary>
        /// <returns>String representation of parameter value</returns>
        public virtual string ValueToString()
        {
            return ValueToString(Value);
        }
        /// <summary>
        /// Convert parameter <paramref name="value"/> to string representation using invariant culture info.
        /// Return null if given value is null.
        /// </summary>
        /// <param name="value">Value of this parameter to convert to string representation</param>
        /// <returns>String representation of parameter <paramref name="value"/> or null if given value is null</returns>
        public virtual string ValueToString(object value)
        {
            return value == null ? null : string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String value to convert to some derived type</param>
        public abstract void ValueFromString(string value);
        /// <summary>
        /// Get enumerable type of this parameter
        /// </summary>
        /// <returns>Enumerator type describing this parameter</returns>
        public abstract ParamType ValueType();

        #region Constructors

        /// <summary>
        /// Create a new instance of parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public ParamValue(string id) : base(id) { }

        #endregion
    }
}
