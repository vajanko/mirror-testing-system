using MTS.IO;

namespace MTS.IO.Channel
{
    class AnalogOutput : AnalogInput, IAnalogOutput
    {
        #region IAnalogOutput Members

        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value efects <see cref="RealValue"/>
        /// Minimum possible value is <see cref="RawLow"/>. Maximum possible value is <see cref="RawHigh"/>
        /// </summary>
        public new uint Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }

        #endregion
    }
}
