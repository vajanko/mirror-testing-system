using System;

using MTS.IO;
using MTS.Editor;
using MTS.Tester;
using MTS.Tester.Result;
using MTS.Data.Types;

namespace MTS.Tester
{
    public abstract class PeakTest : TestTask
    {
        #region Fields

        protected int maxMeasuredOverloadTime = 0;
        protected bool isOverloaded = false;
        protected DateTime overloaded;

        DoubleParam maxCurrent;
        DoubleParam maxOverloadTime;

        #endregion

        #region Properties

        protected double MaxCurrent { get; private set; }
        protected int MaxOverloadTime { get; private set; }

        #endregion

        /// <summary>
        /// Generate the result (final state) of this test after it has been finished
        /// </summary>
        /// <returns></returns>
        protected TaskResultType getTaskState()
        {
            if (exState == ExState.Aborting)
                return TaskResultType.Aborted;
            else if (maxMeasuredOverloadTime > MaxOverloadTime)
                return TaskResultType.Failed;    // current was overloaded for a certain period of time
            else
                return TaskResultType.Completed;
        }
        /// <summary>
        /// Measure current on a particular channel. It is checked if current is not overloaded
        /// for certain period of time.
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="channel">Channel on which to measure the current</param>
        protected void measureCurrent(DateTime time, IAnalogInput channel)
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

        protected override TaskResult getResult()
        {
            TaskResult result = base.getResult();

            result.Params.Add(new ParamResult(maxCurrent));
            result.Params.Add(new ParamResult(maxOverloadTime, maxMeasuredOverloadTime));

            return result;
        }

        #region Constructors

        public PeakTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MaxCurrent item and throw exception if it is not found
            maxCurrent = testParam.GetParam<DoubleParam>(TestValue.MaxCurrent);
            if (maxCurrent == null)
                throw new ParamNotFoundException(TestValue.MaxCurrent);
            // from test parameters get MaxOverloadTime item and throw exception if it is not found
            maxOverloadTime = testParam.GetParam<DoubleParam>(TestValue.MaxOverloadTime);
            if (maxOverloadTime == null)
                throw new ParamNotFoundException(TestValue.MaxOverloadTime);
        }

        #endregion
    }
}
