using System;
//using System.Collections.Generic;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.IO.Settings;
using MTS.Properties;

namespace MTS.TesterModule
{
    class Calibrate : Task
    {
        /// <summary>
        /// Read distances, caluculate zero plane normal and save this setting
        /// </summary>
        /// <param name="time"></param>
        public override void Update(TimeSpan time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    StartWatch(time);
                    exState = ExState.Measuring;
                    break;
                case ExState.Measuring:
                    HWSettings.Default.ZeroPlaneNormal = channels.GetMirrorNormal();                    
                    if (TimeElapsed(time) > 1000)
                        exState = ExState.Finalizing;
                    break;
                case ExState.Finalizing:
                    HWSettings.Default.Save();
                    HWSettings.Default.Reload();
                    Finish(time, TaskState.Completed);
                    break;
                case ExState.Aborting:
                    Finish(time, TaskState.Aborted);
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
