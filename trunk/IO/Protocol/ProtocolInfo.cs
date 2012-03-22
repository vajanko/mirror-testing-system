using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO.Protocol
{
    /// <summary>
    /// Auxiliary data structure holding basic information about particular communication protocol
    /// </summary>
    public class ProtocolInfo
    {
        /// <summary>
        /// (Get) Name of this protocol
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// (Get) Short localized description of this protocol
        /// </summary>
        public string Description { get; private set; }

        private Func<IModule> protocolFactory;
        /// <summary>
        /// Create a new instance of protocol dependant communication layer accessed by common interface
        /// </summary>
        /// <returns>Instance of protocol <see cref="IModule"/> implementation</returns>
        public IModule CreateModule()
        {
            return protocolFactory();
        }

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="ProtocolInfo"/> describing basic information about particular
        /// protocol
        /// </summary>
        /// <param name="name">Name of the protocol</param>
        /// <param name="description">Short description of the protocol</param>
        /// <param name="protocolFactory">Factory method creating a new instance of <see cref="IModule"/> implementation
        /// for a specific protocol</param>
        public ProtocolInfo(string name, string description, Func<IModule> protocolFactory)
        {
            Name = name;
            Description = description;
            this.protocolFactory = protocolFactory;
        }

        #endregion
    }
}
