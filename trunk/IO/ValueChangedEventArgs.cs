using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    /// <summary>
    /// Provides a data for the event that is raised when value of a channel change
    /// </summary>
    public class ChannelChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Instance of channel that has been changed
        /// </summary>
        public IChannel Channel { get; private set; }

        /// <summary>
        /// Create a new instance of <see cref="ChannelChangedEventArgs"/> holding a reference to the channel
        /// that has been changed
        /// </summary>
        /// <param name="channel">Instance of <see cref="IChannel"/> or derived interface implementation. This
        /// is the channel that has been changed</param>
        public ChannelChangedEventArgs(IChannel channel)
        {
            Channel = channel;
        }
    }
}
