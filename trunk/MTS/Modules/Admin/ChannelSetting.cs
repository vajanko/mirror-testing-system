using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Data;
using System.Xml.Serialization;

namespace MTS.Admin
{
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    [XmlRoot("Channel")]
    public class ChannelSetting: IValueConverter
    {
        [XmlAttribute("Id")]
        public string Id { get; private set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int RawLow { get; set; }
        public int RawHigh { get; set; }
        public double RealLow { get; set; }
        public double RealHigh { get; set; }

        #region Constructors

        public ChannelSetting() { }
        public ChannelSetting(string id)
        {
            this.Id = id;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    [XmlRoot("Channels")]
    public class ChannelSettingsCollection : List<ChannelSetting>
    {
        public ChannelSettingsCollection() { }
    }
}

