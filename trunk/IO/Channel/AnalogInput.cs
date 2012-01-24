using System;
using MTS.IO;

namespace MTS.IO.Channel
{
    public class AnalogInput<TAddress> : ChannelBase<TAddress>, IAnalogInput where TAddress :IAddress
    {
        #region Constants

        /// <summary>
        /// Constant string "RealValue"
        /// </summary>
        public const string RealValueString = "RealValue";

        #endregion

        #region IAnalog Members

        /// <summary>
        /// (Get/Set) Minimal possible raw value of this channel. Raw value is value of <see cref="Value"/>
        /// property without interpretation.
        /// </summary>
        public int RawLow { get; set; }
        /// <summary>
        /// (Get/Set) Maximal possible raw value of this channel. Raw value is value of <see cref="Value"/>
        /// property without interpretation.
        /// </summary>
        public int RawHigh { get; set; }
        /// <summary>
        /// (Get/Set) Minimal possible real value of this channel. Real value if value of <see cref="RealValue"/>
        /// property interpreted as some quantity.
        /// </summary>
        public double RealLow { get; set; }
        /// <summary>
        /// (Get/Set) Maximal possible real value of this channel. Real value if value of <see cref="RealValue"/>
        /// property interpreted as some quantity.
        /// </summary>
        public double RealHigh { get; set; }

        /// <summary>
        /// Raw value of this channel
        /// </summary>
        protected uint value;
        /// <summary>
        /// (Get) Integer (raw) value of this channel. Setting this value effects <see cref="RealValue"/>
        /// Minimum possible value is <see cref="RawLow"/>. Maximum possible value is <see cref="RawHigh"/>
        /// </summary>
        public uint Value { get { return value; } }

        /// <summary>
        /// (Get/Set) Real value of this channel. Setting this value effects <see cref="Value"/>
        /// Minimum possible value is <see cref="RealLow"/>. Maximum possible value is <see cref="RealHigh"/>
        /// </summary>
        public double RealValue
        {
            get { return RawToReal(Value); }
            set { throw new ChannelException("Trying to set real value on analog channel") { ChannelName = Name, ChannelValue = value }; }
        }

        /// <summary>
        /// (Get/Set) Delegate that converts raw value to real value (<see cref="Value"/> to 
        /// <see cref="RealValue"/> in this case) according to values <see cref="RawLow"/>,
        /// <see cref="RawHigh"/>, <see cref="RealLow"/> and <see cref="RealHigh"/>
        /// </summary>
        public Converter<uint, double> RawToReal { get; set; }

        /// <summary>
        /// Set value of property <see cref="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValue(uint value)
        {
            if (this.value != value)    // only change if necessary - no event will be raised
            {
                this.value = value;
                OnValueChanged();
            }
        }

        public double GetRealValue() { return RealValue; }

        /// <summary>
        /// (Get/Set) Array of memory bytes containing <see cref="Value"/> of this channel. This 
        /// is necessary for network communication
        /// </summary>
        public override byte[] ValueBytes
        {   // in modbus we only use UInt32
            get { return BitConverter.GetBytes(value); }
            set {
                switch (value.Length)
                {
                    case 1: SetValue(value[0]); break;
                    case 2: SetValue(BitConverter.ToUInt16(value, 0)); break;  // start at 0 byte
                    case 4: SetValue(BitConverter.ToUInt32(value, 0)); break;
                    default: SetValue(0); break;    // no value defined
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default method for converting raw values to real
        /// </summary>
        /// <param name="rawValue">Integer (raw) value to convert to double (real)</param>
        public double ConvertLinear(uint rawValue)
        {
            //if (RawHigh - RawLow == 0) return Value;    // prevent to divide by zero
            return (double)(((rawValue - RawLow) * (RealHigh - RealLow)) / (double)(RawHigh - RawLow) + RealLow);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of analog input channel. Default linear converter from raw to real value
        /// is set.
        /// </summary>
        public AnalogInput()
        {
            RawToReal = ConvertLinear;  // initialize with default (linear) converter
        }

        #endregion
    }
}
