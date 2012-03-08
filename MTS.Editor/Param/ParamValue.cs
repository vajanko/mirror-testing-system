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
        /// (Get/Set) Value of parameter
        /// </summary>
        public object Value { get; set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public ParamValue(string id) : base(id) { }

        #endregion
    }
}
