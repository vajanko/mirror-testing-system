using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OK.Collections.Generic;

namespace MTS.Tester
{
    class TaskEdge : IGraphEdge
    {
        #region IGraphEdge Members

        public int VertexA { get; set; }

        public int VertexB { get; set; }

        #endregion
    }
}
