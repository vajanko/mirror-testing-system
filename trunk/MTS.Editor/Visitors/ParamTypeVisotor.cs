using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    /// <summary>
    /// Converts parameter type to a corresponding enumerator value
    /// </summary>
    public class ParamTypeVisotor : IValueVisitor
    {
        private ParamType result;
        public ParamType GetParamType(ValueBase param)
        {
            param.Accept(this);
            return result;
        }
        public byte GetDbParamType(ValueBase param)
        {
            param.Accept(this);
            return (byte)result;
        }

        #region IParamVisitor Members

        public void Visit(BoolParam param)
        {
            result = ParamType.Bool;
        }

        public void Visit(EnumParam param)
        {
            result = ParamType.Enum;
        }

        public void Visit(StringParam param)
        {
            result = ParamType.String;
        }

        public void Visit(IntParam param)
        {
            result = ParamType.Int;
        }

        public void Visit(DoubleParam param)
        {
            result = ParamType.Double;
        }

        public void Visit(TestValue test)
        {
            throw new InvalidOperationException("It is forbidden to convert test to parameter enumerator type");
        }

        #endregion
    }
}
