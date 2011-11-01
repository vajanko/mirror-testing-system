using System;
using System.Collections.Generic;

namespace MTS.Modules.TesterModule
{
    abstract class HardwareComponent
    {

        public abstract int Read(Int32 connection);

        public HardwareComponent() { }
    }
}
