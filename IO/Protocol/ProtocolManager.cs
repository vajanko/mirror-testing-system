using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO.Protocol
{
    /// <summary>
    /// Singleton class managing different types of communication protocols. Allows to register a new protocol
    /// or iterate through all supporting ones.
    /// </summary>
    public class ProtocolManager
    {
        #region Singleton

        private static ProtocolManager instance = new ProtocolManager();
        /// <summary>
        /// (Get) Singleton instance of <see cref="ProtocolManager"/> class
        /// </summary>
        public static ProtocolManager Instance { get { return instance; } }

        #endregion

        /// <summary>
        /// Collection of all known (registered) protocols identified by unique name
        /// </summary>
        private Dictionary<string, ProtocolInfo> protocols = new Dictionary<string, ProtocolInfo>();

        /// <summary>
        /// (Get) Collection of all known protocols
        /// </summary>
        public IEnumerable<ProtocolInfo> Protocols { get { return protocols.Values; } }

        /// <summary>
        /// Add a new protocol to the collection of known protocols with specified name, description and 
        /// factory method that will create an instance of <see cref="IModule"/> communication layer
        /// </summary>
        /// <param name="name">Name of protocol to register. Must be unique</param>
        /// <param name="description">Description of protocol to register</param>
        /// <param name="protocolFactory">Factory method that will create an instance of <see cref="IModule"/>
        /// communication layer with given name as a first parameter</param>
        /// <exception cref="System.ArgumentException">Protocol definition with the same name
        /// already exists in the collection of known <see cref="Protocols"/></exception>
        public void RegisterProtocol(string name, string description, Func<string, IModule> protocolFactory)
        {
            ProtocolInfo protocol = new ProtocolInfo(name, description, protocolFactory);
            protocols.Add(name, protocol);
        }
        
        #region Constructors

        /// <summary>
        /// Prevent for creating an instance of <see cref="ProtocolManager"/> as this is a singleton class
        /// </summary>
        private ProtocolManager() { }

        #endregion
    }
}
