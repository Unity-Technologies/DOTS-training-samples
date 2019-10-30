using System;
using System.IO;
using Unity.PerformanceTesting.Editor;
using Unity.PerformanceTesting.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;


[assembly: PrebuildSetup(typeof(TestRunBuilder))]
[assembly: PostBuildCleanup(typeof(TestRunBuilder))]

namespace Unity.PerformanceTesting.Editor
{
    public class TestRunBuilder : IPrebuildSetup, IPostBuildCleanup
    {
        private const string cleanResources = "PT_ResourcesCleanup";

        public void Setup()
        {
            var run = ReadPerformanceTestRunJson();
            run.EditorVersion = GetEditorInfo();
            run.PlayerSettings = GetPlayerSettings(run.PlayerSettings);
            run.BuildSettings = GetPlayerBuildInfo();
            run.StartTime = Utils.DateToInt(DateTime.Now);

            CreateResourcesFolder();
            CreatePerformanceTestRunJson(run);
        }

        public void Cleanup()
        {
            if (File.Exists(Utils.TestRunPath))
            {
                File.Delete(Utils.TestRunPath);
            }

            if (EditorPrefs.GetBool(cleanResources))
            {
                Directory.Delete("Assets/Resources/", true);
            }

            AssetDatabase.Refresh();
        }

        private static EditorVersion GetEditorInfo()
        {
            return new EditorVersion
            {
                FullVersion = UnityEditorInternal.InternalEditorUtility.GetFullUnityVersion(),
                DateSeconds = int.Parse(UnityEditorInternal.InternalEditorUtility.GetUnityVersionDate().ToString()),
                Branch = GetEditorBranch(),
                RevisionValue = int.Parse(UnityEditorInternal.InternalEditorUtility.GetUnityRevision().ToString())
            };
        }

        private static string GetEditorBranch()
        {
            foreach (var method in typeof(UnityEditorInternal.InternalEditorUtility).GetMethods())
            {
                if (method.Name.Contains("GetUnityBuildBranch"))
                {
                    return (string) method.Invoke(null, null);
                }
            }

            return "null";
        }

        private static PlayerSettings GetPlayerSettings(PlayerSettings playerSettings)
        {
            playerSettings.VrSupported = UnityEditor.PlayerSettings.virtualRealitySupported;
            playerSettings.MtRendering = UnityEditor.PlayerSettings.MTRendering;
            playerSettings.GpuSkinning = UnityEditor.PlayerSettings.gpuSkinning;
            playerSettings.GraphicsJobs = UnityEditor.PlayerSettings.graphicsJobs;
            playerSettings.GraphicsApi =
                UnityEditor.PlayerSettings.GetGraphicsAPIs(EditorUserBuildSettings.activeBuildTarget)[0]
                    .ToString();
            playerSettings.ScriptingBackend = UnityEditor.PlayerSettings
                .GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup)
                .ToString();
            playerSettings.StereoRenderingPath = UnityEditor.PlayerSettings.stereoRenderingPath.ToString();
            playerSettings.RenderThreadingMode = UnityEditor.PlayerSettings.graphicsJobs ? "GraphicsJobs" :
                UnityEditor.PlayerSettings.MTRendering ? "MultiThreaded" : "SingleThreaded";
            playerSettings.AndroidMinimumSdkVersion = UnityEditor.PlayerSettings.Android.minSdkVersion.ToString();
            playerSettings.AndroidTargetSdkVersion = UnityEditor.PlayerSettings.Android.targetSdkVersion.ToString();
            playerSettings.Batchmode = UnityEditorInternal.InternalEditorUtility.inBatchMode.ToString();
            return playerSettings;
        }

        private static BuildSettings GetPlayerBuildInfo()
        {
            var buildSettings = new BuildSettings
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
                DevelopmentPlayer = EditorUserBuildSettings.development,
                AndroidBuildSystem = EditorUserBuildSettings.androidBuildSystem.ToString()
            };
            return buildSettings;
        }

        private PerformanceTestRun ReadPerformanceTestRunJson()
        {
            try
            {
                var json = Resources.Load<TextAsset>(Utils.TestRunPath).text;
                return JsonUtility.FromJson<PerformanceTestRun>(json);
            }
            catch
            {
                return new PerformanceTestRun {PlayerSettings = new PlayerSettings()};
            }
        }


        private void CreateResourcesFolder()
        {
            if (Directory.Exists(Utils.ResourcesPath))
            {
                EditorPrefs.SetBool(cleanResources, false);
                return;
            }

            EditorPrefs.SetBool(cleanResources, true);
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        private void CreatePerformanceTestRunJson(PerformanceTestRun run)
        {
            var json = JsonUtility.ToJson(run, true);
            PlayerPrefs.SetString(Utils.PlayerPrefKeyRunJSON, json);
            File.WriteAllText(Utils.TestRunPath, json);
            AssetDatabase.Refresh();
        }
    }
}