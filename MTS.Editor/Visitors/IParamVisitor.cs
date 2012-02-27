using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Editor
{
    public interface IValueVisitor
    {
        void Visit(BoolParam param);
        void Visit(EnumParam param);
        void Visit(StringParam param);
        void Visit(IntParam param);
        void Visit(DoubleParam param);
        void Visit(TestValue test);
    }
}
