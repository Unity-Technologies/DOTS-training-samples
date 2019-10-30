using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Build.Pipeline.Utilities
{
    /// <summary>
    /// Default implementation of the Build Cache
    /// </summary>
    public class BuildCache : IBuildCache, IDisposable
    {
        const string k_CachePath = "Library/BuildCache";
        const int k_Version = 2;

        Dictionary<KeyValuePair<GUID, int>, CacheEntry> m_GuidToHash = new Dictionary<KeyValuePair<GUID, int>, CacheEntry>();
        Dictionary<KeyValuePair<string, int>, CacheEntry> m_PathToHash = new Dictionary<KeyValuePair<string, int>, CacheEntry>();

        Thread m_ActiveWriteThread;

        [NonSerialized]
        Hash128 m_GlobalHash;

        [NonSerialized]
        CacheServerUploader m_Uploader;

        [NonSerialized]
        CacheServerDownloader m_Downloader;

        public BuildCache()
        {
            m_GlobalHash = CalculateGlobalArtifactVersionHash();
        }

        public BuildCache(string host, int port = 8126)
        {
            m_GlobalHash = CalculateGlobalArtifactVersionHash();

            if (string.IsNullOrEmpty(host))
                return;

            m_Uploader = new CacheServerUploader(host, port);
            m_Downloader = new CacheServerDownloader(this, host, port);
        }

        // internal for testing purposes only
        internal void OverrideGlobalHash(Hash128 hash)
        {
            m_GlobalHash = hash;
            if (m_Uploader != null)
                m_Uploader.SetGlobalHash(m_GlobalHash);
            if (m_Downloader != null)
                m_Downloader.SetGlobalHash(m_GlobalHash);
        }

        static Hash128 CalculateGlobalArtifactVersionHash()
        {
#if UNITY_2019_3_OR_NEWER
            return HashingMethods.Calculate(Application.unityVersion, k_Version).ToHash128();
#else
            return HashingMethods.Calculate(PlayerSettings.scriptingRuntimeVersion, Application.unityVersion, k_Version).ToHash128();
#endif
        }

        internal void ClearCacheEntryMaps()
        {
            m_GuidToHash.Clear();
            m_PathToHash.Clear();
        }

        public void Dispose()
        {
            SyncPendingSaves();
            if (m_Uploader != null)
                m_Uploader.Dispose();
            if (m_Downloader != null)
                m_Downloader.Dispose();
            m_Uploader = null;
            m_Downloader = null;
        }

        /// <inheritdoc />
        public CacheEntry GetCacheEntry(GUID asset, int version = 1)
        {
            CacheEntry entry;
            KeyValuePair<GUID, int> key = new KeyValuePair<GUID, int>(asset, version);
            if (m_GuidToHash.TryGetValue(key, out entry))
                return entry;

            entry = new CacheEntry { Guid = asset, Version = version };
            string path = AssetDatabase.GUIDToAssetPath(asset.ToString());
            entry.Type = CacheEntry.EntryType.Asset;

            if (path.Equals(CommonStrings.UnityBuiltInExtraPath, StringComparison.OrdinalIgnoreCase) || path.Equals(CommonStrings.UnityDefaultResourcePath, StringComparison.OrdinalIgnoreCase))
                entry.Hash = HashingMethods.Calculate(Application.unityVersion, path).ToHash128();
            else
            {
                entry.Hash = AssetDatabase.GetAssetDependencyHash(path);
                if (!entry.Hash.isValid && File.Exists(path))
                    entry.Hash = HashingMethods.CalculateFile(path).ToHash128();
            }

            if (entry.Hash.isValid)
                entry.Hash = HashingMethods.Calculate(entry.Hash, entry.Version).ToHash128();

            m_GuidToHash[key] = entry;
            return entry;
        }

        /// <inheritdoc />
        public CacheEntry GetCacheEntry(string path, int version = 1)
        {
            CacheEntry entry;
            KeyValuePair<string, int> key = new KeyValuePair<string, int>(path, version);
            if (m_PathToHash.TryGetValue(key, out entry))
                return entry;

            var guid = AssetDatabase.AssetPathToGUID(path);
            if (!string.IsNullOrEmpty(guid))
                return GetCacheEntry(new GUID(guid), version);

            entry = new CacheEntry { File = path, Version = version };
            entry.Guid = HashingMethods.Calculate("FileHash", entry.File).ToGUID();
            if (File.Exists(entry.File))
                entry.Hash = HashingMethods.Calculate(HashingMethods.CalculateFile(entry.File), entry.Version).ToHash128();
            entry.Type = CacheEntry.EntryType.File;

            m_PathToHash[key] = entry;
            return entry;
        }

        /// <inheritdoc />
        public CacheEntry GetCacheEntry(ObjectIdentifier objectID, int version = 1)
        {
            if (objectID.guid.Empty())
                return GetCacheEntry(objectID.filePath, version);
            return GetCacheEntry(objectID.guid, version);
        }

        internal CacheEntry GetUpdatedCacheEntry(CacheEntry entry)
        {
            if (entry.Type == CacheEntry.EntryType.File)
                return GetCacheEntry(entry.File, entry.Version);
            if (entry.Type == CacheEntry.EntryType.Asset)
                return GetCacheEntry(entry.Guid, entry.Version);
            return entry;
        }

        /// <inheritdoc />
        public bool HasAssetOrDependencyChanged(CachedInfo info)
        {
            if (info == null || !info.Asset.IsValid() || info.Asset != GetUpdatedCacheEntry(info.Asset))
                return true;

            foreach (var dependency in info.Dependencies)
            {
                if (!dependency.IsValid() || dependency != GetUpdatedCacheEntry(dependency))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public string GetCachedInfoFile(CacheEntry entry)
        {
            var guid = entry.Guid.ToString();
            string finalHash = HashingMethods.Calculate(m_GlobalHash, entry.Hash).ToString();
            return string.Format("{0}/{1}/{2}/{3}/{2}.info", k_CachePath, guid.Substring(0, 2), guid, finalHash);
        }

        /// <inheritdoc />
        public string GetCachedArtifactsDirectory(CacheEntry entry)
        {
            var guid = entry.Guid.ToString();
            string finalHash = HashingMethods.Calculate(m_GlobalHash, entry.Hash).ToString();
            return string.Format("{0}/{1}/{2}/{3}", k_CachePath, guid.Substring(0, 2), guid, finalHash);
        }

        class FileOperations
        {
            public FileOperations(int size)
            {
                data = new FileOperation[size];
                waitLock = new Semaphore(0, size);
            }

            public FileOperation[] data;
            public Semaphore waitLock;
        }

        struct FileOperation
        {
            public string file;
            public MemoryStream bytes;
        }

        static void Read(object data)
        {
            var ops = (FileOperations)data;
            for (int index = 0; index < ops.data.Length; index++, ops.waitLock.Release())
            {
                try
                {
                    var op = ops.data[index];
                    if (File.Exists(op.file))
                    {
                        byte[] bytes = File.ReadAllBytes(op.file);
                        if (bytes.Length > 0)
                            op.bytes = new MemoryStream(bytes, false);
                    }
                    ops.data[index] = op;
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                }
            }
        }

        static void Write(object data)
        {
            var ops = (FileOperations)data;
            for (int index = 0; index < ops.data.Length; index++)
            {
                // Basic spin lock
                ops.waitLock.WaitOne();

                var op = ops.data[index];
                if (op.bytes != null && op.bytes.Length > 0)
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(op.file));
                        File.WriteAllBytes(op.file, op.bytes.GetBuffer());
                    }
                    catch (Exception e)
                    {
                        BuildLogger.LogException(e);
                    }
                }
            }
            ((IDisposable)ops.waitLock).Dispose();
        }

        /// <inheritdoc />
        public void LoadCachedData(IList<CacheEntry> entries, out IList<CachedInfo> cachedInfos)
        {
            if (entries == null)
            {
                cachedInfos = null;
                return;
            }

            if (entries.Count == 0)
            {
                cachedInfos = new List<CachedInfo>();
                return;
            }

            // Setup Operations
            var ops = new FileOperations(entries.Count);
            for (int i = 0; i < entries.Count; i++)
            {
                var op = ops.data[i];
                op.file = GetCachedInfoFile(entries[i]);
                ops.data[i] = op;
            }

            // Start file reading
            Thread thread = new Thread(Read);
            thread.Start(ops);

            cachedInfos = new List<CachedInfo>(entries.Count);

            // Deserialize as files finish reading
            var formatter = new BinaryFormatter();
            for (int index = 0; index < entries.Count; index++)
            {
                // Basic wait lock
                ops.waitLock.WaitOne();

                CachedInfo info = null;
                try
                {
                    var op = ops.data[index];
                    if (op.bytes != null && op.bytes.Length > 0)
                        info = formatter.Deserialize(op.bytes) as CachedInfo;
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                }
                cachedInfos.Add(info);
            }
            thread.Join();
            ((IDisposable)ops.waitLock).Dispose();

            // Validate cached data is reusable
            for (int i = 0; i < cachedInfos.Count; i++)
            {
                if (HasAssetOrDependencyChanged(cachedInfos[i]))
                    cachedInfos[i] = null;
            }

            // If we have a cache server connection, download & check any missing info
            if (m_Downloader != null)
                m_Downloader.DownloadMissing(entries, cachedInfos);

            Assert.AreEqual(entries.Count, cachedInfos.Count);
        }

        /// <inheritdoc />
        public void SaveCachedData(IList<CachedInfo> infos)
        {
            if (infos == null || infos.Count == 0)
                return;

            // Setup Operations
            var ops = new FileOperations(infos.Count);
            for (int i = 0; i < infos.Count; i++)
            {
                var op = ops.data[i];
                op.file = GetCachedInfoFile(infos[i].Asset);
                ops.data[i] = op;
            }

            // Start writing thread
            SyncPendingSaves();
            m_ActiveWriteThread = new Thread(Write);
            m_ActiveWriteThread.Start(ops);

            // Serialize data as previous data is being written out
            var formatter = new BinaryFormatter();
            for (int index = 0; index < infos.Count; index++, ops.waitLock.Release())
            {
                try
                {
                    var op = ops.data[index];
                    var stream = new MemoryStream();
                    formatter.Serialize(stream, infos[index]);
                    if (stream.Length > 0)
                    {
                        op.bytes = stream;
                        ops.data[index] = op;

                        // If we have a cache server connection, upload the cached data
                        if (m_Uploader != null)
                            m_Uploader.QueueUpload(infos[index].Asset, GetCachedArtifactsDirectory(infos[index].Asset), new MemoryStream(stream.GetBuffer(), false));
                    }
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                }
            }
        }

        internal void SyncPendingSaves()
        {
            if (m_ActiveWriteThread != null)
            {
                m_ActiveWriteThread.Join();
                m_ActiveWriteThread = null;
            }
        }

        internal struct CacheFolder
        {
            public DirectoryInfo directory;
            public long Length { get; set; }
            public void Delete() => directory.Delete(true);
            public DateTime LastAccessTimeUtc
            {
                get => directory.LastAccessTimeUtc;
                internal set => directory.LastAccessTimeUtc = value;
            }
        }

        public static void PurgeCache(bool prompt)
        {
            if (!Directory.Exists(k_CachePath))
            {
                if (prompt)
                    Debug.Log("Current build cache is empty.");
                return;
            }

            if (prompt)
            {
                if (!EditorUtility.DisplayDialog("Purge Build Cache", "Do you really want to purge your entire build cache?", "Yes", "No"))
                    return;

                EditorUtility.DisplayProgressBar(BuildCachePreferences.BuildCacheProperties.purgeCache.text, BuildCachePreferences.BuildCacheProperties.pleaseWait.text, 0.0F);
                Directory.Delete(k_CachePath, true);
                EditorUtility.ClearProgressBar();
            }
            else
                Directory.Delete(k_CachePath, true);
        }

        public static void PruneCache()
        {
            int maximumSize = EditorPrefs.GetInt("BuildCache.maximumSize", 200);
            long maximumCacheSize = maximumSize * 1073741824L; // gigabytes to bytes

            // Get sizes based on common directory root for a guid / hash
            ComputeCacheSizeAndFolders(out long currentCacheSize, out List<CacheFolder> cacheFolders);

            if (currentCacheSize < maximumCacheSize)
            {
                Debug.LogFormat("Current build cache currentCacheSize {0}, prune threshold {1} GB. No prune performed. You can change this value in the \"Edit/Preferences...\" window.", EditorUtility.FormatBytes(currentCacheSize), maximumSize);
                return;
            }

            if (!EditorUtility.DisplayDialog("Prune Build Cache", string.Format("Current build cache currentCacheSize is {0}, which is over the prune threshold of {1}. Do you want to prune your build cache now?", EditorUtility.FormatBytes(currentCacheSize), EditorUtility.FormatBytes(maximumCacheSize)), "Yes", "No"))
                return;

            EditorUtility.DisplayProgressBar(BuildCachePreferences.BuildCacheProperties.pruneCache.text, BuildCachePreferences.BuildCacheProperties.pleaseWait.text, 0.0F);

            PruneCacheFolders(maximumCacheSize, currentCacheSize, cacheFolders);

            EditorUtility.ClearProgressBar();
        }

        public static void PruneCache_Background(long maximumCacheSize)
        {
            // Get sizes based on common directory root for a guid / hash
            ComputeCacheSizeAndFolders(out long currentCacheSize, out List<CacheFolder> cacheFolders);
            if (currentCacheSize < maximumCacheSize)
                return;

            PruneCacheFolders(maximumCacheSize, currentCacheSize, cacheFolders);
        }

        internal static void ComputeCacheSizeAndFolders(out long currentCacheSize, out List<CacheFolder> cacheFolders)
        {
            currentCacheSize = 0;
            cacheFolders = new List<CacheFolder>();

            var directory = new DirectoryInfo(k_CachePath);
            if (!directory.Exists)
                return;

            int length = directory.FullName.Count(x => x == Path.DirectorySeparatorChar) + 3;
            DirectoryInfo[] subDirectories = directory.GetDirectories("*", SearchOption.AllDirectories);
            foreach (var subDirectory in subDirectories)
            {
                if (subDirectory.FullName.Count(x => x == Path.DirectorySeparatorChar) != length)
                    continue;

                FileInfo[] files = subDirectory.GetFiles("*", SearchOption.AllDirectories);
                var cacheFolder = new CacheFolder { directory = subDirectory, Length = files.Sum(x => x.Length) };
                cacheFolders.Add(cacheFolder);

                currentCacheSize += cacheFolder.Length;
            }
        }

        internal static void PruneCacheFolders(long maximumCacheSize, long currentCacheSize, List<CacheFolder> cacheFolders)
        {
            cacheFolders.Sort((a, b) => a.LastAccessTimeUtc.CompareTo(b.LastAccessTimeUtc));
            // Need to delete sets of files as the .info might reference a specific file artifact
            foreach (var cacheFolder in cacheFolders)
            {
                currentCacheSize -= cacheFolder.Length;
                cacheFolder.Delete();
                if (currentCacheSize < maximumCacheSize)
                    break;
            }
        }
    }
}
