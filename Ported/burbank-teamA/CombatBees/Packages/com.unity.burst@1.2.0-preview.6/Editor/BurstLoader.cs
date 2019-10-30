using System;
using System.IO;
using System.Reflection;
using Unity.Burst.LowLevel;
using UnityEditor;
using UnityEditor.Compilation;

namespace Unity.Burst.Editor
{
    /// <summary>
    /// Main entry point for initializing the burst compiler service for both JIT and AOT
    /// </summary>
    [InitializeOnLoad]
    internal class BurstLoader
    {
        /// <summary>
        /// Gets the location to the runtime path of burst.
        /// </summary>
        public static string RuntimePath { get; private set; }

        public static bool IsDebugging { get; private set; }

        public static int DebuggingLevel { get; private set; }

        static BurstLoader()
        {
            // This can be setup to get more diagnostics
            var debuggingStr = Environment.GetEnvironmentVariable("UNITY_BURST_DEBUG");
            IsDebugging = debuggingStr != null;
            if(IsDebugging)
            {
                UnityEngine.Debug.LogWarning("[com.unity.burst] Extra debugging is turned on.");
                int debuggingLevel;
                int.TryParse(debuggingStr, out debuggingLevel);
                if (debuggingLevel <= 0) debuggingLevel = 1;
                DebuggingLevel = debuggingLevel;
            }

            // Try to load the runtime through an environment variable
            RuntimePath = Environment.GetEnvironmentVariable("UNITY_BURST_RUNTIME_PATH");

            // Otherwise try to load it from the package itself
            if (!Directory.Exists(RuntimePath))
            {
                RuntimePath = Path.GetFullPath("Packages/com.unity.burst/.Runtime");
            }

            if(IsDebugging)
            {
                UnityEngine.Debug.LogWarning($"[com.unity.burst] Runtime directory set to {RuntimePath}");
            }

            BurstEditorOptions.EnsureSynchronized();

            if (DebuggingLevel > 2)
            {
                UnityEngine.Debug.Log("Burst - Domain Reload");
            }

            BurstCompilerService.Initialize(RuntimePath, TryGetOptionsFromMember);

            EditorApplication.quitting += BurstCompiler.Shutdown;

            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
        }

        private static void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (DebuggingLevel > 2)
            {
                UnityEngine.Debug.Log($"Burst - Change of Editor State: {state}");
            }

            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                if (DebuggingLevel > 2)
                {
                    UnityEngine.Debug.Log("Burst - Exiting PlayMode - cancelling any pending jobs");
                }

                // Cancel also on exit PlayMode
                BurstCompiler.Cancel();
            }
        }

        private static void OnAssemblyCompilationFinished(string arg1, CompilerMessage[] arg2)
        {
            // On assembly compilation finished, we cancel all pending compilation
            if (DebuggingLevel > 2)
            {
                UnityEngine.Debug.Log("Burst - Assembly compilation finished - cancelling any pending jobs");
            }

            BurstCompiler.Cancel();
        }

        private static void OnAssemblyCompilationStarted(string obj)
        {
            if (DebuggingLevel > 2)
            {
                UnityEngine.Debug.Log("Burst - Assembly compilation started - cancelling any pending jobs");
            }
        }

        private static bool TryGetOptionsFromMember(MemberInfo member, out string flagsOut)
        {
            return BurstCompiler.Options.TryGetOptions(member, true, out flagsOut);
        }
    }
}
