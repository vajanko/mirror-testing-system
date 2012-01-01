using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester
{
    public class ParamNotFoundException : TestException
    {
        public string ParamName { get; private set; }

        public ParamNotFoundException(string paramName)
            : base()
        {
            ParamName = paramName;
        }
    }
}
