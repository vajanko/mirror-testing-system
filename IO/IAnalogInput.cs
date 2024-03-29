using System;

namespace MTS.IO
{
    /// <summary>
    /// Provide read only access to analog communication channel. Value is of this channel is raw value of a memory
    /// unit on remote communication terminal. To get a real value relative to application a conversion
    /// method is used
    /// </summary>
    public interface IAnalogInput : IChannel
    {
        /// <summary>
        /// (Get/Set) Minimal possible raw value of this channel. Raw value is value of <see cref="Value"/>
        /// property without interpretation.
        /// </summary>
        int RawLow { get; set; }
        /// <summary>
        /// (Get/Set) Maximal possible raw value of this channel. Raw value is value of <see cref="Value"/>
        /// property without interpretation.
        /// </summary>
        int RawHigh { get; set; }
        /// <summary>
        /// (Get/Set) Minimal possible real value of this channel. Real value if value of <see cref="RealValue"/>
        /// property interpreted as some quantity.
        /// </summary>
        double RealLow { get; set; }
        /// <summary>
        /// (Get/Set) Maximal possible real value of this channel. Real value if value of <see cref="RealValue"/>
        /// property interpreted as some quantity.
        /// </summary>
        double RealHigh { get; set; }

        /// <summary>
        /// (Get) Integer value of this channel. Setting this value effects <see cref="RealValue"/>
        /// Minimum possible value is <see cref="RawLow"/>. Maximum possible value is <see cref="RawHigh"/>
        /// </summary>
        uint Value { get; }

        /// <summary>
        /// (Get) Real value of this channel. Setting this value effects <see cref="Value"/>
        /// Minimum possible value is <see cref="RealLow"/>. Maximum possible value is <see cref="RealHigh"/>
        /// </summary>
        double RealValue { get; }

        /// <summary>
        /// Set value of property <see cref="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetValue(uint value);

        /// <summary>
        /// Get real value of this channel. Same as <see cref="RealValue"/> property. Use this method if 
        /// you want to reference it in a delegate.
        /// </summary>
        /// <returns>Real converted value of this channel using <see cref="RawToReal"/> delegate</returns>
        double GetRealValue();

        /// <summary>
        /// (Get/Set) Delegate that converts raw value to real value (<see cref="Value"/> to 
        /// <paramref name="RealValue"/> in this case) according to values <see cref="RawLow"/>,
        /// <paramref name="RawHigh"/>, <see cref="RealLow"/> and <see cref="RealHigh"/>
        /// </summary>
        Converter<uint, double> RawToReal { get; set; }
    }
}
