using System;

namespace MTS.IO
{
    public interface IAnalogOutput : IAnalogInput
    {
        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value effects <see cref="RealValue"/>
        /// Minimum possible value is <see cref="RawLow"/>. Maximum possible value is <see cref="RawHigh"/>
        /// </summary>
        new uint Value { get; set; }
    }
}
