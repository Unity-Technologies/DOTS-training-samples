using System;
using System.Collections.Generic;
using UnityEditor;

namespace Unity.Build
{
    /// <summary>
    /// Base class for build steps that are executed througout a <see cref="BuildPipeline"/>.
    /// </summary>
    public abstract class BuildStep : IBuildStep
    {
        /// <summary>
        /// Description of this <see cref="BuildStep"/> displayed in build progress reporting.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Determine if this <see cref="BuildStep"/> will be executed or not by the <see cref="BuildPipeline"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="BuildStep"/>.</param>
        /// <returns><see langword="true"/> if this BuildStep must be executed, otherwise <see langword="false"/>.</returns>
        public virtual bool IsEnabled(BuildContext context) => true;

        /// <summary>
        /// Execute this <see cref="BuildStep"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="BuildStep"/>.</param>
        /// <returns>The result of the execution of this <see cref="BuildStep"/>.</returns>
        public abstract BuildStepResult RunStep(BuildContext context);

        /// <summary>
        /// Clean-up the results of executing this <see cref="BuildStep"/>.
        /// </summary>
        /// <param name="context">The <see cref="BuildContext"/> used by the execution of this <see cref="BuildStep"/>.</param>
        public virtual BuildStepResult CleanupStep(BuildContext context) => Success();

        /// <summary>
        /// Retrieves a list of valid types for build steps.
        /// </summary>
        /// <param name="filter">Optional filter function for types.</param>
        /// <returns>List of available build step types.</returns>
        public static IReadOnlyCollection<Type> GetAvailableTypes(Func<Type, bool> filter = null)
        {
            var types = new HashSet<Type>();
            foreach (var type in TypeCache.GetTypesDerivedFrom<IBuildStep>())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                if (filter != null && !filter(type))
                {
                    continue;
                }
                types.Add(type);
            }
            return types;
        }

        /// <summary>
        /// Construct <see cref="BuildStepResult"/> from this <see cref="IBuildStep"/> that represent a successful execution.
        /// </summary>
        /// <returns>A new <see cref="BuildStepResult"/> instance.</returns>
        public BuildStepResult Success() => BuildStepResult.Success(this);

        /// <summary>
        /// Construct <see cref="BuildStepResult"/> from this <see cref="IBuildStep"/> that represent a failed execution.
        /// </summary>
        /// <param name="message">Message that explain why the <see cref="IBuildStep"/> execution failed.</param>
        /// <returns>A new <see cref="BuildStepResult"/> instance.</returns>
        public BuildStepResult Failure(string message) => BuildStepResult.Failure(this, message);

        /// <summary>
        /// Construct <see cref="BuildStepResult"/> from this <see cref="IBuildStep"/> that represent an exception during execution.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A new <see cref="BuildStepResult"/> instance.</returns>
        public BuildStepResult Exception(Exception exception) => BuildStepResult.Exception(this, exception);
    }
}
