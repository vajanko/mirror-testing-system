using System;
using System.ComponentModel;

namespace MTS.IO
{
    /// <summary>
    /// Base interface for IDigital and IAnalog. Both have property Value of different type. There will be two
    /// implementation of each interface (for input and output channel). In input channel property Value is
    /// only readed by the user (programmer), but it is wirited by program (and PropertyChanged event is raised)
    /// Output channel may be writed by user (programmer). Program reads this value and writes it to hardware
    /// terminal.
    /// </summary>
    public interface IChannel : INotifyPropertyChanged
    {
        /// <summary>
        /// (Get/Set) Name or short description of this channel
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Raise an PropertyChanged event that signalized that some property has been changed
        /// </summary>
        /// <param name="name">Name of the propety that has been changed</param>
        void NotifyPropretyChanged(string name);

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
        /// (Get/Set) Address of channel inside tha hardware. This allows us to access (read/write)
        /// data (from/to) this channel
        /// </summary>
        object Address { get; set; }
    }
}
