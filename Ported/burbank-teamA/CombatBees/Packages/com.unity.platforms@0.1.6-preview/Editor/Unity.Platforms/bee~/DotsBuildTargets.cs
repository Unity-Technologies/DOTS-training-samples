using Bee.NativeProgramSupport.Building;
using System.Collections.Generic;
using Unity.BuildSystem.CSharpSupport;
using Unity.BuildSystem.NativeProgramSupport;

namespace DotsBuildTargets
{
    abstract class DotsBuildSystemTarget
    {
        public virtual IEnumerable<DotsRuntimeCSharpProgramConfiguration> GetConfigs()
        {
            if (!ToolChain.CanBuild)
                yield break;

            yield return new DotsRuntimeCSharpProgramConfiguration(
                csharpCodegen: CSharpCodeGen.Debug,
                cppCodegen: CodeGen.Debug,
                nativeToolchain: ToolChain,
                scriptingBackend: ScriptingBackend,
                dotsConfiguration: DotsConfiguration.Debug,
                enableUnityCollectionsChecks: true,
                enableManagedDebugging: false,
                identifier: $"{Identifier}-debug",
                multiThreadedJobs: CanRunMultiThreadedJobs,
                useBurst: CanUseBurst,
                executableFormat: GetExecutableFormatForConfig(DotsConfiguration.Debug, enableManagedDebugger: false));

            yield return new DotsRuntimeCSharpProgramConfiguration(
                csharpCodegen: CSharpCodeGen.Debug,
                cppCodegen: CodeGen.Release,
                nativeToolchain: ToolChain,
                scriptingBackend: ScriptingBackend,
                dotsConfiguration: DotsConfiguration.Debug,
                enableUnityCollectionsChecks: true,
                enableManagedDebugging: true,
                identifier: $"{Identifier}-mdb",
                multiThreadedJobs: CanRunMultiThreadedJobs,
                useBurst: CanUseBurst,
                executableFormat: GetExecutableFormatForConfig(DotsConfiguration.Debug, enableManagedDebugger: true));

            yield return new DotsRuntimeCSharpProgramConfiguration(
                csharpCodegen: CSharpCodeGen.Release,
                cppCodegen: CodeGen.Release,
                nativeToolchain: ToolChain,
                scriptingBackend: ScriptingBackend,
                dotsConfiguration: DotsConfiguration.Develop,
                enableUnityCollectionsChecks: true,
                enableManagedDebugging: false,
                identifier: $"{Identifier}-develop",
                multiThreadedJobs: CanRunMultiThreadedJobs,
                useBurst: CanUseBurst,
                executableFormat: GetExecutableFormatForConfig(DotsConfiguration.Develop, enableManagedDebugger: false));

            yield return new DotsRuntimeCSharpProgramConfiguration(
                csharpCodegen: CSharpCodeGen.Release,
                cppCodegen: CodeGen.Release,
                nativeToolchain: ToolChain,
                scriptingBackend: ScriptingBackend,
                dotsConfiguration: DotsConfiguration.Release,
                enableUnityCollectionsChecks: false,
                enableManagedDebugging: false,
                identifier: $"{Identifier}-release",
                multiThreadedJobs: CanRunMultiThreadedJobs,
                useBurst: CanUseBurst,
                executableFormat: GetExecutableFormatForConfig(DotsConfiguration.Release, enableManagedDebugger: false));
        }

        protected virtual bool CanRunMultiThreadedJobs => false; // Disabling by default; Eventually: ScriptingBackend == ScriptingBackend.Dotnet;
        /*
         * disabled by default because it takes work to enable each platform for burst
         */
        protected virtual bool CanUseBurst => false;
        protected abstract string Identifier { get; }
        protected abstract ToolChain ToolChain { get; }
        protected virtual ScriptingBackend ScriptingBackend => ScriptingBackend.TinyIl2cpp;
        protected virtual NativeProgramFormat GetExecutableFormatForConfig(DotsConfiguration config, bool enableManagedDebugger) => null;

    }
}
