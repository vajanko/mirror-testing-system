using System;
using System.Collections.Generic;
using System.Linq;

namespace MTS.IO.Module
{
    public class ConnectionException : ModuleException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        public ConnectionException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public ConnectionException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }
}
