using System;
using System.Collections.Generic;

using MTS.AdminModule;

namespace MTS.TesterModule
{
    class Wait : Task
    {
        private int miliseconds;

        public override void Initialize(TimeSpan time)
        {
            Output.WriteLine("Waiting for {0} ms", miliseconds);
            base.Initialize(time);
        }
        public override void Update(TimeSpan time)
        {
            EndTime = time;
            if (Duration.TotalMilliseconds > miliseconds)
                Finish(time, TaskState.Completed);
            base.Update(time);
        }
        public override void Finish(TimeSpan time, TaskState state)
        {
            Output.WriteLine("Waiting finished!");
            base.Finish(time, state);
        }

        #region Constructors

        public Wait(Channels channels, int miliseconds)
            : base(channels) 
        {
            this.miliseconds = miliseconds;
        }

        #endregion
    }
}
