// As BurstCompiler.Compile is not supported on Tiny/ZeroPlayer, we can ifdef the entire file
#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if !BURST_INTERNAL
using Unity.Jobs.LowLevel.Unsafe;
#endif

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// NOTE: This file is shared via a csproj cs link in Burst.Compiler.IL
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

namespace Unity.Burst
{
    /// <summary>
    /// Options available at Editor time and partially at runtime to control the behavior of the compilation and to enable/disable burst jobs.
    /// </summary>
    public sealed partial class BurstCompilerOptions
    {
        private const string DisableCompilationArg = "--burst-disable-compilation";

        private const string ForceSynchronousCompilationArg = "--burst-force-sync-compilation";

        internal const string DefaultLibraryName = "lib_burst_generated";

        internal const string BurstInitializeName = "burst.initialize";

        // -------------------------------------------------------
        // Common options used by the compiler
        // -------------------------------------------------------
        internal const string OptionGroup = "group";
        internal const string OptionPlatform = "platform=";
        internal const string OptionBackend = "backend=";
        internal const string OptionSafetyChecks = "safety-checks";
        internal const string OptionDisableSafetyChecks = "disable-safety-checks";
        internal const string OptionNoAlias = "noalias";
        internal const string OptionDisableNoAlias = "disable-noalias";
        internal const string OptionDisableOpt = "disable-opt";
        internal const string OptionFastMath = "fastmath";
        internal const string OptionTarget = "target=";
        internal const string OptionIROpt = "ir-opt";
        internal const string OptionCpuOpt = "cpu-opt=";
        internal const string OptionFloatPrecision = "float-precision=";
        internal const string OptionFloatMode = "float-mode=";
        internal const string OptionDump = "dump=";
        internal const string OptionFormat = "format=";
        internal const string OptionDebugTrap = "debugtrap";
        internal const string OptionDisableVectors = "disable-vectors";
        internal const string OptionDebug = "debug";
        internal const string OptionDisableDebugSymbols = "disable-load-debug-symbols";
        internal const string OptionStaticLinkage = "generate-static-linkage-methods";

        // -------------------------------------------------------
        // Options used by the Jit compiler
        // -------------------------------------------------------

        internal const string OptionJitDisableFunctionCaching = "disable-function-caching";
        internal const string OptionJitDisableAssemblyCaching = "disable-assembly-caching";
        internal const string OptionJitEnableAssemblyCachingLogs = "enable-assembly-caching-logs";
        internal const string OptionJitEnableModuleCaching = "enable-module-caching";
        internal const string OptionJitEnableModuleCachingDebugger = "enable-module-caching-debugger";
        internal const string OptionJitEnableSynchronousCompilation = "enable-synchronous-compilation";

        // TODO: Remove this option and use proper dump flags or revisit how we log timings
        internal const string OptionJitLogTimings = "log-timings";
        internal const string OptionJitCacheDirectory = "cache-directory";

        internal const string OptionJitIsForFunctionPointer = "is-for-function-pointer";

        internal const string OptionJitManagedFunctionPointer = "managed-function-pointer=";

        // -------------------------------------------------------
        // Options used by the Aot compiler
        // -------------------------------------------------------
        internal const string OptionAotAssemblyFolder = "assembly-folder=";
        internal const string OptionRootAssembly = "root-assembly=";
        internal const string OptionAotMethod = "method=";
        internal const string OptionAotType = "type=";
        internal const string OptionAotAssembly = "assembly=";
        internal const string OptionAotOutputPath = "output=";
        internal const string OptionAotKeepIntermediateFiles = "keep-intermediate-files";
        internal const string OptionAotNoLink = "nolink";
        internal const string OptionAotPatchedAssembliesOutputFolder = "patch-assemblies-into=";
        internal const string OptionAotPinvokeNameToPatch = "pinvoke-name=";
        internal const string OptionAotOnlyStaticMethods = "only-static-methods";
        internal const string OptionMethodPrefix = "method-prefix=";
        internal const string OptionAotNoNativeToolchain = "no-native-toolchain";        
        internal const string OptionAotKeyFolder = "key-folder=";
        internal const string OptionAotDecodeFolder = "decode-folder=";
        internal const string OptionVerbose = "verbose";
        internal const string OptionValidateExternalToolChain = "validate-external-tool-chain";
        internal const string OptionCompilerThreads = "threads=";
        internal const string OptionChunkSize= "chunk-size=";

        internal const string CompilerCommandShutdown = "$shutdown";
        internal const string CompilerCommandCancel = "$cancel";
        internal const string CompilerCommandEnableCompiler = "$enable_compiler";
        internal const string CompilerCommandDisableCompiler = "$disable_compiler";

        // All the following content is exposed to the public interface

#if !BURST_INTERNAL
        internal const string GlobalSettingsName = "global";

        private static bool _enableBurstCompilation;
        private static bool _forceDisableBurstCompilation;
        private static bool _enableBurstCompileSynchronously;
        private static bool _forceBurstCompilationSynchronously;
        private static bool _enableBurstSafetyChecks;
        private static bool _enableBurstTimings;

        private BurstCompilerOptions() : this(null)
        {
        }

        internal BurstCompilerOptions(string name)
        {
            Name = name;
            // By default, burst is enabled as well as safety checks
            EnableBurstCompilation = true;
            EnableBurstSafetyChecks = true;
        }


        private bool _enableEnhancedAssembly;
        private bool _disableOptimizations;
        private bool _enableFastMath;

        internal string Name { get; }

        /// <summary>
        /// Gets a boolean indicating whether burst is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => EnableBurstCompilation && !_forceDisableBurstCompilation;
        }

        /// <summary>
        /// Gets or sets a boolean to enable or disable compilation of burst jobs.
        /// </summary>
        public bool EnableBurstCompilation
        {
            get => _enableBurstCompilation;
            set
            {
                // If we are in the global settings, and we are forcing to no burst compilation
                if (Name == GlobalSettingsName && _forceDisableBurstCompilation) value = false;

                bool changed = _enableBurstCompilation != value;

#if UNITY_EDITOR
                // Prevent Burst compilation being enabled while in PlayMode, because
                // we can't currently support this for jobs.
                if (Name == GlobalSettingsName && changed && value && UnityEngine.Application.isPlaying)
                {
                    throw new InvalidOperationException("Burst compilation can't be switched on while in PlayMode");
                }
#endif

                _enableBurstCompilation = value;

                // Modify only JobsUtility.JobCompilerEnabled when modifying global settings
                if (Name == GlobalSettingsName)
                {
                    // We need also to disable jobs as functions are being cached by the job system
                    // and when we ask for disabling burst, we are also asking the job system
                    // to no longer use the cached functions
                    JobsUtility.JobCompilerEnabled = value;

                    if (changed)
                    {
                        // Send the command to the compiler service
                        if (value)
                        {
                            BurstCompiler.Enable();
                        }
                        else
                        {
                            BurstCompiler.Disable();
                        }
                    }
                }

                if (changed)
                {
                    OnOptionsChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean to force the compilation of all burst jobs synchronously.
        /// </summary>
        /// <remarks>
        /// This is only available at Editor time. Does not have an impact on player mode.
        /// </remarks>
        public bool EnableBurstCompileSynchronously
        {
            get => _enableBurstCompileSynchronously;
            set
            {
                bool changed = _enableBurstCompileSynchronously != value;
                _enableBurstCompileSynchronously = value;
                if (changed) OnOptionsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a boolean to enable or disable safety checks.
        /// </summary>
        /// <remarks>
        /// This is only available at Editor time. Does not have an impact on player mode.
        /// </remarks>
        public bool EnableBurstSafetyChecks
        {
            get => _enableBurstSafetyChecks;
            set
            {
                bool changed = _enableBurstSafetyChecks != value;
                _enableBurstSafetyChecks = value;
                if (changed) OnOptionsChanged();
            }
        }

        internal bool EnableEnhancedAssembly
        {
            get => _enableEnhancedAssembly;
            set
            {
                bool changed = _enableEnhancedAssembly != value;
                _enableEnhancedAssembly = value;
                if (changed) OnOptionsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a boolean to enable or disable compiler optimizations
        /// </summary>
        /// <remarks>
        /// This is only available at Editor time. Does not have an impact on player mode.
        /// </remarks>
        public bool DisableOptimizations
        {
            get => _disableOptimizations;
            set
            {
                bool changed = _disableOptimizations != value;
                _disableOptimizations = value;
                if (changed) OnOptionsChanged();
            }
        }

        /// <summary>
        /// Gets or sets a boolean to enable or disable fast math calculation.
        /// </summary>
        /// <remarks>
        /// This is only available at Editor time. Does not have an impact on player mode.
        /// </remarks>
        public bool EnableFastMath
        {
            get => _enableFastMath;
            set
            {
                bool changed = _enableFastMath != value;
                _enableFastMath = value;
                if (changed) OnOptionsChanged();
            }
        }

        internal bool EnableBurstTimings
        {
            get => _enableBurstTimings;
            set
            {
                bool changed = _enableBurstTimings != value;
                _enableBurstTimings = value;
                if (changed) OnOptionsChanged();
            }
        }

        internal Action OptionsChanged { get; set; }

        internal BurstCompilerOptions Clone()
        {
            // WARNING: for some reason MemberwiseClone() is NOT WORKING on Mono/Unity
            // so we are creating a manual clone
            var clone = new BurstCompilerOptions
            {
                EnableBurstCompilation = EnableBurstCompilation,
                EnableBurstCompileSynchronously = EnableBurstCompileSynchronously,
                EnableBurstSafetyChecks = EnableBurstSafetyChecks,
                EnableEnhancedAssembly = EnableEnhancedAssembly,
                EnableFastMath = EnableFastMath,
                DisableOptimizations = DisableOptimizations,
                EnableBurstTimings = EnableBurstTimings
            };
            return clone;
        }

        private static bool TryGetAttribute(MemberInfo member, out BurstCompileAttribute attribute)
        {
            attribute = null;
            // We don't fail if member == null as this method is being called by native code and doesn't expect to crash
            if (member == null)
            {
                return false;
            }

            // Fetch options from attribute
            attribute = GetBurstCompileAttribute(member);
            return attribute != null;
        }

        private static BurstCompileAttribute GetBurstCompileAttribute(MemberInfo memberInfo)
        {
            var result = memberInfo.GetCustomAttribute<BurstCompileAttribute>();
            if (result != null)
            {
                return result;
            }

            foreach (var a in memberInfo.GetCustomAttributes())
            {
                if (a.GetType().FullName == "Burst.Compiler.IL.Tests.TestCompilerAttribute")
                {
                    return new BurstCompileAttribute(FloatPrecision.Standard, FloatMode.Default) { CompileSynchronously = true };
                }
            }

            return null;
        }

        internal static bool HasBurstCompileAttribute(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            BurstCompileAttribute attr;
            return TryGetAttribute(member, out attr);
        }

        /// <summary>
        /// Gets the options for the specified member. Returns <c>false</c> if the `[BurstCompile]` attribute was not found
        /// </summary>
        /// <returns><c>false</c> if the `[BurstCompile]` attribute was not found; otherwise <c>true</c></returns>
        internal bool TryGetOptions(MemberInfo member, bool isJit, out string flagsOut)
        {
            flagsOut = null;
            BurstCompileAttribute attr;
            if (!TryGetAttribute(member, out attr))
            {
                return false;
            }

            // Add debug to Jit options instead of passing it here
            // attr.Debug

            var flagsBuilderOut = new StringBuilder();

            if (isJit && (attr.CompileSynchronously || _forceBurstCompilationSynchronously || EnableBurstCompileSynchronously))
            {
                AddOption(flagsBuilderOut, GetOption(OptionJitEnableSynchronousCompilation));
            }

            if (attr.FloatMode != FloatMode.Default)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFloatMode, attr.FloatMode));
            }

            if (attr.FloatPrecision != FloatPrecision.Standard)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFloatPrecision, attr.FloatPrecision));
            }

            if (isJit && EnableEnhancedAssembly)
            {
                AddOption(flagsBuilderOut, GetOption(OptionDebug));
            }

            if (DisableOptimizations)
            {
                AddOption(flagsBuilderOut, GetOption(OptionDisableOpt));
            }

            if (EnableFastMath)
            {
                AddOption(flagsBuilderOut, GetOption(OptionFastMath));
            }

            if (attr.Options != null)
            {
                foreach (var option in attr.Options)
                {
                    if (!String.IsNullOrEmpty(option))
                    {
                        AddOption(flagsBuilderOut, option);
                    }
                }
            }

            // Fetch options from attribute
            if (EnableBurstSafetyChecks)
            {
                AddOption(flagsBuilderOut, GetOption(OptionSafetyChecks));
            }
            else
            {
                AddOption(flagsBuilderOut, GetOption(OptionDisableSafetyChecks));
                // Enable NoAlias ahen safety checks are disable
                AddOption(flagsBuilderOut, GetOption(OptionNoAlias));
            }

            if (isJit && EnableBurstTimings)
            {
                AddOption(flagsBuilderOut, GetOption(OptionJitLogTimings));
            }

            flagsOut = flagsBuilderOut.ToString();
            return true;
        }

        private static void AddOption(StringBuilder builder, string option)
        {
            if (builder.Length != 0)
                builder.Append('\n'); // Use \n to separate options

            builder.Append(option);
        }
        internal static string GetOption(string optionName, object value = null)
        {
            if (optionName == null) throw new ArgumentNullException(nameof(optionName));
            return "--" + optionName + (value ?? String.Empty);
        }

        private void OnOptionsChanged()
        {
            OptionsChanged?.Invoke();
        }

#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
        /// <summary>
        /// Static initializer based on command line arguments
        /// </summary>
        static BurstCompilerOptions()
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                switch (arg)
                {
                    case DisableCompilationArg:
                        _forceDisableBurstCompilation = true;
                        break;
                    case ForceSynchronousCompilationArg:
                        _forceBurstCompilationSynchronously = false;
                        break;
                }
            }
        }
#endif
#endif // !BURST_INTERNAL
    }

#if UNITY_EDITOR
    // NOTE: This must be synchronized with Backend.TargetPlatform
    internal enum TargetPlatform
    {
        Windows = 0,
        macOS = 1,
        Linux = 2,
        Android = 3,
        iOS = 4,
        PS4 = 5,
        XboxOne = 6,
        WASM = 7,
        UWP = 8,
        Lumin = 9,
        Switch = 10,
        Stadia = 11,
    }

    // NOTE: This must be synchronized with Backend.TargetCpu
    internal enum TargetCpu
    {
        Auto = 0,
        X86_SSE2 = 1,
        X86_SSE4 = 2,
        X64_SSE2 = 3,
        X64_SSE4 = 4,
        AVX = 5,
        AVX2 = 6,
        AVX512 = 7,
        WASM32 = 8,
        ARMV7A_NEON32 = 9,
        ARMV8A_AARCH64 = 10,
        THUMB2_NEON32 = 11,
    }
#endif

    /// <summary>
    /// Flags used by <see cref="NativeCompiler.CompileMethod"/> to dump intermediate compiler results.
    /// </summary>
    [Flags]
#if BURST_INTERNAL
    public enum NativeDumpFlags
#else
    internal enum NativeDumpFlags
#endif
    {
        /// <summary>
        /// Nothing is selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Dumps the IL of the method being compiled
        /// </summary>
        IL = 1 << 0,

        /// <summary>
        /// Dumps the reformated backend API Calls
        /// </summary>
        Backend = 1 << 1,

        /// <summary>
        /// Dumps the generated module without optimizations
        /// </summary>
        IR = 1 << 2,

        /// <summary>
        /// Dumps the generated backend code after optimizations (if enabled)
        /// </summary>
        IROptimized = 1 << 3,

        /// <summary>
        /// Dumps the generated ASM code (by default will also compile the function as using <see cref="Function"/> flag)
        /// </summary>
        Asm = 1 << 4,

        /// <summary>
        /// Generate the native code
        /// </summary>
        Function = 1 << 5,

        /// <summary>
        /// Dumps the result of analysis
        /// </summary>
        Analysis = 1 << 6,

        /// <summary>
        /// Dumps the diagnostics from optimisation
        /// </summary>
        IRPassAnalysis = 1 << 7,

        /// <summary>
        /// Dumps the IL before all transformation of the method being compiled
        /// </summary>
        ILPre = 1 << 8,

        /// <summary>
        /// Dumps all normal output.
        /// </summary>
        All = IL | ILPre | IR | IROptimized | Asm | Function | Analysis | IRPassAnalysis
    }
}
#endif