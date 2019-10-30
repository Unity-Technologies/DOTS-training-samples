using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Build
{
    /// <summary>
    /// Context object for setting the executable file path.
    /// </summary>
    public class ExecutableFile
    {
        /// <summary>
        /// The file path.
        /// </summary>
        public FileInfo Path { get; set; }
    }

    /// <summary>
    /// Holds the results of the execution of a <see cref="BuildPipeline"/>.
    /// </summary>
    public class BuildPipelineResult
    {
        /// <summary>
        /// Determine if the execution of the <see cref="BuildPipeline"/> succeeded.
        /// </summary>
        [Property] public bool Succeeded { get; internal set; }

        /// <summary>
        /// Determine if the execution of the <see cref="BuildPipeline"/> failed.
        /// </summary>
        public bool Failed { get => !Succeeded; }

        /// <summary>
        /// The message resulting from the execution of this <see cref="BuildPipeline"/>.
        /// </summary>
        [Property] public string Message { get; internal set; }

        /// <summary>
        /// The total duration of the <see cref="BuildPipeline"/> execution.
        /// </summary>
        [Property] public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// The BuildSettings that got executed <see cref="BuildPipeline"/> execution.
        /// </summary>
        [Property] public BuildSettings BuildSettings { get; internal set; }

        /// <summary>
        /// A list of <see cref="BuildStepResult"/> collected during the <see cref="BuildPipeline"/> execution for each <see cref="IBuildStep"/>.
        /// </summary>
        [Property] public List<BuildStepResult> BuildStepsResults { get; internal set; } = new List<BuildStepResult>();

        internal BuildStepResult GetFirstFailedBuildStep()
        {
            foreach (var step in BuildStepsResults)
            {
                if (step.Failed)
                {
                    return step;
                }
            }
            return Success();
        }

        /// <summary>
        /// Get the <see cref="BuildStepResult"/> for the specified <see cref="IBuildStep"/>.
        /// </summary>
        /// <param name="buildStep">The build step to search for the result.</param>
        /// <param name="value">The <see cref="BuildStepResult"/> if found, otherwise default(<see cref="BuildStepResult"/>)</param>
        /// <returns><see langword="true"/> if the IBuildStep was found, otherwise <see langword="false"/>.</returns>
        public bool TryGetBuildStepResult(IBuildStep buildStep, out BuildStepResult value)
        {
            foreach (var result in BuildStepsResults)
            {
                if (result.BuildStep == buildStep)
                {
                    value = result;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Implicit conversion to <see cref="bool"/>.
        /// </summary>
        /// <param name="result">Instance of <see cref="BuildPipelineResult"/>.</param>
        public static implicit operator bool(BuildPipelineResult result) => result.Succeeded;

        /// <summary>
        /// Implicit conversion to <see cref="BuildStepResult"/>.
        /// </summary>
        /// <param name="result">Instance of <see cref="BuildPipelineResult"/>.</param>
        public static implicit operator BuildStepResult(BuildPipelineResult result) => new BuildStepResult
        {
            Succeeded = result.Succeeded,
            Message = result.Message
        };

        /// <summary>
        /// Get the <see cref="BuildPipelineResult"/> as a string that can be used for logging.
        /// </summary>
        /// <returns>The <see cref="BuildPipelineResult"/> as a string.</returns>
        public override string ToString()
        {
            var name = BuildSettings.name;
            var what = !string.IsNullOrEmpty(name) ? $" {name.ToHyperLink()}" : string.Empty;

            if (Succeeded)
            {
                return $"Build{what} succeeded after {Duration.ToShortString()}.";
            }
            else
            {
                var step = GetFirstFailedBuildStep();
                if (step.Failed)
                {
                    return $"Build{what} failed in '{step.BuildStep.GetType().Name}' after {Duration.ToShortString()}.\n{Message}";
                }
                else
                {
                    return $"Build{what} failed after {Duration.ToShortString()}.\n{Message}";
                }
            }
        }

        internal BuildPipelineResult() { }

        internal static BuildPipelineResult Success() => new BuildPipelineResult
        {
            Succeeded = true
        };

        internal static BuildPipelineResult Failure(string message) => new BuildPipelineResult
        {
            Succeeded = false,
            Message = message
        };

        internal void LogResult()
        {
            if (Succeeded)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, BuildSettings, ToString());
            }
            else
            {
                Debug.LogFormat(LogType.Error, LogOption.None, BuildSettings, ToString());
            }
        }
    }
}
