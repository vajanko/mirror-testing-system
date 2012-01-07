using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MTS.IO
{
    public class ChannelSettings : CollectionBase, INotifyPropertyChanged
    {
        public ChannelSetting this[int index]
        {
            get { return (ChannelSetting)List[index]; }
        }
        public void Add(ChannelSetting channel)
        {
            channel.PropertyChanged += new PropertyChangedEventHandler(channel_PropertyChanged);
            List.Add(channel);
        }
        public void Remove(ChannelSetting channel)
        {
            List.Remove(channel);
            channel.PropertyChanged += new PropertyChangedEventHandler(channel_PropertyChanged);
        }

        public ChannelSetting GetSetting(string channelName)
        {
            channelName = channelName.ToLower();
            foreach (ChannelSetting channel in List)
                if (channel.Id.ToLower() == channelName)
                    return channel;
            return null;
        }

        private void channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e.PropertyName);
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        private void OnPropertyChanged(object collectionItem, string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(collectionItem, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Constructors

        public ChannelSettings() { }
        public ChannelSettings(int capacity) : base(capacity) { }

        #endregion
    }
}
