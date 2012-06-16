using MTS.IO;

namespace MTS.IO.Channel
{
    public class AnalogOutput<TAddress> : AnalogInput<TAddress>, IAnalogOutput where TAddress : IAddress
    {
        #region IAnalogOutput Members

        /// <summary>
        /// (Get/Set) Integer value of this channel. Setting this value effects <see cref="RealValue"/>
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
