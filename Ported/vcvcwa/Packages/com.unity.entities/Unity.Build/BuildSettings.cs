using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.Build
{
    /// <summary>
    /// Can stores a set of unique components, which can be inherited or overridden using dependencies.
    /// </summary>
    public sealed class BuildSettings : ComponentContainer<IBuildSettingsComponent>
    {
        const string k_LastBuildFileName = "lastbuild.txt";

        /// <summary>
        /// Check whether a BuildSettings object is buildable.  It must contain a BuildPipeline component with a valid BuildPipeline object.
        /// </summary>
        /// <returns>True if the object can be built.</returns>
        public bool CanBuild => TryGetComponent<IBuildProfileComponent>(out var profile) && profile.GetBuildPipeline() != null;

        /// <summary>
        /// Check whether a settings object can be run.
        /// </summary>
        /// <returns>True if the conditions for running are met.  This depends on having a lastbuild.txt in the BuildProfile.BuildPath folder with the name of the executable to run.</returns>
        public bool CanRun
        {
            get
            {
                if (TryGetComponent<IBuildProfileComponent>(out var profile) && profile.GetBuildTarget() != null)
                {
                    var lastBuildPath = GetLastBuildPath(this);
                    return lastBuildPath != null && lastBuildPath.Exists;
                }
                return false;
            }
        }

        /// <summary>
        /// Check if a build settings is running.
        /// </summary>
        /// <returns>True if the build settings is running, false otherwise.</returns>
        public bool IsRunning => false; // @TODO: not implemented

        /// <summary>
        /// Build a BuildSettings object.
        /// </summary>
        /// <returns>The result of the build.</returns>
        public BuildPipelineResult Build()
        {
            if (!TryGetComponent<IBuildProfileComponent>(out var bar))
            {
                return BuildPipelineResult.Failure($"{nameof(BuildSettings)} object {name} does not have a {nameof(IBuildProfileComponent)} component.");
            }

            var pipeline = bar.GetBuildPipeline();
            if (pipeline == null)
            {
                return BuildPipelineResult.Failure($"No {nameof(BuildPipeline)} provided.");
            }

            var what = !string.IsNullOrEmpty(name) ? $" {name}" : string.Empty;
            using (var progress = new BuildProgress($"Building{what}", "Please wait..."))
            {
                return pipeline.RunSteps(new BuildContext(this, progress));
            }
        }

        /// <summary>
        /// Run a previously build. This depends on having a lastbuild.txt in the BuildProfile.BuildPath folder with the name of the executable to run.
        /// </summary>
        /// <returns>True if the build is run.</returns>
        public BuildPipelineResult Run()
        {
            if (!TryGetComponent<IBuildProfileComponent>(out var bar))
            {
                return BuildPipelineResult.Failure($"{nameof(BuildSettings)} object {name} does not have a {nameof(IBuildProfileComponent)} component.");
            }

            var target = bar.GetBuildTarget();
            if (target == null)
            {
                return BuildPipelineResult.Failure($"No {nameof(Platforms.BuildTarget)} provided.");
            }

            var lastBuildPath = GetLastBuildPath(this);
            if (lastBuildPath == null || string.IsNullOrEmpty(lastBuildPath.FullName))
            {
                return BuildPipelineResult.Failure($"Last build path is null or empty.");
            }

            if (!lastBuildPath.Exists)
            {
                return BuildPipelineResult.Failure($"Last build path '{lastBuildPath.FullName}' does not exist.");
            }

            if (!target.Run(lastBuildPath))
            {
                return BuildPipelineResult.Failure($"Failed to run '{lastBuildPath.FullName}'.");
            }

            return BuildPipelineResult.Success();
        }

        /// <summary>
        /// Stop a running build.
        /// </summary>
        public void StopRunning() => Debug.LogWarning($"{nameof(BuildSettings.StopRunning)} not implemented.");

        /// <summary>
        /// Clears the saved last build path.
        /// </summary>
        public void ClearLastBuild() => SetLastBuildPath(this, null);

        /// <summary>
        /// Create a new <see cref="BuildSettings"/> asset.
        /// </summary>
        /// <param name="path">File path where to create the asset.</param>
        /// <returns>The new <see cref="BuildSettings"/> asset instance.</returns>
        public static BuildSettings CreateAsset(string path)
        {
            var buildSettings = CreateInstance<BuildSettings>();
            AssetDatabase.CreateAsset(buildSettings, path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<BuildSettings>(path);
        }

        /// <summary>
        /// Save this <see cref="BuildSettings"/> to the asset database.
        /// </summary>
        /// <param name="path">Optional file path where to save the asset.</param>
        /// <returns><see langword="true"/> if asset was successfully saved, otherwise <see langword="false"/>.</returns>
        public bool SaveAsset(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = AssetDatabase.GetAssetPath(this);
            }

            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            try
            {
                SerializeToPath(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize {nameof(BuildSettings)} {name} to '{path}'.\n{e.Message}");
                return false;
            }

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            return true;
        }

        /// <summary>
        /// Loads a build settings object with the specified GUID.
        /// </summary>
        /// <param name="buildSettingsGUID">The GUID of the asset to load.</param>
        /// <returns>The loaded <see cref="BuildSettings"/> object.</returns>
        public static BuildSettings LoadBuildSettings(GUID buildSettingsGUID)
        {
            var buildSettingGuidString = buildSettingsGUID.ToString();
            var assetGuid = AssetDatabase.GUIDToAssetPath(buildSettingGuidString);
            return AssetDatabase.LoadAssetAtPath<BuildSettings>(assetGuid);
        }

        [InitializeOnLoadMethod]
        static void RegisterBuildPipelineCallbacks()
        {
            BuildPipeline.BuildCompleted += OnBuildPipelineCompleted;
        }

        static string GetBuildInfoPath(BuildSettings settings)
        {
            if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(settings, out var guid, out long fileId))
            {
                return string.Empty;
            }
            return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/BuildInfo", guid);
        }

        static FileInfo GetLastBuildPath(BuildSettings settings)
        {
            if (settings == null)
            {
                Debug.LogError("Invalid BuildSettings object");
                return default;
            }

            var path = Path.Combine(GetBuildInfoPath(settings), k_LastBuildFileName);
            if (!File.Exists(path))
            {
                return default;
            }

            var exePath = File.ReadAllText(path);
            return new FileInfo(exePath);
        }

        static void SetLastBuildPath(BuildSettings settings, string value)
        {
            if (settings == null)
            {
                Debug.LogError("Invalid BuildSettings object");
                return;
            }

            var assetPath = GetBuildInfoPath(settings);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            var path = Path.Combine(assetPath, k_LastBuildFileName);
            if (string.IsNullOrEmpty(value))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, value);
            }
        }

        static void OnBuildPipelineCompleted(BuildContext context)
        {
            try
            {
                if (context.BuildPipelineStatus.Succeeded)
                {
                    var exe = context.Get<ExecutableFile>();
                    if (exe == null || !File.Exists(exe.Path.FullName) || context.BuildSettings == null)
                    {
                        return;
                    }
                    SetLastBuildPath(context.BuildSettings, exe.Path.FullName);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
