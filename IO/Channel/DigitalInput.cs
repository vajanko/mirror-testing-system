using System;

namespace MTS.IO.Channel
{
    class DigitalInput : ChannelBase, IDigitalInput
    {
        #region IDigitalInput Members

        /// <summary>
        /// Logical value of this channel.
        /// </summary>
        protected bool value;
        /// <summary>
        /// (Get) Logical value of this channel.
        /// </summary>
        public bool Value { get { return value; } }
        /// <summary>
        /// Set value of property <paramref name="Value"/>. An event will be raised
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValue(bool value)
        {
            if (this.value != value)    // only change if necessary - no event will be raised
            {
                this.value = value;
                OnValueChanged();
            }
        }
        /// <summary>
        /// (Get/Set) Array of memory bytes containing <paramref name="Value"/> of this channel. This 
        /// is necessary for network communication
        /// </summary>
        public override byte[] ValueBytes
        {
            get { return BitConverter.GetBytes(value); }
            set { SetValue(BitConverter.ToBoolean(value, 0)); }     // event will be raised raised
        }

        #endregion
    }
}
