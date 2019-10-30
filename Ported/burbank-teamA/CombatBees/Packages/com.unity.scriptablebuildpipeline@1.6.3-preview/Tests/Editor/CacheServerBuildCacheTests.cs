using System.IO;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tests
{
    // Test suite against Build Cache with Cache Server backend, tests are implemented in BuildCacheTestBase
    class CacheServerBuildCacheTests : BuildCacheTestBase
    {
        internal override void OneTimeSetupDerived()
        {
            var cachePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            LocalCacheServer.Setup(1024 * 1024, cachePath);
        }

        internal override void OneTimeTearDownDerived()
        {
            LocalCacheServer.Clear();
        }

        protected override void RecreateBuildCache()
        {
            // purge the local cache to make sure we don't load anything out of it again.
            // these tests need to pull from the cache server
            if (m_Cache != null)
                m_Cache.Dispose();
            PurgeBuildCache();
            m_Cache = new BuildCache("localhost", LocalCacheServer.instance.m_port);
        }
    }
}
