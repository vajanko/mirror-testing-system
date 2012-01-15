using System;
using MTS.Editor;

namespace MTS.Tester.Result
{
    public class ParamResult : ResultBase
    {
        /// <summary>
        /// (Get) Value of parameter result
        /// </summary>
        public object ResultValue { get; private set; }
        public string ResultStringValue { get { return Param.ValueToString(ResultValue); } }
        /// <summary>
        /// (Get) Parameter used to produce this output
        /// </summary>
        public ParamValue Param { get { return Value as ParamValue; } }
        public ParamType ValueType { get { return Param.ValueType(); } }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ParamResult"/> initializing it with used parameter string
        /// identifier <see cref="ValueId"/> and database id of used parameter
        /// </summary>
        /// <param name="valueId">String id of used parameter</param>
        /// <param name="databaseId">Database id of used parameter</param>
        /// <param name="value">Value of parameter. If this is null value will not be save to database
        /// but will be referenced by test output as used parameter</param>
        public ParamResult(ParamValue value, object resultValue = null)
            : base(value)
        {
            ResultValue = resultValue;
            // if parameter should not be saved to database, must be explicitly set
            // by default all parameters are saved to database (even if result value is null)
            HasData = true;
        }

        #endregion
    }
}
