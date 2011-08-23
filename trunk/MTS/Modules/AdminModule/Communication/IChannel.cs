using System;
using System.ComponentModel;
using System.IO;

namespace MTS.AdminModule
{
    /// <summary>
    /// Base interface for IDigital and IAnalog. Both have property Value of different type. There will be two
    /// implementation of each interface (for input and output channel). In input channel property Value is
    /// only readed by the user (programmer), but it is wirited by program (and PropertyChanged event is raised)
    /// Output channel may be writed by user (programmer). Program reads this value and writes it to hardware
    /// terminal.
    /// </summary>
    public interface IChannel : INotifyPropertyChanged
    {
        /// <summary>
        /// (Get/Set) Name or short description of this channel
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Raise an PropertyChanged event that signalized that some property has been changed
        /// </summary>
        /// <param name="name">Name of the propety that has been changed</param>
        void NotifyPropretyChanged(string name);

        /// <summary>
        /// (Get/Set) Array of memory bytes containing Value of this channel
        /// </summary>
        byte[] ValueBytes { get; set; }
    }
    public interface IDigitalInput : IChannel
    {
        /// <summary>
        /// (Get) Logical value of this channel
        /// </summary>
        bool Value { get; }

        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetValue(bool value);
    }
    public interface IDigitalOutput : IDigitalInput
    {
        /// <summary>
        /// (Get/Set) Logical value of this channel
        /// </summary>
        new bool Value { get; set; }
    }
    public interface IAnalogInput : IChannel
    {
        int RawLow { get; set; }
        int RawHigh { get; set; }
        int RealLow { get; set; }
        int RealHigh { get; set; }

        /// <summary>
        /// (Get) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        uint Value { get; }

        /// <summary>
        /// (Get) Real value of this channel. Setting this value afects <paramref name="Value"/>
        /// Minimum possible value is <paramref name="RealLow"/>. Maximum possible value is <paramref name="RealHigh"/>
        /// </summary>
        double RealValue { get; }

        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        void SetValue(uint value);

        /// <summary>
        /// (Get/Set) Delegate that converts raw value to real value (<paramref name="Value"/> to <paramref name="RealValue"/>
        /// in this case) according to values <paramref name="RawLow"/>, <paramref name="RawHigh"/>, <paramref name="RealLow"/>
        /// and <paramref name="RealHigh"/>
        /// </summary>
        Converter<uint, double> RawToReal { get; set; }
    }

    public interface IAnalogOutput : IAnalogInput
    {
        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        new uint Value { get; set; }
    }


}
