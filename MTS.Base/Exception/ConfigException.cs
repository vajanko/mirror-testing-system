using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    /// <summary>
    /// Exception that is thrown when configuration of application is not set correctly. For example configuration
    /// file is missing or does not contain required data etc.
    /// </summary>
    public class ConfigException : ApplicationException
    {
        /// <summary>
        /// (Get/Set) Path of configuration file that caused this exception
        /// </summary>
        public string ConfigPath { get; set; }

        #region Constructors

        public ConfigException()
        {
        }
        public ConfigException(string message)
            : base(message)
        {
        }
        public ConfigException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
        public ConfigException(string configPath, string message, Exception innerException = null)
            : this(message, innerException)
        {
            ConfigPath = configPath;
        }
        public ConfigException(string configPath, string message)
            : this(configPath, message, null)
        { }

        #endregion
    }
}
