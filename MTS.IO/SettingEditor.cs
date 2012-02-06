using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Design;
using System.Collections;
using System.Collections.Generic;

namespace MTS.IO.Properties
{
    /// <summary>
    /// This class is used by Visual Studio when editing application settings of analog channels through user interface
    /// </summary>
    public class ChannelCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Get text that will be displayed for each channel in a collection editor in Visual Studio
        /// </summary>
        /// <param name="value">Instance of channel settings to display</param>
        /// <returns>String describing settings of a particular channel</returns>
        protected override string GetDisplayText(object value)
        {
            ChannelSetting item = value as ChannelSetting;

            if (item != null)
            {   // there may be added something in the base class
                return base.GetDisplayText(string.Format("{0}: {1}-{2} {3}-{4}", item.Name,
                    item.RawLow, item.RawHigh, item.RealLow, item.RealHigh));
            }
            else
            {   // type of this settings is not recognized
                return "not channel setting";
            }
        }

        #region Constructors

        /// <summary>
        /// Create a new instance of collection editor for analog channel settings. This allows you to
        /// edit application settings of channels through graphical user interface in Visual Studio
        /// </summary>
        /// <param name="type">Type of class holding channels settings</param>
        public ChannelCollectionEditor(Type type)
            : base(type)
        {
        }

        #endregion
    }
}
