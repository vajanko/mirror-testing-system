using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MTS.IO;
using MTS.Editor;

namespace MTS.Tester
{
    sealed class TravelWestTest : TravelTest
    {
        public TravelWestTest(Channels channels, TestValue testParam)
            : base(channels, testParam, MoveDirection.Left)
        { }
    }
}
