namespace Unity.Build
{
    /// <summary>
    /// Interface for <see cref="BuildStep"/>. NOTE: When writing a new build step, derive from <see cref="BuildStep"/> instead of this interface.
    /// </summary>
    public interface IBuildStep
    {
        /// <summary>
        /// Description of this <see cref="IBuildStep"/> displayed in build progress reporting.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Determine if this <see cref="IBuildStep"/> will be executed or not by the <see cref="BuildPipeline"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="IBuildStep"/>.</param>
        /// <returns><see langword="true"/> if this BuildStep must be executed, otherwise <see langword="false"/>.</returns>
        bool IsEnabled(BuildContext context);

        /// <summary>
        /// Execute this <see cref="IBuildStep"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="IBuildStep"/>.</param>
        /// <returns>The result of the execution of this <see cref="IBuildStep"/>.</returns>
        BuildStepResult RunStep(BuildContext context);

        /// <summary>
        /// Clean-up the artifacts from executing this <see cref="IBuildStep"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="IBuildStep"/>.</param>
        /// <returns>The result of the execution of this <see cref="IBuildStep"/>.</returns>
        BuildStepResult CleanupStep(BuildContext context);
    }
}
