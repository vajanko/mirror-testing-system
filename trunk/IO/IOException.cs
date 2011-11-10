using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    public abstract class IOException : Exception
    {
        public const string EtherCatString = "EtherCAT";
        public const string ModbusString = "Modbus";
        public const string DummyString = "Dummy";

        /// <summary>
        /// (Get) Name of procotol which was in use when this exception was thrown
        /// </summary>
        public string ProtocolName { get; set; }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <paramref name="IOException"/> class.
        /// </summary>
        public IOException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="IOException"/> class with a specified 
        /// error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IOException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the <paramref name="IOException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException"> The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public IOException(string message, Exception innerException) : base(message, innerException) { }

        #endregion
    }
}
