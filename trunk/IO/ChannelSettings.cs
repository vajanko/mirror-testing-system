using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MTS.IO
{
    /// <summary>
    /// Collection of <see cref="ChannelSetting"/> instances holding settings of all analog channels.
    /// This class could be used by Visual Studio designer to manually change values of channel setting 
    /// through user interface
    /// </summary>
    public class ChannelSettings : CollectionBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Get a <see cref="ChannelSetting"/> instance of channel settings at a specified index
        /// </summary>
        /// <param name="index">Index at which to get channel settings</param>
        /// <returns>Instance of channel settings class</returns>
        public ChannelSetting this[int index]
        {
            get { return (ChannelSetting)List[index]; }
        }
        /// <summary>
        /// Add a new instance of <see cref="ChannelSetting"/> to the <see cref="ChannelSettings"/> collection
        /// </summary>
        /// <param name="channel">Instance of channel settings to add</param>
        public void Add(ChannelSetting channel)
        {   // register property changed event handler we this collection will get to know about any change in the collection
            channel.PropertyChanged += new PropertyChangedEventHandler(channel_PropertyChanged);
            // add to inner list of items
            List.Add(channel);
        }
        /// <summary>
        /// Remove given instance of <see cref="ChannelSetting"/> from the <see cref="ChannelSettings"/> collection
        /// </summary>
        /// <param name="channel">Instance of channel settings to be removed</param>
        public void Remove(ChannelSetting channel)
        {
            List.Remove(channel);
            // delete event handler - we do not know whether this instance will not be used on other place
            channel.PropertyChanged -= new PropertyChangedEventHandler(channel_PropertyChanged);
        }
        /// <summary>
        /// Get instance of <see cref="ChannelSetting"/> class for a channel identified by the name
        /// </summary>
        /// <param name="channelName">Name of the channel to get its settings class. Case insensitive</param>
        /// <returns>Instance of channel setting with given name</returns>
        public ChannelSetting GetSetting(string channelName)
        {   // case insensitive comparing
            channelName = channelName.ToLower();
            // find channel setting with given name - search case insensitive
            // there are no linq methods on IList
            foreach (ChannelSetting channel in List)
                if (channel.Id.ToLower() == channelName)
                    return channel;
            // channel not found
            return null;
        }

        /// <summary>
        /// This method is called by an instance of <see cref="ChannelSetting"/> in the collection
        /// which property has been changed. In this method <see cref="PropertyChanged"/> event will be raised
        /// </summary>
        /// <param name="sender">Instance of <see cref="ChannelSetting"/> collection item that has been changed</param>
        /// <param name="e"><see cref="ChannelSetting.PropertyChanged"/> event arguments</param>
        private void channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e.PropertyName);
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler propertyChanged;
        /// <summary>
        /// Event which occurs when property of channel settings change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }
        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for a given property on a given item of this collection
        /// </summary>
        /// <param name="collectionItem">Collection item on which property has been changed</param>
        /// <param name="propertyName">Name of the property that has been change on a specific collection item</param>
        private void OnPropertyChanged(object collectionItem, string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(collectionItem, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="ChannelSettings"/> class with the default initial capacity
        /// </summary>
        public ChannelSettings() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelSettings"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The number of <see cref="ChannelSetting"/> elements that the new list can 
        /// initially store.</param>
        public ChannelSettings(int capacity) : base(capacity) { }

        #endregion
    }
}
