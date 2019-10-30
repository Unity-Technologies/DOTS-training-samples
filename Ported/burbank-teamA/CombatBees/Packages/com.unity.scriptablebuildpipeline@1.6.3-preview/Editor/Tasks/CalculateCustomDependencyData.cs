using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class CalculateCustomDependencyData : IBuildTask
    {
        public int Version { get { return 2; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.InOut)]
        IBundleBuildContent m_Content;

        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;
#pragma warning restore 649

        BuildUsageTagGlobal m_GlobalUsage;

        public ReturnCode Run()
        {
            m_GlobalUsage = m_DependencyData.GlobalUsage;
            foreach (SceneDependencyInfo sceneInfo in m_DependencyData.SceneInfo.Values)
                m_GlobalUsage |= sceneInfo.globalUsage;

            for (int i = 0; i < m_Content.CustomAssets.Count; i++)
            {
                CustomContent info = m_Content.CustomAssets[i];
                info.Processor(info.Asset, this);
            }
            return ReturnCode.Success;
        }

        public void GetObjectIdentifiersAndTypesForSerializedFile(string path, out ObjectIdentifier[] objectIdentifiers, out Type[] types)
        {
            objectIdentifiers = ContentBuildInterface.GetPlayerObjectIdentifiersInSerializedFile(path, m_Parameters.Target);
            types = ContentBuildInterface.GetTypeForObjects(objectIdentifiers);
        }

        public void CreateAssetEntryForObjectIdentifiers(ObjectIdentifier[] includedObjects, string path, string bundleName, string address, Type mainAssetType)
        {
            AssetLoadInfo assetInfo = new AssetLoadInfo();
            BuildUsageTagSet usageTags = new BuildUsageTagSet();

            assetInfo.asset = HashingMethods.Calculate(address).ToGUID();
            if (m_DependencyData.AssetInfo.ContainsKey(assetInfo.asset))
                throw new ArgumentException(string.Format("Custom Asset '{0}' already exists. Building duplicate asset entries is not supported.", address));

            assetInfo.includedObjects = new List<ObjectIdentifier>(includedObjects);
            var referencedObjects = ContentBuildInterface.GetPlayerDependenciesForObjects(includedObjects, m_Parameters.Target, m_Parameters.ScriptInfo);
            assetInfo.referencedObjects = new List<ObjectIdentifier>(referencedObjects);
            ContentBuildInterface.CalculateBuildUsageTags(referencedObjects, includedObjects, m_GlobalUsage, usageTags, m_DependencyData.DependencyUsageCache);

            List<GUID> assets;
            m_Content.BundleLayout.GetOrAdd(bundleName, out assets);
            assets.Add(assetInfo.asset);

            m_Content.Addresses[assetInfo.asset] = address;
            m_Content.FakeAssets[assetInfo.asset] = path;
            SetOutputInformation(assetInfo.asset, assetInfo, usageTags);
        }

        void SetOutputInformation(GUID asset, AssetLoadInfo assetInfo, BuildUsageTagSet usageTags)
        {
            // Add generated asset information to IDependencyData
            m_DependencyData.AssetInfo.Add(asset, assetInfo);
            m_DependencyData.AssetUsage.Add(asset, usageTags);
        }
    }
}