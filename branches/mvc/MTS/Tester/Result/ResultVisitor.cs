using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.Tester
{
    //public class ResultVisitor : IValueVisitor
    //{
    //    private ResultBase result;
    //    public ResultBase CreateResult(ValueBase value)
    //    {
    //        value.Accept(this);
    //        return result;
    //    }

    //    #region IValueVisitor Members

    //    public void Visit(BoolParam param)
    //    {
    //        result = new ParamResult(param, new BoolParam(param.ValueId));
    //    }

    //    public void Visit(EnumParam param)
    //    {
    //        result = new ParamResult(param, new EnumParam(param.ValueId, param.Values));
    //    }

    //    public void Visit(StringParam param)
    //    {
    //        result = new ParamResult(param, new StringParam(param.ValueId));
    //    }

    //    public void Visit(IntParam param)
    //    {
    //        result = new ParamResult(param, new IntParam(param.ValueId));
    //    }

    //    public void Visit(DoubleParam param)
    //    {
    //        result = new ParamResult(param, new DoubleParam(param.ValueId));
    //    }

    //    public void Visit(TestValue test)
    //    {
    //        result = new TestResult(test);
    //    }

    //    #endregion
    //}
}
