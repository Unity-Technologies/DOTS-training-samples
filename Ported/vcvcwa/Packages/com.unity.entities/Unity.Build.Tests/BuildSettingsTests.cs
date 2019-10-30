using NUnit.Framework;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using BuildTarget = Unity.Platforms.BuildTarget;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Build.Tests
{
    public class BuildSettingsTests
    {
        static Regex s_BuildFailureRegex = new Regex(@"Build .*failed in .*after \d+\.?\d*\w+\..*", RegexOptions.Compiled);

        const string k_TestBuildStepSuccessOutputFile = "Assets/TestBuildStepSuccess.txt";

        class TestBuildTarget : BuildTarget
        {
            public override bool CanBuild => false;
            public override bool HideInBuildTargetPopup => true;
            public override string BeeTargetName => string.Empty;
            public override string DisplayName => nameof(TestBuildTarget);
            public override string ExecutableExtension => string.Empty;
            public override string UnityPlatformName => string.Empty;
            public override bool Run(FileInfo buildTarget) => buildTarget.Exists;
        }

        class TestBuildProfileComponent : IBuildProfileComponent
        {
            [Property] public BuildTarget Target { get; set; }
            [Property] public BuildPipeline Pipeline { get; set; }
            public BuildPipeline GetBuildPipeline() => Pipeline;
            public BuildTarget GetBuildTarget() => Target;
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestBuildStepSuccess : BuildStep
        {
            public override string Description => nameof(TestBuildStepSuccess);
            public override BuildStepResult RunStep(BuildContext context)
            {
                File.WriteAllText(k_TestBuildStepSuccessOutputFile, "success");
                context.Set(new ExecutableFile { Path = new FileInfo(k_TestBuildStepSuccessOutputFile) });
                return Success();
            }
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestBuildStepFailure : BuildStep
        {
            public override string Description => nameof(TestBuildStepFailure);
            public override BuildStepResult RunStep(BuildContext context) => Failure(nameof(TestBuildStepFailure));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AssetDatabase.DeleteAsset(k_TestBuildStepSuccessOutputFile);
        }

        [Test]
        public void CanBuild_ReturnsFalse_WithoutBuildProfileComponent()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            Assert.That(settings.CanBuild, Is.False);
        }

        [Test]
        public void CanBuild_ReturnsFalse_WithBuildProfileComponent_WithoutPipeline()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepSuccess));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = null
            });
            Assert.That(settings.CanBuild, Is.False);
        }

        [Test]
        public void CanBuild_ReturnsTrue_WithValidBuildProfileComponent()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepSuccess));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = pipeline
            });
            Assert.That(settings.CanBuild, Is.True);
        }

        [Test, Ignore("BuildSettings.CanRun doesn't work for non-saved assets.")]
        public void CanRun_ReturnsTrue_AfterBuild()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepSuccess));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = pipeline
            });
            Assert.That(settings.CanRun, Is.False);
            Assert.That(settings.Build().Succeeded, Is.True);
            Assert.That(settings.CanRun, Is.True);
        }

        [Test]
        public void CanRun_ReturnsFalse_AfterFailedBuild()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepFailure));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = pipeline
            });
            Assert.That(settings.CanRun, Is.False);
            LogAssert.Expect(LogType.Error, s_BuildFailureRegex);
            Assert.That(settings.Build().Succeeded, Is.False);
            Assert.That(settings.CanRun, Is.False);
        }

        [Test, Ignore("BuildSettings.IsRunning is not implemented.")]
        public void IsRunning_ReturnsTrue_AfterRun()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepSuccess));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = pipeline
            });
            Assert.That(settings.Build().Succeeded, Is.True);
            Assert.That(settings.IsRunning, Is.False);
            Assert.That(settings.Run(), Is.True);
            Assert.That(settings.IsRunning, Is.True);
        }

        [Test, Ignore("BuildSettings.StopRunning is not implemented.")]
        public void StopRunning()
        {
            var settings = ScriptableObject.CreateInstance<BuildSettings>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStepSuccess));
            settings.SetComponent(new TestBuildProfileComponent
            {
                Target = new TestBuildTarget(),
                Pipeline = pipeline
            });
            Assert.That(settings.Build().Succeeded, Is.True);
            Assert.That(settings.IsRunning, Is.True);
            Assert.That(settings.Run(), Is.True);
            settings.StopRunning();
            Assert.That(settings.IsRunning, Is.False);
        }
    }
}
