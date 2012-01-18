using System;
using System.ComponentModel;

namespace MTS.IO
{
    /// <summary>
    /// Base interface for IDigital and IAnalog. Both have property Value of different type. There will be two
    /// implementation of each interface (for input and output channel). In input channel property Value is
    /// only read by the user (programmer), but it is wrote by program (and PropertyChanged event is raised)
    /// Output channel may be wrote by user (programmer). Program reads this value and writes it to hardware
    /// terminal.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// (Get/Set) Name or short description of this channel
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Event that occurs when value of channel is changed
        /// </summary>
        event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// (Get/Set) Array of memory bytes containing <paramref name="Value"/> of this channel. This 
        /// is necessary for network communication
        /// </summary>
        byte[] ValueBytes { get; set; }

        /// <summary>
        /// (Get/Set) Size of channel value in bytes. (Size of <paramref name="ValueBytes"/> array)
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// (Get/Set) Address of channel inside the hardware. This allows us to access (read/write)
        /// data (from/to) this channel
        /// </summary>
        object Address { get; set; }
    }

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs args);
}
