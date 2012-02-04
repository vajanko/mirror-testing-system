using System;
//using System.Collections.Generic;
using System.Windows.Media.Media3D;

using MTS.IO;
using MTS.Base;
using MTS.Properties;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    sealed class Calibrate : Task
    {
        private Vector3D mirrorNormal;

        private Channels channels;

        /// <summary>
        /// Read distances, calculate zero plane normal and save this setting
        /// </summary>
        /// <param name="time"></param>
        public override void Update(DateTime time)
        {
            switch (exState)
            {
                case ExState.Initializing:
                    StartWatch(time);
                    goTo(ExState.Measuring);
                    Output.Write("Calibrating ...");
                    break;
                case ExState.Measuring:
                    mirrorNormal = channels.GetMirrorNormal();
                    if (TimeElapsed(time) > 1000)
                        goTo(ExState.Finalizing);
                    break;
                case ExState.Finalizing:
                    Output.WriteLine("Finished");
                    Finish(time);
                    break;
                case ExState.Aborting:
                    Finish(time);
                    break;
            }
        }
        protected override TaskResult getResult()
        {
            TaskResult res = new TaskResult(new TestValue("Calibration") { Name = "Calibration" })
            {
                ResultCode = getResultCode(),
                Begin = this.Begin,
                End = this.End,
                HasData = false
            };
            
            // as result return measured calibration values - distances of measuring calibrators
            res.Params.Add(new ParamResult(new DoubleParam("DistanceX"), mirrorNormal.X));
            res.Params.Add(new ParamResult(new DoubleParam("DistanceY"), mirrorNormal.Y));
            res.Params.Add(new ParamResult(new DoubleParam("DistanceZ"), mirrorNormal.Z));

            return res;
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of task that will read distance values of mirror distance sensors
        /// and save it to application settings
        /// </summary>
        /// <param name="channels"></param>
        public Calibrate(Channels channels)
        {
            this.channels = channels;
        }

        #endregion
    }
}
