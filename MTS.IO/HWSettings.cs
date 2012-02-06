using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Media.Media3D;

namespace MTS.IO {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class HWSettings {

        public HWSettings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //

            this.SettingsLoaded += new SettingsLoadedEventHandler(HWSettings_SettingsLoaded);

            // recalculate positions of calibrators from current settings - distances between each calibrator
            calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        /// <summary>
        /// This method is called when <see cref="HWSettings"/> is loaded. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWSettings_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {   // recalculate positions of calibrators from current settings - distances between each calibrator
            calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }
        
        //private void HWSettingsChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
        //    // Add code to handle the SettingChangingEvent event here.
        //}
        
        //private void HWSettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
        //    // Add code to handle the SettingsSaving event here.
        //}

        #region Calibretors

        private Point3D calibretorX;
        public Point3D CalibretorX { get { return calibretorX; } }
        private Point3D calibretorY;
        public Point3D CalibretorY { get { return calibretorY; } }
        private Point3D calibretorZ;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // property that has been changed is one of calibrators distances
            if (e.PropertyName.EndsWith("Distance"))
                calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        #endregion
    }
}
