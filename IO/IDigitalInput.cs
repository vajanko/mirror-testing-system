using System;

namespace MTS.IO
{
    /// <summary>
    /// Provide read only access to digital input communication channel. Digital means that there are only two possible
    /// values - true or false. Input is referred from the side of application
    /// </summary>
    public interface IDigitalInput : IChannel
    {
        /// <summary>
        /// (Get) Logical value of this channel
        /// </summary>
        bool Value { get; }

        /// <summary>
        /// Set value of property <see cref="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetValue(bool value);
    }
}
