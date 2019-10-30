using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Profiler;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Basic static class containing default implementations for BuildTask validation and running.
    /// </summary>
    public static class BuildTasksRunner
    {
        /// <summary>
        /// Basic run implementation that takes a set of tasks, a context, and runs returning the build results.
        /// <seealso cref="IBuildTask"/>, <seealso cref="IBuildContext"/>, and <seealso cref="ReturnCode"/>
        /// </summary>
        /// <param name="pipeline">The set of build tasks to run.</param>
        /// <param name="context">The build context to use for this run.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        public static ReturnCode Run(IList<IBuildTask> pipeline, IBuildContext context)
        {
            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (pipeline == null)
            {
                BuildLogger.LogException(new ArgumentNullException("pipeline"));
                return ReturnCode.Exception;
            }

            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (context == null)
            {
                BuildLogger.LogException(new ArgumentNullException("context"));
                return ReturnCode.Exception;
            }

            IProgressTracker tracker;
            if (context.TryGetContextObject(out tracker))
                tracker.TaskCount = pipeline.Count;

            foreach (IBuildTask task in pipeline)
            {
                try
                {
                    if (!tracker.UpdateTaskUnchecked(task.GetType().Name.HumanReadable()))
                        return ReturnCode.Canceled;

                    ContextInjector.Inject(context, task);
                    var result = task.Run();
                    if (result < ReturnCode.Success)
                        return result;
                    ContextInjector.Extract(context, task);
                }
                catch (Exception e)
                {
                    BuildLogger.LogError("Build Task {0} failed with exception:\n{1}", task.GetType().Name, e.Message);
                    return ReturnCode.Exception;
                }
            }

            return ReturnCode.Success;
        }

        /// <summary>
        /// Run implementation with task profiler that takes a set of tasks, a context, runs returning the build results and prints out the profiler details.
        /// <seealso cref="IBuildTask"/>, <seealso cref="IBuildContext"/>, and <seealso cref="ReturnCode"/>
        /// </summary>
        /// <param name="pipeline">The set of build tasks to run.</param>
        /// <param name="context">The build context to use for this run.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        internal static ReturnCode RunProfiled(IList<IBuildTask> pipeline, IBuildContext context)
        {
            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (pipeline == null)
            {
                BuildLogger.LogException(new ArgumentNullException("pipeline"));
                return ReturnCode.Exception;
            }

            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (context == null)
            {
                BuildLogger.LogException(new ArgumentNullException("context"));
                return ReturnCode.Exception;
            }

            var profiler = new BuildProfiler(pipeline.Count + 1);
            profiler.Start(pipeline.Count, "TotalTime");
            int count = 0;

            IProgressTracker tracker;
            if (context.TryGetContextObject(out tracker))
                tracker.TaskCount = pipeline.Count;

            foreach (IBuildTask task in pipeline)
            {
                try
                {
                    if (!tracker.UpdateTaskUnchecked(task.GetType().Name.HumanReadable()))
                    {
                        profiler.Stop(pipeline.Count);
                        profiler.Print();
                        return ReturnCode.Canceled;
                    }

                    ContextInjector.Inject(context, task);
                    profiler.Start(count, task.GetType().Name);
                    var result = task.Run();
                    profiler.Stop(count++);

                    if (result < ReturnCode.Success)
                    {
                        profiler.Stop(pipeline.Count);
                        profiler.Print();
                        return result;
                    }
                    ContextInjector.Extract(context, task);
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                    profiler.Stop(count);
                    profiler.Print();
                    return ReturnCode.Exception;
                }
            }

            profiler.Stop(pipeline.Count);
            profiler.Print();
            return ReturnCode.Success;
        }

        /// <summary>
        /// Basic validate implementation that takes a set of tasks, a context, and does checks to ensure the task requirements are all satisfied.
        /// <seealso cref="IBuildTask"/>, <seealso cref="IBuildContext"/>, and <seealso cref="ReturnCode"/>
        /// </summary>
        /// <param name="pipeline">The set of build tasks to run.</param>
        /// <param name="context">The build context to use for this run.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        public static ReturnCode Validate(IList<IBuildTask> pipeline, IBuildContext context)
        {
            //// Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            //if (pipeline == null)
            //{
            //    BuildLogger.LogException(new ArgumentNullException("pipeline"));
            //    return ReturnCode.Exception;
            //}

            //// Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            //if (context == null)
            //{
            //    BuildLogger.LogException(new ArgumentNullException("context"));
            //    return ReturnCode.Exception;
            //}

            //var requiredTypes = new HashSet<Type>();
            //foreach (IBuildTask task in pipeline)
            //    requiredTypes.UnionWith(task.RequiredContextTypes);

            //var missingTypes = new List<string>();
            //foreach (Type requiredType in requiredTypes)
            //{
            //    if (!context.ContainsContextObject(requiredType))
            //        missingTypes.Add(requiredType.Name);
            //}

            //if (missingTypes.Count > 0)
            //{
            //    BuildLogger.LogError("Missing required object types to run build pipeline:\n{0}", string.Join(", ", missingTypes.ToArray()));
            //    return ReturnCode.MissingRequiredObjects;
            //}
            return ReturnCode.Success;
        }
    }
}
