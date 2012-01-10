using System;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;
using MTS.Data.Types;

namespace MTS.Tester
{
    /// <summary>
    /// Base class for test that check for current in some range
    /// </summary>
    public abstract class RangeCurrentTest : TestTask
    {
        #region Fields

        /// <summary>
        /// Minimal value of current that has been measured during this task
        /// </summary>
        protected double minMeasuredCurrent;
        /// <summary>
        /// Maximal value of current that has been measured during this task
        /// </summary>
        protected double maxMeasuredCurrent;
        /// <summary>
        /// Mininal allowed current for this test
        /// </summary>
        protected DoubleParam minCurrent;
        /// <summary>
        /// Maximal allowed current for this test
        /// </summary>
        protected DoubleParam maxCurrent;

        #endregion

        #region Properties

        /// <summary>
        /// (Get) Mininal allowed current for this test
        /// </summary>
        protected double MinCurrent { get { return minCurrent.DoubleValue; } }
        /// <summary>
        /// (Get) Maximal allowed current for this test
        /// </summary>
        protected double MaxCurrent { get { return maxCurrent.DoubleValue; } }

        #endregion

        /// <summary>
        /// Check if current is inside required range
        /// </summary>
        /// <param name="channel">Channel to measure current on</param>
        protected void measureCurrent(IAnalogInput channel)
        {
            // value of current measured on current channel
            double measuredCurrent = channel.RealValue;

            // save max a min measured values of current
            if (measuredCurrent > maxMeasuredCurrent)
                maxMeasuredCurrent = measuredCurrent;
            if (measuredCurrent < minMeasuredCurrent)
                minMeasuredCurrent = measuredCurrent;
        }
        /// <summary>
        /// Get the final state of this task: Completed if everythig is OK, Failed otherwise
        /// </summary>
        protected override TaskResultType getResultCode()
        {
            if (exState == ExState.Aborting)    // execution state is aborting - so result is aborted
                return TaskResultType.Aborted;
            else if (maxMeasuredCurrent > MaxCurrent || minMeasuredCurrent < MinCurrent)
                return TaskResultType.Failed;
            else
                return TaskResultType.Completed;
        }
        protected override TaskResult getResult()
        {
            // only add parameters to already creatred test result
            TaskResult result = base.getResult();

            result.Params.Add(new ParamResult(minCurrent, minMeasuredCurrent));
            result.Params.Add(new ParamResult(maxCurrent, maxMeasuredCurrent));

            return result;
        }

        #region Constructors

        public RangeCurrentTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MinCurrent parameter
            minCurrent = testParam.GetParam<DoubleParam>(TestValue.MinCurrent);
            if (minCurrent == null)
                throw new ParamNotFoundException(TestValue.MinCurrent);
            // from test parameters get MaxCurrent parameter
            maxCurrent = testParam.GetParam<DoubleParam>(TestValue.MaxCurrent);
            if (maxCurrent == null)
                throw new ParamNotFoundException(TestValue.MaxCurrent);
        }

        #endregion
    }
}
