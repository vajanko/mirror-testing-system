using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Base;

namespace MTS.Editor
{
    public abstract class UnitParam : ParamValue
    {
        /// <summary>
        /// (Get/Set) Unit of this parameter value
        /// </summary>
        public Unit Unit { get; set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of unit parameter identified by unique string identifier
        /// </summary>
        /// <param name="id">Unique string identifier. Parameter contained in a test must have unique id</param>
        public UnitParam(string id) : base(id) { }

        #endregion

    }
}
