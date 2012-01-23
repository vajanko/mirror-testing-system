using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.IO
{
    public interface IChannels
    {
        void LoadConfiguration(string filename);
        void Initialize();
        void Connect();
        void UpdateInputs();
        void UpdateOutputs();
        void Disconnect();
    }
}
