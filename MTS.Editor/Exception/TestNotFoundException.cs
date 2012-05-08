using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor.Properties;

namespace MTS.Editor
{
    /// <summary>
    /// Exception that is thrown when test with given id is not found.
    /// </summary>
    public class TestNotFoundException : EditorException
    {
        /// <summary>
        /// (Get) Id of test that could not be found.
        /// </summary>
        public string TestId { get; private set; }

        #region Constructors

        /// <summary>
        /// Initialize an instance of <see cref="TestNotFoundException"/> with given message.
        /// This exception is thrown when test with given id is not found.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="testId">Id of test that could not be found.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public TestNotFoundException(string message, string testId, Exception innerException = null)
            : base(message, innerException)
        {
            this.TestId = testId;
        }

        /// <summary>
        /// Initialize an instance of <see cref="TestNotFoundException"/> with default message.
        /// This exception is thrown when test with given id is not found.
        /// </summary>
        /// <param name="testId">Id of test that could not be found.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public TestNotFoundException(string testId, Exception innerException = null)
            : this(string.Format(Resources.TestNotFoundMsg, testId), testId, innerException)
        {

        }

        #endregion
    }
}
