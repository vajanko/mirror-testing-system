using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    /// <summary>
    /// Interface of <see cref="IChannel"/> interface implementation for a specific protocol that defines channel
    /// addressing. This interface is only used by protocol implementation classes.
    /// </summary>
    /// <typeparam name="TAddress">Type of <see cref="IAddress"/> interface implementation defining particular
    /// address data for a specific protocol</typeparam>
    public interface IChannelAddress<TAddress> where TAddress : IAddress
    {
        /// <summary>
        /// (Get/Set) Address of channel inside the hardware. This allows us to access (read/write)
        /// data (from/to) this channel
        /// </summary>
        TAddress Address { get; set; }
    }
}
