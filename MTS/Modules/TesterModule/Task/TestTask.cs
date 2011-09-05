using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    /// <summary>
    /// Base class for all testing tasks. This task must have a result from its execution
    /// </summary>
    public class TestTask : Task
    {
        protected TestValue testParam;
        protected bool Enabled { get; private set; }

        public override void Initialize(TimeSpan time)
        {
            Output.WriteLine("{0} started", Name);
            base.Initialize(time);
        }

        public override void Finish(TimeSpan time, TaskState state)
        {
            Output.WriteLine("{0} finished in {1}", Name, Duration);
            base.Finish(time, state);
        }

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
