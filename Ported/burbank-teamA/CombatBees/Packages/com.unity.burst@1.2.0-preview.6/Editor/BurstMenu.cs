using Unity.Burst.LowLevel;
using UnityEditor;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Burst.Editor
{
    /// <summary>
    /// Register all menu entries for burst to the Editor
    /// </summary>
    internal static class BurstMenu
    {
        private const string EnableBurstCompilationText = "Jobs/Burst/Enable Compilation";
        private const string EnableSafetyChecksText = "Jobs/Burst/Safety Checks";
        private const string ForceSynchronousCompilesText = "Jobs/Burst/Synchronous Compilation";
        private const string ShowBurstTimingsText = "Jobs/Burst/Show Timings";
        private const string BurstInspectorText = "Jobs/Burst/Open Inspector...";

        // ----------------------------------------------------------------------------------------------
        // #1 Enable Compilation
        // ----------------------------------------------------------------------------------------------
        [MenuItem(EnableBurstCompilationText, false)]
        private static void EnableBurstCompilation()
        {
            BurstEditorOptions.EnableBurstCompilation = !BurstEditorOptions.EnableBurstCompilation;
        }

        [MenuItem(EnableBurstCompilationText, true)]
        private static bool EnableBurstCompilationValidate()
        {
            Menu.SetChecked(EnableBurstCompilationText, BurstEditorOptions.EnableBurstCompilation);
            return BurstCompilerService.IsInitialized && (BurstEditorOptions.EnableBurstCompilation || !EditorApplication.isPlayingOrWillChangePlaymode);
        }

        // ----------------------------------------------------------------------------------------------
        // #2 Safety Checks
        // ----------------------------------------------------------------------------------------------
        [MenuItem(EnableSafetyChecksText, false)]
        private static void EnableBurstSafetyChecks()
        {
            BurstEditorOptions.EnableBurstSafetyChecks = !BurstEditorOptions.EnableBurstSafetyChecks;
        }

        [MenuItem(EnableSafetyChecksText, true)]
        private static bool EnableBurstSafetyChecksValidate()
        {
            Menu.SetChecked(EnableSafetyChecksText, BurstEditorOptions.EnableBurstSafetyChecks);
            return BurstCompilerService.IsInitialized && BurstEditorOptions.EnableBurstCompilation;
        }

        // ----------------------------------------------------------------------------------------------
        // #3 Synchronous Compilation
        // ----------------------------------------------------------------------------------------------
        [MenuItem(ForceSynchronousCompilesText, false)]
        private static void ForceSynchronousCompiles()
        {
            BurstEditorOptions.EnableBurstCompileSynchronously = !BurstEditorOptions.EnableBurstCompileSynchronously;
        }

        [MenuItem(ForceSynchronousCompilesText, true)]
        private static bool ForceSynchronousCompilesValidate()
        {
            Menu.SetChecked(ForceSynchronousCompilesText, BurstEditorOptions.EnableBurstCompileSynchronously);
            return BurstCompilerService.IsInitialized && BurstEditorOptions.EnableBurstCompilation;
        }

        // ----------------------------------------------------------------------------------------------
        // #4 Show Timings
        // ----------------------------------------------------------------------------------------------
        [MenuItem(ShowBurstTimingsText, false)]
        private static void ShowBurstTimings()
        {
            BurstEditorOptions.EnableBurstTimings = !BurstEditorOptions.EnableBurstTimings;
        }
        
        [MenuItem(ShowBurstTimingsText, true)]
        private static bool ShowBurstTimingsValidate()
        {
            Menu.SetChecked(ShowBurstTimingsText, BurstEditorOptions.EnableBurstTimings);
            return BurstCompilerService.IsInitialized && BurstEditorOptions.EnableBurstCompilation;
        }

        // ----------------------------------------------------------------------------------------------
        // #5 Open Inspector...
        // ----------------------------------------------------------------------------------------------
        [MenuItem(BurstInspectorText)]
        private static void BurstInspector()
        {
            // Get existing open window or if none, make a new one:
            BurstInspectorGUI window = EditorWindow.GetWindow<BurstInspectorGUI>("Burst Inspector");
            window.Show();
        }
    }
}
