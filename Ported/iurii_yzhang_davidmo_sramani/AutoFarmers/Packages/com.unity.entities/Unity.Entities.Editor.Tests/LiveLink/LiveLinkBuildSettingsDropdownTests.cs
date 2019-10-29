using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.Build;
using Unity.Build.Common;
using UnityEditor;
using BuildPipeline = Unity.Build.BuildPipeline;

namespace Unity.Entities.Editor.Tests.LiveLink
{
    [TestFixture]
    class LiveLinkBuildSettingsDropdownTests : LiveLinkTestFixture
    {
        [Test]
        public void Should_Filter_BuildSettings()
        {
            var invalid_Emtpy = CreateBuildSettings();
            var invalid_NoTarget = CreateBuildSettings(b => b.WithClassicBuildProfileComponent(BuildTarget.NoTarget, TestDirectory, new BuildStepBuildClassicLiveLink()));
            var invalid_NoLiveLinkBuildStep = CreateBuildSettings(b => b.WithClassicBuildProfileComponent(BuildTarget.StandaloneWindows, TestDirectory));
            var valid = CreateBuildSettings(b => b.WithClassicBuildProfileComponent(BuildTarget.StandaloneWindows, TestDirectory, new BuildStepBuildClassicLiveLink()));

            LiveLinkBuildSettingsDropdown.LiveLinkBuildSettingsCache.Reload();

            var validBuildSettings = LiveLinkBuildSettingsDropdown.LiveLinkBuildSettingsCache.BuildSettings.Select(b => b.Asset).ToArray();
            Assert.That(validBuildSettings, Does.Contain(valid.asset));
            Assert.That(validBuildSettings, Has.No.Member(invalid_Emtpy.asset));
            Assert.That(validBuildSettings, Has.No.Member(invalid_NoTarget.asset));
            Assert.That(validBuildSettings, Has.No.Member(invalid_NoLiveLinkBuildStep.asset));
        }
    }

    static class BuildSettingsExtensions
    {
        public static BuildSettings WithClassicBuildProfileComponent(this BuildSettings buildSettings, BuildTarget target, DirectoryInfo testDirectory, params IBuildStep[] steps)
        {
            var buildPipeline = BuildPipeline.CreateAsset(Path.Combine(testDirectory.GetRelativePath(), Path.ChangeExtension(Path.GetRandomFileName(), "asset")));

            if (steps != null)
            {
                buildPipeline.AddSteps(steps);
                AssetDatabase.SaveAssets();
            }

            var profile = new ClassicBuildProfile
            {
                Target = target,
                Pipeline = buildPipeline
            };

            return buildSettings.With(profile);
        }

        static BuildSettings With<T>(this BuildSettings buildSettings, T component) where T : IBuildSettingsComponent
        {
            buildSettings.SetComponent(component);
            return buildSettings;
        }
    }
}