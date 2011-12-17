using System;

namespace MTS.Tester.Result
{
    public class ParamResult
    {
        /// <summary>
        /// (Get) Unique identifier of parameter
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// (Get) Value of parameter
        /// </summary>
        public object Value { get; private set; }

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="ParamResult"/>. This value could be saved to database
        /// </summary>
        /// <param name="id">Unique identifier of parameter</param>
        /// <param name="value">Value of parameter</param>
        public ParamResult(string id, object value)
        {
            Id = id;
            Value = value;
        }

        #endregion
    }
}
