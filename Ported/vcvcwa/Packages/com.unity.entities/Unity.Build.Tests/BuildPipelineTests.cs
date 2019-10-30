using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.Build.Tests
{
    [TestFixture]
    class BuildPipelineTests
    {
        static Regex s_BuildFailureRegex = new Regex(@"Build .*failed in .*after \d+\.?\d*\w+\..*", RegexOptions.Compiled);

        class NotABuildStep
        {
        }

        class StepRunResults
        {
            public List<string> stepsRan = new List<string>();
        }

        class StepCleanupResults
        {
            public List<string> stepsCleaned = new List<string>();
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestStep1 : BuildStep
        {
            public class Data
            {
                public string value;
            }

            public bool enabled = true;
            string addedValue;

            public override string Description => "TestStep1";

            public override bool IsEnabled(BuildContext context) => enabled;

            public override BuildStepResult RunStep(BuildContext context)
            {
                addedValue = context.Get<Data>().value;
                context.GetOrCreate<StepRunResults>().stepsRan.Add(addedValue);
                return Success();
            }

            public override BuildStepResult CleanupStep(BuildContext context)
            {
                context.GetOrCreate<StepCleanupResults>().stepsCleaned.Add(addedValue);
                return Success();
            }
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestStep2 : BuildStep
        {
            public class Data
            {
                public string value;
            }

            public bool enabled = true;
            string addedValue;

            public override string Description => "TestStep2";

            public override bool IsEnabled(BuildContext context) => enabled;

            public override BuildStepResult RunStep(BuildContext context)
            {
                addedValue = context.Get<Data>().value;
                context.GetOrCreate<StepRunResults>().stepsRan.Add(addedValue);
                return Success();
            }

            public override BuildStepResult CleanupStep(BuildContext context)
            {
                context.GetOrCreate<StepCleanupResults>().stepsCleaned.Add(addedValue);
                return Success();
            }
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class FailStep : BuildStep
        {
            public override string Description => "";
            public bool Throw;
            
            public override BuildStepResult RunStep(BuildContext context)
            {
                context.GetOrCreate<StepRunResults>().stepsRan.Add("fail");
                if (Throw)
                    throw new System.ArgumentException("Run Step throw");
                else
                    return Failure("Run step failed.");
            }

            public override BuildStepResult CleanupStep(BuildContext context)
            {
                context.GetOrCreate<StepCleanupResults>().stepsCleaned.Add("fail");
                return Failure("Cleanup step failed.");
            }
        }

        
        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestBuildStep_RunSuccess_CleanupSuccess : BuildStep
        {
            public class Data
            {
                public string value;
            }

            public bool enabled = true;
            string addedValue;

            public override string Description => nameof(TestBuildStep_RunSuccess_CleanupSuccess);

            public override bool IsEnabled(BuildContext context) => enabled;

            public override BuildStepResult RunStep(BuildContext context)
            {
                addedValue = context.Get<Data>().value;
                context.GetOrCreate<StepRunResults>().stepsRan.Add(addedValue);
                return Success();
            }

            public override BuildStepResult CleanupStep(BuildContext context)
            {
                if (context.BuildPipelineStatus.TryGetBuildStepResult(this, out var result) && result.Succeeded)
                {
                    context.GetOrCreate<StepCleanupResults>().stepsCleaned.Add(addedValue);
                    return Success();
                }
                else
                {
                    return Failure("Cleanup step not executed because run step failed.");
                }
            }
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        sealed class TestBuildStep_RunFailure_CleanupFailure : BuildStep
        {
            public class Data
            {
                public string value;
            }

            public bool enabled = true;
            string addedValue;

            public override string Description => nameof(TestBuildStep_RunFailure_CleanupFailure);

            public override bool IsEnabled(BuildContext context) => enabled;

            public override BuildStepResult RunStep(BuildContext context)
            {
                addedValue = context.Get<Data>().value;
                return Failure("Step not ran.");
            }

            public override BuildStepResult CleanupStep(BuildContext context)
            {
                if (context.BuildPipelineStatus.TryGetBuildStepResult(this, out var result) && result.Succeeded)
                {
                    context.GetOrCreate<StepCleanupResults>().stepsCleaned.Add(addedValue);
                    return Success();
                }
                else
                {
                    return Failure("Cleanup step not executed because run step failed.");
                }
            }
        }

        const string k_TestStep1PipelineAssetPath = "Assets/TestStep1BuildPipeline.asset";
        const string k_TestStep2PipelineAssetPath = "Assets/TestStep2BuildPipeline.asset";
        const string k_FailPipelineAssetPath = "Assets/TestFailBuildPipeline.asset";

        [OneTimeSetUp]
        public void CreateAssets()
        {
            var pipeline1 = BuildPipeline.CreateAsset(k_TestStep1PipelineAssetPath);
            pipeline1.AddSteps(typeof(TestStep1), typeof(TestStep1));

            var pipeline2 = BuildPipeline.CreateAsset(k_TestStep2PipelineAssetPath);
            pipeline2.AddSteps(typeof(TestStep2), typeof(TestStep2));

            var failPipeline = BuildPipeline.CreateAsset(k_FailPipelineAssetPath);
            failPipeline.AddSteps(typeof(FailStep));
        }

        [OneTimeTearDown]
        public void CleanupAssets()
        {
            UnityEditor.AssetDatabase.DeleteAsset(k_TestStep1PipelineAssetPath);
            UnityEditor.AssetDatabase.DeleteAsset(k_TestStep2PipelineAssetPath);
            UnityEditor.AssetDatabase.DeleteAsset(k_FailPipelineAssetPath);
        }

        [Test]
        public void CanCreateStep_FromType()
        {
            var step = BuildPipeline.CreateStepFromType(typeof(TestStep1));
            Assert.IsNotNull(step);
            Assert.IsInstanceOf<IBuildStep>(step);
        }

        [Test]
        public void CreateStepWithInvalidType_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, new Regex($"Build step type '.*' does not derive from {nameof(IBuildStep)}."));
            Assert.IsNull(BuildPipeline.CreateStepFromType(typeof(NotABuildStep)));
        }

        [Test]
        public void CreateStepWithInvalidTypeString_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, new Regex($"Build step type '.*' does not derive from {nameof(IBuildStep)}."));
            Assert.IsNull(BuildPipeline.DeserializeStep(typeof(NotABuildStep).AssemblyQualifiedName));
        }

        [Test]
        public void CreateStepWithInvalidStepData_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, "Could not resolve build step type from type name 'InvalidData'.");
            Assert.IsNull(BuildPipeline.DeserializeStep("InvalidData"));
        }

        [Test]
        public void CanSetAndGetSteps()
        {
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            var steps = new IBuildStep[] { new TestStep1(), new TestStep1(), new TestStep2(), new TestStep1(), new TestStep1() };
            pipeline.SetSteps(steps);
            var stepList = new List<IBuildStep>();
            var result = pipeline.GetSteps(stepList);
            Assert.True(result);
            Assert.AreEqual(steps.Length, pipeline.StepCount);
            Assert.AreEqual(stepList.Count, pipeline.StepCount);
            Assert.IsAssignableFrom<TestStep1>(pipeline.GetStep(0));
            Assert.IsAssignableFrom<TestStep2>(pipeline.GetStep(2));
        }

        [Test]
        public void CanCreateStep_FromTypeName()
        {
            var step = BuildPipeline.DeserializeStep(typeof(TestStep1).AssemblyQualifiedName);
            Assert.IsNotNull(step);
            Assert.IsInstanceOf<IBuildStep>(step);
        }

        [Test]
        public void CanCreateStep_FromData()
        {
            var step = new TestStep1();
            var data = BuildPipeline.SerializeStep(step);
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            var step2 = BuildPipeline.DeserializeStep(data);
            Assert.IsNotNull(step2);
        }

        BuildContext CreateTestContext()
        {
            return new BuildContext(new TestStep1.Data() { value = "step1" }, new TestStep2.Data() { value = "step2" });
        }

        void ValidateTestContext(BuildContext context)
        {
            var results = context.Get<StepRunResults>();
            Assert.IsNotNull(results);
            Assert.That(results.stepsRan, Is.EqualTo(new[] { "step1", "step2" }));
        }

        [Test]
        public void CanRunPipeline_WithTypeArray()
        {
            var context = CreateTestContext();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestStep1), typeof(TestStep2));
            pipeline.RunSteps(context);
            ValidateTestContext(context);
        }

        [Test]
        public void WhenPipelineSucceeds_StepsAndCleanupRan()
        {
            var context = CreateTestContext();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestStep1), typeof(TestStep2));
            pipeline.RunSteps(context);
            
            var runResults = context.Get<StepRunResults>();
            Assert.That(runResults.stepsRan, Is.EqualTo(new[] { "step1", "step2" }));

            var cleanResults = context.Get<StepCleanupResults>();
            Assert.That(cleanResults.stepsCleaned, Is.EqualTo(new[] { "step2", "step1" }));
        }

        [Test]
        public void WhenPipelineFails_StepsAndCleanupRan([Values]bool shouldThrow)
        {
            var context = CreateTestContext();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(new TestStep1(), new FailStep { Throw  = shouldThrow }, new TestStep2());

            LogAssert.Expect(LogType.Error, s_BuildFailureRegex);

            pipeline.RunSteps(context);
            
            var runResults = context.Get<StepRunResults>();
            Assert.That(runResults.stepsRan, Is.EqualTo(new[] { "step1", "fail" }));

            var cleanResults = context.Get<StepCleanupResults>();
            Assert.That(cleanResults.stepsCleaned, Is.EqualTo(new[] { "fail", "step1" }));
        }

        [Test]
        public void WhenNestedPipelineFails_ParentPipelineFails()
        {
            var parentPipe = ScriptableObject.CreateInstance<BuildPipeline>();
            var subPipe = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildPipeline>(k_FailPipelineAssetPath);

            parentPipe.AddStep(subPipe);

            LogAssert.Expect(LogType.Error, s_BuildFailureRegex);
            LogAssert.Expect(LogType.Error, s_BuildFailureRegex);
            var result = parentPipe.RunSteps(new BuildContext());

            Assert.That(result.Failed, Is.True);
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        class UnityObjectBuildStep : ScriptableObject, IBuildStep
        {
            public string Description => "";
            public bool IsEnabled(BuildContext context) => true;
            public BuildStepResult RunStep(BuildContext context) => BuildStepResult.Success(this);
            public BuildStepResult CleanupStep(BuildContext context) => BuildStepResult.Success(this);
        }

        [BuildStep(flags = BuildStepAttribute.Flags.Hidden)]
        class DisabledBuildStep : IBuildStep
        {
            public string Description => "";
            public bool IsEnabled(BuildContext context) => false;
            public BuildStepResult RunStep(BuildContext context) => throw new Exception("Should not run");
            public BuildStepResult CleanupStep(BuildContext context) => throw new Exception("Should not run");
        }

        [Test]
        public void CannotAdd_NonSerializedUnityObjectBuildStep()
        {
            var subPipe = ScriptableObject.CreateInstance<UnityObjectBuildStep>();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            Assert.IsFalse(pipeline.AddStep(subPipe));
        }

        [Test]
        public void CannotAdd_NullBuildStepObject()
        {
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            Assert.IsFalse(pipeline.AddStep(default(IBuildStep)));
        }

        [Test]
        public void CannotAdd_NullBuildStepType()
        {
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            Assert.IsFalse(pipeline.AddStep(default(Type)));
        }

        [Test]
        public void DisabledStep_DoesntRun()
        {
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddStep<DisabledBuildStep>();
            Assert.DoesNotThrow(() => pipeline.RunStep(new BuildContext()), "");
        }

        [Test]
        public void CanRunPipeline_WithNestedPipelines()
        {
            var context = CreateTestContext();
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            var subPipe1 = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildPipeline>(k_TestStep1PipelineAssetPath);
            var subPipe2 = UnityEditor.AssetDatabase.LoadAssetAtPath<BuildPipeline>(k_TestStep2PipelineAssetPath);

            pipeline.AddSteps(
                subPipe1,
                BuildPipeline.CreateStepFromType(typeof(TestStep1)),
                subPipe2,
                BuildPipeline.CreateStepFromType(typeof(TestStep2))
            );
            pipeline.RunSteps(context);

            var results = context.Get<StepRunResults>();
            Assert.That(results, Is.Not.Null);
            Assert.That(results.stepsRan, Is.EqualTo(new[] { "step1", "step1", "step1", "step2", "step2", "step2" }));
        }

        [Test]
        public void EventsCalled_WhenPipelineRuns()
        {
            bool startCalled = false;
            bool completeCalled = false;
            BuildPipeline.BuildStarted += p => startCalled = true;
            BuildPipeline.BuildCompleted += (r) => completeCalled = true;
            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.RunSteps(new BuildContext());
            Assert.IsTrue(startCalled);
            Assert.IsTrue(completeCalled);
        }

        [Test]
        public void BuildSteps_CanChooseToNotRunCleanup_BasedOnRunResult()
        {
            var context = new BuildContext();
            context.Set(new TestBuildStep_RunSuccess_CleanupSuccess.Data { value = "success" });
            context.Set(new TestBuildStep_RunFailure_CleanupFailure.Data { value = "failure" });

            var pipeline = ScriptableObject.CreateInstance<BuildPipeline>();
            pipeline.AddSteps(typeof(TestBuildStep_RunSuccess_CleanupSuccess), typeof(TestBuildStep_RunFailure_CleanupFailure));

            LogAssert.Expect(LogType.Error, s_BuildFailureRegex);
            pipeline.RunSteps(context);

            var results = context.Get<StepCleanupResults>();
            Assert.That(results.stepsCleaned, Is.EqualTo(new[] { "success" }));
        }
    }
}
