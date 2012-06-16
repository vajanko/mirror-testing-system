using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTS.Editor;

using MTS.Data.Types;

namespace MTS.Data
{
    public partial class MTSContext
    {
        private ToStringVisitor toString = new ToStringVisitor();
        private ParamTypeVisitor paramType = new ParamTypeVisitor();

        /// <summary>
        /// (Get) Database id of currently running shift
        /// </summary>
        public int ShiftId { get; private set; }

        /// <summary>
        /// Create a new instance of shift on database side and save all tests used (enabled) in this shift
        /// </summary>
        /// <param name="mirrorId">Database id of mirror being tested</param>
        /// <param name="operatorId">Database id of testing operator</param>
        /// <param name="tests">Collection of all tests</param>
        /// <returns>Instance of just started shift</returns>
        public Shift StartShift(int mirrorId, int operatorId, TestCollection tests)
        {
            // 1) create a new instance of shift and save it to database
            Shift dbShift = this.StartShift(mirrorId, operatorId).Single();
            ShiftId = dbShift.Id;

            // 2) save information about used tests in current shift
            foreach (TestValue test in tests.Where(t => t.Enabled))
            {   // add used tests in this shift (only enabled)
                AddTest(dbShift.Id, test);
            }

            return dbShift;
        }
        /// <summary>
        /// Create if not exists new used test in database and reference it with current shift
        /// </summary>
        /// <param name="shiftId">Database id of shift where given test is used</param>
        /// <param name="test">Instance of used test</param>
        /// <returns>Instance of added or existing test from database</returns>
        public Test AddTest(int shiftId, TestValue test)
        {
            // 1) create a new instance of test and save it to database
            Test dbTest = this.AddTest(test.ValueId, shiftId).Single();
            test.DatabaseId = dbTest.Id;

            // 2) save information about parameters in current test
            foreach (ParamValue param in test)
            {
                this.AddParam(dbTest.Id, param);
            }

            return dbTest;
        }
        /// <summary>
        /// Create if not exists new used parameter in database and reference it with current test
        /// </summary>
        /// <param name="testId">Database id of test where given parameter belongs</param>
        /// <param name="param">Instance of test parameter</param>
        /// <returns>Instance of added or existing parameter from database</returns>
        public Param AddParam(int testId, ParamValue param)
        {
            string unit = null;
            if (param is UnitParam)
            {
                unit = (param as UnitParam).Unit.Name;
            }

            string paramValue = toString.ConvertToString(param);
            byte paramDbType = paramType.GetDbParamType(param);

            Param dbParam = this.AddParam(testId, param.ValueId, paramValue, paramDbType, unit).Single();
            param.DatabaseId = dbParam.Id;

            return dbParam;
        }
    }
}
