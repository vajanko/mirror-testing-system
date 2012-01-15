using System;
using MTS.Data.Types;

namespace MTS.Tester.Result
{
    public class TaskExecutedEventArgs : EventArgs
    {
        public TaskResult Result { get; protected set; }

        public TaskResultType ResultCode { get { return Result.ResultCode; } }

        public TaskExecutedEventArgs(TaskResult result)
        {
            Result = result;
        }
    }
}
