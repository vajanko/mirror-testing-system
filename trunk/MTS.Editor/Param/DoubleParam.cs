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
        /// Convert double parameter value to specified unit value. Returns unchanged parameter value if conversion
        /// is not possible (Incompatible unit etc.)
        /// </summary>
        /// <param name="unit">Unit to convert to</param>
        /// <returns>Converted parameter value to specified unit</returns>
        public override double ConvertTo(Unit unit)
        {   // convert to double value
            return this.Unit.ConvertTo(unit, NumericValue);
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
        /// Create a new instance of double parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public DoubleParam(string id) : base(id) { }

        #endregion
    }
}
