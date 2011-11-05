using System;

using MTS.IO;
using MTS.Editor;

namespace MTS.TesterModule
{
    public abstract class PeakTest : TestTask
    {
        #region Fields

        protected int maxMeasuredOverloadTime = 0;
        protected bool isOverloaded = false;
        protected TimeSpan overloaded;

        #endregion

        #region Properties

        protected double MaxCurrent { get; private set; }
        protected int MaxOverloadTime { get; private set; }

        #endregion

        /// <summary>
        /// Generate the result (final state) of this test after it has been finished
        /// </summary>
        /// <returns></returns>
        protected TaskState getTaskState()
        {
            if (maxMeasuredOverloadTime > MaxOverloadTime)
                return TaskState.Failed;    // current was overloaded for a certain period of time
            else return TaskState.Passed;
        }
        /// <summary>
        /// Measure current on a particular channel. It is checked if current is not overloaded
        /// for certain period of time.
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="channel">Channel on which to measure the current</param>
        protected void measureCurrent(TimeSpan time, IAnalogInput channel)
        {
            // value of current measured on current channel
            double measuredCurrent = channel.RealValue;
            if (!isOverloaded && measuredCurrent > MaxCurrent)
            {   // current was not overloaded and started to be right now
                isOverloaded = true;
                overloaded = time;      // start to measure overload time
            }
            else if (isOverloaded && measuredCurrent < MaxCurrent)
            {   // current was overloaded and stopted to be right now
                isOverloaded = false;
                // time of current being overloaded
                int timeOverloaded = (int)(time - overloaded).TotalMilliseconds;

                if (timeOverloaded > maxMeasuredOverloadTime)   // save maximum value
                    maxMeasuredOverloadTime = timeOverloaded;
            }
        }

        #region Constructors

        public PeakTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MaxCurrent item
            DoubleParam dValue = testParam.GetParam<DoubleParam>(TestValue.MaxCurrent);
            if (dValue != null)     // it must be of type double
                MaxCurrent = dValue.DoubleValue;
            // from test parameters get MaxOverloadTime item
            IntParam iValue = testParam.GetParam<IntParam>(TestValue.MaxOverloadTime);
            if (iValue != null)     // param is of other type then double
                MaxOverloadTime = iValue.IntValue;
        }

        #endregion
    }
}
