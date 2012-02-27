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
        /// Call visitor method on this instance of parameter value adding new functions
        /// </summary>
        /// <param name="visitor">Instance of visitor adding new function to parameter value</param>
        public override void Accept(IValueVisitor visitor)
        {
            visitor.Visit(this);
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
