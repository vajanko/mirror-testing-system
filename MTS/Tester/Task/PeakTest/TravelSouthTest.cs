using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;
using MTS.Editor;

namespace MTS.Tester
{
    class TravelSouthTest : TravelTest
    {
        public TravelSouthTest(Channels channels, TestValue testParam)
            : base(channels, testParam, MoveDirection.Down)
        { }
    }
}
