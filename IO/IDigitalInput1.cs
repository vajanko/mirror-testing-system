using System;

namespace MTS.IO
{
    public interface IDigitalInput : IChannel
    {
        /// <summary>
        /// (Get) Logical value of this channel
        /// </summary>
        bool Value { get; }

        /// <summary>
        /// Set value of property <see nacrefme="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetValue(bool value);
    }
}
