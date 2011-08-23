using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MTS.AdminModule
{
    abstract class Channel : IChannel
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

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

    abstract class DigitalInput : Channel, IDigitalInput
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

        public override byte[] ValueBytes
        {
            get { return BitConverter.GetBytes(value); }
            set { SetValue(BitConverter.ToBoolean(value, 0)); }     // event raised
        }

        #endregion
    }

    abstract class DigitalOutput : DigitalInput, IDigitalOutput
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
}
