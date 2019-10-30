using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class CalculateSceneDependencyData : IBuildTask
    {
        public int Version { get { return 2; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IBuildContent m_Content;

        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;

        [InjectContext(ContextUsage.In, true)]
        IBuildCache m_Cache;
#pragma warning restore 649

        CachedInfo GetCachedInfo(GUID scene, IEnumerable<ObjectIdentifier> references, SceneDependencyInfo sceneInfo, BuildUsageTagSet usageTags)
        {
            var info = new CachedInfo();
            info.Asset = m_Cache.GetCacheEntry(scene, Version);

            var dependencies = new HashSet<CacheEntry>();
            foreach (var reference in references)
                dependencies.Add(m_Cache.GetCacheEntry(reference));
            var prefabEntries = AssetDatabase.GetDependencies(AssetDatabase.GUIDToAssetPath(scene.ToString())).Where(path => path.EndsWith(".prefab")).Select(m_Cache.GetCacheEntry);
            dependencies.UnionWith(prefabEntries);
            info.Dependencies = dependencies.ToArray();

            info.Data = new object[] { sceneInfo, usageTags };

            return info;
        }

        public ReturnCode Run()
        {
            if (m_Content.Scenes.IsNullOrEmpty())
                return ReturnCode.SuccessNotRun;

            IList<CachedInfo> cachedInfo = null;
            IList<CachedInfo> uncachedInfo = null;
            if (m_Parameters.UseCache && m_Cache != null)
            {
                IList<CacheEntry> entries = m_Content.Scenes.Select(x => m_Cache.GetCacheEntry(x, Version)).ToList();
                m_Cache.LoadCachedData(entries, out cachedInfo);

                uncachedInfo = new List<CachedInfo>();
            }

            BuildSettings settings = m_Parameters.GetContentBuildSettings();
            for (int i = 0; i < m_Content.Scenes.Count; i++)
            {
                GUID scene = m_Content.Scenes[i];
                string scenePath = AssetDatabase.GUIDToAssetPath(scene.ToString());

                SceneDependencyInfo sceneInfo;
                BuildUsageTagSet usageTags;

                if (cachedInfo != null && cachedInfo[i] != null)
                {
                    if (!m_Tracker.UpdateInfoUnchecked(string.Format("{0} (Cached)", scenePath)))
                        return ReturnCode.Canceled;

                    sceneInfo = (SceneDependencyInfo)cachedInfo[i].Data[0];
                    usageTags = cachedInfo[i].Data[1] as BuildUsageTagSet;
                }
                else
                {
                    if (!m_Tracker.UpdateInfoUnchecked(scenePath))
                        return ReturnCode.Canceled;

                    usageTags = new BuildUsageTagSet();
                    string outputFolder = m_Parameters.TempOutputFolder;
                    if (m_Parameters.UseCache && m_Cache != null)
                        outputFolder = m_Cache.GetCachedArtifactsDirectory(m_Cache.GetCacheEntry(scene, Version));
                    Directory.CreateDirectory(outputFolder);


#if UNITY_2019_3_OR_NEWER
                    sceneInfo = ContentBuildInterface.CalculatePlayerDependenciesForScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache);
#else
                    sceneInfo = ContentBuildInterface.PrepareScene(scenePath, settings, usageTags, m_DependencyData.DependencyUsageCache, outputFolder);
#endif

                    if (uncachedInfo != null)
                        uncachedInfo.Add(GetCachedInfo(scene, sceneInfo.referencedObjects, sceneInfo, usageTags));
                }

                SetOutputInformation(scene, sceneInfo, usageTags);
            }

            if (m_Parameters.UseCache && m_Cache != null)
                m_Cache.SaveCachedData(uncachedInfo);

            return ReturnCode.Success;
        }

        void SetOutputInformation(GUID asset, SceneDependencyInfo sceneInfo, BuildUsageTagSet usageTags)
        {
            // Add generated scene information to BuildDependencyData
            m_DependencyData.SceneInfo.Add(asset, sceneInfo);
            m_DependencyData.SceneUsage.Add(asset, usageTags);
        }
    }
}
