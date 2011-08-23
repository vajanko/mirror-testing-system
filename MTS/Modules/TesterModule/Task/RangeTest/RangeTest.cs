using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
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
        /// (Get/Set) Mininal allowed current for this test
        /// </summary>
        protected double MinCurrent { get; private set; }
        /// <summary>
        /// (Get/Set) Maximal allowed current for this test
        /// </summary>
        protected double MaxCurrent { get; private set; }
        /// <summary>
        /// (Get/Set) Output channel which allows to control electric circuit. Set this channel in constructor
        /// </summary>
        protected IDigitalOutput ControlChannel { get; set; }
        /// <summary>
        /// (Get/Set) Input channel which contains current value. Set this channel in constructor
        /// </summary>
        protected IAnalogInput CurrentChannel { get; set; }

        #endregion

        public override void Initialize(TimeSpan time)
        {
            ControlChannel.Value = true;// start current

            // these values (for sure) will be changed
            maxMeasuredCurrent = double.MinValue;
            minMeasuredCurrent = double.MaxValue;

            base.Initialize(time);    // save begin time
        }
        public override void UpdateOutputs(TimeSpan time)
        {
            ControlChannel.Value = true;// start current
            base.UpdateOutputs(time);
        }
        public override void Update(TimeSpan time)
        {   // notice that this update implementation is running foreaver - override to finish it                        
            // value of current measured on current channel
            double measuredCurrent = CurrentChannel.RealValue;

            // save max a min measured values of current
            if (measuredCurrent > maxMeasuredCurrent)
                maxMeasuredCurrent = measuredCurrent;
            else if (measuredCurrent < minMeasuredCurrent)
                minMeasuredCurrent = measuredCurrent;

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {            
            ControlChannel.Value = false;   // stop current

            if (maxMeasuredCurrent > MaxCurrent)
                Output.WriteLine("Max current exceeded: {0} mA measured, {1} mA expected", maxMeasuredCurrent, MaxCurrent);
            if (minMeasuredCurrent < MinCurrent)
                Output.WriteLine("Min current exceeded: {0} mA measured, {1} mA expected", minMeasuredCurrent, MinCurrent);

            base.Finish(time, state);   // save end time
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
