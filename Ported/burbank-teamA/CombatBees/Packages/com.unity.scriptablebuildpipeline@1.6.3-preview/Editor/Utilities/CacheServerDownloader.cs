using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor.Build.CacheServer;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Build.Pipeline.Utilities
{
    class CacheServerDownloader : IDisposable
    {
        const string k_CachePath = "Temp/CacheServer";

        IBuildCache m_Cache;
        Client m_Client;
        Hash128 m_GlobalHash;

        bool m_Disposed;

        Semaphore m_Semaphore;

        public CacheServerDownloader(IBuildCache cache, string host, int port = 8126)
        {
            m_Cache = cache;
            m_Client = new Client(host, port);
            m_Client.Connect();
        }

        public void Dispose()
        {
            if (!m_Disposed)
            {
                m_Disposed = true;
                m_Client.Close();
            }
        }

        public void SetGlobalHash(Hash128 hash)
        {
            m_GlobalHash = hash;
        }

        string GetCachedInfoFile(CacheEntry entry)
        {
            string finalHash = HashingMethods.Calculate(entry.Hash, m_GlobalHash).ToString();
            return string.Format("{0}/{1}_{2}.info", k_CachePath, entry.Guid.ToString(), finalHash);
        }

        string GetCachedArtifactsFile(CacheEntry entry)
        {
            string finalHash = HashingMethods.Calculate(entry.Hash, m_GlobalHash).ToString();
            return string.Format("{0}/{1}_{2}.sbpGz", k_CachePath, entry.Guid.ToString(), finalHash);
        }

        // Called on background thread
        void ThreadedDownloadFinished(object sender, DownloadFinishedEventArgs args)
        {
            // Only run this for Info files
            if (args.DownloadItem.Type != FileType.Info)
                return;

            m_Semaphore.Release();
        }

        // We don't return from this function until all downloads are processed. So it is safe to dispose immediately after.
        public void DownloadMissing(IList<CacheEntry> entries, IList<CachedInfo> cachedInfos)
        {
            Assert.AreEqual(entries.Count, cachedInfos.Count);
            Directory.CreateDirectory(k_CachePath);

            m_Semaphore = new Semaphore(0, entries.Count);
            m_Client.DownloadFinished += ThreadedDownloadFinished;

            // Queue up downloads for the missing or invalid local data
            for (var index = 0; index < entries.Count; index++)
            {
                // Only download data for cachedInfos that are invalid
                if (cachedInfos[index] != null)
                    continue;

                var entry = entries[index];

                string finalHash = HashingMethods.Calculate(entry.Hash, m_GlobalHash).ToHash128().ToString();
                var fileId = FileId.From(entry.Guid.ToString(), finalHash);

                // Download artifacts before info to ensure both are available when download for info returns
                var downloadArtifact = new FileDownloadItem(fileId, FileType.Resource, GetCachedArtifactsFile(entry));
                m_Client.QueueDownload(downloadArtifact);

                var downloadInfo = new FileDownloadItem(fileId, FileType.Info, GetCachedInfoFile(entry));
                m_Client.QueueDownload(downloadInfo);
            }

            // Check downloads to see if it is usable data
            var formatter = new BinaryFormatter();
            for (var index = 0; index < entries.Count; index++)
            {
                // find the next invalid cachedInfo
                while (index < entries.Count && cachedInfos[index] != null)
                    index++;
                // make sure we didn't go out of bounds looking for invalid entries
                if (index >= entries.Count)
                    break;

                // Wait for info download
                m_Semaphore.WaitOne();
                
                string tempInfoFile = GetCachedInfoFile(entries[index]);
                if (!File.Exists(tempInfoFile))
                    continue;

                try
                {
                    CachedInfo info;
                    using (var fileStream = new FileStream(tempInfoFile, FileMode.Open, FileAccess.Read))
                        info = formatter.Deserialize(fileStream) as CachedInfo;

                    if (m_Cache.HasAssetOrDependencyChanged(info))
                        continue;

                    // Not every info file will have artifacts. So just check to see if we downloaded something.
                    // TODO: May want to extend CachedInfo with Artifact knowledge if there is a performance benefit?
                    string tempArtifactFile = GetCachedArtifactsFile(entries[index]);
                    string tempArtifactDir = Path.ChangeExtension(tempArtifactFile, "");
                    if (File.Exists(tempArtifactFile) && !FileCompressor.Decompress(tempArtifactFile, tempArtifactDir))
                        continue;

                    // All valid, move downloaded data into place
                    cachedInfos[index] = info;

                    string targetInfoFile = m_Cache.GetCachedInfoFile(info.Asset);
                    if (File.Exists(targetInfoFile))
                        File.Delete(targetInfoFile);
                    else
                        Directory.CreateDirectory(Path.GetDirectoryName(targetInfoFile));
                    File.Move(tempInfoFile, targetInfoFile);

                    if (Directory.Exists(tempArtifactDir))
                    {
                        string targetArtifactDir = m_Cache.GetCachedArtifactsDirectory(info.Asset);
                        if (Directory.Exists(targetArtifactDir))
                            Directory.Delete(targetArtifactDir, true);
                        Directory.Move(tempArtifactDir, targetArtifactDir);
                    }
                }
                catch (Exception e)
                {
                    BuildLogger.LogException(e);
                }
            }

            m_Client.ResetDownloadFinishedEventHandler();

            ((IDisposable)m_Semaphore).Dispose();
            m_Semaphore = null;

            Directory.Delete(k_CachePath, true);
        }
    }
}
