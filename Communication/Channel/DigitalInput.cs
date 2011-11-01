using System;
using System.ComponentModel;

namespace MTS.IO
{
    class DigitalInput : ChannelBase, IDigitalInput
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
        /// <summary>
        /// (Get/Set) Array of bytes representing channel value in memory. This is necessary for network
        /// communication
        /// </summary>
        public override byte[] ValueBytes
        {
            get { return BitConverter.GetBytes(value); }
            set { SetValue(BitConverter.ToBoolean(value, 0)); }     // event raised
        }

        #endregion
    }
}
