using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using MTS.IO.Settings;

namespace MTS.Properties
{
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class HWSettings {
        
        public HWSettings() {
            // To add event handlers for saving and changing settings, uncomment the lines below:

            this.SettingChanging += this.SettingChangingEventHandler;

            this.SettingsSaving += this.SettingsSavingEventHandler;

            this.SettingsLoaded += new System.Configuration.SettingsLoadedEventHandler(HWSettings_SettingsLoaded);

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

        #region XmlConstants

        private static readonly XName ChannelsElem = "channels";
        private static readonly XName ChannelElem = "channel";
        private static readonly XName IdAttr = "id";
        private static readonly XName NameElem = "name";
        private static readonly XName DescriptionElem = "description";
        private static readonly XName RawLowElem = "rawLow";
        private static readonly XName RawHighElem = "rawHigh";
        private static readonly XName RealLowElem = "realLow";
        private static readonly XName RealHighElem = "realHigh";

        #endregion

        /// <summary>
        /// Load configuration of all analog channels from configuration file saved in these settings.
        /// </summary>
        /// <param name="path">Absolute path to file where configuration settings for analog channels are
        /// saved</param>
        /// <returns>Collection of settings for analog channels loaded from a file</returns>
        public ChannelSettingsCollection LoadChannelSettings(string path)
        {
            // load channels configuration file to memory
            XElement root = XElement.Load(path);

            ChannelSettingsCollection settings = new ChannelSettingsCollection();

            try
            {
                foreach (XElement channel in root.Elements(ChannelElem))
                {
                    string id = channel.Attribute(IdAttr).Value;
                    ChannelSetting chs = new ChannelSetting(id);

                    chs.Name = channel.Element(NameElem).Value;
                    chs.Description = channel.Element(DescriptionElem).Value;
                    chs.RawLow = int.Parse(channel.Element(RawLowElem).Value);
                    chs.RawHigh = int.Parse(channel.Element(RawHighElem).Value);
                    chs.RealLow = double.Parse(channel.Element(RealLowElem).Value);
                    chs.RealHigh = double.Parse(channel.Element(RealHighElem).Value);

                    settings.AddSetting(chs);
                }
            }
            catch
            {
            }

            return settings;
        }

        public void SaveChannelSettings(ChannelSettingsCollection settings)
        {
            string path = Settings.Default.GetChannelsConfigPath();
            XElement root = new XElement(ChannelsElem);

            foreach (var item in settings)
            {
                XElement channel = new XElement(ChannelElem);
                channel.Add(new XAttribute(IdAttr, item.Id));
                channel.Add(new XElement(NameElem, item.Name));
                channel.Add(new XElement(DescriptionElem, item.Description));
                channel.Add(new XElement(RawLowElem, item.RawLow));
                channel.Add(new XElement(RawHighElem, item.RawHigh));
                channel.Add(new XElement(RealLowElem, item.RealLow));
                channel.Add(new XElement(RealHighElem, item.RealHigh));

                root.Add(channel);
            }
            root.Save(path);
        }
    }
}
