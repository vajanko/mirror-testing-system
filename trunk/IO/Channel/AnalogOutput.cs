namespace MTS.IO.Channel
{
    class AnalogOutput : AnalogInput, IAnalogOutput
    {
        #region IAnalogOutput Members

        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value efects <paramref name="RealValue"/>
        /// Minimum possible value is <paramref name="RawLow"/>. Maximum possible value is <paramref name="RawHigh"/>
        /// </summary>
        public new uint Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }

        #endregion
    }
}
