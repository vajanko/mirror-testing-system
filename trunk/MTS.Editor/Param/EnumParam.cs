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
        /// Call visitor method on this instance of parameter value adding new functions
        /// </summary>
        /// <param name="visitor">Instance of visitor adding new function to parameter value</param>
        public override void Accept(IValueVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Creates a deep copy of <see cref="EnumParam"/> instance
        /// </summary>
        /// <returns>New instance of <see cref="EnumParam"/> class</returns>
        public override object Clone()
        {
            return new EnumParam(ValueId, Values)
            {
                DatabaseId = this.DatabaseId,
                Name = this.Name,
                Description = this.Description,
                SelectedIndex = this.SelectedIndex
            };
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
