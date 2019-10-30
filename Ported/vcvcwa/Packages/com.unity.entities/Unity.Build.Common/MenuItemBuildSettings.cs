using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.Build.Common
{
    static class MenuItemBuildSettings
    {
        const string kBuildSettingsClassic = "Assets/Create/Build/BuildSettings Classic";
        const string kBuildPipelineClassicAssetPath = "Packages/com.unity.entities/Unity.Build.Common/Assets/HybridBuildPipeline.asset";
        const string kBuildPipeline = "Assets/Create/Build/BuildPipeline";

        //@TODO: Use ProjectWindowUtil for better creation workflows

        [MenuItem(kBuildSettingsClassic, true)]
        static bool CreateNewBuildSettingsAssetValidationClassic()
        {
            return Directory.Exists(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem(kBuildSettingsClassic)]
        static void CreateNewBuildSettingsAssetClassic()
        {
            var pipeline = AssetDatabase.LoadAssetAtPath<BuildPipeline>(kBuildPipelineClassicAssetPath);
            Selection.activeObject = CreateNewBuildSettingsAsset("Classic", new ClassicBuildProfile { Pipeline = pipeline });
        }

        [MenuItem(kBuildPipeline, true)]
        static bool AddPipelineContexMenuValidation()
        {
            return Directory.Exists(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        [MenuItem(kBuildPipeline)]
        static void AddPipelineContexMenu()
        {
            Selection.activeObject = BuildPipeline.CreateAsset(CreateAssetPathInActiveDirectory("BuildPipeline.asset"));
        }

        static BuildSettings CreateNewBuildSettingsAsset(string assetPrefix, IBuildSettingsComponent bar)
        {
            var dependency = Selection.activeObject as BuildSettings;
            var path = CreateAssetPathInActiveDirectory(assetPrefix + "_BuildSettings.buildsettings");
            var buildSettings = ScriptableObject.CreateInstance<BuildSettings>();
            buildSettings.SetComponent(new GeneralSettings());
            buildSettings.SetComponent(new SceneList());
            buildSettings.SetComponent(bar.GetType(), bar);
            if (dependency != null)
                buildSettings.AddDependency(dependency);
            buildSettings.SerializeToPath(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<BuildSettings>(path);
        }

        static string CreateAssetPathInActiveDirectory(string defaultFilename)
        {
            string path = null;
            if (Selection.activeObject != null)
            {
                var aoPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(aoPath))
                {
                    if (Directory.Exists(aoPath))
                        path = Path.Combine(aoPath, defaultFilename);
                    else
                        path = Path.Combine(Path.GetDirectoryName(aoPath), defaultFilename);
                }
            }
            return AssetDatabase.GenerateUniqueAssetPath(path);
        }
    }
}