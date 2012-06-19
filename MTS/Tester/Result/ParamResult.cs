using System;
using MTS.Editor;

namespace MTS.Tester.Result
{
    /// <summary>
    /// Class describing result of a test parameter
    /// </summary>
    public class ParamResult
    {
        /// <summary>
        /// (Get) Result parameter value produced during testing
        /// </summary>
        public Nullable<double> ResultParam { get; private set; }

        /// <summary>
        /// (Get) Database id of parameter or test used to produce this result. Set to zero if there 
        /// will not be any reference to database
        /// </summary>
        public int DatabaseId { get; private set; }

        /// <summary>
        /// (Get) Unique identifier of parameter used when measuring current result data
        /// </summary>
        public string ValueId { get; private set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ParamResult"/> initializing it with used parameter string
        /// identifier <see cref="ValueId"/> and database id of used parameter
        /// </summary>
        /// <param name="usedParam">Instance of parameter data used to measure current result data</param>
        /// <param name="resultValue">Measured value of current parameter or null if current parameter
        /// has no result data</param>
        public ParamResult(ParamValue usedParam, Nullable<double> resultValue = null)
        {
            DatabaseId = usedParam.DatabaseId;
            ValueId = usedParam.ValueId;
            ResultParam = resultValue;
        }

        #endregion
    }
}
