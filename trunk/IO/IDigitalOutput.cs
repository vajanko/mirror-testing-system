using System;

namespace MTS.IO
{
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
        void SwitchOff();
    }
}
