using System;

using MTS.AdminModule;
using MTS.EditorModule;

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

        /// <summary>
        /// (Get/Set) Input channel which contains current value. Set this channel in constructor
        /// </summary>
        protected IAnalogInput CurrentChannel { get; set; }

        #endregion

        protected TaskState getTaskState()
        {
            if (maxMeasuredOverloadTime > MaxOverloadTime)
                return TaskState.Failed;
            else return TaskState.Passed;
        }
        protected void measureCurrent(TimeSpan time)
        {
            // value of current measured on current channel
            double measuredCurrent = CurrentChannel.RealValue;
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

        public override void Update(TimeSpan time)
        {
            // value of current measured on current channel
            double measuredCurrent = CurrentChannel.RealValue;
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

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            if (maxMeasuredOverloadTime > MaxOverloadTime)
                Output.WriteLine("Max allowed current: {0} mA was exceeded for {1} ms, only {2} ms overload time is allowed",
                    MaxCurrent, maxMeasuredOverloadTime, MaxOverloadTime);

            base.Finish(time, state);   // save end time
        }

        #region Constructors

        public PeakTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            ParamCollection param = testParam.Parameters;
            DoubleParamValue dValue;
            // from test parameters get MAX_CURRENT item
            if (param.ContainsKey(ParamDictionary.MAX_CURRENT))
            {   // it must be double type value
                dValue = param[ParamDictionary.MAX_CURRENT] as DoubleParamValue;
                if (dValue != null)     // param is of other type then double
                    MaxCurrent = dValue.Value;
            }
            IntParamValue iValue;
            // from test parameters get MAX_OVERLOAD_TIME item
            if (param.ContainsKey(ParamDictionary.MAX_OVERLOAD_TIME))
            {   // it must be double type value
                iValue = param[ParamDictionary.MAX_OVERLOAD_TIME] as IntParamValue;
                if (iValue != null)     // param is of other type then double
                    MaxOverloadTime = iValue.Value;
            }
        }

        #endregion
    }
}
