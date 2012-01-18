using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    public class ValueChangedEventArgs : EventArgs
    {
        public IChannel Channel { get; private set; }

        public ValueChangedEventArgs(IChannel channel)
        {
            Channel = channel;
        }
    }
}
