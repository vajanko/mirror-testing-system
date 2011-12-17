using System;

namespace MTS.Tester.Result
{
    public class TaskExecutedEventArgs : EventArgs
    {
        public TaskResult Result { get; protected set; }

        public TaskResultCode ResultCode { get { return Result.ResultCode; } }

        public TaskExecutedEventArgs(TaskResult result)
        {
            Result = result;
        }
    }
}
