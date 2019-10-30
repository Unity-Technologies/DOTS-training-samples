using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.WriteTypes;
using UnityEditor.Build.Utilities;

#if !UNITY_2019_1_OR_NEWER
using System;
using UnityEngine;
#endif

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class GenerateBundleCommands : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBundleBuildContent m_BuildContent;

        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext]
        IBundleWriteData m_WriteData;

        [InjectContext(ContextUsage.In)]
        IDeterministicIdentifiers m_PackingMethod;
#pragma warning restore 649

        bool ValidAssetBundle(List<GUID> assets)
        {
            return assets.All(x => ValidationMethods.ValidAsset(x) == ValidationMethods.Status.Asset || m_BuildContent.FakeAssets.ContainsKey(x));
        }

        public ReturnCode Run()
        {
            foreach (var bundlePair in m_BuildContent.BundleLayout)
            {
                if (ValidAssetBundle(bundlePair.Value))
                {
                    // Use generated internalName here as we could have an empty asset bundle used for raw object storage (See CreateStandardShadersBundle)
                    var internalName = string.Format(CommonStrings.AssetBundleNameFormat, m_PackingMethod.GenerateInternalFileName(bundlePair.Key));
                    CreateAssetBundleCommand(bundlePair.Key, internalName, bundlePair.Value);
                }
                else if (ValidationMethods.ValidSceneBundle(bundlePair.Value))
                {
                    CreateSceneBundleCommand(bundlePair.Key, m_WriteData.AssetToFiles[bundlePair.Value[0]][0], bundlePair.Value[0], bundlePair.Value);
                    for (int i = 1; i < bundlePair.Value.Count; ++i)
                        CreateSceneDataCommand(m_WriteData.AssetToFiles[bundlePair.Value[i]][0], bundlePair.Value[i]);
                }
            }
            return ReturnCode.Success;
        }

        static WriteCommand CreateWriteCommand(string internalName, List<ObjectIdentifier> objects, IDeterministicIdentifiers packingMethod)
        {
            var command = new WriteCommand();
            command.internalName = internalName;
            command.fileName = Path.GetFileName(internalName);

            command.serializeObjects = objects.Select(x => new SerializationInfo
            {
                serializationObject = x,
                serializationIndex = packingMethod.SerializationIndexFromObjectIdentifier(x)
            }).ToList();
            return command;
        }

        void CreateAssetBundleCommand(string bundleName, string internalName, List<GUID> assets)
        {
            var abOp = new AssetBundleWriteOperation();

            var fileObjects = m_WriteData.FileToObjects[internalName];
            abOp.Command = CreateWriteCommand(internalName, fileObjects, m_PackingMethod);

            abOp.UsageSet = new BuildUsageTagSet();
            m_WriteData.FileToUsageSet.Add(internalName, abOp.UsageSet);

            abOp.ReferenceMap = new BuildReferenceMap();
            abOp.ReferenceMap.AddMappings(internalName, abOp.Command.serializeObjects.ToArray());
            m_WriteData.FileToReferenceMap.Add(internalName, abOp.ReferenceMap);

            {
                abOp.Info = new AssetBundleInfo();
                abOp.Info.bundleName = bundleName;
                abOp.Info.bundleAssets = assets.Select(x => m_DependencyData.AssetInfo[x]).ToList();
                foreach (var loadInfo in abOp.Info.bundleAssets)
                    loadInfo.address = m_BuildContent.Addresses[loadInfo.asset];
            }

            m_WriteData.WriteOperations.Add(abOp);
        }

#if !UNITY_2019_1_OR_NEWER
        static int GetSortIndex(Type type)
        {
            // ContentBuildInterface.GetTypeForObjects returns null for some MonoBehaviours, this will fix it until the API is fixed.
            if (type == null)
                return Int32.MaxValue - 3;
            if (type == typeof(MonoScript))
                return Int32.MinValue;
            if (typeof(ScriptableObject).IsAssignableFrom(type))
                return Int32.MaxValue - 4;
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
                return Int32.MaxValue - 3;
            if (typeof(TerrainData).IsAssignableFrom(type))
                return Int32.MaxValue - 2;
            return BitConverter.ToInt32(HashingMethods.Calculate(type.Name).ToBytes(), 0);
        }

        struct SortObject
        {
            public ObjectIdentifier objectId;
            public int sortIndex;
        }

        static List<ObjectIdentifier> GetSortedSceneObjectIdentifiers(List<ObjectIdentifier> objects)
        {
            var types = new List<Type>(ContentBuildInterface.GetTypeForObjects(objects.ToArray()));
            var sortedObjects = new List<SortObject>();
            for (int i = 0; i < objects.Count; i++)
                sortedObjects.Add(new SortObject { sortIndex = GetSortIndex(types[i]), objectId = objects[i] });
            return sortedObjects.OrderBy(x => x.sortIndex).Select(x => x.objectId).ToList();
        }
#endif

        void CreateSceneBundleCommand(string bundleName, string internalName, GUID asset, List<GUID> assets)
        {
            var sbOp = new SceneBundleWriteOperation();

            var fileObjects = m_WriteData.FileToObjects[internalName];
#if !UNITY_2019_1_OR_NEWER
            // ContentBuildInterface.PrepareScene was not returning stable sorted references, causing a indeterminism and loading errors in some cases
            // Add correct sorting here until patch lands to fix the API.
            fileObjects = GetSortedSceneObjectIdentifiers(fileObjects);
#endif
            sbOp.Command = CreateWriteCommand(internalName, fileObjects, new LinearPackedIdentifiers(3)); // Start at 3: PreloadData = 1, AssetBundle = 2

            sbOp.UsageSet = new BuildUsageTagSet();
            m_WriteData.FileToUsageSet.Add(internalName, sbOp.UsageSet);

            sbOp.ReferenceMap = new BuildReferenceMap();
            m_WriteData.FileToReferenceMap.Add(internalName, sbOp.ReferenceMap);

            var sceneInfo = m_DependencyData.SceneInfo[asset];
            sbOp.Scene = sceneInfo.scene;
#if !UNITY_2019_3_OR_NEWER
            sbOp.ProcessedScene = sceneInfo.processedScene;
#endif

            {
                sbOp.PreloadInfo = new PreloadInfo { preloadObjects = sceneInfo.referencedObjects.Where(x => !fileObjects.Contains(x)).ToList() };
            }

            {
                sbOp.Info = new SceneBundleInfo();
                sbOp.Info.bundleName = bundleName;
                sbOp.Info.bundleScenes = assets.Select(x => new SceneLoadInfo
                {
                    asset = x,
                    internalName = Path.GetFileNameWithoutExtension(m_WriteData.AssetToFiles[x][0]),
                    address = m_BuildContent.Addresses[x]
                }).ToList();
            }

            m_WriteData.WriteOperations.Add(sbOp);
        }

        void CreateSceneDataCommand(string internalName, GUID asset)
        {
            var sdOp = new SceneDataWriteOperation();

            var fileObjects = m_WriteData.FileToObjects[internalName];
#if !UNITY_2019_1_OR_NEWER
            // ContentBuildInterface.PrepareScene was not returning stable sorted references, causing a indeterminism and loading errors in some cases
            // Add correct sorting here until patch lands to fix the API.
            fileObjects = GetSortedSceneObjectIdentifiers(fileObjects);
#endif
            sdOp.Command = CreateWriteCommand(internalName, fileObjects, new LinearPackedIdentifiers(2)); // Start at 2: PreloadData = 1

            sdOp.UsageSet = new BuildUsageTagSet();
            m_WriteData.FileToUsageSet.Add(internalName, sdOp.UsageSet);

            sdOp.ReferenceMap = new BuildReferenceMap();
            m_WriteData.FileToReferenceMap.Add(internalName, sdOp.ReferenceMap);

            var sceneInfo = m_DependencyData.SceneInfo[asset];
            sdOp.Scene = sceneInfo.scene;
#if !UNITY_2019_3_OR_NEWER
            sdOp.ProcessedScene = sceneInfo.processedScene;
#endif

            {
                sdOp.PreloadInfo = new PreloadInfo();
                sdOp.PreloadInfo.preloadObjects = sceneInfo.referencedObjects.Where(x => !fileObjects.Contains(x)).ToList();
            }

            m_WriteData.WriteOperations.Add(sdOp);
        }
    }
}
