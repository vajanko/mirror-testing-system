using System;

namespace MTS.IO.Channel
{
    public class ChannelException : IOException
    {
        /// <summary>
        /// Name of channel which caused this exception
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// Value of channel in the time when exception was thrown or null, if value was
        /// not specified
        /// </summary>
        public object ChannelValue { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <paramref name="ChannelException"/> class.
        /// </summary>
        public ChannelException() { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ChannelException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ChannelException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="ChannelException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public ChannelException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }
}
