using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;

namespace MTS.Tester.Result
{
    /// <summary>
    /// Class describing test execution activity
    /// </summary>
    public class TestResult : TaskResult
    {
        /// <summary>
        /// (Get) Database id of parameter or test used to produce this result. Set to zero if there 
        /// will not be any reference to database
        /// </summary>
        public int DatabaseId { get; private set; }        

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="TestResult"/> describing for result of given test.
        /// </summary>
        /// <param name="test">Instance of test generating this result data</param>
        public TestResult(TestValue test)
        {
            DatabaseId = test.DatabaseId;
        }

        #endregion
    }
}
