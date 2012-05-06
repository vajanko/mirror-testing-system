using System;

using MTS.IO;
using MTS.Editor;
using MTS.Data.Types;

namespace MTS.Tester
{
    sealed class PresenceTest : TestTask
    {
        /// <summary>
        /// Channel on which presence of particular mirror component is signalized
        /// </summary>
        private readonly IDigitalInput presenceChannel;

        /// <summary>
        /// Value indicating if mirror component should be present
        /// </summary>
        private readonly BoolParam presenceParam;

        /// <summary>
        /// Check for presence of particular mirror component
        /// </summary>
        /// <param name="time">Time of calling this method</param>
        public override void Update(DateTime time)
        {
            if (presenceChannel.Value == presenceParam.BoolValue)
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
            presenceChannel = channel;

            // from test parameters get TestPresence parameter
            presenceParam = testParam.GetParam<BoolParam>(ParamIds.TestPresence);
        }

        #endregion
    }
}
