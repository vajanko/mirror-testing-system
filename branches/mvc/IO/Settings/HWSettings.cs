using System;
//using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace MTS.IO.Settings
{       
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class HWSettings {
        
        public HWSettings() {
            // To add event handlers for saving and changing settings, uncomment the lines below:

            this.SettingChanging += this.SettingChangingEventHandler;

            this.SettingsSaving += this.SettingsSavingEventHandler;

            //this.SettingsLoaded += new System.Configuration.SettingsLoadedEventHandler(HWSettings_SettingsLoaded);

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(HWSettings_PropertyChanged);            
        }

        void HWSettings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            // recalculate Calibretors positions
            throw new System.NotImplementedException("recalculate Calibretors positions");
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.                
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }

        #region Calibretors

        private Point3D calibretorX;
        public Point3D CalibretorX { get { return calibretorX; } }
        private Point3D calibretorY;
        public Point3D CalibretorY { get { return calibretorY; } }
        private Point3D calibretorZ;
        public Point3D CalibretorZ { get { return calibretorZ; } }

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

        void HWSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "XYDistance" || e.PropertyName == "YZDistance"
                || e.PropertyName == "XZDistance")
                calculateCalibratorsPositions(XYDistance, YZDistance, XZDistance);
        }

        #endregion
    }
}
