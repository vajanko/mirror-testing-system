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

        public void Visit(BoolParam param)
        {
            result = string.Format(CultureInfo.InvariantCulture, "{0}", param.Value);
        }

        public void Visit(EnumParam param)
        {
            result = string.Format(CultureInfo.InvariantCulture, "{0}", param.SelectedIndex);
        }

        public void Visit(StringParam param)
        {
            result = string.Format(CultureInfo.InvariantCulture, "{0}", param.Value);
        }

        public void Visit(IntParam param)
        {
            result = string.Format(CultureInfo.InvariantCulture, "{0}", param.Value);
        }

        public void Visit(DoubleParam param)
        {
            result = string.Format(CultureInfo.InvariantCulture, "{0}", param.Value);
        }

        public void Visit(TestValue test)
        {

        }

        #endregion
    }
}
