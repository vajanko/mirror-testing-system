using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace MTS.Editor
{
    /// <summary>
    /// Sets parameter value converting from its string representation
    /// </summary>
    public class FromStringVisitor : IValueVisitor
    {
        private string value;
        public void ConvertFromString(ValueBase param, string value)
        {
            this.value = value;
            param.Accept(this);
        }

        #region IParamVisitor Members

        public void Visit(BoolParam param)
        {
            // throw an exception if value is not in correct format
            param.BoolValue = bool.Parse(value);
        }

        public void Visit(EnumParam param)
        {
            param.SelectedIndex = int.Parse(value);
        }

        public void Visit(StringParam param)
        {
            param.StringValue = value;
        }

        public void Visit(IntParam param)
        {
            // throw an exception if value is not in correct format
            param.NumericValue = int.Parse(value, CultureInfo.InvariantCulture);
        }

        public void Visit(DoubleParam param)
        {
            // throw an exception if value is not in correct format
            param.NumericValue = double.Parse(value, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        public void Visit(TestValue test)
        {

        }

        #endregion
    }
}
