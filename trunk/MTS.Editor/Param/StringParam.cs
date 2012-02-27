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
        /// Call visitor method on this instance of parameter value adding new functions
        /// </summary>
        /// <param name="visitor">Instance of visitor adding new function to parameter value</param>
        public override void Accept(IValueVisitor visitor)
        {
            visitor.Visit(this);
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
