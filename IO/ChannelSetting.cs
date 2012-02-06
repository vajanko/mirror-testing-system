using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTS.IO
{
    /// <summary>
    /// Class holding settings of analog channel. This is required by Visual Studio when setting configuration
    /// file of this application through user interface
    /// </summary>
    public class ChannelSetting : INotifyPropertyChanged
    {
        /// <summary>
        /// (Get/Set) Unique identifier of this channel. This is a reference to channel configuration file
        /// </summary>
        [Category("Channel")]
        [DisplayName("Id")]
        public string Id { get; set; }

        /// <summary>
        /// (Get/Set) Localized (user adapted) name or short description of channel. This value could be displayed
        /// in user interface
        /// </summary>
        [Category("Channel")]
        [DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// (Get/Set) Localized (user adapted) description of channel. This value could be displayed in user interface
        /// </summary>
        [Category("Channel")]
        [DisplayName("Description")]
        public string Description { get; set; }

        private int rowLow;
        /// <summary>
        /// (Get/Set) Minimal possible raw value of channel.
        /// </summary>
        [Category("Channel")]
        [DisplayName("RawLow")]
        public int RawLow
        {
            get { return rowLow; }
            set { rowLow = value; OnPropertyChanged("RawLow"); }
        }

        private int rawHigh;
        /// <summary>
        /// (Get/Set) Maximal possible raw value of this channel.
        /// </summary>
        [Category("Channel")]
        [DisplayName("RawHigh")]
        public int RawHigh {
            get { return rawHigh; }
            set { rawHigh = value; OnPropertyChanged("RawHigh"); }
        }

        private double realLow;
        /// <summary>
        /// (Get/Set) Minimal possible real value of this channel.
        /// </summary>
        [Category("Channel")]
        [DisplayName("RealLow")]
        public double RealLow
        {
            get { return realLow; }
            set { realLow = value; OnPropertyChanged("RealLow"); }
        }

        private double realHigh;
        /// <summary>
        /// (Get/Set) Maximal possible real value of this channel.
        [Category("Channel")]
        [DisplayName("RealHigh")]
        public double RealHigh {
            get { return realHigh; }
            set { realHigh = value; OnPropertyChanged("RealHigh"); }
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler propertyChanged;
        /// <summary>
        /// Event which occurs when property of channel settings chaned
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }
        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for a given property
        /// </summary>
        /// <param name="name">Name of property which value has been changed</param>
        private void OnPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new empty setting of a channel
        /// </summary>
        public ChannelSetting() { }
        /// <summary>
        /// Create a new setting of a channel identified by given id
        /// </summary>
        /// <param name="id">Unique id of a channel for which setting is created</param>
        public ChannelSetting(string id)
        {
            Id = id;
        }

        #endregion
    }
}