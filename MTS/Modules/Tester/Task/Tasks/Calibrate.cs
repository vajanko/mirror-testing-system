using System;
//using System.Collections.Generic;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Properties;
using MTS.Tester.Result;

namespace MTS.TesterModule
{
    class Calibrate : Task
    {
        /// <summary>
        /// Read distances, caluculate zero plane normal and save this setting
        /// </summary>
        /// <param name="time"></param>
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    StartWatch(time);
                    goTo(ExState.Measuring);
                    break;
                case ExState.Measuring:
                    HWSettings.Default.ZeroPlaneNormal = channels.GetMirrorNormal();
                    if (TimeElapsed(time) > 1000)
                        goTo(ExState.Finalizing);
                    break;
                case ExState.Finalizing:
                    HWSettings.Default.Save();
                    HWSettings.Default.Reload();
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }
        }


        #region Constructors

        /// <summary>
        /// Create a new instace of task that will read distance values of mirror distance sensors
        /// and save it to application settings
        /// </summary>
        /// <param name="channels"></param>
        public Calibrate(Channels channels)
            : base(channels) { }

        #endregion
    }
}
