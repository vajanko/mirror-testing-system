using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester
{
    public class ShiftExecutedEventArgs : EventArgs
    {
        public int Total { get; set; }
        public int Finished { get { return Passed + Failed; } }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Ramining { get { return Total - Finished; } }

        public TimeSpan Duration { get; set; }


        #region Constructors

        public ShiftExecutedEventArgs()
        {
        }

        #endregion
    }
}
