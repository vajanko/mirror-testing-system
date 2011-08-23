using System;
using System.ComponentModel;

namespace MTS.AdminModule
{
    public class DummyChannel : IChannel
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";

        #region IChannel Members

        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropretyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public byte[] ValueBytes
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

    public class DummyDigitalInput : DummyChannel, IDigitalInput
    {
        #region IDigitalInput Members

        protected bool value;
        /// <summary>
        /// (Get) Logical value of this channel.
        /// </summary>
        public bool Value
        {
            get { return value; }
        }
        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValue(bool value)
        {
            if (this.value != value)    // only change if necessary - no event will be raised
            {
                this.value = value;
                NotifyPropretyChanged(ValueString);
            }
        }

        #endregion
    }

    public class DummyDigitalOutput : DummyDigitalInput, IDigitalOutput
    {
        /// <summary>
        /// (Get/Set) Logical value of this channel.
        /// </summary>
        public new bool Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }
    }

    public class DummyAnalogInput : DummyChannel, IAnalogInput
    {
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
                NotifyPropretyChanged(ValueString);
            }
        }

        #endregion

        /// <summary>
        /// Default method for converting raw values to real
        /// </summary>
        /// <param name="rawValue">Interger (raw) value to convert to double (real)</param>
        public double ConvertLinear(uint rawValue)
        {
            return (double)(((rawValue - RawLow) * (RealHigh - RealLow)) / (double)(RawHigh - RawLow) + RealLow);
        }

        public DummyAnalogInput()
        {
            RawLow = 0;
            RawHigh = 4095;     // linear maping
            RealLow = 0;
            RealHigh = 4095;
            RawToReal = ConvertLinear;  // initialize with default (linear) converter
        }
    }

    public class DummyAnalogOutput : DummyAnalogInput
    {
        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        public new uint Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }
    }
}
