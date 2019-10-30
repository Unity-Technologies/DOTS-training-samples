namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the build progress tracker
    /// </summary>
    public interface IProgressTracker : IContextObject
    {
        /// <summary>
        /// Number of build tasks to run
        /// </summary>
        int TaskCount { get; set; }

        /// <summary>
        /// Current 0.0f to 1.0f progress though the TaskCount
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Increments and updated the title of the progress bar.
        /// </summary>
        /// <param name="taskTitle">The title to display on the progress bar.</param>
        /// <returns><c>false</c> if the build should not continue due to user interaction with the progress bar; otherwise, <c>true</c>.</returns>
        bool UpdateTask(string taskTitle);

        /// <summary>
        /// Updates the secondary information display of the progress bar.
        /// </summary>
        /// <param name="taskInfo">The secondary information to display on the progress bar.</param>
        /// <returns><c>false</c> if the build should not continue due to user interaction with the progress bar; otherwise, <c>true</c>.</returns>
        bool UpdateInfo(string taskInfo);
    }
}
