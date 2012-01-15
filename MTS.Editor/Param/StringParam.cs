using System;
using System.Globalization;

namespace MTS.Editor
{
    /// <summary>
    /// Test parameter holding string value
    /// </summary>
    public sealed class StringParam : ParamValue
    {
        /// <summary>
        /// Constant string "StringValue"
        /// </summary>
        public const string StringValueString = "StringValue";

        /// <summary>
        /// (Get/Set) Parameter value as a string. An event is raised when value is changed.
        /// </summary>
        public string StringValue
        {
            get { return (string)Value; }
            set { Value = value; OnPropertyChanged(StringValueString); }
        }
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String to convert to string value</param>
        public override void ValueFromString(string value)
        {
            // nothing to parse
            Value = value;
        }
        /// <summary>
        /// Get enumerable type of this parameter: <see cref="ParamType.String"/>
        /// </summary>
        /// <returns><see cref="ParamType.String"/></returns>
        public override ParamType ValueType()
        {
            return ParamType.String;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of string parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public StringParam(string id) : base(id) { }

        #endregion
    }
}
