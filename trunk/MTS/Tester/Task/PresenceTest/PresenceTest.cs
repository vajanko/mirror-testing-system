using System;

using MTS.IO;
using MTS.Editor;
using MTS.Data.Types;

namespace MTS.Tester
{
    public class PresenceTest : TestTask
    {
        /// <summary>
        /// Channel on which presence of particular mirror component is signalized
        /// </summary>
        protected IDigitalInput PresenceChannel;
        /// <summary>
        /// Value indicating if mirror component should be present
        /// </summary>
        private bool shouldBePresent;

        /// <summary>
        /// Check for presence of particular mirror component
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(DateTime time)
        {
            if (PresenceChannel.Value == shouldBePresent)
                resultCode = TaskResultType.Completed;
            else
                resultCode = TaskResultType.Failed;

            Finish(time);
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of test that will check for presence of particular mirror component
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="testParam"></param>
        /// <param name="channel">Channel on which the presence is signalized</param>
        public PresenceTest(Channels channels, TestValue testParam, IDigitalInput channel)
            : base(channels, testParam)
        {
            PresenceChannel = channel;

            // from test parameters get TestPresence parameter
            BoolParam bValue = testParam.GetParam<BoolParam>(TestValue.TestPresence);
            if (bValue != null)     // it must be of type Boolean
                shouldBePresent = bValue.BoolValue;
        }

        #endregion
    }
}
