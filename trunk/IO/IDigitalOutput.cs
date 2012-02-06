using System;

namespace MTS.IO
{
    /// <summary>
    /// Provide read-write access to digital output communication channel. Digital means that there are only two possible
    /// values - true or false. Output is referred from the side of application
    /// </summary>
    public interface IDigitalOutput : IDigitalInput
    {
        /// <summary>
        /// (Get/Set) Logical value of this channel
        /// </summary>
        new bool Value { get; set; }

        /// <summary>
        /// Set logical value of this channel to true
        /// </summary>
        void On();
        /// <summary>
        /// Set logical value of this channel to false
        /// </summary>
        void Off();
    }
}
