namespace MTS.IO
{
    /// <summary>
    /// Describes type of communication protocol with tester hardware
    /// </summary>
    public enum ProtocolType
    {
        /// <summary>
        /// EtherCAT communication protocol
        /// </summary>
        EtherCAT,
        /// <summary>
        /// TCP Modbus communication protocol
        /// </summary>
        Modbus,
        /// <summary>
        /// Special communication protocol designed for application testing
        /// </summary>
        Dummy
    }
}
