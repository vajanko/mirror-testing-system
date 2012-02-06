using System;
using System.Collections.Generic;
using System.Linq;

namespace MTS.IO.Address
{
    /// <summary>
    /// Exception that is thrown when an error occurs on channel address.
    /// </summary>
    public class AddressException : IOException
    {
        /// <summary>
        /// (Get) Name of channel that was addressed when this exception was thrown
        /// </summary>
        public string ChannelName { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <paramref name="AddressException"/> class.
        /// </summary>
        public AddressException() { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="AddressException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AddressException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="AddressException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public AddressException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }
}
