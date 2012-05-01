using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MTS.Editor
{
    /// <summary>
    /// Converts parameter value to string representation.
    /// </summary>
    public class ToStringVisitor : IValueVisitor
    {
        private string result;

        public string ConvertToString(ValueBase param)
        {
            param.Accept(this);
            return result;
        }

        #region IParamVisitor Members

        private void defaultConvert(object value)
        {
            if (value == null)
            {
                result = null;
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            }
        }

        public void Visit(BoolParam param)
        {
            defaultConvert(param.Value);
        }

        public void Visit(EnumParam param)
        {
            defaultConvert(param.Value);
            //result = string.Format(CultureInfo.InvariantCulture, "{0}", param.SelectedIndex);
        }

        public void Visit(StringParam param)
        {
            defaultConvert(param.Value);
        }

        public void Visit(IntParam param)
        {
            defaultConvert(param.Value);
        }

        public void Visit(DoubleParam param)
        {
            defaultConvert(param.Value);
        }

        public void Visit(TestValue test)
        {

        }

        #endregion
    }
}
