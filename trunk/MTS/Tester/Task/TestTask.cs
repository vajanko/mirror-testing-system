using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Base;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    /// <summary>
    /// Base class for all testing tasks. This task must have a result from its execution
    /// </summary>
    public abstract class TestTask : Task
    {
        /// <summary>
        /// Collection of channels from communication with remote hardware. This collection is regularly
        /// updated in a loop. New values are wrote to remote hardware memory and values from hardware
        /// are wrote to this collection
        /// </summary>
        protected Channels channels;
        /// <summary>
        /// Parameter necessary for execution of this task. Include information such as
        /// maximum duration of task or allowed range of current etc.
        /// </summary>
        protected TestValue testParam;

        /// <summary>
        /// Generate object holding result data for this task such as time of execution and results of 
        /// used parameters.
        /// </summary>
        /// <returns>Object describing all results of this task</returns>
        protected override TaskResult getResult()
        {
            return new TaskResult(testParam)
            {
                Begin = Begin,
                End = End,
                ResultCode = getResultCode(),   // this method may be overridden and return result depending on test
                HasData = Enabled               // value indicating that this result has data to be saved to database
            };
        }

        /// <summary>
        /// Validate given value. This value will saved as a string to database later parsed back to double.
        /// When parsing a value it can not be <see cref="double.MinValue"/> or <see cref="double.MaxValue"/>.
        /// If so given value will be changed by <see cref="double.Epsilon"/>
        /// </summary>
        /// <param name="value">Value to validate if it can be saved to database. If not value is changed
        /// by <see cref="double.Epsilon"/></param>
        protected static void validate(ref double value)
        {
            if (value == double.MinValue)
                value += double.Epsilon;
            else if (value == double.MaxValue)
                value -= double.Epsilon;
        }

        /// <summary>
        /// Convert value of given parameter to given unit
        /// </summary>
        /// <param name="param">Parameter which value want to convert</param>
        /// <param name="unit">Unit to convert value of given parameter to</param>
        /// <returns>Value converted to given unit</returns>
        protected static double convert(DoubleParam param, Unit unit)
        {
            return param.Unit.ConvertTo(unit, param.NumericValue);
        }
        /// <summary>
        /// Convert output value back to its parameter unit
        /// </summary>
        /// <param name="param">Parameter for which output value was measured</param>
        /// <param name="unit">Unit in which output value was measured</param>
        /// <param name="value">Measured value</param>
        /// <returns>Output value in given parameter unit</returns>
        protected static double convertBack(DoubleParam param, Unit unit, double value)
        {
            return unit.ConvertTo(param.Unit, value);
        }

        #region Constructors

        public TestTask(Channels channels, TestValue testParam)
        {
            this.channels = channels;
            this.testParam = testParam;
            Name = testParam.Name;
            Id = testParam.ValueId;
            Enabled = testParam.Enabled;
        }

        #endregion
    }
}
