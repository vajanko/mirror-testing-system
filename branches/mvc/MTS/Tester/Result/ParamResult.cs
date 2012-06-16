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
        public ParamValue ResultParam { get; private set; }

        /// <summary>
        /// (Get) Database id of parameter or test used to produce this result. Set to zero if there 
        /// will not be any reference to database
        /// </summary>
        public int DatabaseId { get { return ResultParam.DatabaseId; } }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ParamResult"/> initializing it with used parameter string
        /// identifier <see cref="ValueId"/> and database id of used parameter
        /// </summary>
        /// <param name="valueId">String id of used parameter</param>
        /// <param name="databaseId">Database id of used parameter</param>
        /// <param name="paramValue">Value of parameter. If this is null value will not be save to database
        /// but will be referenced by test output as used parameter</param>
        public ParamResult(ParamValue usedParam, object resultValue = null)
        {
            ResultParam = usedParam.Clone() as ParamValue;
            ResultParam.Value = resultValue;
        }

        #endregion
    }
}
