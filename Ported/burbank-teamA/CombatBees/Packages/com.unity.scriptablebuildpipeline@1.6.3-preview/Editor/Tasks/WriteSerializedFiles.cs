using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.WriteTypes;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class WriteSerializedFiles : IBuildTask
    {
        public int Version { get { return 2; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In)]
        IWriteData m_WriteData;

        [InjectContext]
        IBuildResults m_Results;

        [InjectContext(ContextUsage.In, true)]
        IProgressTracker m_Tracker;

        [InjectContext(ContextUsage.In, true)]
        IBuildCache m_Cache;
#pragma warning restore 649

        CacheEntry GetCacheEntry(IWriteOperation operation, BuildSettings settings, BuildUsageTagGlobal globalUsage)
        {
            var entry = new CacheEntry();
            entry.Type = CacheEntry.EntryType.Data;
            entry.Guid = HashingMethods.Calculate("WriteSerializedFiles", operation.Command.internalName).ToGUID();
            entry.Hash = HashingMethods.Calculate(Version, operation.GetHash128(), settings.GetHash128(), globalUsage).ToHash128();
            entry.Version = Version;
            return entry;
        }

        CachedInfo GetCachedInfo(CacheEntry entry, IWriteOperation operation, WriteResult result)
        {
            var info = new CachedInfo();
            info.Asset = entry;

            var dependencies = new HashSet<CacheEntry>();
#if !UNITY_2019_3_OR_NEWER
            var sceneBundleOp = operation as SceneBundleWriteOperation;
            if (sceneBundleOp != null)
                dependencies.Add(m_Cache.GetCacheEntry(sceneBundleOp.ProcessedScene));
            var sceneDataOp = operation as SceneDataWriteOperation;
            if (sceneDataOp != null)
                dependencies.Add(m_Cache.GetCacheEntry(sceneDataOp.ProcessedScene));
#endif
            foreach (var serializeObject in operation.Command.serializeObjects)
                dependencies.Add(m_Cache.GetCacheEntry(serializeObject.serializationObject));
            info.Dependencies = dependencies.ToArray();

            info.Data = new object[] { result };

            return info;
        }

        public ReturnCode Run()
        {
            BuildUsageTagGlobal globalUsage = m_DependencyData.GlobalUsage;
            foreach (var sceneInfo in m_DependencyData.SceneInfo)
                globalUsage |= sceneInfo.Value.globalUsage;

            IList<CacheEntry> entries = m_WriteData.WriteOperations.Select(x => GetCacheEntry(x, m_Parameters.GetContentBuildSettings(), globalUsage)).ToList();
            IList<CachedInfo> cachedInfo = null;
            IList<CachedInfo> uncachedInfo = null;
            if (m_Parameters.UseCache && m_Cache != null)
            {
                m_Cache.LoadCachedData(entries, out cachedInfo);

                uncachedInfo = new List<CachedInfo>();
            }

            for (int i = 0; i < m_WriteData.WriteOperations.Count; i++)
            {
                IWriteOperation op = m_WriteData.WriteOperations[i];

                WriteResult result;
                if (cachedInfo != null && cachedInfo[i] != null)
                {
                    if (!m_Tracker.UpdateInfoUnchecked(string.Format("{0} (Cached)", op.Command.internalName)))
                        return ReturnCode.Canceled;

                    result = (WriteResult)cachedInfo[i].Data[0];
                }
                else
                {
                    if (!m_Tracker.UpdateInfoUnchecked(op.Command.internalName))
                        return ReturnCode.Canceled;

                    var outputFolder = m_Parameters.TempOutputFolder;
                    if (m_Parameters.UseCache && m_Cache != null)
                        outputFolder = m_Cache.GetCachedArtifactsDirectory(entries[i]);
                    Directory.CreateDirectory(outputFolder);

                    result = op.Write(outputFolder, m_Parameters.GetContentBuildSettings(), globalUsage);

                    if (uncachedInfo != null)
                        uncachedInfo.Add(GetCachedInfo(entries[i], op, result));
                }

                SetOutputInformation(op.Command.internalName, result);
            }

            if (m_Parameters.UseCache && m_Cache != null)
                m_Cache.SaveCachedData(uncachedInfo);

            return ReturnCode.Success;
        }

        void SetOutputInformation(string fileName, WriteResult result)
        {
            m_Results.WriteResults.Add(fileName, result);
        }
    }
}
