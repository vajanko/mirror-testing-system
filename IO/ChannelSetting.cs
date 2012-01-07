using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTS.IO
{
    public class ChannelSetting : INotifyPropertyChanged
    {
        [Category("Channel")]
        [DisplayName("Id")]
        public string Id { get; set; }

        [Category("Channel")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Category("Channel")]
        [DisplayName("Description")]
        public string Description { get; set; }

        private int rowLow;
        [Category("Channel")]
        [DisplayName("RawLow")]
        public int RawLow
        {
            get { return rowLow; }
            set { rowLow = value; OnPropertyChanged("RawLow"); }
        }

        private int rawHigh;
        [Category("Channel")]
        [DisplayName("RawHigh")]
        public int RawHigh {
            get { return rawHigh; }
            set { rawHigh = value; OnPropertyChanged("RawHigh"); }
        }

        private double realLow;
        [Category("Channel")]
        [DisplayName("RealLow")]
        public double RealLow
        {
            get { return realLow; }
            set { realLow = value; OnPropertyChanged("RealLow"); }
        }

        private double realHigh;
        [Category("Channel")]
        [DisplayName("RealHigh")]
        public double RealHigh {
            get { return realHigh; }
            set { realHigh = value; OnPropertyChanged("RealHigh"); }
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { propertyChanged += value; }
            remove { propertyChanged -= value; }
        }

        private void OnPropertyChanged(string name)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Constructors

        public ChannelSetting() { }
        public ChannelSetting(string id)
        {
            Id = id;
        }

        #endregion
    }
}