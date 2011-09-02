using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

using MTS.AdminModule;
using MTS.Properties;

namespace MTS.TesterModule
{
    class Calibrate : Task
    {
        public override void Update(TimeSpan time)
        {
            base.Update(time);
            // read positions of distance sonds from configuration file
            Point3D x = HWSettings.Default.SondeXPosition;
            Point3D y = HWSettings.Default.SondeYPosition;
            Point3D z = HWSettings.Default.SondeZPosition;

            // read distance values
            x.Z = channels.DistanceX.RealValue;
            y.Z = channels.DistanceY.RealValue;
            z.Z = channels.DistanceZ.RealValue;

            // calculate perpendicular vector to two vectors made from three points
            HWSettings.Default.ZeroPlaneNormal = Vector3D.CrossProduct(new Vector3D(y.X - x.X, y.Y - x.Y, y.Z - x.Z), 
                new Vector3D(z.X - x.X, z.Y - x.Y, z.Z - x.Z));
            // save settings
            HWSettings.Default.Save();
            HWSettings.Default.Reload();

            Output.WriteLine("{0}: ZeroPlaneNormal: {1}, PointX: {2}, PointY: {3}, PointZ: {4}", Name,
                HWSettings.Default.ZeroPlaneNormal, x, y, z);

            Finish(time, TaskState.Completed);
        }


        #region Constructors

        public Calibrate(Channels channels)
            : base(channels) 
        {
        }

        #endregion
    }
}
