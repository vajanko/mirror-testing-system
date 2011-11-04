﻿using System;
using System.ComponentModel;

namespace MTS.AdminModule
{
    abstract class ChannelBase : IChannel
    {
        /// <summary>
        /// Constant string "Value"
        /// </summary>
        public const string ValueString = "Value";

        /// <summary>
        /// Event that is raised when property of this channel change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// <summary>
        /// (Get/Set) Array of bytes representing channel value in memory. This is necessary for network
        /// communication
        /// </summary>
        public abstract byte[] ValueBytes
        {
            get;
            set;
        }

        /// <summary>
        /// (Get/Set) Size of channel value in bytes. (Size of <paramref name="ValueBytes"/> array)
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// (Get/Set) Address of channel inside tha hardware. This allows us to access (read/write)
        /// data (from/to) this channel
        /// </summary>
        public object Address { get; set; }

        #endregion
    }
}