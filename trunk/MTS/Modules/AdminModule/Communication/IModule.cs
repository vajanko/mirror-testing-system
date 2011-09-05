using System;

namespace MTS.AdminModule
{
    /// <summary>
    /// Abstract layer for accessing particular remote hardware module
    /// Module contains several channels digital or analog, input or output
    /// </summary>
    public interface IModule
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
        void Connect();

        /// <summary>
        /// Prepare (initialize) channels for reading and writing. When this method is called, connection
        /// must be established already.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Read all input and write all output channels
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
        /// Get an instance of paricular channel identified by its name. Return null if ther is no such a channel
        /// </summary>
        /// <param name="name">Unic name (identifier) of required channel</param>
        IChannel GetChannelByName(string name);

        /// <summary>
        /// (Get) Value indicating that this module is connected to remote hardware
        /// </summary>
        bool IsConnected { get; }

        void SwitchOffDigitalOutputs();
    }
}
