using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;

namespace MTS.Tester.Result
{
    public abstract class ResultBase
    {
        /// <summary>
        /// (Get) 
        /// </summary>
        public string ValueId { get { return Value.ValueId; } }
        /// <summary>
        /// (Get) Database id of parameter or test used to produce this result. Set to zero if there 
        /// will not be any reference to database
        /// </summary>
        public int DatabaseId { get { return Value.DatabaseId; } }
        /// <summary>
        /// (Get) 
        /// </summary>
        public ValueBase Value { get; protected set; }
        /// <summary>
        /// (Get/Set) Value indication whether this result has data to be stored to database
        /// </summary>
        public bool HasData { get; set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of test or parameter result intializing it with used test or parameter string
        /// identifier <see cref="ValueId"/> and database id of used test or paramerter value
        /// </summary>
        /// <param name="valueId">String id of used test or parameter</param>
        /// <param name="databaseId">Database id of used test or parameter</param>
        public ResultBase(ValueBase value)
        {
            Value = value;
        }

        #endregion
    }
}
