#if ENABLE_BURST_AOT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.Burst.LowLevel;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEditor.Scripting;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;
using CompilerMessageType = UnityEditor.Scripting.Compilers.CompilerMessageType;
using Debug = UnityEngine.Debug;

namespace Unity.Burst.Editor
{
    // For static builds, there are two different approaches:
    // Postprocessing adds the libraries after Unity is done building,
    // for platforms that need to build a project file, etc.
    // Preprocessing simply adds the libraries to the Unity build,
    // for platforms where Unity can directly build an app.
    using static BurstCompilerOptions;
    internal class StaticPreProcessor : IPreprocessBuildWithReport
    {
        private const string TempSourceLibrary = @"Temp/StagingArea/SourcePlugins";
        private const string TempStaticLibrary = @"Temp/StagingArea/NativePlugins";
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            var aotSettingsForTarget = BurstPlatformAotSettings.GetOrCreateSettings(report.summary.platform);

            // Early exit if burst is not activated
            if (aotSettingsForTarget.DisableBurstCompilation)
            {
                return;
            }
            if(report.summary.platform == BuildTarget.Switch)
            {
                // add the static lib, and the c++ shim
                string burstCppLinkFile = "lib_burst_generated.cpp";
                string burstStaticLibFile = "lib_burst_generated.a";
                string cppPath = Path.Combine(TempSourceLibrary, burstCppLinkFile);
                string libPath = Path.Combine(TempStaticLibrary, burstStaticLibFile);
                if(!Directory.Exists(TempSourceLibrary))
                {
                    Directory.CreateDirectory(TempSourceLibrary);
                    Directory.CreateDirectory(TempSourceLibrary);
                }
                File.WriteAllText(cppPath, @"
extern ""C""
{
    void Staticburst_initialize(void* );
    void* StaticBurstStaticMethodLookup(void* );

    int burst_enable_static_linkage = 1;
    void burst_initialize(void* i) { Staticburst_initialize(i); }
    void* BurstStaticMethodLookup(void* i) { return StaticBurstStaticMethodLookup(i); }
}
");
            }
        }
    }
    /// <summary>
    /// Integration of the burst AOT compiler into the Unity build player pipeline
    /// </summary>
    internal class BurstAotCompiler : IPostBuildPlayerScriptDLLs
    {
        private const string BurstAotCompilerExecutable = "bcl.exe";
        private const string TempStaging = @"Temp/StagingArea/";
        private const string TempStagingManaged = TempStaging + @"Data/Managed/";
        private const string TempStagingPlugins = @"Data/Plugins/";
        private const string LibraryPlayerScriptAssemblies = "Library/PlayerScriptAssemblies";

        int IOrderedCallback.callbackOrder => 0;

        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            var step = report.BeginBuildStep("burst");
            try
            {
                OnPostBuildPlayerScriptDLLsImpl(report);
            }
            finally
            {
                report.EndBuildStep(step);
            }
        }

        private void OnPostBuildPlayerScriptDLLsImpl(BuildReport report)
        {
            var aotSettingsForTarget = BurstPlatformAotSettings.GetOrCreateSettings(report.summary.platform);

            // Early exit if burst is not activated or the platform is not supported
            if (aotSettingsForTarget.DisableBurstCompilation || !IsSupportedPlatform(report.summary.platform))
            {
                return;
            }

            var commonOptions = new List<string>();
            var stagingFolder = Path.GetFullPath(TempStagingManaged);

            var playerAssemblies = GetPlayerAssemblies(report);

            // grab the location of the root of the player folder - for handling nda platforms that require keys
            var keyFolder = BuildPipeline.GetPlaybackEngineDirectory(report.summary.platform, BuildOptions.None);
            commonOptions.Add(GetOption(OptionAotKeyFolder, keyFolder));
            commonOptions.Add(GetOption(OptionAotDecodeFolder, Path.Combine(Environment.CurrentDirectory, "Library", "Burst")));

            // Extract the TargetPlatform and Cpu from the current build settings
            TargetCpu targetCpu;
            var targetPlatform = GetTargetPlatformAndDefaultCpu(report.summary.platform, out targetCpu);
            commonOptions.Add(GetOption(OptionPlatform, targetPlatform));

            // --------------------------------------------------------------------------------------------------------
            // 1) Calculate AssemblyFolders
            // These are the folders to look for assembly resolution
            // --------------------------------------------------------------------------------------------------------
            var assemblyFolders = new List<string> { stagingFolder };
            if (report.summary.platform == BuildTarget.WSAPlayer
                || report.summary.platform == BuildTarget.XboxOne)
            {
                // On UWP, not all assemblies are copied to StagingArea, so we want to
                // find all directories that we can reference assemblies from
                // If we don't do this, we will crash with AssemblyResolutionException
                // when following type references.
                foreach (var assembly in playerAssemblies)
                {
                    foreach (var assemblyRef in assembly.compiledAssemblyReferences)
                    {
                        // Exclude folders with assemblies already compiled in the `folder`
                        var assemblyName = Path.GetFileName(assemblyRef);
                        if (assemblyName != null && File.Exists(Path.Combine(stagingFolder, assemblyName)))
                        {
                            continue;
                        }

                        var directory = Path.GetDirectoryName(assemblyRef);
                        if (directory != null)
                        {
                            var fullPath = Path.GetFullPath(directory);
                            if (IsMonoReferenceAssemblyDirectory(fullPath) || IsDotNetStandardAssemblyDirectory(fullPath))
                            {
                                // Don't pass reference assemblies to burst because they contain methods without implementation
                                // If burst accidentally resolves them, it will emit calls to burst_abort.
                                fullPath = Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/lib/mono/unityaot");
                                fullPath = Path.GetFullPath(fullPath); // GetFullPath will normalize path separators to OS native format
                                if (!assemblyFolders.Contains(fullPath))
                                    assemblyFolders.Add(fullPath);

                                fullPath = Path.Combine(fullPath, "Facades");
                                if (!assemblyFolders.Contains(fullPath))
                                    assemblyFolders.Add(fullPath);
                            }
                            else if (!assemblyFolders.Contains(fullPath))
                            {
                                assemblyFolders.Add(fullPath);
                            }
                        }
                    }
                }
            }

            // Copy assembly used during staging to have a trace
            if (BurstLoader.IsDebugging)
            {
                try
                {
                    var copyAssemblyFolder = Path.Combine(Environment.CurrentDirectory, "Logs", "StagingAssemblies");
                    try
                    {
                        if (Directory.Exists(copyAssemblyFolder)) Directory.Delete(copyAssemblyFolder);
                    }
                    catch
                    {
                    }

                    if (!Directory.Exists(copyAssemblyFolder)) Directory.CreateDirectory(copyAssemblyFolder);
                    foreach (var file in Directory.EnumerateFiles(stagingFolder))
                    {
                        File.Copy(file, Path.Combine(copyAssemblyFolder, Path.GetFileName(file)));
                    }
                }
                catch
                {
                }
            }

            // --------------------------------------------------------------------------------------------------------
            // 2) Calculate root assemblies
            // These are the assemblies that the compiler will look for methods to compile
            // This list doesn't typically include .NET runtime assemblies but only assemblies compiled as part
            // of the current Unity project
            // --------------------------------------------------------------------------------------------------------
            var rootAssemblies = new List<string>();
            foreach (var playerAssembly in playerAssemblies)
            {
                // the file at path `playerAssembly.outputPath` is actually not on the disk
                // while it is in the staging folder because OnPostBuildPlayerScriptDLLs is being called once the files are already
                // transferred to the staging folder, so we are going to work from it but we are reusing the file names that we got earlier
                var playerAssemblyPathToStaging = Path.Combine(stagingFolder, Path.GetFileName(playerAssembly.outputPath));
                if (!File.Exists(playerAssemblyPathToStaging))
                {
                    Debug.LogWarning($"Unable to find player assembly: {playerAssemblyPathToStaging}");
                }
                else
                {
                    rootAssemblies.Add(playerAssemblyPathToStaging);
                }
            }

            commonOptions.AddRange(assemblyFolders.Select(folder => GetOption(OptionAotAssemblyFolder, folder)));


            // --------------------------------------------------------------------------------------------------------
            // 3) Calculate the different target CPU combinations for the specified OS
            //
            // Typically, on some platforms like iOS we can be asked to compile a ARM32 and ARM64 CPU version
            // --------------------------------------------------------------------------------------------------------
            var combinations = CollectCombinations(targetPlatform, targetCpu, report);

            // --------------------------------------------------------------------------------------------------------
            // 4) Compile each combination
            //
            // Here bcl.exe is called for each target CPU combination
            // --------------------------------------------------------------------------------------------------------

            string debugLogFile = null;
            if (BurstLoader.IsDebugging)
            {
                // Reset log files
                try
                {
                    var logDir = Path.Combine(Environment.CurrentDirectory, "Logs");
                    debugLogFile = Path.Combine(logDir, "burst_bcl_editor.log");
                    if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
                    File.WriteAllText(debugLogFile, string.Empty);
                }
                catch
                {
                    debugLogFile = null;
                }
            }

            // Log the targets generated by BurstReflection.FindExecuteMethods
            foreach (var combination in combinations)
            {
                // Gets the output folder
                var stagingOutputFolder = Path.GetFullPath(Path.Combine(TempStaging, combination.OutputPath));
                var outputFilePrefix = Path.Combine(stagingOutputFolder, combination.LibraryName);

                var options = new List<string>(commonOptions)
                {
                    GetOption(OptionAotOutputPath, outputFilePrefix),
                    GetOption(OptionTarget, combination.TargetCpu)
                };

                if (targetPlatform == TargetPlatform.iOS || targetPlatform == TargetPlatform.Switch)
                {
                    options.Add(GetOption(OptionStaticLinkage));
                }

                // finally add method group options
                options.AddRange(rootAssemblies.Select(path => GetOption(OptionRootAssembly, path)));

                // Log the targets generated by BurstReflection.FindExecuteMethods
                if (BurstLoader.IsDebugging && debugLogFile != null)
                {
                    try
                    {
                        var writer = new StringWriter();
                        writer.WriteLine("-----------------------------------------------------------");
                        writer.WriteLine("Combination: " + combination);
                        writer.WriteLine("-----------------------------------------------------------");

                        foreach (var option in options)
                        {
                            writer.WriteLine(option);
                        }

                        writer.WriteLine("Assemblies in AssemblyFolders:");
                        foreach (var assemblyFolder in assemblyFolders)
                        {
                            writer.WriteLine("|- Folder: " + assemblyFolder);
                            foreach (var assemblyOrDll in Directory.EnumerateFiles(assemblyFolder, "*.dll"))
                            {
                                var fileInfo = new FileInfo(assemblyOrDll);
                                writer.WriteLine("   |- " + assemblyOrDll +  " Size: " + fileInfo.Length + " Date: " + fileInfo.LastWriteTime);
                            }
                        }
                        
                        File.AppendAllText(debugLogFile, writer.ToString());
                    }
                    catch
                    {
                        // ignored
                    }
                }

                // Write current options to the response file
                var responseFile = Path.GetTempFileName();
                File.WriteAllLines(responseFile, options);
                
                if (BurstLoader.IsDebugging)
                {
                    Debug.Log($"bcl @{responseFile}\n\nResponse File:\n" + string.Join("\n", options));
                }

                try
                {
                    string generatedDebugInformationInOutput = "";
                    if ((report.summary.options & BuildOptions.Development) != 0)
                    {
                        generatedDebugInformationInOutput = GetOption(OptionDebug);
                    }

                    BclRunner.RunManagedProgram(Path.Combine(BurstLoader.RuntimePath, BurstAotCompilerExecutable),
                        $"{generatedDebugInformationInOutput} \"@{responseFile}\"",
                        new BclOutputErrorParser(),
                        report);
                }
                catch (BuildFailedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new BuildFailedException(e);
                }
            }
        }

        /// <summary>
        /// Collect CPU combinations for the specified TargetPlatform and TargetCPU
        /// </summary>
        /// <param name="targetPlatform">The target platform (e.g Windows)</param>
        /// <param name="targetCpu">The target CPU (e.g X64_SSE4)</param>
        /// <param name="report">Error reporting</param>
        /// <returns>The list of CPU combinations</returns>
        private static List<BurstOutputCombination> CollectCombinations(TargetPlatform targetPlatform, TargetCpu targetCpu, BuildReport report)
        {
            var combinations = new List<BurstOutputCombination>();

            if (targetPlatform == TargetPlatform.macOS)
            {
                // NOTE: OSX has a special folder for the plugin
                // Declared in GetStagingAreaPluginsFolder
                // PlatformDependent\OSXPlayer\Extensions\Managed\OSXDesktopStandalonePostProcessor.cs
#if UNITY_2019_3_OR_NEWER
                combinations.Add(new BurstOutputCombination(Path.Combine(Path.GetFileName(report.summary.outputPath), "Contents", "Plugins"), targetCpu));
#else
                combinations.Add(new BurstOutputCombination("UnityPlayer.app/Contents/Plugins", targetCpu));
#endif
            }
            else if (targetPlatform == TargetPlatform.iOS)
            {
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    Debug.LogWarning("Burst Cross Compilation to iOS for standalone player, is only supported on OSX Editor at this time, burst is disabled for this build.");
                }
                else
                {
                    var targetArchitecture = (IOSArchitecture) UnityEditor.PlayerSettings.GetArchitecture(report.summary.platformGroup);
                    if (targetArchitecture == IOSArchitecture.ARMv7 || targetArchitecture == IOSArchitecture.Universal)
                    {
                        // PlatformDependent\iPhonePlayer\Extensions\Common\BuildPostProcessor.cs
                        combinations.Add(new BurstOutputCombination("StaticLibraries", TargetCpu.ARMV7A_NEON32, DefaultLibraryName + "32"));
                    }

                    if (targetArchitecture == IOSArchitecture.ARM64 || targetArchitecture == IOSArchitecture.Universal)
                    {
                        // PlatformDependent\iPhonePlayer\Extensions\Common\BuildPostProcessor.cs
                        combinations.Add(new BurstOutputCombination("StaticLibraries", TargetCpu.ARMV8A_AARCH64, DefaultLibraryName + "64"));
                    }
                }
            }
            else if (targetPlatform == TargetPlatform.Android)
            {
                // TODO: would be better to query AndroidNdkRoot (but thats not exposed from unity)
                string ndkRoot = null;
                var targetAPILevel = PlayerSettings.Android.GetMinTargetAPILevel();
#if UNITY_2019_3_OR_NEWER && UNITY_ANDROID
                ndkRoot = UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath;
#elif UNITY_2019_1_OR_NEWER
                // 2019.1 now has an embedded ndk
                if (EditorPrefs.HasKey("NdkUseEmbedded"))
                {
                    if (EditorPrefs.GetBool("NdkUseEmbedded"))
                    {
                        ndkRoot = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None), "NDK");
                    }
                    else
                    {
                        ndkRoot = EditorPrefs.GetString("AndroidNdkRootR16b");
                    }
                }
#endif

                // If we still don't have a valid root, try the old key
                if (string.IsNullOrEmpty(ndkRoot))
                {
                    ndkRoot = EditorPrefs.GetString("AndroidNdkRoot");
                }

                // Verify the directory at least exists, if not we fall back to ANDROID_NDK_ROOT current setting
                if (!string.IsNullOrEmpty(ndkRoot) && !Directory.Exists(ndkRoot))
                {
                    ndkRoot = null;
                }

                // Always set the ANDROID_NDK_ROOT (if we got a valid result from above), so BCL knows where to find the Android toolchain and its the one the user expects
                if (!string.IsNullOrEmpty(ndkRoot))
                {
                    Environment.SetEnvironmentVariable("ANDROID_NDK_ROOT", ndkRoot);
                }

                Environment.SetEnvironmentVariable("BURST_ANDROID_MIN_API_LEVEL", $"{targetAPILevel}");

                var androidTargetArch = UnityEditor.PlayerSettings.Android.targetArchitectures;
                if ((androidTargetArch & AndroidArchitecture.ARMv7) != 0)
                {
                    combinations.Add(new BurstOutputCombination("libs/armeabi-v7a", TargetCpu.ARMV7A_NEON32));
                }

                if ((androidTargetArch & AndroidArchitecture.ARM64) != 0)
                {
                    combinations.Add(new BurstOutputCombination("libs/arm64-v8a", TargetCpu.ARMV8A_AARCH64));
                }
#if !UNITY_2019_2_OR_NEWER
                if ((androidTargetArch & AndroidArchitecture.X86) != 0)
                {
                    combinations.Add(new BurstOutputCombination("libs/x86", TargetCpu.X86_SSE2));
                }
#endif
            }
            else if (targetPlatform == TargetPlatform.UWP)
            {
                // TODO: Make it configurable for x86 (sse2, sse4)
                combinations.Add(new BurstOutputCombination("Plugins/x64", TargetCpu.X64_SSE4));
                combinations.Add(new BurstOutputCombination("Plugins/x86", TargetCpu.X86_SSE2));
                combinations.Add(new BurstOutputCombination("Plugins/ARM", TargetCpu.THUMB2_NEON32));
                combinations.Add(new BurstOutputCombination("Plugins/ARM64", TargetCpu.ARMV8A_AARCH64));
            }
            else if (targetPlatform == TargetPlatform.Lumin)
            {
                // Set the LUMINSDK_UNITY so bcl.exe will be able to find the SDK
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LUMINSDK_UNITY")))
                {
                    var sdkRoot = EditorPrefs.GetString("LuminSDKRoot");
                    if (!string.IsNullOrEmpty(sdkRoot))
                    {
                        Environment.SetEnvironmentVariable("LUMINSDK_UNITY", sdkRoot);
                    }
                }
                combinations.Add(new BurstOutputCombination("Data/Plugins/", targetCpu));
            }
            else if (targetPlatform == TargetPlatform.Switch)
            {
                combinations.Add(new BurstOutputCombination("NativePlugins/", targetCpu));
            }
#if UNITY_2019_3_OR_NEWER
            else if (targetPlatform == TargetPlatform.Stadia)
            {
                combinations.Add(new BurstOutputCombination("NativePlugins", targetCpu));
            }
#endif
            else
            {
                combinations.Add(new BurstOutputCombination("Data/Plugins/", targetCpu));
            }

            return combinations;
        }

        private static Assembly[] GetPlayerAssemblies(BuildReport report)
        {
            // We need to build the list of root assemblies based from the "PlayerScriptAssemblies" folder.
            // This is so we compile the versions of the library built for the individual platforms, not the editor version. 
            var oldOutputDir = EditorCompilationInterface.GetCompileScriptsOutputDirectory();
            try
            {
                EditorCompilationInterface.SetCompileScriptsOutputDirectory(LibraryPlayerScriptAssemblies);

                var shouldIncludeTestAssemblies = report.summary.options.HasFlag(BuildOptions.IncludeTestAssemblies);

#if UNITY_2019_3_OR_NEWER
                return CompilationPipeline.GetAssemblies(shouldIncludeTestAssemblies ? AssembliesType.Player : AssembliesType.PlayerWithoutTestAssemblies);
#else
                var compilationOptions = EditorCompilationInterface.GetAdditionalEditorScriptCompilationOptions();
                if (shouldIncludeTestAssemblies)
                {
                    compilationOptions |= EditorScriptCompilationOptions.BuildingIncludingTestAssemblies;
                }

#if UNITY_2019_2_OR_NEWER
                return CompilationPipeline.GetPlayerAssemblies(EditorCompilationInterface.Instance, compilationOptions, null);
#else
                return CompilationPipeline.GetPlayerAssemblies(EditorCompilationInterface.Instance, compilationOptions);
#endif
#endif
            }
            finally
            {
                EditorCompilationInterface.SetCompileScriptsOutputDirectory(oldOutputDir);  // restore output directory back to original value
            }
        }

        private static bool IsMonoReferenceAssemblyDirectory(string path)
        {
            var editorDir = Path.GetFullPath(EditorApplication.applicationContentsPath);
            return path.IndexOf(editorDir, StringComparison.OrdinalIgnoreCase) != -1 && path.IndexOf("MonoBleedingEdge", StringComparison.OrdinalIgnoreCase) != -1 && path.IndexOf("-api", StringComparison.OrdinalIgnoreCase) != -1;
        }

        private static bool IsDotNetStandardAssemblyDirectory(string path)
        {
            var editorDir = Path.GetFullPath(EditorApplication.applicationContentsPath);
            return path.IndexOf(editorDir, StringComparison.OrdinalIgnoreCase) != -1 && path.IndexOf("netstandard", StringComparison.OrdinalIgnoreCase) != -1 && path.IndexOf("shims", StringComparison.OrdinalIgnoreCase) != -1;
        }

        private static TargetPlatform GetTargetPlatformAndDefaultCpu(BuildTarget target, out TargetCpu targetCpu)
        {
            var platform = TryGetTargetPlatform(target, out targetCpu);
            if (!platform.HasValue)
            {
                throw new NotSupportedException("The target platform " + target + " is not supported by the burst compiler");
            }
            return platform.Value;
        }

        private static bool IsSupportedPlatform(BuildTarget target)
        {
            TargetCpu cpu;
            return TryGetTargetPlatform(target, out cpu).HasValue;
        }

        private static TargetPlatform? TryGetTargetPlatform(BuildTarget target, out TargetCpu targetCpu)
        {
            // TODO: Add support for multi-CPU switch
            targetCpu = TargetCpu.Auto;
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    targetCpu = TargetCpu.X86_SSE4;
                    return TargetPlatform.Windows;
                case BuildTarget.StandaloneWindows64:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.Windows;
                case BuildTarget.StandaloneOSX:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.macOS;
#if !UNITY_2019_2_OR_NEWER
                //32 bit linux support was deprecated
                case BuildTarget.StandaloneLinux:
                    targetCpu = TargetCpu.X86_SSE4;
                    return TargetPlatform.Linux;
#endif
                case BuildTarget.StandaloneLinux64:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.Linux;
                case BuildTarget.WSAPlayer:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.UWP;
                case BuildTarget.XboxOne:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.XboxOne;
                case BuildTarget.PS4:
                    targetCpu = TargetCpu.X64_SSE4;
                    return TargetPlatform.PS4;
                case BuildTarget.Android:
                    targetCpu = TargetCpu.ARMV7A_NEON32;
                    return TargetPlatform.Android;
                case BuildTarget.iOS:
                    targetCpu = TargetCpu.ARMV7A_NEON32;
                    return TargetPlatform.iOS;
                case BuildTarget.Lumin:
                    targetCpu = TargetCpu.ARMV8A_AARCH64;
                    return TargetPlatform.Lumin;
                case BuildTarget.Switch:
                    targetCpu = TargetCpu.ARMV8A_AARCH64;
                    return TargetPlatform.Switch;
#if UNITY_2019_3_OR_NEWER
                case BuildTarget.Stadia:
                    targetCpu = TargetCpu.AVX2;
                    return TargetPlatform.Stadia;
#endif
            }
            return null;
        }

        /// <summary>
        /// Not exposed by Unity Editor today.
        /// This is a copy of the Architecture enum from `PlatformDependent\iPhonePlayer\Extensions\Common\BuildPostProcessor.cs`
        /// </summary>
        private enum IOSArchitecture
        {
            ARMv7,
            ARM64,
            Universal
        }

        /// <summary>
        /// Defines an output path (for the generated code) and the target CPU
        /// </summary>
        private struct BurstOutputCombination
        {
            public readonly TargetCpu TargetCpu;
            public readonly string OutputPath;
            public readonly string LibraryName;

            public BurstOutputCombination(string outputPath, TargetCpu targetCpu, string libraryName = DefaultLibraryName)
            {
                TargetCpu = targetCpu;
                OutputPath = outputPath;
                LibraryName = libraryName;
            }

            public override string ToString()
            {
                return $"{nameof(TargetCpu)}: {TargetCpu}, {nameof(OutputPath)}: {OutputPath}, {nameof(LibraryName)}: {LibraryName}";
            }
        }

        private class BclRunner
        {
            private static readonly Regex MatchVersion = new Regex(@"com.unity.burst@(\d+.*?)[\\/]");

            public static void RunManagedProgram(string exe, string args, CompilerOutputParserBase parser, BuildReport report)
            {
                RunManagedProgram(exe, args, Application.dataPath + "/..", parser, report);
            }

            private static void RunManagedProgram(
              string exe,
              string args,
              string workingDirectory,
              CompilerOutputParserBase parser,
              BuildReport report)
            {
                Program p;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    ProcessStartInfo si = new ProcessStartInfo()
                    {
                        Arguments = args,
                        CreateNoWindow = true,
                        FileName = exe
                    };
                    p = new Program(si);
                }
                else
                {
                    p = (Program) new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), (string) null, exe, args, false, null);
                }

                RunProgram(p, exe, args, workingDirectory, parser, report);
            }

            private static void RunProgram(
              Program p,
              string exe,
              string args,
              string workingDirectory,
              CompilerOutputParserBase parser,
              BuildReport report)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                using (p)
                {
                    p.GetProcessStartInfo().WorkingDirectory = workingDirectory;
                    p.Start();
                    p.WaitForExit();
                    stopwatch.Stop();

                    Console.WriteLine("{0} exited after {1} ms.", (object)exe, (object)stopwatch.ElapsedMilliseconds);
                    IEnumerable<UnityEditor.Scripting.Compilers.CompilerMessage> compilerMessages = null;
                    string[] errorOutput = p.GetErrorOutput();
                    string[] standardOutput = p.GetStandardOutput();
                    if (parser != null)
                    {
                        compilerMessages = parser.Parse(errorOutput, standardOutput, true, "n/a (burst)");
                    }

                    var errorMessageBuilder = new StringBuilder();
                    if (p.ExitCode != 0)
                    {
                        if (compilerMessages != null)
                        {
                            foreach (UnityEditor.Scripting.Compilers.CompilerMessage compilerMessage in compilerMessages)
                            {
                                Debug.LogPlayerBuildError(compilerMessage.message, compilerMessage.file, compilerMessage.line, compilerMessage.column);
                            }
                        }

                        // We try to output the version in the heading error if we can
                        var matchVersion = MatchVersion.Match(exe);
                        errorMessageBuilder.Append(matchVersion.Success ?
                            "Burst compiler (" + matchVersion.Groups[1].Value + ") failed running" :
                            "Burst compiler failed running");
                        errorMessageBuilder.AppendLine();
                        errorMessageBuilder.AppendLine();
                        // Don't output the path if we are not burst-debugging or the exe exist
                        if (BurstLoader.IsDebugging || !File.Exists(exe))
                        {
                            errorMessageBuilder.Append(exe).Append(" ").Append(args);
                            errorMessageBuilder.AppendLine();
                            errorMessageBuilder.AppendLine();
                        }

                        errorMessageBuilder.AppendLine("stdout:");
                        foreach (string str in standardOutput)
                            errorMessageBuilder.AppendLine(str);
                        errorMessageBuilder.AppendLine("stderr:");
                        foreach (string str in errorOutput)
                            errorMessageBuilder.AppendLine(str);

                        throw new BuildFailedException(errorMessageBuilder.ToString());
                    }
                    Console.WriteLine(p.GetAllOutput());
                }
            }
        }

        /// <summary>
        /// Internal class used to parse bcl output errors
        /// </summary>
        private class BclOutputErrorParser : CompilerOutputParserBase
        {
            // Format of an error message:
            //
            //C:\work\burst\src\Burst.Compiler.IL.Tests\Program.cs(17,9): error: Loading a managed string literal is not supported by burst
            // at Buggy.NiceBug() (at C:\work\burst\src\Burst.Compiler.IL.Tests\Program.cs:17)
            //
            //
            //                                                                [1]    [2]         [3]        [4]         [5]
            //                                                                path   line        col        type        message
            private static readonly Regex MatchLocation = new Regex(@"^(.*?)\((\d+)\s*,\s*(\d+)\):\s*(\w+)\s*:\s*(.*)");

            // Matches " at "
            private static readonly Regex MatchAt = new Regex(@"^\s+at\s+");

            public override IEnumerable<UnityEditor.Scripting.Compilers.CompilerMessage> Parse(
                string[] errorOutput,
                string[] standardOutput,
                bool compilationHadFailure,
                string assemblyName)
            {
                var messages = new List<UnityEditor.Scripting.Compilers.CompilerMessage>();
                var textBuilder = new StringBuilder();
                for (var i = 0; i < errorOutput.Length; i++)
                {
                    string line = errorOutput[i];

                    var message = new UnityEditor.Scripting.Compilers.CompilerMessage {assemblyName = assemblyName};

                    // If we are able to match a location, we can decode it including the following attached " at " lines
                    textBuilder.Clear();

                    var match = MatchLocation.Match(line);
                    if (match.Success)
                    {
                        var path = match.Groups[1].Value;
                        int.TryParse(match.Groups[2].Value, out message.line);
                        int.TryParse(match.Groups[3].Value, out message.column);
                        if (match.Groups[4].Value == "error")
                        {
                            message.type = CompilerMessageType.Error;
                        }
                        else
                        {
                            message.type = CompilerMessageType.Warning;
                        }
                        message.file = !string.IsNullOrEmpty(path) ? path : "unknown";
                        // Replace '\' with '/' to let the editor open the file
                        message.file = message.file.Replace('\\', '/');

                        // Make path relative to project path path
                        var projectPath = Path.GetDirectoryName(Application.dataPath)?.Replace('\\', '/');
                        if (projectPath != null && message.file.StartsWith(projectPath))
                        {
                            message.file = message.file.Substring(projectPath.EndsWith("/") ? projectPath.Length : projectPath.Length + 1);
                        }

                        // debug
                        // textBuilder.AppendLine("line: " + message.line + " column: " + message.column + " error: " + message.type + " file: " + message.file);
                        textBuilder.Append(match.Groups[5].Value);
                    }
                    else
                    {
                        // Don't output any blank line
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        // Otherwise we output an error, but without source location information
                        // so that at least the user can see it directly in the log errors
                        message.type = CompilerMessageType.Error;
                        message.line = 0;
                        message.column = 0;
                        message.file = "unknown";


                        textBuilder.Append(line);
                    }

                    // Collect attached location call context information ("at ...")
                    // we do it for both case (as if we have an exception in bcl we want to print this in a single line)
                    bool isFirstAt = true;
                    for (int j = i + 1; j < errorOutput.Length; j++)
                    {
                        var nextLine = errorOutput[j];
                        if (MatchAt.Match(nextLine).Success)
                        {
                            i++;
                            if (isFirstAt)
                            {
                                textBuilder.AppendLine();
                                isFirstAt = false;
                            }
                            textBuilder.AppendLine(nextLine);
                        }
                        else
                        {
                            break;
                        }
                    }
                    message.message = textBuilder.ToString();

                    messages.Add(message);
                }
                return messages;
            }

            protected override string GetErrorIdentifier()
            {
                throw new NotImplementedException(); // as we overriding the method Parse()
            }

            protected override Regex GetOutputRegex()
            {
                throw new NotImplementedException(); // as we overriding the method Parse()
            }
        }

#if UNITY_EDITOR_OSX
        private class StaticLibraryPostProcessor
        {
            private const string TempSourceLibrary = @"Temp/StagingArea/StaticLibraries";
            [PostProcessBuildAttribute(1)]
            public static void OnPostProcessBuild(BuildTarget target, string path)
            {
                // We only support AOT compilation for ios from a macos host (we require xcrun and the apple tool chains)
                //for other hosts, we simply act as if burst is not being used (an error will be generated by the build aot step)
                //this keeps the behaviour consistent with how it was before static linkage was introduced
                if (target == BuildTarget.iOS)
                {
                    PostAddStaticLibraries(path);
                }
            }

            private static void PostAddStaticLibraries(string path)
            {
                var assm = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly =>
                    assembly.GetName().Name == "UnityEditor.iOS.Extensions.Xcode");
                Type PBXType = assm?.GetType("UnityEditor.iOS.Xcode.PBXProject");
                Type PBXSourceTree = assm?.GetType("UnityEditor.iOS.Xcode.PBXSourceTree");
                if (PBXType != null && PBXSourceTree != null)
                {
                    var project = Activator.CreateInstance(PBXType, null);

                    var _sGetPBXProjectPath = PBXType.GetMethod("GetPBXProjectPath");
                    var _ReadFromFile = PBXType.GetMethod("ReadFromFile");
                    var _sGetUnityTargetName = PBXType.GetMethod("GetUnityTargetName");
                    var _AddFileToBuild = PBXType.GetMethod("AddFileToBuild");
                    var _AddFile = PBXType.GetMethod("AddFile");
                    var _WriteToString = PBXType.GetMethod("WriteToString");

                    var sourcetree = new EnumConverter(PBXSourceTree).ConvertFromString("Source");

                    string sPath = (string)_sGetPBXProjectPath?.Invoke(null, new object[] { path });
                    _ReadFromFile?.Invoke(project, new object[] { sPath });

#if UNITY_2019_3_OR_NEWER
                    var _TargetGuidByName = PBXType.GetMethod("GetUnityFrameworkTargetGuid");
                    string g = (string) _TargetGuidByName?.Invoke(project, null);
#else
                    var _TargetGuidByName = PBXType.GetMethod("TargetGuidByName");
                    string tn = (string) _sGetUnityTargetName?.Invoke(null, null);
                    string g = (string) _TargetGuidByName?.Invoke(project, new object[] {tn});
#endif

                    string srcPath = TempSourceLibrary;
                    string dstPath = "Libraries";
                    string dstCopyPath = Path.Combine(path, dstPath);

                    string burstCppLinkFile = "lib_burst_generated.cpp";
                    string libName = DefaultLibraryName + "32.a";
                    if (File.Exists(Path.Combine(srcPath, libName)))
                    {
                        File.Copy(Path.Combine(srcPath, libName), Path.Combine(dstCopyPath, libName));
                        string fg = (string)_AddFile?.Invoke(project,
                            new object[] { Path.Combine(dstPath, libName), Path.Combine(dstPath, libName), sourcetree });
                        _AddFileToBuild?.Invoke(project, new object[] { g, fg });
                    }

                    libName = DefaultLibraryName + "64.a";
                    if (File.Exists(Path.Combine(srcPath, libName)))
                    {
                        File.Copy(Path.Combine(srcPath, libName), Path.Combine(dstCopyPath, libName));
                        string fg = (string)_AddFile?.Invoke(project,
                            new object[] { Path.Combine(dstPath, libName), Path.Combine(dstPath, libName), sourcetree });
                        _AddFileToBuild?.Invoke(project, new object[] { g, fg });
                    }

                    // Additionally we need a small cpp file (weak symbols won't unfortunately override directly from the libs
                    //presumably due to link order?
                    string cppPath = Path.Combine(dstCopyPath, burstCppLinkFile);
                    File.WriteAllText(cppPath, @"
extern ""C""
{
    void Staticburst_initialize(void* );
    void* StaticBurstStaticMethodLookup(void* );

    int burst_enable_static_linkage = 1;
    void burst_initialize(void* i) { Staticburst_initialize(i); }
    void* BurstStaticMethodLookup(void* i) { return StaticBurstStaticMethodLookup(i); }
}
");
                    cppPath = Path.Combine(dstPath, burstCppLinkFile);
                    string fileg = (string)_AddFile?.Invoke(project, new object[] { cppPath, cppPath, sourcetree });
                    _AddFileToBuild?.Invoke(project, new object[] { g, fileg });

                    string pstring = (string)_WriteToString?.Invoke(project, null);
                    File.WriteAllText(sPath, pstring);
                }
            }
        }
#endif
    }
}
#endif
