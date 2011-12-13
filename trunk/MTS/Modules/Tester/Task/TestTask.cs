using System;
using System.Collections.Generic;

using MTS.IO;
using MTS.Editor;
using MTS.Tester.Result;

namespace MTS.TesterModule
{
    /// <summary>
    /// Base class for all testing tasks. This task must have a result from its execution
    /// </summary>
    public abstract class TestTask : Task
    {
        /// <summary>
        /// Parameter necesaary for execution of this task. Include inforamtion such as
        /// maximum duration of task or allowed range of current etc.
        /// </summary>
        protected TestValue testParam;
        /// <summary>
        /// (Get) Value indiating if this test task is enabled. If true, task is executed,
        /// otherwise not.
        /// </summary>
        protected bool Enabled { get; private set; }

        #region Constructors

        public TestTask(Channels channels, TestValue testParam) : base(channels) 
        {
            this.testParam = testParam;
            Name = testParam.Name;
            Enabled = testParam.Enabled;
        }

        #endregion
    }
}
