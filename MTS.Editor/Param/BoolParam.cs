using System;
using System.Globalization;

namespace MTS.Editor
{
    public sealed class BoolParam : ParamValue
    {
        /// <summary>
        /// Constant string "BoolValue"
        /// </summary>
        public const string BoolValueString = "BoolValue";

        /// <summary>
        /// (Get/Set) Parameter value as Boolean. An event is raised when value is changed.
        /// </summary>
        public bool BoolValue
        {
            get { return (bool)Value; }
            set { Value = value; OnPropertyChanged(BoolValueString); }
        }
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String to convert to Boolean value</param>
        public override void ValueFromString(string value)
        {
            // throw an exception if value is not in correct format
            Value = bool.Parse(value);
        }
        /// <summary>
        /// Get enumerable type of this parameter: <see cref="ParamType.Bool"/>
        /// </summary>
        /// <returns><see cref="ParamType.Bool"/></returns>
        public override ParamType ValueType()
        {
            return ParamType.Bool;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of Boolean parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public BoolParam(string id) : base(id) { }

        #endregion
    }
}
