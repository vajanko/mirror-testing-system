using System;
using System.Collections.Generic;
using MTS.Editor;
using MTS.Data.Types;

namespace MTS.Tester.Result
{
    public class TaskResult : ResultBase
    {
        #region Properties

        /// <summary>
        /// (Get/Set) Time when task started its execution
        /// </summary>
        public DateTime Begin { get; set; }
        /// <summary>
        /// (Get/Set) Time when task finished its execution
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// (Get/Set) Duration of task execution. Difference of <see cref="End"/> and <see cref="Begin"/> properties
        /// </summary>
        public TimeSpan Duration
        {
            get { return new TimeSpan((End - Begin).Ticks); }
        }
        /// <summary>
        /// (Get/Set) Value describing final state of task execution. For more information see list of values
        /// of <see cref="TaskResultCode"/> enumerator
        /// </summary>
        public TaskResultType ResultCode { get; set; }
        /// <summary>
        /// (Get) Collection of parameter results for this test. This values contains parameter identifier
        /// and its value in string representation. It is expected that these values will be saved in database.
        /// For more information see <see cref="ParamResult"/> class implementation.
        /// </summary>
        public List<ParamResult> Params { get; private set; }

        #endregion

        #region Constructors

        public TaskResult(TestValue value = null)
            : base(value)
        {
            Params = new List<ParamResult>();
        }

        #endregion
    }
}
