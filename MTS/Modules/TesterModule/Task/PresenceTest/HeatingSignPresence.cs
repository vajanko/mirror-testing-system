using System;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public sealed class HeatingSignPresence : PresenceTest
    {
        #region Constructors

        public HeatingSignPresence(Channels channels, TestValue testParam)
            : base(channels, testParam)
        {
            PresenceChannel = channels.HeatingFoilSignSensor;
        }

        #endregion
    }
}
