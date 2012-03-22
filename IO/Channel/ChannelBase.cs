using System;
using System.Globalization;
using System.Xml.Linq;
using System.ComponentModel;

namespace MTS.IO.Channel
{
    /// <summary>
    /// Base class for all channel implementations. There are two possible ways of looking at a channel:
    /// IChannel interface which provide access to channel value and IChannelAddress<<typeparamref name="TAddress"/>>
    /// which define protocol dependant channel addressing inside a module
    /// </summary>
    /// <typeparam name="TAddress">Type of channel address. This type depends on used protocol</typeparam>
    public abstract class ChannelBase<TAddress> : IChannel, IChannelAddress<TAddress> where TAddress:IAddress
    {
        #region Constants

        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";

        #endregion

        #region IChannel Members

        /// <summary>
        /// (Get/Set) Unique identifier of channel. This value should be used when referencing channel from it's module
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// (Get/Set) Name or short description of this channel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Get/Set) Long description of this channel
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// (Get/Set) Array of memory bytes containing <see cref="Value"/> of this channel. This 
        /// is necessary for network communication
        /// </summary>
        public abstract byte[] ValueBytes
        {
            get;
            set;
        }

        /// <summary>
        /// (Get/Set) Size of channel value in bytes. (Size of <see cref="ValueBytes"/> array)
        /// </summary>
        public int Size { get; set; }

        private event ChannelChangedEventHandler valueChanged;
        /// <summary>
        /// Occurs when channel value changed
        /// </summary>
        public event ChannelChangedEventHandler ValueChanged
        {
            add { valueChanged += value; }
            remove { valueChanged -= value; }
        }

        /// <summary>
        /// Raise <see cref="ValueChanged"/> event
        /// </summary>
        protected void OnValueChanged()
        {
            if (valueChanged != null)
                valueChanged(this, new ChannelChangedEventArgs(this));
        }

        #endregion

        /// <summary>
        /// (Get/Set) Address of channel in the hardware module. This allows us to access (read/write)
        /// data (from/to) this channel
        /// </summary>
        public TAddress Address { get; set; }
    }
}
