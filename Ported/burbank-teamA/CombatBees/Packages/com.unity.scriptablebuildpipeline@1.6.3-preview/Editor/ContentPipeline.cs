using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Utilities;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Static class containing the main content building entry points into the Scriptable Build Pipeline.
    /// </summary>
    public static class ContentPipeline
    {
        /// <summary>
        /// Default temporary path used for building content data.
        /// </summary>
        public const string kTempBuildPath = "Temp/ContentBuildData";

        /// <summary>
        /// Default callback implementation.
        /// </summary>
        public static BuildCallbacks BuildCallbacks = new BuildCallbacks();

        /// <summary>
        /// Default implementation of generating Asset Bundles using the Scriptable Build Pipeline.
        /// </summary>
        /// <param name="parameters">Set of parameters used for building asset bundles.</param>
        /// <param name="content">Set of content and explicit asset bundle layout to build.</param>
        /// <param name="result">Results from building the content and explicit asset bundle layout.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        public static ReturnCode BuildAssetBundles(IBundleBuildParameters parameters, IBundleBuildContent content, out IBundleBuildResults result)
        {
            var taskList = DefaultBuildTasks.Create(DefaultBuildTasks.Preset.AssetBundleCompatible);
            return BuildAssetBundles(parameters, content, out result, taskList);
        }

        /// <summary>
        /// Default implementation of generating Asset Bundles using the Scriptable Build Pipeline.
        /// </summary>
        /// <param name="parameters">Set of parameters used for building asset bundles.</param>
        /// <param name="content">Set of content and explicit asset bundle layout to build.</param>
        /// <param name="result">Results from building the content and explicit asset bundle layout.</param>
        /// <param name="taskList">Custom task list for building asset bundles.</param>
        /// <param name="contextObjects">Additional context objects to make available to the build.</param>
        /// <returns>Return code with status information about success or failure causes.</returns>
        public static ReturnCode BuildAssetBundles(IBundleBuildParameters parameters, IBundleBuildContent content, out IBundleBuildResults result, IList<IBuildTask> taskList, params IContextObject[] contextObjects)
        {
            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (parameters == null)
            {
                result = null;
                BuildLogger.LogException(new ArgumentNullException("parameters"));
                return ReturnCode.Exception;
            }

            // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
            if (taskList.IsNullOrEmpty())
            {
                result = null;
                BuildLogger.LogException(new ArgumentException("Argument cannot be null or empty.", "taskList"));
                return ReturnCode.Exception;
            }

            // Don't run if there are unsaved changes
            if (ValidationMethods.HasDirtyScenes())
            {
                result = null;
                return ReturnCode.UnsavedChanges;
            }

            AssetDatabase.SaveAssets();

            ReturnCode exitCode;
            result = new BundleBuildResults();

            using (var interfacesWrapper = new BuildInterfacesWrapper())
#if !CI_TESTRUNNER_PROJECT
            using (new SceneStateCleanup())
            using (var progressTracker = new ProgressTracker())
#else
            using (var progressTracker = new ProgressLoggingTracker())
#endif
            using (var buildCache = new BuildCache(parameters.CacheServerHost, parameters.CacheServerPort))
            {
                Directory.CreateDirectory(parameters.TempOutputFolder);

                BuildContext buildContext;
                try
                {
                    buildContext = new BuildContext(contextObjects);
                    buildContext.SetContextObject(parameters);
                    buildContext.SetContextObject(content);
                    buildContext.SetContextObject(result);
                    buildContext.SetContextObject(interfacesWrapper);
                    buildContext.SetContextObject(progressTracker);
                    buildContext.SetContextObject(buildCache);
                    // If IDeterministicIdentifiers was passed in with contextObjects, don't add the default
                    if (!buildContext.ContainsContextObject(typeof(IDeterministicIdentifiers)))
                        buildContext.SetContextObject(new Unity5PackedIdentifiers());
                    buildContext.SetContextObject(new BuildDependencyData());
                    buildContext.SetContextObject(new BundleWriteData());
                    buildContext.SetContextObject(BuildCallbacks);
                }
                catch (Exception e)
                {
                    // Avoid throwing exceptions in here as we don't want them bubbling up to calling user code
                    result = null;
                    BuildLogger.LogException(e);
                    return ReturnCode.Exception;
                }

                exitCode = BuildTasksRunner.Validate(taskList, buildContext);
                if (exitCode >= ReturnCode.Success)
#if SBP_PROFILER_ENABLE
                    exitCode = BuildTasksRunner.RunProfiled(taskList, buildContext);
#else
                    exitCode = BuildTasksRunner.Run(taskList, buildContext);
#endif

                if (Directory.Exists(parameters.TempOutputFolder))
                    Directory.Delete(parameters.TempOutputFolder, true);
            }


            int maximumSize = EditorPrefs.GetInt("BuildCache.maximumSize", 200);
            long maximumCacheSize = maximumSize * 1073741824L; // gigabytes to bytes
            ThreadPool.QueueUserWorkItem(PruneCache, maximumCacheSize);
            return exitCode;
        }

        static void PruneCache(object state)
        {
            BuildCache.PruneCache_Background((long)state);
        }
    }
}
