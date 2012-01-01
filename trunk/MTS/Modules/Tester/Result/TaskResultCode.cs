namespace MTS.Tester.Result
{
    /// <summary>
    /// Describes state of any task after its execution
    /// </summary>
    public enum TaskResultCode
    {
        /// <summary>
        /// Task has been executed and completed correctly. Integer value is 0
        /// </summary>
        Completed = 0,
        /// <summary>
        /// An error occurred while executing task. Integer value is 1
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Task has been aborted by some external force. Integer value is 2
        /// </summary>
        Aborted = 2
    }
}
