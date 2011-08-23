using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public abstract class PresenceTest : TestTask
    {
        protected bool shouldBePresent;
        protected IDigitalInput PresenceChannel;

        protected bool isPresent;

        public override void Update(TimeSpan time)
        {
            isPresent = PresenceChannel.Value;
            Finish(time, TaskState.Completed);

            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            string msg;
            if (isPresent) msg = "Is present ";
            else msg = "Is not present ";
            if (shouldBePresent) msg += "and should be installed";
            else msg += "and should not be installed";
            Output.WriteLine("{0}: {1}, Time: {2}, Duration: {3}", Name, msg, time, Duration);
            
            base.Finish(time, state);
        }

        #region Constructors

        public PresenceTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {            
        }

        #endregion
    }
}
