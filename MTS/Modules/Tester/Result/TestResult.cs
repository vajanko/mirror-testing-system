using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTS.Tester.Result
{
    public class TestResult : TaskResult
    {
        /// <summary>
        /// (Get) Collection of parameter results for this test. This values contains parameter identifier
        /// and its value in string representation. It is exprected that these values will be saved in database.
        /// For more information see <see cref="ParamResult"/> class implementation.
        /// </summary>
        public List<ParamResult> Params { get; private set; }

        #region Constructors

        public TestResult()
        {
            Params = new List<ParamResult>();
        }

        #endregion
    }
}
