using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Media.Media3D;
using MTS.IO.Protocol;

namespace MTS.IO 
{
    /// <summary>
    /// Singleton settings class for tester hardware. Contains configuration of remote terminal channels
    /// and positions of calibrators. Any other setting associated with tester hardware add here. 
    /// </summary>
    public sealed partial class HWSettings 
    {
        /// <summary>
        /// This method is called when <see cref="HWSettings"/> is loaded. 
        /// </summary>
        /// <param name="sender">Instance of singleton <see cref="HWSettings"/> class.</param>
        /// <param name="e">Settings loaded event argument</param>
        private void HWSettings_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {   // recalculate positions of calibrators from current settings - distances between each calibrator
            calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        #region Calibretors

        private Point3D calibretorX;
        /// <summary>
        /// (Get) Position of calibrator X in 3D space
        /// </summary>
        public Point3D CalibretorX { get { return calibretorX; } }
        private Point3D calibretorY;
        /// <summary>
        /// (Get) Position of calibrator Y in 3D space
        /// </summary>
        public Point3D CalibretorY { get { return calibretorY; } }
        private Point3D calibretorZ;
        /// <summary>
        /// (Get) Position of calibrator Z in 3D space
        /// </summary>
        public Point3D CalibretorZ { get { return calibretorZ; } }

        /// <summary>
        /// Recalculate position of calibrators in 2D space from given distances between each of them.
        /// Notice that X calibrator position is always [0,0] and x-coordinate of Y calibrator is always 0
        /// </summary>
        /// <param name="xy">Distance between X and Y calibrator</param>
        /// <param name="yz">Distance between Y and Z calibrator</param>
        /// <param name="xz">Distance between X and Z calibrator</param>
        private void calculateCalibratorsPositions(double xy, double yz, double xz)
        {
            double cosRes = ((xy * xy) + (xz * xz) - (yz * yz)) / (2 * xy * xz);
            double beta = Math.Acos(cosRes);

            calibretorX.X = 0;
            calibretorX.Y = 0;

            calibretorY.X = 0;
            calibretorY.Y = xy;

            calibretorZ.Y = Math.Cos(beta) * xz;
            calibretorZ.X = Math.Sin(beta) * xz;
        }

        /// <summary>
        /// This method is called when any of <see cref="HWSettings"/> property change. If this property is one
        /// of calibrators distances, positions of calibrators are recalculated
        /// </summary>
        /// <param name="sender">Instance of object which property has changed</param>
        /// <param name="e">Property change event argument</param>
        private void HWSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // property that has been changed is one of calibrators distances
            if (e.PropertyName.EndsWith("Distance"))
                calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of setting class dedicated for handling hardware settings
        /// </summary>
        public HWSettings()
        {
            this.SettingsLoaded += new SettingsLoadedEventHandler(HWSettings_SettingsLoaded);

            // recalculate positions of calibrators from current settings - distances between each calibrator
            calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        #endregion
    }
}
