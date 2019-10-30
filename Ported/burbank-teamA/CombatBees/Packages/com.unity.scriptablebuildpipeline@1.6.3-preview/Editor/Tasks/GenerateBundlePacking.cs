using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class GenerateBundlePacking : IBuildTask
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
            Dictionary<GUID, List<GUID>> assetToReferences = new Dictionary<GUID, List<GUID>>();

            // Pack each asset bundle
            foreach (var bundle in m_BuildContent.BundleLayout)
            {
                if (ValidAssetBundle(bundle.Value))
                    PackAssetBundle(bundle.Key, bundle.Value, assetToReferences);
                else if (ValidationMethods.ValidSceneBundle(bundle.Value))
                    PackSceneBundle(bundle.Key, bundle.Value, assetToReferences);
            }

            // Calculate Asset file load dependency list
            foreach (var bundle in m_BuildContent.BundleLayout)
            {
                foreach (var asset in bundle.Value)
                {
                    List<string> files = m_WriteData.AssetToFiles[asset];
                    List<GUID> references = assetToReferences[asset];
                    foreach (var reference in references)
                    {
                        List<string> referenceFiles = m_WriteData.AssetToFiles[reference];
                        if (!files.Contains(referenceFiles[0]))
                            files.Add(referenceFiles[0]);
                    }
                }
            }

            return ReturnCode.Success;
        }

        void PackAssetBundle(string bundleName, List<GUID> includedAssets, Dictionary<GUID, List<GUID>> assetToReferences)
        {
            var internalName = string.Format(CommonStrings.AssetBundleNameFormat, m_PackingMethod.GenerateInternalFileName(bundleName));

            var allObjects = new HashSet<ObjectIdentifier>();
            foreach (var asset in includedAssets)
            {
                AssetLoadInfo assetInfo = m_DependencyData.AssetInfo[asset];
                allObjects.UnionWith(assetInfo.includedObjects);

                var references = new List<ObjectIdentifier>();
                references.AddRange(assetInfo.referencedObjects);
                assetToReferences[asset] = FilterReferencesForAsset(asset, references);

                allObjects.UnionWith(references);
                m_WriteData.AssetToFiles[asset] = new List<string> { internalName };
            }

            m_WriteData.FileToBundle.Add(internalName, bundleName);
            m_WriteData.FileToObjects.Add(internalName, allObjects.ToList());
        }

        void PackSceneBundle(string bundleName, List<GUID> includedScenes, Dictionary<GUID, List<GUID>> assetToReferences)
        {
            if (includedScenes.IsNullOrEmpty())
                return;

            string firstFileName = "";
            HashSet<ObjectIdentifier> previousSceneObjects = new HashSet<ObjectIdentifier>();
            List<string> sceneInternalNames = new List<string>();
            foreach (var scene in includedScenes)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(scene.ToString());
                var internalSceneName = m_PackingMethod.GenerateInternalFileName(scenePath);
                if (string.IsNullOrEmpty(firstFileName))
                    firstFileName = internalSceneName;
                var internalName = string.Format(CommonStrings.SceneBundleNameFormat, firstFileName, internalSceneName);

                SceneDependencyInfo sceneInfo = m_DependencyData.SceneInfo[scene];

                var references = new List<ObjectIdentifier>();
                references.AddRange(sceneInfo.referencedObjects);
                assetToReferences[scene] = FilterReferencesForAsset(scene, references, previousSceneObjects);
                previousSceneObjects.UnionWith(references);

                m_WriteData.FileToObjects.Add(internalName, references);
                m_WriteData.FileToBundle.Add(internalName, bundleName);

                var files = new List<string>{ internalName };
                files.AddRange(sceneInternalNames);
                m_WriteData.AssetToFiles[scene] = files;

                sceneInternalNames.Add(internalName);
            }
        }

        List<GUID> FilterReferencesForAsset(GUID asset, List<ObjectIdentifier> references, HashSet<ObjectIdentifier> previousSceneObjects = null)
        {
            var referencedAssets = new HashSet<AssetLoadInfo>();

            // First pass: Remove Default Resources and Includes for Assets assigned to Bundles
            for (int i = references.Count - 1; i >= 0; --i)
            {
                var reference = references[i];
                if (reference.filePath.Equals(CommonStrings.UnityDefaultResourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    references.RemoveAt(i);
                    continue; // TODO: Fix this so we can pull these in
                }

                AssetLoadInfo referenceInfo;
                if (m_DependencyData.AssetInfo.TryGetValue(reference.guid, out referenceInfo))
                {
                    references.RemoveAt(i);
                    referencedAssets.Add(referenceInfo);
                    continue;
                }
            }

            // Second pass: Remove References also included by non-circular Referenced Assets
            foreach (var referencedAsset in referencedAssets)
            {
                var circularRef = referencedAsset.referencedObjects.Select(x => x.guid).Contains(asset);
                if (circularRef)
                    continue;

                references.RemoveAll(x => referencedAsset.referencedObjects.Contains(x));
            }

            // Final pass: Remove References also included by circular Referenced Assets if Asset's GUID is higher than Referenced Asset's GUID
            foreach (var referencedAsset in referencedAssets)
            {
                var circularRef = referencedAsset.referencedObjects.Select(x => x.guid).Contains(asset);
                if (!circularRef)
                    continue;

                if (asset < referencedAsset.asset)
                    continue;

                references.RemoveAll(x => referencedAsset.referencedObjects.Contains(x));
            }

            // Special path for scenes, they can use data from previous sharedAssets in the same bundle
            if (!previousSceneObjects.IsNullOrEmpty())
                references.RemoveAll(previousSceneObjects.Contains);

            return referencedAssets.Select(x => x.asset).ToList();
        }
    }
}
