using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester.Result
{
    public class TaskResult
    {
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

        #region Constructors

        public TaskResult()
        {
            
        }

        #endregion
    }
}
