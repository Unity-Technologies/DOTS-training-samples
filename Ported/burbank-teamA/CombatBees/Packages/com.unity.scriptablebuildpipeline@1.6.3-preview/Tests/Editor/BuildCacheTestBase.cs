using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    abstract internal class BuildCacheTestBase
    {
        protected const string kBuildCacheTestPath = "Assets/BuildCacheTestAssets";

        protected string kTestFile1
        {
            get { return Path.Combine(kBuildCacheTestPath, "testfile1.txt"); }
        }

        protected string kUncachedTestFilename
        {
            get { return Path.Combine(kBuildCacheTestPath, "uncached.txt"); }
        }

        protected string kTempAssetFilename
        {
            get { return Path.Combine(kBuildCacheTestPath, "temporary.txt"); }
        }

        protected GUID TestFile1GUID
        {
            get { return new GUID(AssetDatabase.AssetPathToGUID(kTestFile1)); }
        }

        protected GUID UncachedGUID
        {
            get { return new GUID(AssetDatabase.AssetPathToGUID(kUncachedTestFilename)); }
        }

        protected GUID TempAssetGUID
        {
            get { return new GUID(AssetDatabase.AssetPathToGUID(kTempAssetFilename)); }
        }

        protected BuildCache m_Cache;

        internal virtual void OneTimeSetupDerived() { }
        internal virtual void OneTimeTearDownDerived() { }
        internal virtual void SetupDerived() { }
        internal virtual void TeardownDerived() { }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.CreateDirectory(kBuildCacheTestPath);
            File.WriteAllText(kTestFile1, "t1");
            File.WriteAllText(kUncachedTestFilename, "uncached");
            File.WriteAllText(kTempAssetFilename, "delete me");
            AssetDatabase.Refresh();
            OneTimeSetupDerived();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(kBuildCacheTestPath, true);
            File.Delete(kBuildCacheTestPath + ".meta");
            AssetDatabase.Refresh();
            OneTimeTearDownDerived();
        }

        [SetUp]
        public void Setup()
        {
            PurgeBuildCache();
            RecreateBuildCache();
            SetupDerived();
        }

        [TearDown]
        public void TearDown()
        {
            if (m_Cache != null)
            {
                m_Cache.Dispose();
                m_Cache = null;
            }
            TeardownDerived();
        }

        protected virtual void RecreateBuildCache()
        {
            if (m_Cache != null)
            {
                m_Cache.Dispose();
                m_Cache = null;
            }
            m_Cache = new BuildCache();
        }

        protected virtual void PurgeBuildCache()
        {
            if (m_Cache != null)
                m_Cache.SyncPendingSaves();
            BuildCache.PurgeCache(false);
        }

        static protected CachedInfo LoadCachedInfoForGUID(BuildCache cache, GUID guid)
        {
            IList<CachedInfo> infos;
            CacheEntry entry1 = cache.GetCacheEntry(guid);
            cache.LoadCachedData(new List<CacheEntry>() { entry1 }, out infos);
            return infos[0];
        }

        static protected void StoreDataInCacheWithGUID(BuildCache cache, GUID guid, object data, GUID depGUID = new GUID())
        {
            List<CacheEntry> deps = new List<CacheEntry>();
            if (!depGUID.Empty())
                deps.Add(cache.GetCacheEntry(depGUID));

            CacheEntry entry1 = cache.GetCacheEntry(guid);
            CachedInfo info = new CachedInfo();
            info.Asset = entry1;
            info.Dependencies = deps.ToArray();
            info.Data = new object[] { data };
            cache.SaveCachedData(new List<CachedInfo>() { info });
            cache.SyncPendingSaves();
        }

        static protected GUID CreateTestTextAsset(string contents)
        {
            string filename;
            return CreateTestTextAsset(contents, out filename);
        }

        static protected GUID CreateTestTextAsset(string contents, out string filename)
        {
            int fileIndex = 0;
            while (true)
            {
                filename = Path.Combine(kBuildCacheTestPath, "testasset" + fileIndex);
                if (!File.Exists(filename))
                {
                    File.WriteAllText(filename, contents);
                    AssetDatabase.Refresh();
                    return new GUID(AssetDatabase.AssetPathToGUID(filename));
                }

                fileIndex++;
            }
        }

        static protected void ModifyTestTextAsset(GUID guid, string text)
        {
            string filename = AssetDatabase.GUIDToAssetPath(guid.ToString());
            File.WriteAllText(filename, text);
            AssetDatabase.ImportAsset(filename);
        }

        [Test]
        public void WhenLoadingCachedDataForGUIDWithModifiedDependency_CachedInfoIsNull()
        {
            GUID depGuid = CreateTestTextAsset("mytext");
            StoreDataInCacheWithGUID(m_Cache, TestFile1GUID, "data", depGuid);
            ModifyTestTextAsset(depGuid, "mytext2");
            RecreateBuildCache();
            CachedInfo info = LoadCachedInfoForGUID(m_Cache, TestFile1GUID);
            Assert.IsNull(info);
        }

        [Test]
        public void WhenLoadingCachedDataForModifiedGUID_CachedInfoIsNull()
        {
            GUID guid = CreateTestTextAsset("mytext");
            StoreDataInCacheWithGUID(m_Cache, guid, "data");
            ModifyTestTextAsset(guid, "mytext2");
            RecreateBuildCache();
            CachedInfo info = LoadCachedInfoForGUID(m_Cache, guid);
            Assert.IsNull(info);
        }

        [Test]
        public void WhenLoadingStoredCachedData_CachedInfoIsValid()
        {
            StoreDataInCacheWithGUID(m_Cache, TestFile1GUID, "data");
            RecreateBuildCache();
            CachedInfo info = LoadCachedInfoForGUID(m_Cache, TestFile1GUID);
            Assert.AreEqual("data", (string)info.Data[0]);
        }

        [Test]
        public void WhenLoadingUncachedData_CachedInfoIsNull()
        {
            CachedInfo info = LoadCachedInfoForGUID(m_Cache, UncachedGUID);
            Assert.IsNull(info);
        }

        [Test]
        public void WhenGlobalVersionChanges_OriginalCachedInfoDoesNotNeedRebuild()
        {
            m_Cache.OverrideGlobalHash(new Hash128(0, 1, 0, 0));

            StoreDataInCacheWithGUID(m_Cache, TestFile1GUID, "data");
            CachedInfo info1 = LoadCachedInfoForGUID(m_Cache, TestFile1GUID);

            RecreateBuildCache();
            m_Cache.OverrideGlobalHash(new Hash128(0, 2, 0, 0));
            CachedInfo info2 = LoadCachedInfoForGUID(m_Cache, TestFile1GUID);

            Assert.IsFalse(m_Cache.HasAssetOrDependencyChanged(info1)); // original info still doesn't need rebuild. Required for content update
            Assert.IsNull(info2); // however the data cannot be recovered
        }

        [Test]
        public void WhenLocalVersionChanges_AssetReturnsDifferentCacheEntry()
        {
            GUID guid = CreateTestTextAsset("mytext");

            var entry1 = m_Cache.GetCacheEntry(guid, 2);
            var entry2 = m_Cache.GetCacheEntry(guid, 4);

            Assert.AreEqual(entry1.Guid, entry2.Guid);
            Assert.AreEqual(entry1.File, entry2.File);
            Assert.AreEqual(entry1.Type, entry2.Type);
            Assert.AreNotEqual(entry1.Version, entry2.Version);
            Assert.AreNotEqual(entry1.Hash, entry2.Hash);
        }

        [Test]
        public void WhenLocalVersionChanges_FileReturnsDifferentCacheEntry()
        {
            string filename;
            CreateTestTextAsset("mytext", out filename);

            var entry1 = m_Cache.GetCacheEntry(filename, 2);
            var entry2 = m_Cache.GetCacheEntry(filename, 4);

            Assert.AreEqual(entry1.Guid, entry2.Guid);
            Assert.AreEqual(entry1.File, entry2.File);
            Assert.AreEqual(entry1.Type, entry2.Type);
            Assert.AreNotEqual(entry1.Version, entry2.Version);
            Assert.AreNotEqual(entry1.Hash, entry2.Hash);
        }

        [Test]
        public void GetUpdatedCacheEntry_ReturnsCacheEntryWithSameVersionAndHash_IfAssetHasNotChanged()
        {
            GUID guid = CreateTestTextAsset("mytext");

            var entry1 = m_Cache.GetCacheEntry(guid, 2);
            m_Cache.ClearCacheEntryMaps();
            var entry2 = m_Cache.GetUpdatedCacheEntry(entry1);

            Assert.AreEqual(entry1.Guid, entry2.Guid);
            Assert.AreEqual(entry1.File, entry2.File);
            Assert.AreEqual(entry1.Type, entry2.Type);
            Assert.AreEqual(entry1.Version, entry2.Version);
            Assert.AreEqual(entry1.Hash, entry2.Hash);
        }

        [Test]
        public void GetUpdatedCacheEntry_ReturnsCacheEntryWithSameVersionAndHash_IfFileHasNotChanged()
        {
            string filename;
            CreateTestTextAsset("mytext", out filename);

            var entry1 = m_Cache.GetCacheEntry(filename, 2);
            m_Cache.ClearCacheEntryMaps();
            var entry2 = m_Cache.GetUpdatedCacheEntry(entry1);

            Assert.AreEqual(entry1.Guid, entry2.Guid);
            Assert.AreEqual(entry1.File, entry2.File);
            Assert.AreEqual(entry1.Type, entry2.Type);
            Assert.AreEqual(entry1.Version, entry2.Version);
            Assert.AreEqual(entry1.Hash, entry2.Hash);
        }

        [Test]
        public void GetCacheEntry_InvalidPath_ReturnsInvalidCacheEntry()
        {
            var entry = m_Cache.GetCacheEntry("this/path/does/not/exist");
            Assert.IsFalse(entry.IsValid());
        }

        [Test]
        public void GetCacheEntry_InvalidGUID_ReturnsInvalidCacheEntry()
        {
            var entry = m_Cache.GetCacheEntry(new GUID("00000000000000000000000000000000"));
            Assert.IsFalse(entry.IsValid());
        }

        [Test]
        public void GetCacheEntry_FormerValidPathAndGUID_ReturnsInvalidCacheEntry()
        {
            var tempAssetGUID = TempAssetGUID;

            var entry1 = m_Cache.GetCacheEntry(kTempAssetFilename);
            var entry2 = m_Cache.GetCacheEntry(tempAssetGUID);
            Assert.IsTrue(entry1.IsValid());
            Assert.IsTrue(entry2.IsValid());

            File.Delete(kTempAssetFilename);
            AssetDatabase.Refresh();
            m_Cache.ClearCacheEntryMaps();

            entry1 = m_Cache.GetCacheEntry(kTempAssetFilename);
            entry2 = m_Cache.GetCacheEntry(tempAssetGUID);
            Assert.IsFalse(entry1.IsValid());
            Assert.IsFalse(entry2.IsValid());
        }

        [Test]
        public void GetCacheEntry_PathForAsset_ReturnsAssetBasedCacheEntry()
        {
            var entry1 = m_Cache.GetCacheEntry(kTestFile1);
            var entry2 = m_Cache.GetCacheEntry(TestFile1GUID);

            Assert.IsTrue(entry1.Type == CacheEntry.EntryType.Asset);
            Assert.AreEqual(entry2, entry1);
        }

        [Test]
        public void GetCachedInfoFile_IsInside_GetCachedArtifactsDirectory()
        {
            var entry = m_Cache.GetCacheEntry(kTestFile1);
            var infoFile = m_Cache.GetCachedInfoFile(entry);
            var artifactsDirectory = m_Cache.GetCachedArtifactsDirectory(entry);

            StringAssert.Contains(artifactsDirectory, infoFile);
        }

        void WriteRandomFile(string fileName, int sizeInMB)
        {
            byte[] data = new byte[sizeInMB * 1024 * 1024];
            var rng = new System.Random();
            rng.NextBytes(data);
            File.WriteAllBytes(fileName, data);
        }

        void PopulateCache(out int filesWritten, out string[] artifactsDirectories)
        {
            var entries = new[]
            {
                m_Cache.GetCacheEntry(kTestFile1),
                m_Cache.GetCacheEntry(kUncachedTestFilename)
            };
            artifactsDirectories = new[]
            {
                Path.GetFullPath(m_Cache.GetCachedArtifactsDirectory(entries[0])),
                Path.GetFullPath(m_Cache.GetCachedArtifactsDirectory(entries[1]))
            };

            // Setup cache
            filesWritten = 0;
            foreach (var directory in artifactsDirectories)
            {
                Directory.CreateDirectory(directory);
                for (int i = 0; i < 2; i++)
                {
                    WriteRandomFile($"{directory}/cachefile_{i}.bytes", 2);
                    filesWritten++;
                }
            }
        }

        [Test]
        public void PruneCache_ComputeCacheSizeAndFolders_ReturnsCorrectSizeAndFolders()
        {
            PopulateCache(out int filesWritten, out string[] artifactsDirectories);

            BuildCache.ComputeCacheSizeAndFolders(out long currentCacheSize, out List<BuildCache.CacheFolder> cacheFolders);
            //filesWritten * 2mb each * 1024 to kb * 1024 to b
            Assert.AreEqual(filesWritten * 2 * 1024 * 1024, currentCacheSize);
            Assert.AreEqual(artifactsDirectories.Length, cacheFolders.Count);
            var folders = cacheFolders.Select(x => x.directory.FullName);
            CollectionAssert.AreEquivalent(artifactsDirectories, folders);
        }

        [Test]
        public void PruneCache_PruneCacheFolders_WillRemoveOldestFolders()
        {
            PopulateCache(out int filesWritten, out string[] artifactsDirectories);
            BuildCache.ComputeCacheSizeAndFolders(out long currentCacheSize, out List<BuildCache.CacheFolder> cacheFolders);

            // Set folder older
            var folder = cacheFolders[0];
            folder.LastAccessTimeUtc = folder.LastAccessTimeUtc.Subtract(new TimeSpan(1, 0, 0));
            cacheFolders[0] = folder;

            // delete just under the first folder size
            long maximumCacheSize = currentCacheSize - folder.Length + 1;
            BuildCache.PruneCacheFolders(maximumCacheSize, currentCacheSize, cacheFolders);
            BuildCache.ComputeCacheSizeAndFolders(out long newCurrentCacheSize, out List<BuildCache.CacheFolder> newCacheFolders);

            Assert.AreNotEqual(0, newCurrentCacheSize);
            Assert.GreaterOrEqual(maximumCacheSize, newCurrentCacheSize);
            Assert.AreEqual(artifactsDirectories.Length - 1, newCacheFolders.Count);
        }
    }
}