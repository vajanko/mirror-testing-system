using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester.Result
{
    public class TaskResult
    {
        /// <summary>
        /// (Get/Set) Unique idnetifier of this task
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// (Get/Set) Time when task started its execution
        /// </summary>
        public DateTime Begin { get; set; }
        /// <summary>
        /// (Get/Set) Time when task finished its execution
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// (Get/Set) Duration of task execution. Diference of <see cref="End"/> and <see cref="Begin"/> properties
        /// </summary>
        public TimeSpan Duration
        {
            get { return new TimeSpan((End - Begin).Ticks); }
        }
        /// <summary>
        /// (Get/Set) Value describing final state of task execution. For more infomation see list of values
        /// of <see cref="TaskResultCode"/> enum
        /// </summary>
        public TaskResultCode ResultCode { get; set; }

        /// <summary>
        /// (Get) Collection of parameter results for this test. This values contains parameter identifier
        /// and its value in string representation. It is exprected that these values will be saved in database.
        /// For more information see <see cref="ParamResult"/> class implementation.
        /// </summary>
        public List<ParamResult> Params { get; private set; }

        #region Constructors

        public TaskResult(string id)
        {
            this.Id = id;
            Params = new List<ParamResult>();
        }

        #endregion
    }
}
