using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Unity.Build.Common
{
    [BuildStep(description = k_Description, category = "Classic")]
    sealed class BuildStepBuildClassicPlayer : BuildStep
    {
        const string k_Description = "Build Player";
        const string k_BootstrapFilePath = "Assets/StreamingAssets/livelink-bootstrap.txt";

        TemporaryFileTracker m_TemporaryFileTracker;

        public override string Description => k_Description;

        public static bool Prepare(BuildContext context, IBuildStep step, bool liveLink, TemporaryFileTracker tracker, out BuildStepResult failure, out BuildPlayerOptions options)
        {
            options = default;
            var settings = context.BuildSettings;
            var profile = settings.GetComponent<ClassicBuildProfile>();
            if (profile.Target <= 0)
            {
                failure = BuildStepResult.Failure(step, $"Invalid build target in build settings object {settings.name}.");
                return false;
            }

            if (profile.Target != EditorUserBuildSettings.activeBuildTarget)
            {
                failure = BuildStepResult.Failure(step, $"ActiveBuildTarget must be switched before the Build Player step {settings.name}.");
                return false;
            }

            
            var scenesList = settings.GetComponent<SceneList>().GetScenePathsForBuild();
            if (scenesList.Length == 0)
            {
                failure = BuildStepResult.Failure(step, "There are no scenes to build.");
                return false;
            }

            var outputPath =  OutputBuildDirectory.GetOutputDirectoryFor(context);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var productName = settings.GetComponent<GeneralSettings>().ProductName;
            var extension = profile.GetExecutableExtension();
            var locationPathName = Path.Combine(outputPath, productName + extension);

            options = new BuildPlayerOptions()
            {
                scenes = scenesList,
                target = profile.Target,
                locationPathName = locationPathName,
                targetGroup = UnityEditor.BuildPipeline.GetBuildTargetGroup(profile.Target),
            };

            options.options = BuildOptions.None;
            switch (profile.Configuration)
            {
                case BuildConfiguration.Debug:
                    options.options |= BuildOptions.AllowDebugging | BuildOptions.Development;
                    break;
                case BuildConfiguration.Develop:
                    options.options |= BuildOptions.Development;
                    break;
            }

            InternalSourceBuildConfiguration sourceBuild;
            if (settings.TryGetComponent(out sourceBuild) && sourceBuild.Enabled)
                options.options |= BuildOptions.InstallInBuildFolder;

            if (liveLink)
            {
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(context.BuildSettings));
                File.WriteAllText(tracker.TrackFile(k_BootstrapFilePath), guid);
            }
            else
            {
                // Make sure we didn't leak a bootstrap file from a previous crashed build
                tracker.EnsureFileDoesntExist(k_BootstrapFilePath);
            }

            failure = default;
            return true;
        }

        public static BuildStepResult ReturnBuildPlayerResult(BuildContext context, IBuildStep step, BuildReport report)
        {
            var result = new BuildStepResult
            {
                BuildStep = step,
                Succeeded = report.summary.result == BuildResult.Succeeded,
                Message = report.summary.result != BuildResult.Succeeded ? report.summary.ToString() : null
            };

            if (result.Succeeded)
            {
                var exe = context.GetOrCreate<ExecutableFile>();
                exe.Path = new FileInfo(report.summary.outputPath);
            }
            return result;
        }

        public override BuildStepResult RunStep(BuildContext context)
        {
            m_TemporaryFileTracker = new TemporaryFileTracker();
            if (!Prepare(context, this, false, m_TemporaryFileTracker, out var failure, out var options))
                return failure;

            var report = UnityEditor.BuildPipeline.BuildPlayer(options);
            return ReturnBuildPlayerResult(context, this, report);
        }

        public override BuildStepResult CleanupStep(BuildContext context)
        {
            m_TemporaryFileTracker.Dispose();
            return Success();
        }
    }
}
