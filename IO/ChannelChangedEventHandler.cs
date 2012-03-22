using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    /// <summary>
    /// Represents a method that will handle <see cref="IChannel.ValueChanged"/> event of <see cref="IChannel"/>
    /// interface implementation
    /// </summary>
    /// <param name="sender">Instance of channel that has been changed</param>
    /// <param name="args">Instance of <see cref="ChannelChangedEventArgs"/> holding event data</param>
    public delegate void ChannelChangedEventHandler(object sender, ChannelChangedEventArgs args);
}
