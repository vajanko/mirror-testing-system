using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Base
{
    public class ConfigNotFoundException : ConfigException
    {

        #region Constructors

        public ConfigNotFoundException()
        {
        }
        public ConfigNotFoundException(string message)
            : base(message)
        {
        }
        public ConfigNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public ConfigNotFoundException(string configPath, string message, Exception innerException)
            : this(message, innerException)
        {
            ConfigPath = configPath;
        }
        public ConfigNotFoundException(string configPath, string message)
            : this(configPath, message, null)
        { }

        #endregion
    }
}
