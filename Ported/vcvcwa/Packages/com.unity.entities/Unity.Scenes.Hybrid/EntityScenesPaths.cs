using System;
using System.IO;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    class EntityScenesPaths
    {
        public static Type SubSceneImporterType = null;
        
        public enum PathType
        {
            EntitiesUnityObjectReferences,
            EntitiesBinary,
            EntitiesConversionLog,
            EntitiesHeader
        }

        public static string GetExtension(PathType pathType)
        {
            switch (pathType)
            {
                // these must all be lowercase
                case PathType.EntitiesUnityObjectReferences: return "asset";
                case PathType.EntitiesBinary : return "entities";
                case PathType.EntitiesHeader : return "entityheader";
                case PathType.EntitiesConversionLog : return "conversionlog";
            }

            throw new System.ArgumentException("Unknown PathType");
        }
        
#if UNITY_EDITOR
        public struct SceneWithBuildSettingsGUIDs
        {
            public Hash128 SceneGUID;
            public Hash128 BuildSettings;
        }

        public static unsafe Hash128 CreateBuildSettingSceneFile(Hash128 sceneGUID, Hash128 buildSettingGUID)
        {
            var guids = new SceneWithBuildSettingsGUIDs { SceneGUID = sceneGUID, BuildSettings = buildSettingGUID};
            
            Hash128 guid;
            guid.Value.x = math.hash(&guids, sizeof(SceneWithBuildSettingsGUIDs));
            guid.Value.y = math.hash(&guids, sizeof(SceneWithBuildSettingsGUIDs), 0x96a755e2);
            guid.Value.z = math.hash(&guids, sizeof(SceneWithBuildSettingsGUIDs), 0x4e936206);
            guid.Value.w = math.hash(&guids, sizeof(SceneWithBuildSettingsGUIDs), 0xac602639);

            string dir = "Assets/SceneDependencyCache";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fileName = $"{dir}/{guid}.sceneWithBuildSettings";
            if (!File.Exists(fileName))
            {
                using(var writer = new Entities.Serialization.StreamBinaryWriter(fileName))
                {
                    writer.WriteBytes(&guids, sizeof(SceneWithBuildSettingsGUIDs));
                }
                File.WriteAllText(fileName + ".meta", 
                    $"fileFormatVersion: 2\nguid: {guid}\nDefaultImporter:\n  externalObjects: {{}}\n  userData:\n  assetBundleName:\n  assetBundleVariant:\n");
                
                // Refresh is necessary because it appears the asset pipeline
                // can't depend on an asset on disk that has not yet been refreshed.
                AssetDatabase.Refresh();
            }
            return guid;
        }

        public static Hash128 GetSubSceneArtifactHash(Hash128 sceneGUID, Hash128 buildSettingGUID, UnityEditor.Experimental.AssetDatabaseExperimental.ImportSyncMode syncMode)
        {
            var guid = CreateBuildSettingSceneFile(sceneGUID, buildSettingGUID);
            var res = UnityEditor.Experimental.AssetDatabaseExperimental.GetArtifactHash(guid.ToString(), SubSceneImporterType, syncMode);
            return res;
        }        
        
        public static string GetLoadPathFromArtifactPaths(string[] paths, PathType type, int? sectionIndex = null)
        {
            var extension = GetExtension(type);
            if (sectionIndex != null)
                extension = $"{sectionIndex}.{extension}";

            return paths.FirstOrDefault(p => p.EndsWith(extension));
        }
#endif // UNITY_EDITOR

        public static string GetLoadPath(Hash128 sceneGUID, PathType type, int sectionIndex)
        {
            var extension = GetExtension(type);
            if (type == PathType.EntitiesBinary)
                return $"{Application.streamingAssetsPath}/SubScenes/{sceneGUID}.{sectionIndex}.{extension}";
            else if (type == PathType.EntitiesHeader)
                return $"{Application.streamingAssetsPath}/SubScenes/{sceneGUID}.{extension}";
            else if (type == PathType.EntitiesUnityObjectReferences)
                return $"{Application.streamingAssetsPath}/SubScenes/{sceneGUID}.{sectionIndex}.bundle";
            else
                return "";
        }

        public static int GetSectionIndexFromPath(string path)
        {
            var components = Path.GetFileNameWithoutExtension(path).Split('.');
            if (components.Length == 1)
                return 0;
            return int.Parse(components[1]);
        }
    }
}
