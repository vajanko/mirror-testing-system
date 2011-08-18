using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    abstract class PeakTest : TestTask
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
        /// (Get/Set) Output channel which allows to control electric circuit. Set this channel in constructor
        /// </summary>
        protected IDigitalOutput ControlChannel { get; set; }
        /// <summary>
        /// (Get/Set) Input channel which contains current value. Set this channel in constructor
        /// </summary>
        protected IAnalogInput CurrentChannel { get; set; }

        #endregion

        public override void BeginExecute(TimeSpan time)
        {
            if (IsRunning) return;      // prevent to start again already strated task

            //ControlChannel.Value = true;// start current

            base.BeginExecute(time);    // save begin time
        }
        public override void Update(TimeSpan time)
        {
            if (!IsRunning) return;     // do not update if task is not running

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
        public override void EndExecute(TimeSpan time, TaskState state)
        {
            if (!IsRunning) return;         // prevent to stop again already stoped task

            //ControlChannel.Value = false;   // stop current

            if (maxMeasuredOverloadTime > MaxOverloadTime)
                Output.WriteLine("Max allowed current: {0} mA was exceeded for {1} ms, only {2} ms overload time is allowed",
                    MaxCurrent, maxMeasuredOverloadTime, MaxOverloadTime);

            base.EndExecute(time, state);   // save end time
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
