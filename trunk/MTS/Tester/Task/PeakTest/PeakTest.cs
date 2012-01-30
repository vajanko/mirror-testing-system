using System;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester;
using MTS.Tester.Result;
using MTS.Data.Types;

namespace MTS.Tester
{
    public abstract class PeakTest : TestTask
    {
        #region Fields

        /// <summary>
        /// Currently max value of time when current was overloaded while executing this test.
        /// This value should be initialized when test is begging executed.
        /// </summary>
        protected double maxMeasuredOverloadTime;
        /// <summary>
        /// Value indicating whether current is overloaded. This value should be initialized when test is 
        /// being executed.
        /// </summary>
        protected bool isOverloaded;
        /// <summary>
        /// Time when current was overloaded first time. This value should not be initialized, it will be set
        /// when first time overloaded
        /// </summary>
        private DateTime overloaded;

        /// <summary>
        /// Maximal allowed time of current overloaded in milliamperes
        /// </summary>
        private readonly double maxOverloadTime;
        /// <summary>
        /// Maximal allowed current in milliamperes
        /// </summary>
        private readonly double maxCurrent;

        /// <summary>
        /// Maximal allowed current
        /// </summary>
        private readonly DoubleParam maxCurrentParam;
        /// <summary>
        /// Maximal allowed time of current overloaded
        /// </summary>
        private readonly DoubleParam maxOverloadTimeParam;

        #endregion

        #region Methods

        /// <summary>
        /// Measure current on a particular channel. It is checked if current is not overloaded
        /// for certain period of time.
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        /// <param name="channel">Channel on which to measure the current</param>
        protected void measureCurrent(DateTime time, IAnalogInput channel)
        {
            // value of current measured on current channel (in miliampheres)
            double measuredCurrent = channel.RealValue;
            double timeOverloaded = 0;

            if (!isOverloaded && measuredCurrent > maxCurrent)
            {   // current was not overloaded and started to be right now
                isOverloaded = true;
                overloaded = time;      // start to measure overload time
            }
            else if (isOverloaded && measuredCurrent < maxCurrent)
            {   // current was overloaded and stopted to be right now
                isOverloaded = false;
            }

            if (isOverloaded)
            {
                // time of current being overloaded
                timeOverloaded = (time - overloaded).TotalMilliseconds;
                // always when current is overloaded save max value of time in overloaded state - this will
                // handle also such a situation when time is overloaded up to the end of this test
                if (timeOverloaded > maxMeasuredOverloadTime)   // save maximum value
                    maxMeasuredOverloadTime = timeOverloaded;
            }
        }

        /// <summary>
        /// Generate object holding result data for this task such as time of execution and results of 
        /// used parameters.
        /// </summary>
        /// <returns>Object describing all results of this task</returns>
        protected override TaskResult getResult()
        {   // these are common value such as duration and result code
            TaskResult result = base.getResult();

            // prepare value for saveing to database
            validate(ref maxMeasuredOverloadTime);

            // maxCurrent is used value but has no result
            result.Params.Add(new ParamResult(maxCurrentParam));

            // we have been measuring time in miliseconds, now convert it back to parameter unit
            // in this state will be saved to database
            double overload = convertBack(maxOverloadTimeParam, Units.Miliseconds, maxMeasuredOverloadTime);
            result.Params.Add(new ParamResult(maxOverloadTimeParam, overload));

            return result;
        }
        /// <summary>
        /// Generate the result of this test after it has been finished. Will be <see cref="TaskResultType.Aborted"/>
        /// if excuting state (<see cref="exState"/>) was <see cref="ExState.Aborting"/>, if 
        /// <see cref="maxMeasuredOverloadTime"/> was greater than max allowed current (<see cref="maxOverloadTime"/>)
        /// will be failed. This method is called always only from <see cref="getResult"/>.
        /// </summary>
        protected override TaskResultType getResultCode()
        {
            if (exState == ExState.Aborting)
                return TaskResultType.Aborted;
            else if (maxMeasuredOverloadTime > maxOverloadTime)
                return TaskResultType.Failed;    // current was overloaded for a certain period of time
            else
                return TaskResultType.Completed;
        }

        #endregion

        #region Constructors

        public PeakTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            // from test parameters get MaxCurrent item and throw exception if it is not found
            maxCurrentParam = testParam.GetParam<DoubleParam>(TestValue.MaxCurrent);
            // from test parameters get MaxOverloadTime item and throw exception if it is not found
            maxOverloadTimeParam = testParam.GetParam<DoubleParam>(TestValue.MaxOverloadTime);

            // for measuring current only use miliampheres
            maxCurrent = convert(maxCurrentParam, Units.Miliampheres);
            // for measuring time we only use miliseconds
            maxOverloadTime = convert(maxOverloadTimeParam, Units.Miliseconds);            
        }

        #endregion
    }
}
