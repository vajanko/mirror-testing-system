using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    /// <summary>
    /// Base class for all editor exceptions.
    /// </summary>
    public abstract class EditorException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initialize a new instance of any exception thrown by Editor
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public EditorException(string message, Exception innerException = null)
            : base(message, innerException) { }

        #endregion
    }
}
