namespace MTS.IO.Channel
{
    class DigitalOutput : DigitalInput, IDigitalOutput
    {
        #region IDigitalOutput Members

        /// <summary>
        /// (Get/Set) Logical value of this channel.
        /// </summary>
        public new bool Value
        {
            get { return base.Value; }
            set { this.value = value; }
        }
        /// <summary>
        /// Set logical value of this channel to true. Setting value does not raise an event
        /// </summary>
        public void On()
        {
            Value = true;
        }
        /// <summary>
        /// Set logical value of this channel to false. Setting value does not raise an event
        /// </summary>
        public void SwitchOff()
        {
            Value = false;
        }

        #endregion
    }
}
