using System;

using MTS.IO;
using MTS.Base;
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
        /// Minimal value of current that has been measured during this task. This value should be initialized 
        /// when test is being executed.
        /// </summary>
        protected double minCurrentMeasured;
        /// <summary>
        /// Maximal value of current that has been measured during this task. This value should be initialized 
        /// when test is being executed.
        /// </summary>
        protected double maxCurrentMeasured;

        /// <summary>
        /// Minimal allowed current
        /// </summary>
        private readonly DoubleParam minCurrentParam;
        /// <summary>
        /// Maximal allowed current
        /// </summary>
        private readonly DoubleParam maxCurrentParam;
        /// <summary>
        /// Minimal allowed current in miliampheres
        /// </summary>
        private readonly double minCurrent;
        /// <summary>
        /// Maximal allowed current in miliampheres
        /// </summary>
        private readonly double maxCurrent;

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
            if (measuredCurrent > maxCurrentMeasured)
                maxCurrentMeasured = measuredCurrent;
            if (measuredCurrent < minCurrentMeasured)
                minCurrentMeasured = measuredCurrent;
        }
        /// <summary>
        /// Get the final state of this task: Completed if everything is OK, Failed otherwise
        /// </summary>
        protected override TaskResultType getResultCode()
        {
            if (exState == ExState.Aborting)    // execution state is aborting - so result is aborted
                return TaskResultType.Aborted;
            else if (maxCurrentMeasured > maxCurrent || minCurrentMeasured < minCurrent)
                return TaskResultType.Failed;
            else
                return TaskResultType.Completed;
        }
        /// <summary>
        /// Generate object holding result data for this task such as time of execution and results of 
        /// used parameters.
        /// </summary>
        /// <returns>Object describing all results of this task</returns>
        protected override TestResult getTestResult()
        {   // only add parameters to already created test result
            TestResult result = base.getTestResult();

            // correct values if they are to small or too big - if some of these values is double.MaxValue
            // or double.MinValue it means that testing time has been set to 0
            // max and min value can not be parsed from string
            validate(ref minCurrentMeasured);
            validate(ref maxCurrentMeasured);

            // we have been measuring current in miliampheres, now convert it back to parameter unit
            // in this state will be saved to database
            double min = convertBack(minCurrentParam, Units.Miliampheres, minCurrentMeasured);
            result.Params.Add(new ParamResult(minCurrentParam, min));
            double max = convertBack(maxCurrentParam, Units.Miliampheres, maxCurrentMeasured);
            result.Params.Add(new ParamResult(maxCurrentParam, max));

            return result;
        }

        #region Constructors

        public RangeCurrentTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MinCurrent parameter
            minCurrentParam = testParam.GetParam<DoubleParam>(ParamIds.MinCurrent);
            // from test parameters get MaxCurrent parameter
            maxCurrentParam = testParam.GetParam<DoubleParam>(ParamIds.MaxCurrent);

            // for measuring current only use miliampheres
            maxCurrent = convert(maxCurrentParam, Units.Miliampheres);
            minCurrent = convert(minCurrentParam, Units.Miliampheres);
        }

        #endregion
    }
}
