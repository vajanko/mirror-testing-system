namespace MTS.Tester.Result
{
    /// <summary>
    /// Describes state of any task after its execution
    /// </summary>
    public enum TaskResultCode
    {
        /// <summary>
        /// Task has been executed and completed correctly
        /// </summary>
        Completed,
        /// <summary>
        /// An error occured while executing task
        /// </summary>
        Failed,
        /// <summary>
        /// Task has been aborted by some external force
        /// </summary>
        Aborted
    }
}
