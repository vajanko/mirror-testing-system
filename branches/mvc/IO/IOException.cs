using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MTS.IO
{
    /// <summary>
    /// Base class for exception that is thrown when an error occurs during communication
    /// </summary>
    public abstract class IOException : Exception
    {
        /// <summary>
        /// (Get) Name of protocol which was in use when this exception was thrown
        /// </summary>
        public string ProtocolName { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IOException"/> class.
        /// </summary>
        public IOException() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="IOException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IOException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="IOException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public IOException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initialize a new instance of <see cref="IOException"/> with serialized data
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        protected IOException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }                                                                    

        #endregion
    }
}
