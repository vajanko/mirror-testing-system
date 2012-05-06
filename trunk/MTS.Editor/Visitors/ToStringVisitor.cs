using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using MTS.Editor.Properties;

namespace MTS.Editor
{
    /// <summary>
    /// Converts parameter value to string representation.
    /// </summary>
    public class ToStringVisitor : IValueVisitor
    {
        /// <summary>
        /// Result data of <see cref="ToStringVisitor"/> = string representation of parameter value.
        /// </summary>
        private string result;

        /// <summary>
        /// Converts given <see cref="ValueBase"/> value to its string representation.
        /// </summary>
        /// <param name="param">Instance of test parameter</param>
        /// <returns>String representation of parameter value</returns>
        public string ConvertToString(ValueBase param)
        {
            param.Accept(this);
            return result;
        }

        #region IParamVisitor Members

        /// <summary>
        /// Converts parameter value using default conversion by calling to string method.
        /// </summary>
        /// <param name="value">Parameter value to be converted</param>
        private void defaultConvert(object value)
        {
            // do not throw exception - just return null value
            if (value == null)
            {
                result = null;
            }
            else
            {   // parameter values are save using invariant culture
                result = string.Format(CultureInfo.InvariantCulture, "{0}", value);
            }
        }

        /// <summary>
        /// Converts <see cref="BoolParam"/> value to string representation.
        /// </summary>
        /// <param name="param">Instance of <see cref="BoolParam"/> to be converted to string</param>
        public void Visit(BoolParam param)
        {
            defaultConvert(param.Value);
        }

        /// <summary>
        /// Converts <see cref="EnumParam"/> value to string representation.
        /// </summary>
        /// <param name="param">Instance of <see cref="EnumParam"/> to be converted to string</param>
        public void Visit(EnumParam param)
        {
            defaultConvert(param.Value);
            //result = string.Format(CultureInfo.InvariantCulture, "{0}", param.SelectedIndex);
        }

        /// <summary>
        /// Converts <see cref="StringParam"/> value to string representation.
        /// </summary>
        /// <param name="param">Instance of <see cref="StringParam"/> to be converted to string</param>
        public void Visit(StringParam param)
        {
            defaultConvert(param.Value);
        }

        /// <summary>
        /// Converts <see cref="IntParam"/> value to string representation.
        /// </summary>
        /// <param name="param">Instance of <see cref="IntParam"/> to be converted to string</param>
        public void Visit(IntParam param)
        {
            defaultConvert(param.Value);
        }

        /// <summary>
        /// Converts <see cref="DoubleParam"/> value to string representation.
        /// </summary>
        /// <param name="param">Instance of <see cref="DoubleParam"/> to be converted to string</param>
        public void Visit(DoubleParam param)
        {
            defaultConvert(param.Value);
        }

        /// <summary>
        /// Do not call this methods. <see cref="TestValue"/> is not intended to be converted to string
        /// representation.
        /// </summary>
        /// <param name="param">Unused</param>
        /// <exception cref="NotImplementedException">When this method is called</exception>
        public void Visit(TestValue test)
        {
            throw new NotImplementedException(string.Format(Resources.ToStringConversionMsg, typeof(TestValue)));
        }

        #endregion
    }
}
