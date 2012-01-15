using System;
using System.Globalization;

namespace MTS.Editor
{
    /// <summary>
    /// Test parameter holding enumerator value
    /// </summary>
    public sealed class EnumParam : ParamValue
    {
        /// <summary>
        /// Constant string "SelectedIndex"
        /// </summary>
        public const string SelectedIndexString = "SelectedIndex";

        /// <summary>
        /// (Get) Collection of possible values
        /// </summary>
        public string[] Values { get; private set; }
        /// <summary>
        /// Convert parameter value to string representation
        /// </summary>
        /// <returns>String representation of parameter value</returns>
        public override string ValueToString()
        {
            return SelectedIndex.ToString();
        }
        /// <summary>
        /// (Get/Set) Index of parameter enumerator value. This is the real value of this parameter
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)Value; }
            set { Value = value; OnPropertyChanged(SelectedIndexString); }
        }
        /// <summary>
        /// (Get) String representation of enumerator parameter value
        /// </summary>
        public string SelectedItem
        {
            get { return Values[SelectedIndex]; }
        }
        /// <summary>
        /// Initialize parameter value converted from given string
        /// </summary>
        /// <param name="value">String to convert to enumerator value</param>
        public override void ValueFromString(string value)
        {
            // let to throw an exception if value is not in correct format
            SelectedIndex = int.Parse(value);
            // if index is too large, throw an exception
            if (Values.Length < SelectedIndex)
                throw new ArgumentOutOfRangeException(SelectedIndexString, "Argument is grater than maximum possible value");
        }
        /// <summary>
        /// Get enumerable type of this parameter: <see cref="ParamType.Enum"/>
        /// </summary>
        /// <returns><see cref="ParamType.Enum"/></returns>
        public override ParamType ValueType()
        {
            return ParamType.Enum;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of enumerator parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public EnumParam(string id, string[] values)
            : base(id)
        {
            Values = values;
        }

        #endregion
    }
}
