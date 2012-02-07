using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Simulator
{
    class Timer
    {
        public double Interval { get; set; }
        private bool isFirstCall = true;
        private DateTime start;

        public bool Running { get { return !isFirstCall; } }

        public void Initialize()
        {
            isFirstCall = true;
        }
        public bool Finished(DateTime time)
        {
            if (isFirstCall)
            {
                isFirstCall = false;
                start = time;
            }

            return (time - start).TotalMilliseconds > Interval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">Miliseconds</param>
        public Timer(double interval = 0)
        {
            this.Interval = interval;
        }
    }
}
