using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Unity.Build
{
    /// <summary>
    /// Contains a list of build steps to run in order
    /// </summary>
    [Serializable]
    [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
    public class BuildPipeline : ScriptableObject, IBuildStep
    {
        [SerializeField]
        List<string> serializedStepData = new List<string>();

        /// <summary>
        /// Event sent when the build started.
        /// </summary>
        public static event Action<BuildContext> BuildStarted = delegate { };

        /// <summary>
        /// Event sent when the build completed.
        /// </summary>
        public static event Action<BuildContext> BuildCompleted = delegate { };

        /// <summary>
        /// Number of build steps.
        /// </summary>
        public int StepCount => serializedStepData.Count;

        /// <summary>
        /// Creates a new BuildPipeline asset.
        /// </summary>
        /// <param name="path">The asset path for the created pipepline.</param>
        /// <returns>The created pipeline object.</returns>
        public static BuildPipeline CreateAsset(string path)
        {
            var buildSettings = CreateInstance<BuildPipeline>();
            AssetDatabase.CreateAsset(buildSettings, path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<BuildPipeline>(path);
        }

        /// <summary>
        /// Checks for the existence of a build step type.
        /// </summary>
        /// <param name="type">The type of build step.</param>
        /// <returns>True if there is at least one step of this type.</returns>
        public bool HasStep(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return false;
            }

            for (int i = 0; i < StepCount; i++)
            {
                var step = GetStep(i);
                if (type.IsAssignableFrom(step.GetType()))
                {
                    return true;
                }

                if (typeof(BuildPipeline).IsAssignableFrom(step.GetType()))
                {
                    var pipelineStep = step as BuildPipeline;
                    if (pipelineStep.HasStep(type))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks for the existence of a build step type.
        /// </summary>
        /// <typeparam name="T">The type of build step.</typeparam>
        /// <returns>True if there is at least one step of this type.</returns>
        public bool HasStep<T>() where T : IBuildStep => HasStep(typeof(T));

        /// <summary>
        /// Checks for the existence of a build step.
        /// </summary>
        /// <param name="step">The build step to look for.</param>
        /// <returns>True if there is at least one step of this type.</returns>
        public bool HasStep(IBuildStep step) => HasStep(step?.GetType());

        /// <summary>
        /// Get a build step by index.
        /// </summary>
        /// <param name="index">The step index.</param>
        /// <returns>The build step or null if the index is out of range.</returns>
        public IBuildStep GetStep(int index)
        {
            if (index < 0 || index >= serializedStepData.Count)
            {
                return null;
            }

            try
            {
                return DeserializeStep(serializedStepData[index]);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize build steps. {e.ToString()}");
                return null;
            }
        }

        /// <summary>
        /// Creates build steps from serialized data and adds to the specified list.  It is up to the caller to clear the list if desired.
        /// </summary>
        /// <param name="steps">The list to fill with steps.</param>
        /// <returns>True if all serialized steps were added, false if any failed to create from serialized data.</returns>
        public bool GetSteps(List<IBuildStep> steps)
        {
            try
            {
                var result = true;
                foreach (var s in serializedStepData)
                {
                    var step = DeserializeStep(s);
                    if (step == null)
                    {
                        result &= false;
                    }
                    steps.Add(step);
                }
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize build steps. {e.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// Get the display text for a step.
        /// </summary>
        /// <param name="stepIndex">The setp index.</param>
        /// <returns>The display name of the step.</returns>
        public string GetStepDisplayName(int stepIndex) => GetStep(stepIndex)?.Description;

        /// <summary>
        /// Add a build step to the pipeline.
        /// </summary>
        /// <param name="type">The type of the step to add.</param>
        /// <returns>True if the step was added.</returns>
        public bool AddStep(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return false;
            }

            var step = CreateStepFromType(type);
            if (step == null)
            {
                return false;
            }

            var data = SerializeStep(step);
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }

            serializedStepData.Add(data);
            return true;
        }

        /// <summary>
        /// Add a build step to the pipeline.
        /// </summary>
        /// <typeparam name="T">The type of the step to add.</typeparam>
        /// <returns>True if the step was added.</returns>
        public bool AddStep<T>() where T : IBuildStep => AddStep(typeof(T));

        /// <summary>
        /// Adds a build step to the pipeline.
        /// </summary>
        /// <param name="step">The step to add.</param>
        public bool AddStep(IBuildStep step)
        {
            var data = SerializeStep(step);
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }

            serializedStepData.Add(data);
            return true;
        }

        /// <summary>
        /// Adds a set of build steps by type.
        /// </summary>
        /// <param name="types">The list of build step types to add.</param>
        /// <returns>True if all build steps were added.</returns>
        public bool AddSteps(params Type[] types)
        {
            var result = true;
            foreach (var type in types)
            {
                result &= AddStep(type);
            }
            return result;
        }

        /// <summary>
        /// Adds a set of build steps.
        /// </summary>
        /// <param name="steps">The list of build steps to add.</param>
        /// <returns>True if all build steps were added.</returns>
        public bool AddSteps(params IBuildStep[] steps)
        {
            var result = true;
            foreach (var step in steps)
            {
                result &= AddStep(step);
            }
            return result;
        }

        /// <summary>
        /// Overwrites all build steps.
        /// </summary>
        /// <param name="steps">The set of steps to overwrite with.</param>
        /// <returns>True if all build steps were added.</returns>
        public bool SetSteps(params Type[] types)
        {
            serializedStepData.Clear();
            return AddSteps(types);
        }

        /// <summary>
        /// Overwrites all build steps.
        /// </summary>
        /// <param name="steps">The set of steps to overwrite with.</param>
        /// <returns>True if all build steps were added.</returns>
        public bool SetSteps(params IBuildStep[] steps)
        {
            serializedStepData.Clear();
            return AddSteps(steps);
        }

        /// <summary>
        /// Run the build pipeline.
        /// </summary>
        /// <param name="context">Context for running.</param>
        /// <returns>The result of the build.</returns>
        public BuildPipelineResult RunSteps(BuildContext context)
        {
            if (context == null)
            {
                throw new NullReferenceException(nameof(context));
            }

            if (context.BuildSettings == null)
            {
                context.BuildSettings = CreateInstance<BuildSettings>();
            }

            var previousPipeline = context.BuildPipeline;
            context.BuildPipeline = this;

            var steps = new List<IBuildStep>();
            if (!GetSteps(steps))
            {
                return BuildPipelineResult.Failure($"Failed to get build steps from pipeline {name}.");
            }

            var result = RunSteps(context, steps);

            if (previousPipeline != null)
            {
                context.BuildPipeline = previousPipeline;
            }
            return result;
        }

        /// <summary>
        /// Construct <see cref="BuildPipelineResult"/> from this <see cref="BuildPipeline"/> that represent a successful execution.
        /// </summary>
        /// <returns>A new <see cref="BuildPipelineResult"/> instance.</returns>
        public BuildPipelineResult Success() => BuildPipelineResult.Success();

        /// <summary>
        /// Construct <see cref="BuildPipelineResult"/> from this <see cref="BuildPipeline"/> that represent a failed execution.
        /// </summary>
        /// <param name="message">Message that explain why the <see cref="BuildPipeline"/> execution failed.</param>
        /// <returns>A new <see cref="BuildPipelineResult"/> instance.</returns>
        public BuildPipelineResult Failure(string message) => BuildPipelineResult.Failure(message);

        internal static IBuildStep CreateStepFromType(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException(nameof(type));
            }

            if (!typeof(IBuildStep).IsAssignableFrom(type))
            {
                Debug.LogError($"Build step type '{type.FullName}' does not derive from {nameof(IBuildStep)}.");
                return null;
            }

            return TypeConstruction.Construct<IBuildStep>(type);
        }

        internal static string SerializeStep(IBuildStep step)
        {
            if (step == null)
            {
                return null;
            }

            var obj = step as UnityEngine.Object;
            if (obj != null)
            {
                string guid;
                long lfid;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out lfid))
                {
                    return $"@{guid}";
                }
                Debug.LogWarning($"{nameof(IBuildStep)} that are {nameof(UnityEngine.Object)} must be saved to an asset before being added to a {nameof(BuildPipeline)}.");
                return null;
            }

            var type = step.GetType();
            return $"{type}, {type.Assembly.GetName().Name}";
        }

        internal static IBuildStep DeserializeStep(string stepData)
        {
            if (string.IsNullOrEmpty(stepData))
            {
                throw new ArgumentException(nameof(stepData));
            }

            if (stepData[0] == '@')
            {
                var path = AssetDatabase.GUIDToAssetPath(stepData.Substring(1));
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError($"Unable to determine asset path from step data '{stepData}'.");
                    return null;
                }
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) as IBuildStep;
            }

            var type = Type.GetType(stepData);
            if (type == null)
            {
                Debug.LogError($"Could not resolve build step type from type name '{stepData}'.");
                return null;
            }

            return CreateStepFromType(type);
        }

        static BuildPipelineResult RunSteps(BuildContext context, IReadOnlyCollection<IBuildStep> steps)
        {
            if (EditorApplication.isCompiling)
            {
                throw new InvalidOperationException("Building is not allowed while Unity is compiling.");
            }

            if (context.BuildSettings == null)
            {
                throw new NullReferenceException($"{nameof(BuildSettings)} object is null in {nameof(context)}.");
            }

            if (context.BuildPipeline == null)
            {
                throw new NullReferenceException($"{nameof(BuildPipeline)} object is null in {nameof(context)}.");
            }

            // Work-around for assets that can be garbage collected during a build
            context.BuildSettings.GCPin();
            context.BuildPipeline.GCPin();
            
            //@TODO: It is unlikely that anything here will actually throw.
            //     We should probably review the code and ensuree that it definitely can't and remove this.
            try
            {
                BuildStarted.Invoke(context);

                var timer = Stopwatch.StartNew();
                RunBuildSteps(context, steps);
                timer.Stop();

                var status = context.BuildPipelineStatus;
                status.Duration = timer.Elapsed;
                status.BuildSettings = context.BuildSettings;

                var failedStep = status.GetFirstFailedBuildStep();
                if (failedStep.Failed)
                {
                    status.Succeeded = false;
                    status.Message = failedStep.Message;
                }
                
                BuildCompleted.Invoke(context);

                status.LogResult();
            }
            finally
            {
                // Work-around for assets that can be garbage collected during a build
                context.BuildSettings.GCUnPin();
                context.BuildPipeline.GCUnPin();
            }

            return context.BuildPipelineStatus;
        }

        #region IBuildStep

        public string Description => $"Build pipeline {name}";

        public bool IsEnabled(BuildContext context) => true;

        public BuildStepResult RunStep(BuildContext context) => RunSteps(context);

        public BuildStepResult CleanupStep(BuildContext context) => BuildStepResult.Success(this);

        #endregion
        
        static void RunBuildSteps(BuildContext context, IReadOnlyCollection<IBuildStep> steps)
        {
            var timer = new Stopwatch();
            var status = context.BuildPipelineStatus;
            var title = context.BuildProgress?.Title ?? string.Empty;

            // Setup build step actions to perform
            var cleanupSteps = new Stack<IBuildStep>();
            var enabledSteps = steps.Where(step => step.IsEnabled(context)).ToArray();

            // Execute build step actions (Stop executing on first failure - of any kind)
            for (var i = 0; i < enabledSteps.Length; ++i)
            {
                var step = enabledSteps[i];

                var cancelled = context.BuildProgress?.Update($"{title} (Step {i + 1} of {enabledSteps.Length})", step.Description + "...", (float)i / enabledSteps.Length) ?? false;
                if (cancelled)
                {
                    status.Succeeded = false;
                    status.Message = $"{title} was cancelled.";
                    break;
                }

                cleanupSteps.Push(step);

                try
                {
                    timer.Restart();
                    var result = step.RunStep(context);
                    timer.Stop();

                    result.Duration = timer.Elapsed;

                    status.BuildStepsResults.Add(result);
    
                    // Stop execution for normal build  steps after failure
                    if (!result.Succeeded)
                        break;
                }
                catch (Exception exception)
                {
                    // Stop execution for normal build steps after failure
                    status.BuildStepsResults.Add(BuildStepResult.Exception(step, exception));
                    break;
                }
            }
            
            // Execute Cleanup (Even if there are failures)
            // * In opposite order of the run steps (Only run the cleanup steps, for steps that ran)
            // * can't be cancelled, cleanup must always run
            foreach(var step in cleanupSteps)
            {
                context.BuildProgress?.Update($"{title} (Cleanup)", step.Description + "...", 1.0F);

                try
                {
                    timer.Restart();
                    var result = step.CleanupStep(context);
                    timer.Stop();

                    result.Duration = timer.Elapsed;

                    // All clean steps must run even if there are failures
                    status.BuildStepsResults.Add(result);
                }
                catch (Exception exception)
                {
                    // All clean steps must run even if there are failures
                    status.BuildStepsResults.Add(BuildStepResult.Exception(step, exception));
                }
            }
        }
    }
}
