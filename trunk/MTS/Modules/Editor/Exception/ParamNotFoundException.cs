using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    public class ParamNotFoundException : TestValueException
    {
        public string ParamName { get; private set; }

        public ParamNotFoundException(string paramName)
            : base()
        {
            ParamName = paramName;
        }
    }
}
