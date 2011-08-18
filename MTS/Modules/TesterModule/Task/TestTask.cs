using System;
using System.Collections.Generic;

using MTS.AdminModule;
using MTS.EditorModule;

namespace MTS.TesterModule
{
    public class TestTask : Task
    {
        protected TestValue testParam;

        public TestTask(Channels channels, TestValue testParam) : base(channels) 
        {
            this.testParam = testParam;
            Name = testParam.Name;
        }
    }
}
