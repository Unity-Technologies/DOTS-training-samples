using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Entities.Serialization;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Player;
using UnityEditor.Build.Utilities;
using UnityEngine;

namespace Unity.Scenes.Editor
{
    static class LiveLinkBuildPipeline
    {
        const string k_TempBuildPath = "Temp/LLBP";
        private const int AssetBundleBuildVersion = 8;

        static GUID k_UnityBuiltinResources = new GUID("0000000000000000e000000000000000");
        static GUID k_UnityBuiltinExtraResources = new GUID("0000000000000000f000000000000000");

        // TODO: This should be part of the IDeterministicIdentifiers api.
        static string GenerateAssetBundleInternalFileName(this IDeterministicIdentifiers generator, string name)
        {
            var internalName = generator.GenerateInternalFileName(name);
            return $"archive:/{internalName}/{internalName}";
        }

        // TODO: This should be part of the IDeterministicIdentifiers api.
        static string GenerateSceneBundleInternalFileName(this IDeterministicIdentifiers generator, string scenePath)
        {
            // 1 scene per bundle, so don't worry about special cases
            var internalName = generator.GenerateInternalFileName(scenePath);
            return $"archive:/{internalName}/{internalName}.sharedAssets";
        }

        // TODO: This should be part of the IDeterministicIdentifiers api.
        static string GenerateSceneInternalFileName(this IDeterministicIdentifiers generator, string scenePath)
        {
            // 1 scene per bundle, so don't worry about special cases
            var internalName = generator.GenerateInternalFileName(scenePath);
            return $"{internalName}.sharedAssets";
        }

        static void FilterBuiltinResourcesObjectManifest()
        {
            // Builtin Resources in the editor contains a mix of Editor Only & Runtime types.
            // Based on some trial an error, objects with these flags are the Runtime types.
            // TODO: This will probably break someday, and we need a more reliable method for handling this.
            // TODO: Builtin Resources should not contain Editor only types, there are Editor Builtin Resources for this
            var manifest = (AssetObjectManifest)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(LiveLinkAssetBundleBuildSystem.AssetObjectManifestPath)[0];
            var objects = manifest.Objects.Where(x => x.hideFlags == (HideFlags.HideInInspector | HideFlags.HideAndDontSave)).ToArray();
            AssetObjectManifestBuilder.BuildManifest(objects, manifest);

            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { manifest }, LiveLinkAssetBundleBuildSystem.AssetObjectManifestPath, true);
        }

        static void FilterBuiltinExtraResourcesObjectManifest()
        {
            // Builtin Extra Resources in the editor contains a mix of Editor Only & Runtime types.
            // This is easier to filter as the type info is available to C# and we can just filter on UnityEngine vs UnityEditor
            // TODO: This will probably break someday, and we need a more reliable method for handling this.
            // TODO: Builtin Extra Resources should not contain Editor only types, there are Editor Builtin Resources for this
            var manifest = (AssetObjectManifest)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(LiveLinkAssetBundleBuildSystem.AssetObjectManifestPath)[0];
            var objects = manifest.Objects.Where(x => !x.GetType().FullName.Contains("UnityEditor")).ToArray();
            AssetObjectManifestBuilder.BuildManifest(objects, manifest);

            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { manifest }, LiveLinkAssetBundleBuildSystem.AssetObjectManifestPath, true);
        }

        public static bool BuildSceneBundle(GUID sceneGuid, string cacheFilePath, BuildTarget target, bool collectDependencies = false)
        {
            using (new BuildInterfacesWrapper())
            using (new SceneStateCleanup())
            {
                Directory.CreateDirectory(k_TempBuildPath);

                var scene = sceneGuid.ToString();
                var scenePath = AssetDatabase.GUIDToAssetPath(scene);

                // Deterministic ID Generator
                var generator = new Unity5PackedIdentifiers();

                // Target platform settings & script information
                var settings = new BuildSettings
                {
                    buildFlags = ContentBuildFlags.None,
                    target = target,
                    group = BuildPipeline.GetBuildTargetGroup(target),
                    typeDB = null
                };

                // Inter-asset feature usage (shader features, used mesh channels)
                var usageSet = new BuildUsageTagSet();
                var dependencyResults = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageSet);

                // Bundle all the needed write parameters
                var writeParams = new WriteSceneParameters
                {
                    // Target platform settings & script information
                    settings = settings,

                    // Scene / Project lighting information
                    globalUsage = dependencyResults.globalUsage,

                    // Inter-asset feature usage (shader features, used mesh channels)
                    usageSet = usageSet,

                    // Scene being written out
                    scenePath = scenePath,

                    // Serialized File Layout
                    writeCommand = new WriteCommand
                    {
                        fileName = generator.GenerateSceneInternalFileName(scenePath),
                        internalName = generator.GenerateSceneBundleInternalFileName(scenePath),
                        serializeObjects = new List<SerializationInfo>() // Populated Below
                    },

                    // External object references
                    referenceMap = new BuildReferenceMap(), // Populated Below

                    // External object preload
                    preloadInfo = new PreloadInfo(), // Populated Below

                    sceneBundleInfo = new SceneBundleInfo
                    {
                        bundleName = scene,
                        bundleScenes = new List<SceneLoadInfo> { new SceneLoadInfo {
                        asset = sceneGuid,
                        address = scenePath,
                        internalName = generator.GenerateInternalFileName(scenePath)
                    } }
                    }
                };

                // The requirement is that a single asset bundle only contains the ObjectManifest and the objects that are directly part of the asset, objects for external assets will be in their own bundles. IE: 1 asset per bundle layout
                // So this means we need to take manifestObjects & manifestDependencies and filter storing them into writeCommand.serializeObjects and/or referenceMap based on if they are this asset or other assets
                foreach (var obj in dependencyResults.referencedObjects)
                {
                    // MonoScripts need to live beside the MonoBehavior (in this case ScriptableObject) and loaded first. We could move it to it's own bundle, but this is safer and it's a lightweight object
                    var type = ContentBuildInterface.GetTypeForObject(obj);
                    if (!collectDependencies && (obj.guid == k_UnityBuiltinResources))
                    {
                        // For Builtin Resources, we can reference them directly
                        // TODO: Once we switch to using GlobalObjectId for SBP, we will need a mapping for certain special cases of GUID <> FilePath for Builtin Resources
                        writeParams.referenceMap.AddMapping(obj.filePath, obj.localIdentifierInFile, obj);
                    }
                    else if (collectDependencies || type == typeof(MonoScript) || obj.guid == sceneGuid)
                    {
                        writeParams.writeCommand.serializeObjects.Add(new SerializationInfo { serializationObject = obj, serializationIndex = generator.SerializationIndexFromObjectIdentifier(obj) });
                        writeParams.referenceMap.AddMapping(writeParams.writeCommand.internalName, generator.SerializationIndexFromObjectIdentifier(obj), obj);
                    }
                    else if (!obj.guid.Empty())
                        writeParams.referenceMap.AddMapping(generator.GenerateAssetBundleInternalFileName(obj.guid.ToString()), generator.SerializationIndexFromObjectIdentifier(obj), obj);

                    // TODO: MonoScript complications: There is no way to properly determine which MonoScripts are being referenced by which Asset in the returned manifestDependencies
                    // This will be solvable after we move SBP into the asset pipeline as importers.
                }

                // Write the serialized file
                var result = ContentBuildInterface.WriteSceneSerializedFile(k_TempBuildPath, writeParams);
                // Archive and compress the serialized & resource files for the previous operation
                var crc = ContentBuildInterface.ArchiveAndCompress(result.resourceFiles.ToArray(), cacheFilePath, UnityEngine.BuildCompression.Uncompressed);

                // Because the shader compiler progress bar hooks are absolute shit
                EditorUtility.ClearProgressBar();

                //Debug.Log($"Wrote '{writeParams.writeCommand.fileName}' to '{k_TempBuildPath}/{writeParams.writeCommand.internalName}' resulting in {result.serializedObjects.Count} objects in the serialized file.");
                //Debug.Log($"Archived '{k_TempBuildPath}/{writeParams.writeCommand.internalName}' to '{cacheFilePath}' resulting in {crc} CRC.");

                Directory.Delete(k_TempBuildPath, true);

                return crc != 0;
            }
        }

        public static bool BuildAssetBundle(GUID assetGuid, string cacheFilePath, BuildTarget target, bool collectDependencies = false)
        {
            using (new BuildInterfacesWrapper())
            {
                Directory.CreateDirectory(k_TempBuildPath);

                var asset = assetGuid.ToString();

                // Deterministic ID Generator
                var generator = new Unity5PackedIdentifiers();

                // Target platform settings & script information
                var settings = new BuildSettings
                {
                    buildFlags = ContentBuildFlags.None,
                    target = target,
                    group = BuildPipeline.GetBuildTargetGroup(target),
                    typeDB = null
                };

                if (assetGuid == k_UnityBuiltinResources)
                    FilterBuiltinResourcesObjectManifest();

                if (assetGuid == k_UnityBuiltinExtraResources)
                    FilterBuiltinExtraResourcesObjectManifest();

                var method = typeof(ContentBuildInterface).GetMethod("GetPlayerObjectIdentifiersInSerializedFile", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                // Collect all the objects we need for this asset & bundle (returned array order is deterministic)
                var manifestObjects = (ObjectIdentifier[])method.Invoke(null, new object[] { LiveLinkAssetBundleBuildSystem.AssetObjectManifestPath, settings.target });

                // Collect all the objects we need to reference for this asset (returned array order is deterministic)
                var manifestDependencies = ContentBuildInterface.GetPlayerDependenciesForObjects(manifestObjects, settings.target, settings.typeDB);

                // Scene / Project lighting information
                var globalUsage = ContentBuildInterface.GetGlobalUsageFromGraphicsSettings();

                // Inter-asset feature usage (shader features, used mesh channels)
                var usageSet = new BuildUsageTagSet();
                ContentBuildInterface.CalculateBuildUsageTags(manifestDependencies, manifestDependencies, globalUsage, usageSet); // TODO: Cache & Append to the assets that are influenced by this usageTagSet, ideally it would be a nice api to extract just the data for a given asset or object from the result

                // Bundle all the needed write parameters
                var writeParams = new WriteParameters
                {
                    // Target platform settings & script information
                    settings = settings,

                    // Scene / Project lighting information
                    globalUsage = globalUsage,

                    // Inter-asset feature usage (shader features, used mesh channels)
                    usageSet = usageSet,

                    // Serialized File Layout
                    writeCommand = new WriteCommand
                    {
                        fileName = generator.GenerateInternalFileName(asset),
                        internalName = generator.GenerateAssetBundleInternalFileName(asset),
                        serializeObjects = new List<SerializationInfo>() // Populated Below
                    },

                    // External object references
                    referenceMap = new BuildReferenceMap(), // Populated Below

                    // Asset Bundle object layout
                    bundleInfo = new AssetBundleInfo
                    {
                        bundleName = asset,
                        // What is loadable from this bundle
                        bundleAssets = new List<AssetLoadInfo>
                    {
                        // The manifest object and it's dependencies
                        new AssetLoadInfo
                        {
                            address = asset,
                            asset = assetGuid, // TODO: Remove this as it is unused in C++
                            includedObjects = manifestObjects.ToList(), // TODO: In our effort to modernize the public API design we over complicated it trying to take List or return ReadOnlyLists. Should have just stuck with Arrays[] in all places
                            referencedObjects = manifestDependencies.ToList()
                        }
                    }
                    }
                };

                // For Builtin Resources, we just want to reference them directly instead of pull them in.
                //if (assetGuid == k_UnityBuiltinResources)
                //    assetGuid = manifestGuid;

                // The requirement is that a single asset bundle only contains the ObjectManifest and the objects that are directly part of the asset, objects for external assets will be in their own bundles. IE: 1 asset per bundle layout
                // So this means we need to take manifestObjects & manifestDependencies and filter storing them into writeCommand.serializeObjects and/or referenceMap based on if they are this asset or other assets
                foreach (var obj in manifestObjects)
                {
                    writeParams.writeCommand.serializeObjects.Add(new SerializationInfo { serializationObject = obj, serializationIndex = generator.SerializationIndexFromObjectIdentifier(obj) });
                    writeParams.referenceMap.AddMapping(writeParams.writeCommand.internalName, generator.SerializationIndexFromObjectIdentifier(obj), obj);
                }

                foreach (var obj in manifestDependencies)
                {
                    // MonoScripts need to live beside the MonoBehavior (in this case ScriptableObject) and loaded first. We could move it to it's own bundle, but this is safer and it's a lightweight object
                    var type = ContentBuildInterface.GetTypeForObject(obj);
                    if (!collectDependencies && (obj.guid == k_UnityBuiltinResources))
                    {
                        // For Builtin Resources, we can reference them directly
                        // TODO: Once we switch to using GlobalObjectId for SBP, we will need a mapping for certain special cases of GUID <> FilePath for Builtin Resources
                        writeParams.referenceMap.AddMapping(obj.filePath, obj.localIdentifierInFile, obj);
                    }
                    else if (collectDependencies || type == typeof(MonoScript) || obj.guid == assetGuid)
                    {
                        writeParams.writeCommand.serializeObjects.Add(new SerializationInfo { serializationObject = obj, serializationIndex = generator.SerializationIndexFromObjectIdentifier(obj) });
                        writeParams.referenceMap.AddMapping(writeParams.writeCommand.internalName, generator.SerializationIndexFromObjectIdentifier(obj), obj);
                    }
                    else if (!obj.guid.Empty())
                        writeParams.referenceMap.AddMapping(generator.GenerateAssetBundleInternalFileName(obj.guid.ToString()), generator.SerializationIndexFromObjectIdentifier(obj), obj);

                    // TODO: MonoScript complications: There is no way to properly determine which MonoScripts are being referenced by which Asset in the returned manifestDependencies
                    // This will be solvable after we move SBP into the asset pipeline as importers.
                }

                // Write the serialized file
                var result = ContentBuildInterface.WriteSerializedFile(k_TempBuildPath, writeParams);
                // Archive and compress the serialized & resource files for the previous operation
                var crc = ContentBuildInterface.ArchiveAndCompress(result.resourceFiles.ToArray(), cacheFilePath, UnityEngine.BuildCompression.Uncompressed);

                // Because the shader compiler progress bar hooks are absolute shit
                EditorUtility.ClearProgressBar();

                //Debug.Log($"Wrote '{writeParams.writeCommand.fileName}' to '{k_TempBuildPath}/{writeParams.writeCommand.internalName}' resulting in {result.serializedObjects.Count} objects in the serialized file.");
                //Debug.Log($"Archived '{k_TempBuildPath}/{writeParams.writeCommand.internalName}' to '{cacheFilePath}' resulting in {crc} CRC.");

                Directory.Delete(k_TempBuildPath, true);

                return crc != 0;
            }
        }

        static CachedInfo LoadTargetCachedInfo(GUID guid, BuildTarget target)
        {
            var cache = new BuildCache();
            var entries = new List<CacheEntry> { cache.GetCacheEntry(guid, AssetBundleBuildVersion) };
            cache.LoadCachedData(entries, out IList<CachedInfo> cachedInfos);
            if (cachedInfos[0] == null)
            {
                cachedInfos[0] = CalculateTargetCachedInfo(entries[0], target);
                // cache.SaveCachedData(cachedInfos); // TODO: Disabled because we have file contention as "Save" is async only with no wait functionality
            }
            return cachedInfos[0];
        }

        static CachedInfo CalculateTargetCachedInfo(CacheEntry entry, BuildTarget target, TypeDB typeDB = null)
        {
            var cache = new BuildCache();
            List<ObjectIdentifier> objects = new List<ObjectIdentifier>();
            objects.AddRange(ContentBuildInterface.GetPlayerObjectIdentifiersInAsset(entry.Guid, target));
            objects.AddRange(ContentBuildInterface.GetPlayerDependenciesForObjects(objects.ToArray(), target, typeDB));

            var cachedInfo = new CachedInfo();
            cachedInfo.Asset = entry;
            cachedInfo.Data = new[] { objects };

            // TODO: Handle built-in objects, might require some low level build changes like an option to ignore the hideFlags
            var dependencies = objects.Select(x => x.guid).Where(x => !x.Empty() && x != entry.Guid && x != k_UnityBuiltinResources).Distinct();
            cachedInfo.Dependencies = dependencies.Select(cache.GetCacheEntry).ToArray();
            // cache.SaveCachedData(new List<CachedInfo> { cachedInfo }); // TODO: Disabled because we have file contention as "Save" is async only with no wait functionality
            return cachedInfo;
        }

        public static Hash128 CalculateTargetHash(GUID guid, BuildTarget target)
        {
            // Always calculate & return the new hash
            // TODO: target is not taken into account, this will be included when sbp moves into the asset pipeline
            var cache = new BuildCache();
            var cachedInfo = CalculateTargetCachedInfo(cache.GetCacheEntry(guid, AssetBundleBuildVersion), target, null); // TODO: Script Property dependency

            // TODO: SBP returns recursive dependencies which is too much for Live Link, this will need to be changed when sbp moves into asset pipeline
            var targetHash = cachedInfo.Asset.Hash;
            foreach (var dependency in cachedInfo.Dependencies)
            {
                var dependencyHash = dependency.Hash;
                HashUtilities.ComputeHash128(ref dependencyHash, ref targetHash);
            }

            return targetHash;
        }

        public static void CalculateTargetDependencies(GUID guid, BuildTarget target, out ResolvedAssetID[] dependencies)
        {
            // This is only called after CalculateTargetHash, so we can just load the cached results and return those.
            // TODO: target is not taken into account, this will be included when sbp moves into the asset pipeline
            var cachedInfo = LoadTargetCachedInfo(guid, target);

            List<ResolvedAssetID> deps = new List<ResolvedAssetID>();
            foreach (var entry in cachedInfo.Dependencies)
            {
                deps.Add(new ResolvedAssetID
                {
                    GUID = entry.Guid,
                    TargetHash = CalculateTargetHash(entry.Guid, target)
                });
            }
            dependencies = deps.ToArray();
        }
    }
}
