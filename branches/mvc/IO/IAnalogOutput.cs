using System;

namespace MTS.IO
{
    /// <summary>
    /// Provide read-write access to analog communication channel. Value is of this channel is raw value of a memory
    /// unit on remote communication terminal. To get a real value relative to application a conversion
    /// method is used
    /// </summary>
    public interface IAnalogOutput : IAnalogInput
    {
        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value effects <see cref="RealValue"/>
        /// Minimum possible value is <see cref="RawLow"/>. Maximum possible value is <see cref="RawHigh"/>
        /// </summary>
        new uint Value { get; set; }
    }
}
