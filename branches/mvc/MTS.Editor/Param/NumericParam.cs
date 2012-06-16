using System;
using System.Globalization;
using MTS.Base;

namespace MTS.Editor
{
    /// <summary>
    /// Test parameter holding numeric value. Base class for numeric parameters
    /// </summary>
    /// <typeparam name="T">Numeric value type</typeparam>
    public abstract class NumericParam<T> : UnitParam where T : struct, IComparable
    {
        /// <summary>
        /// Constant string "NumericValue"
        /// </summary>
        public const string NumericValueString = "NumericValue";

        /// <summary>
        /// (Get/Set) Parameter value as a number. An event is raised when value is changed.
        /// </summary>
        public T NumericValue
        { 
            get { return (T)Value; }
            set { Value = value; OnPropertyChanged(NumericValueString); }
        }

        /// <summary>
        /// (Get) Minimal allowed value of this parameter
        /// </summary>
        public T MinValue { get; set; }
        /// <summary>
        /// (Get) Maximal allowed value of this parameter
        /// </summar
        public T MaxValue { get; set; }

        /// <summary>
        /// Convert numeric parameter value to specified unit value. Returns unchanged parameter value if conversion
        /// is not possible (Incompatible unit etc.)
        /// </summary>
        /// <param name="unit">Unit to convert to</param>
        /// <returns>Converted parameter value to specified unit</returns>
        public abstract T ConvertTo(Unit unit);

        #region Constructors

        /// <summary>
        /// Create a new instance of numeric parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public NumericParam(string id) : base(id) { }

        #endregion
    }
}
