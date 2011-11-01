using System;

namespace MTS.IO
{
    class AnalogInput : ChannelBase, IAnalogInput
    {
        /// <summary>
        /// Constant string "RealValue"
        /// </summary>
        public const string RealValueString = "RealValue";

        #region IAnalog Members

        public int RawLow { get; set; }

        public int RawHigh { get; set; }

        public int RealLow { get; set; }

        public int RealHigh { get; set; }

        protected uint value;
        /// <summary>
        /// (Get) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        public uint Value { get { return value; } }

        /// <summary>
        /// (Get/Set) Real value of this channel. Setting this value afects <paramref name="Value"/>
        /// Minimum possible value is <paramref name="RealLow"/>. Maximum possible value is <paramref name="RealHigh"/>
        /// </summary>
        public double RealValue
        {
            get { return RawToReal(Value); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Converter<uint, double> RawToReal { get; set; }

        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValue(uint value)
        {
            if (this.value != value)    // only change if necessary - no event will be raised
            {
                this.value = value;
                //NotifyPropretyChanged(ValueString);
                NotifyPropretyChanged(RealValueString);
            }
        }

        public override byte[] ValueBytes
        {   // in modbus we only use UInt32
            get { return BitConverter.GetBytes(value); }
            set
            {
                SetValue(BitConverter.ToUInt32(value, 0));  // start at 0 byte
            }
        }

        #endregion

        /// <summary>
        /// Default method for converting raw values to real
        /// </summary>
        /// <param name="rawValue">Interger (raw) value to convert to double (real)</param>
        public double ConvertLinear(uint rawValue)
        {
            //if (RawHigh - RawLow == 0) return Value;    // prevent to devide by zero
            return (double)(((rawValue - RawLow) * (RealHigh - RealLow)) / (double)(RawHigh - RawLow) + RealLow);
        }

        #region Constructors

        public AnalogInput()
        {
            RawToReal = ConvertLinear;  // initialize with default (linear) converter
        }

        #endregion
    }
}
