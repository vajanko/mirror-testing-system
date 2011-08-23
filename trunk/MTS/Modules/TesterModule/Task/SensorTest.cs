using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    class SensorTest : TestTask
    {
        protected List<bool> ExpectedValues { get; private set; }

        protected List<IDigitalInput> SensorChannels { get; private set; }

        public override void Update(TimeSpan time)
        {
            if (!IsRunning) return;    // do not update if task is not running

            // check if correct

            base.Update(time);
        }

        #region Constructors

        public SensorTest(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            ExpectedValues = new List<bool>();
            SensorChannels = new List<IDigitalInput>();
        }

        #endregion
    }
}
