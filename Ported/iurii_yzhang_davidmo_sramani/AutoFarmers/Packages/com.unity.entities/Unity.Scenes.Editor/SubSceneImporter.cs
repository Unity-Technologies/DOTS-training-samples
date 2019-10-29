using System;
using Unity.Build;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEditor;
using UnityEditor.SceneManagement;
using AssetImportContext = UnityEditor.Experimental.AssetImporters.AssetImportContext;

namespace Unity.Scenes.Editor
{
    [UnityEditor.Experimental.AssetImporters.ScriptedImporter(52, "extDontMatter")]
    [InitializeOnLoad]
    class SubSceneImporter : UnityEditor.Experimental.AssetImporters.ScriptedImporter
    {
        static SubSceneImporter()
        {
            EntityScenesPaths.SubSceneImporterType = typeof(SubSceneImporter);
        }

        static unsafe EntityScenesPaths.SceneWithBuildSettingsGUIDs ReadSceneWithBuildSettings(string path)
        {
            EntityScenesPaths.SceneWithBuildSettingsGUIDs sceneWithBuildSettings = default;
            using (var reader = new StreamBinaryReader(path))
            {
                reader.ReadBytes(&sceneWithBuildSettings, sizeof(EntityScenesPaths.SceneWithBuildSettingsGUIDs));
            }
            return sceneWithBuildSettings;
        }

        public static void ConvertToBuild(GUID buildSettingSceneGuid, UnityEditor.Build.Pipeline.Tasks.CalculateCustomDependencyData task)
        {
            var buildSettingScenePath = AssetDatabase.GUIDToAssetPath(buildSettingSceneGuid.ToString());
            var sceneWithBuildSettings = ReadSceneWithBuildSettings(buildSettingScenePath);

            var hash = UnityEditor.Experimental.AssetDatabaseExperimental.GetArtifactHash(buildSettingSceneGuid.ToString(), typeof(SubSceneImporter));
            string[] paths;
            if (!UnityEditor.Experimental.AssetDatabaseExperimental.GetArtifactPaths(hash, out paths))
                return;

            foreach (var path in paths)
            {
                var ext = System.IO.Path.GetExtension(path).Replace(".", "");
                if (ext == EntityScenesPaths.GetExtension(EntityScenesPaths.PathType.EntitiesHeader))
                {
                    var loadPath = EntityScenesPaths.GetLoadPath(sceneWithBuildSettings.SceneGUID, EntityScenesPaths.PathType.EntitiesHeader, -1);
                    System.IO.File.Copy(path, loadPath, true);
                    continue;
                }

                if (ext == EntityScenesPaths.GetExtension(EntityScenesPaths.PathType.EntitiesBinary))
                {
                    var sectionIndex = EntityScenesPaths.GetSectionIndexFromPath(path);
                    var loadPath = EntityScenesPaths.GetLoadPath(sceneWithBuildSettings.SceneGUID, EntityScenesPaths.PathType.EntitiesBinary, sectionIndex);
                    System.IO.File.Copy(path, loadPath, true);
                    continue;
                }

                if (ext == EntityScenesPaths.GetExtension(EntityScenesPaths.PathType.EntitiesUnityObjectReferences))
                {
                    var sectionIndex = EntityScenesPaths.GetSectionIndexFromPath(path);
                    task.GetObjectIdentifiersAndTypesForSerializedFile(path, out UnityEditor.Build.Content.ObjectIdentifier[] objectIds, out System.Type[] types);
                    var bundlePath = EntityScenesPaths.GetLoadPath(sceneWithBuildSettings.SceneGUID, EntityScenesPaths.PathType.EntitiesUnityObjectReferences, sectionIndex);
                    var bundleName = System.IO.Path.GetFileName(bundlePath);
                    task.CreateAssetEntryForObjectIdentifiers(objectIds, path, bundleName, bundleName, typeof(ReferencedUnityObjects));
                }
            }
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            try
            {
                var sceneWithBuildSettings = ReadSceneWithBuildSettings(ctx.assetPath);

                // Ensure we have as many dependencies as possible registered early in case an exception is thrown
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneWithBuildSettings.SceneGUID.ToString());
                ctx.DependsOnSourceAsset(scenePath);

                if (sceneWithBuildSettings.BuildSettings.IsValid)
                {
                    var buildSettingPath = AssetDatabase.GUIDToAssetPath(sceneWithBuildSettings.BuildSettings.ToString());
                    ctx.DependsOnSourceAsset(buildSettingPath);
                    var buildSettingDependencies = AssetDatabase.GetDependencies(buildSettingPath);
                    foreach (var dependency in buildSettingDependencies)
                        ctx.DependsOnSourceAsset(dependency);
                }

                var dependencies = AssetDatabase.GetDependencies(scenePath);
                foreach (var dependency in dependencies)
                {
                    if (dependency.ToLower().EndsWith(".prefab"))
                        ctx.DependsOnSourceAsset(dependency);
                }

                var buildSettings = BuildSettings.LoadBuildSettings(sceneWithBuildSettings.BuildSettings);

                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                try
                {
                    var settings = new GameObjectConversionSettings();

                    settings.SceneGUID = sceneWithBuildSettings.SceneGUID;
                    settings.BuildSettings = buildSettings;
                    settings.AssetImportContext = ctx;

                    EditorEntityScenes.WriteEntityScene(scene, settings);
                }
                finally
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
            // Currently it's not acceptable to let the asset database catch the exception since it will create a default asset without any dependencies
            // This means a reimport will not be triggered if the scene is subsequently modified
            catch(Exception e)
            {
                Debug.Log($"Exception thrown during SubScene import: {e.Message}");
            }
        }
    }
}