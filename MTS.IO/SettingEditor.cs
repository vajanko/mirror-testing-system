using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Design;
using System.Collections;
using System.Collections.Generic;

namespace MTS.IO.Properties
{
    public class ChannelCollectionEditor : CollectionEditor
    {
        public ChannelCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override string GetDisplayText(object value)
        {
            ChannelSetting item = null;
            item = value as ChannelSetting;

            if (item != null)
                return base.GetDisplayText(string.Format("{0}: {1}-{2} {3}-{4}", item.Name,
                    item.RawLow, item.RawHigh, item.RealLow, item.RealHigh));
            else return "not channel setting";
        }
    }
}
