using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO.Module
{
    public class ModuleException : IOException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <paramref name="ModuleException"/> class.
        /// </summary>
        public ModuleException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ModuleException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ModuleException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ModuleException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public ModuleException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }

    public class ConnectionException : ModuleException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <paramref name="ConnectionException"/> class.
        /// </summary>
        public ConnectionException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ConnectionException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ConnectionException"/> class with a specified
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
