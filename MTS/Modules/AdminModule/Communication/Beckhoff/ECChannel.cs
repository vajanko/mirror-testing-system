using System;
using System.ComponentModel;


namespace MTS.AdminModule
{
    public abstract class ECChannel : IChannel
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";

        /// <summary>
        /// Event raised when some property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// (Get/Set) Identifier of group of channels this one belogs to. Group is a view on some collection of
        /// channels. For example one group may be Inputs or Output. This value will be usually 
        /// <paramref name="AdsReservedIndexGroups.SymbolValueByHandle"/>
        /// </summary>
        public int IndexGroup { get; set; }
        /// <summary>
        /// (Get/Set) Address of this channel inside group identified by <paramref name="IndexGroup"/>
        /// </summary>
        public int IndexOffset { get; set; }

        /// <summary>
        /// (Get/Set) Size of channel value structure in bytes. Digital Channel is always one byte
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// (Get/Set) Full name of this channel. By this identifier access to twincat io server to this channel
        /// is possible. In Twincat system manager also called - Server Symbol Name
        /// </summary>
        public string FullName { get; set; }

        #region IChannel Members

        /// <summary>
        /// (Get/Set) Name or short description of this channel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Raise an PropertyChanged event that signalized that some property has been changed
        /// </summary>
        /// <param name="name">Name of the propety that has been changed</param>
        public void NotifyPropretyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public abstract byte[] ValueBytes
        {
            get;
            set;
        }

        #endregion
    }

    class ECDigitalInput : ECChannel, IDigitalInput
    {
        #region IDigital Members

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

        public override byte[] ValueBytes
        {
            get { return BitConverter.GetBytes(value); }
            set { SetValue(BitConverter.ToBoolean(value, 0)); }     // event raised
        }

        #endregion
    }

    class ECDigitalOutput : ECDigitalInput
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

    class ECAnalogInput : ECChannel, IAnalogInput
    {
        #region IAnalog Members

        public int RawLow { get; set; }

        public int RawHigh { get; set; }

        public int RealLow { get; set; }

        public int RealHigh { get; set; }

        protected int value;
        /// <summary>
        /// (Get) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        public int Value { get { return value; } }

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

        public Converter<int, double> RawToReal { get; set; }

        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValue(int value)
        {
            if (this.value != value)    // only change if necessary - no event will be raised
            {
                this.value = value;
                NotifyPropretyChanged(ValueString);
            }
        }

        public override byte[] ValueBytes
        {
            get 
            {
                switch (Size)   // analog channel may be of different interger type (byte, inte16, inte32)
                {               // return only as much bytes as belogns to integer type of this channel
                    case 1: return BitConverter.GetBytes((byte)value);
                    case 2: return BitConverter.GetBytes((Int16)value);
                    case 4: return BitConverter.GetBytes((Int32)value);
                    default: return BitConverter.GetBytes(value);
                }
            }
            set
            {
                int val = 0;
                switch (Size)
                {
                    case 1: val = value[0]; break;
                    case 2: val = BitConverter.ToInt16(value, 0); break;
                    case 4: val = BitConverter.ToInt32(value, 0); break;
                }
                SetValue(val);
            }
        }

        #endregion

        /// <summary>
        /// Default method for converting raw values to real
        /// </summary>
        /// <param name="rawValue">Interger (raw) value to convert to double (real)</param>
        public double ConvertLinear(int rawValue)
        {
            return (double)((rawValue - RawLow) / (RawHigh - RawLow)) * (RealHigh - RealLow) + RealLow;
        }

        public ECAnalogInput()
        {
            RawToReal = ConvertLinear;  // initialize with default (linear) converter
        }
    }

    class ECAnalogOutput : ECAnalogInput
    {
        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value afects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        public new int Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }
    }
}
