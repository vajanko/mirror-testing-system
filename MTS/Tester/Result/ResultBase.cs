using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;

namespace MTS.Tester.Result
{
    /// <summary>
    /// Base class for test and parameter result
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// (Get) Database id of parameter or test used to produce this result. Set to zero if there 
        /// will not be any reference to database
        /// </summary>
        public int DatabaseId { get; private set; }

        /// <summary>
        /// (Get/Set) Value indication whether this result has data to be stored to database
        /// </summary>
        public bool HasData { get; set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of test or parameter result initializing it with used test or parameter string
        /// identifier <see cref="ValueId"/> and database id of used test or parameter value
        /// </summary>
        public ResultBase(int databaseId)
        {
            DatabaseId = databaseId;
        }

        #endregion
    }
}
