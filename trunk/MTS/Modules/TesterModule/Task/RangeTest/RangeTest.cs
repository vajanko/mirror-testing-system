using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
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

        #endregion

        #region Properties

        /// <summary>
        /// (Get) Mininal allowed current for this test
        /// </summary>
        protected double MinCurrent { get; private set; }
        /// <summary>
        /// (Get) Maximal allowed current for this test
        /// </summary>
        protected double MaxCurrent { get; private set; }

        #endregion

        /// <summary>
        /// Check if current is inside required range
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="channel">Channel to measure current on</param>
        protected void measureCurrent(TimeSpan time, IAnalogInput channel)
        {
            // value of current measured on current channel
            double measuredCurrent = channel.RealValue;

            // save max a min measured values of current
            if (measuredCurrent > maxMeasuredCurrent)
                maxMeasuredCurrent = measuredCurrent;
            else if (measuredCurrent < minMeasuredCurrent)
                minMeasuredCurrent = measuredCurrent;
        }
        /// <summary>
        /// Get the final state of this task: Passed if everythig is OK, Failed otherwise
        /// </summary>
        protected TaskState getTaskState()
        {
            if (maxMeasuredCurrent > MaxCurrent ||
                minMeasuredCurrent < MinCurrent)
                return TaskState.Failed;
            else return TaskState.Passed;
        }        

        #region Constructors

        public RangeCurrentTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            ParamCollection param = testParam.Parameters;
            DoubleParamValue dValue;
            // from test parameters get MIN_CURRENT item
            if (param.ContainsKey(ParamDictionary.MIN_CURRENT))
            {   // it must be double type value
                dValue = param[ParamDictionary.MIN_CURRENT] as DoubleParamValue;
                if (dValue != null)     // param is of other type then double
                    MinCurrent = dValue.Value;
            }
            // from test parameters get MAX_CURRENT item
            if (param.ContainsKey(ParamDictionary.MAX_CURRENT))
            {   // it must be double type value
                dValue = param[ParamDictionary.MAX_CURRENT] as DoubleParamValue;
                if (dValue != null)     // param is of other type then double
                    MaxCurrent = dValue.Value;
            }            
        }

        #endregion
    }
}
