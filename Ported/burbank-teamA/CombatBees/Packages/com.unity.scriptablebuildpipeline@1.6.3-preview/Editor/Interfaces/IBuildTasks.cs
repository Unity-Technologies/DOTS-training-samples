namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface of all build tasks.
    /// </summary>
    public interface IBuildTask
    {
        /// <summary>
        /// Version identifier for the build task.
        /// Primarily for caching.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Run task method
        /// </summary>
        /// <returns>Return code with status information about success or failure causes.</returns>
        ReturnCode Run();
    }
}
