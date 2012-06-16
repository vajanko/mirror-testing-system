using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester
{
    /// <summary>
    /// Arguments of <see cref="Shift.ShiftExecuted"/> event
    /// </summary>
    public class ShiftExecutedEventArgs : EventArgs
    {
        /// <summary>
        /// (Get) Total number of sequences that should be executed
        /// </summary>
        public int Total { get; private set; }
        /// <summary>
        /// (Get) Number of finished sequences = Passed + Failed
        /// </summary>
        public int Finished { get { return Passed + Failed; } }
        /// <summary>
        /// (Get) Number of passed (completed correctly) sequences
        /// </summary>
        public int Passed { get; private set; }
        /// <summary>
        /// (Get) Number of failed (or aborted) sequences
        /// </summary>
        public int Failed { get; private set; }
        /// <summary>
        /// (Get) Number of sequences that are not finished yet and should be
        /// </summary>
        public int Ramining { get { return Total - Finished; } }

        /// <summary>
        /// (Get) Total duration of shift
        /// </summary>
        public TimeSpan Duration { get; private set; }


        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="Shift.ShiftExecuted"/> event arguments
        /// </summary>
        /// <param name="total">Total number of sequences that should be executed</param>
        /// <param name="failed">Number of failed (or aborted) sequences</param>
        /// <param name="passed">Number of passed (completed correctly) sequences</param>
        public ShiftExecutedEventArgs(int total, int failed, int passed)
        {
            Total = total;
            Failed = failed;
            Passed = passed;
        }
        /// <summary>
        /// Create a new instance of <see cref="Shift.ShiftExecuted"/> event arguments
        /// </summary>
        /// <param name="total">Total number of sequences that should be executed</param>
        /// <param name="failed">Number of failed (or aborted) sequences</param>
        /// <param name="passed">Number of passed (completed correctly) sequences</param>
        /// <param name="duration">Total duration of shift</param>
        public ShiftExecutedEventArgs(int total, int failed, int passed, TimeSpan duration)
            : this(total, failed, passed)
        {
            Duration = duration;
        }

        #endregion
    }
}
