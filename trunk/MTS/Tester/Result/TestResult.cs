using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;

namespace MTS.Tester.Result
{
    public class TestResult : TaskResult
    {
        public TestResult(TestValue test)
            : base(test.DatabaseId)
        { }
    }
}
