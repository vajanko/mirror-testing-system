using System;
using System.Collections;

namespace MTS.IO
{
    /// <summary>
    /// Abstract layer for accessing particular remote hardware module
    /// Module contains several channels digital or analog, input or output
    /// </summary>
    public interface IModule : IEnumerable, IDisposable
    {
        /// <summary>
        /// Load configuration of channels form file. At this time connection must not be established
        /// </summary>
        /// <param name="filename">Path to file where configuration of channels is stored</param>
        void LoadConfiguration(string filename);

        /// <summary>
        /// Create a new connection between local computer and some hardware component. At the beginning of
        /// the communication this method must be called.
        /// </summary>
        /// <exception cref="MTS.IO.Module.ConnectionException">Connection could not be established</exception>
        void Connect();

        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        /// <exception cref="MTS.IO.Address.AddressException">Address of some channel does not exists</exception>
        void Initialize();

        /// <summary>
        /// Write all output and read all input channels (in this order)
        /// </summary>
        void Update();

        /// <summary>
        /// Read all inputs and outputs channels
        /// </summary>
        void UpdateInputs();

        /// <summary>
        /// Write all outputs channels
        /// </summary>
        void UpdateOutputs();

        /// <summary>
        /// Close connection between local computer and some hardware component. Call this method at the end of
        /// the communication to release resources.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Get an instance of particular channel identified by its name. Return null if there is no such a channel
        /// </summary>
        /// <param name="name">Unique name (identifier) of required channel</param>
        /// <exception cref="ChannelException">Channel identified by its name does not exists in current
        /// module</exception>
        IChannel GetChannelByName(string name);

        /// <summary>
        /// (Get) Value indicating that this module is Listening to remote hardware
        /// </summary>
        bool IsConnected { get; }
    }
}
