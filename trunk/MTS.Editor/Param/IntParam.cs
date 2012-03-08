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
        /// Convert integer parameter value to specified unit value. Returns unchanged parameter value if conversion
        /// is not possible (Incompatible unit etc.)
        /// </summary>
        /// <param name="unit">Unit to convert to</param>
        /// <returns>Converted parameter value to specified unit</returns>
        public override int ConvertTo(Unit unit)
        {   // convert to integer value
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

        /// <summary>
        /// Creates a deep copy of <see cref="IntParam"/> instance
        /// </summary>
        /// <returns>New instance of <see cref="IntParam"/> class</returns>
        public override object Clone()
        {
            return new IntParam(ValueId)
            {
                DatabaseId = this.DatabaseId,
                Name = this.Name,
                Description = this.Description,
                NumericValue = this.NumericValue
            };
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
