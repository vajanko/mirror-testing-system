using System;
using MTS.Editor;
using MTS.Data.Types;

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
        /// Create a new instance of <see cref="ParamResult"/> intializing it with used parameter string
        /// identifier <see cref="ValueId"/> and database id of used paramerter
        /// </summary>
        /// <param name="valueId">String id of used parameter</param>
        /// <param name="databaseId">Database id of used parameter</param>
        /// <param name="value">Value of parameter. If this is null value will not be save to database
        /// but will be referenced by test output as used parameter</param>
        public ParamResult(ParamValue value, object resultValue = null)
            : base(value)
        {
            ResultValue = resultValue;
        }

        #endregion
    }
}
