using System;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Entities.Tests
{
    public class BlobAssetStoreTests
    {
        protected BlobAssetStore m_Store;
        
        [SetUp]
        public void Setup()
        {
            m_Store = new BlobAssetStore();    
        }

        [TearDown]
        public void TearDown()
        {
            m_Store.Dispose();
        }
        
        protected Hash128 FromInt(int v) => new Hash128((uint)v, 1, 2, 3);
        protected Hash128 FromByte(byte v) => new Hash128((uint)v, 1, 2, 3);
        protected Hash128 FromFloat(float v) => new Hash128((uint)v.GetHashCode(), 1, 2, 3);
        
        [Test]
        public void TestCacheAccess()
        {
            var a0 = BlobAssetReference<int>.Create(0);
            var a1 = BlobAssetReference<int>.Create(1);
            var a2 = BlobAssetReference<float>.Create(2.0f);
            
            var k0 = FromInt(a0.Value);
            var k1 = FromInt(a1.Value);
            var k2 = FromFloat(a2.Value);
            
            Assert.IsTrue(m_Store.TryAdd(k0, a0));
            Assert.IsFalse(m_Store.TryAdd(k0, a0));
            Assert.IsTrue(m_Store.TryGet<int>(k0, out var ra0));
            Assert.AreEqual(0, ra0.Value);
            Assert.AreEqual(0, m_Store.CacheMiss);
            Assert.AreEqual(1, m_Store.CacheHit);
            
            Assert.IsFalse(m_Store.TryGet<int>(k1, out var ra1));
            Assert.IsTrue(m_Store.TryAdd(k1, a1));
            Assert.IsTrue(m_Store.TryGet<int>(k1, out ra1));
            Assert.AreEqual(1, ra1.Value);
            Assert.AreEqual(1, m_Store.CacheMiss);
            Assert.AreEqual(2, m_Store.CacheHit);
            
            Assert.IsFalse(m_Store.TryGet<float>(k2, out var ra2));
            Assert.IsTrue(m_Store.TryAdd(k2, a2));
            Assert.IsTrue(m_Store.TryGet(k2, out ra2));
            Assert.AreEqual(2.0f, ra2.Value);
            Assert.AreEqual(2, m_Store.CacheMiss);
            Assert.AreEqual(3, m_Store.CacheHit);
        }
        
        [Test]
        public void TestCacheAccessWithDifferentTypeSameKey()
        {
            var a0 = BlobAssetReference<int>.Create(10);
            var a1 = BlobAssetReference<byte>.Create(10);
            
            var k = FromInt(a0.Value);
            
            Assert.IsTrue(m_Store.TryAdd(k, a0));
            Assert.IsTrue(m_Store.TryAdd(k, a1));

            m_Store.TryGet<int>(k, out var ra0);
            m_Store.TryGet<byte>(k, out var ra1);
            
            Assert.AreEqual(a0, ra0);
            Assert.AreEqual(a1, ra1);
        }
        
        [Test]
        public unsafe void TestCacheClearWithDispose()
        {
            var a0 = BlobAssetReference<int>.Create(0);
            var a1 = BlobAssetReference<int>.Create(1);
            var a2 = BlobAssetReference<float>.Create(2.0f);
            
            var k0 = FromInt(a0.Value);
            var k1 = FromInt(a1.Value);
            var k2 = FromFloat(a2.Value);
            
            Assert.IsTrue(m_Store.TryAdd(k0, a0));
            Assert.IsTrue(m_Store.TryGet<int>(k0, out var ra0));
            
            Assert.IsTrue(m_Store.TryAdd(k1, a1));
            Assert.IsTrue(m_Store.TryGet<int>(k1, out var ra1));
            
            m_Store.ResetCache(true);

            Assert.Throws<InvalidOperationException>(() => a0.GetUnsafePtr());
            Assert.Throws<InvalidOperationException>(() => a1.GetUnsafePtr());
            
            Assert.IsFalse(m_Store.TryGet(k0, out ra0));
            Assert.IsFalse(m_Store.TryGet(k0, out ra1));
            
            Assert.IsTrue(m_Store.TryAdd(k2, a2));
            Assert.IsTrue(m_Store.TryGet<float>(k2, out var ra2));
        }
        
        
        [Test]
        public unsafe void TestCacheClearWithoutDispose()
        {
            var a0 = BlobAssetReference<int>.Create(0);
            var a1 = BlobAssetReference<int>.Create(1);
            
            var k0 = FromInt(a0.Value);
            var k1 = FromInt(a1.Value);
            
            Assert.IsTrue(m_Store.TryAdd(k0, a0));
            Assert.IsTrue(m_Store.TryAdd(k1, a1));
            
            m_Store.ResetCache(false);

            Assert.DoesNotThrow(() => a0.GetUnsafePtr());
            Assert.DoesNotThrow(() => a1.GetUnsafePtr());
        }

        [Test]
        public void BlobPerOwnerTest()
        {
            var go0 = new GameObject("GO 0");
            var go1 = new GameObject("GO 1");

            var k0 = FromInt(0);
            var k1 = FromInt(1);
            var k2 = FromInt(2);
            var k3 = FromInt(3);
            var k4 = FromInt(4);

            // Associate k0, k2, k4 with GO0, k0, k1, k2 with GO1
            using (var context = new BlobAssetComputationContext<int, int>(m_Store, 16, Allocator.Temp))
            using (var processList = new NativeList<Hash128>(16, Allocator.Temp))
            {
                // Simulate BlobAsset operations with GO0
                processList.Add(k0);
                Assert.IsTrue(context.NeedToComputeBlobAsset(k0));
                var blobAsset = BlobAssetReference<int>.Create(0);
                context.AddComputedBlobAsset(k0, blobAsset);
                
                processList.Add(k2);
                Assert.IsTrue(context.NeedToComputeBlobAsset(k2));
                blobAsset = BlobAssetReference<int>.Create(2);
                context.AddComputedBlobAsset(k2, blobAsset);
                
                processList.Add(k4);
                Assert.IsTrue(context.NeedToComputeBlobAsset(k4));
                blobAsset = BlobAssetReference<int>.Create(4);
                context.AddComputedBlobAsset(k4, blobAsset);

                // Simulate BlobAsset operation with GO1
                processList.Add(k0);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k0));
                
                processList.Add(k1);
                Assert.IsTrue(context.NeedToComputeBlobAsset(k1));
                blobAsset = BlobAssetReference<int>.Create(1);
                context.AddComputedBlobAsset(k1, blobAsset);
                
                processList.Add(k2);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k2));

                // Associate the BlobAssets with GO0
                context.AssociateBlobAssetWithGameObject(k0, go0);
                context.AssociateBlobAssetWithGameObject(k2, go0);
                context.AssociateBlobAssetWithGameObject(k4, go0);
                
                // Associate the BlobAssets with GO1
                context.AssociateBlobAssetWithGameObject(k0, go1);
                context.AssociateBlobAssetWithGameObject(k1, go1);
                context.AssociateBlobAssetWithGameObject(k2, go1);

                // Check the BlobAsset are retrieved correctly
                var replayIndex = 0;
                context.GetBlobAsset(processList[replayIndex++], out var res);
                Assert.AreEqual(0, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(2, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(4, res.Value);
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(0, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(1, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(2, res.Value);
            }

            // Check the GO-BlobAsset associations
            m_Store.GetBlobAssetsOfGameObject(go0, Allocator.Temp, out var hashes);
            Assert.AreEqual(3, hashes.Length);
            Assert.IsTrue(hashes.Contains(k0));
            Assert.IsTrue(hashes.Contains(k2));
            Assert.IsTrue(hashes.Contains(k4));
            hashes.Dispose();

            m_Store.GetBlobAssetsOfGameObject(go1, Allocator.Temp, out hashes);
            Assert.AreEqual(3, hashes.Length);
            Assert.IsTrue(hashes.Contains(k0));
            Assert.IsTrue(hashes.Contains(k1));
            Assert.IsTrue(hashes.Contains(k2));
            hashes.Dispose();

            // 2, 1, 2, 0, 1 as expected RefCounter for k0...4
            Assert.AreEqual(2, m_Store.GetBlobAssetRefCounter(k0));
            Assert.AreEqual(1, m_Store.GetBlobAssetRefCounter(k1));
            Assert.AreEqual(2, m_Store.GetBlobAssetRefCounter(k2));
            Assert.AreEqual(0, m_Store.GetBlobAssetRefCounter(k3));
            Assert.AreEqual(1, m_Store.GetBlobAssetRefCounter(k4));
            
            // Associate k1, k2, k3 with GO0 and k3, k4 with GO1
            using (var context = new BlobAssetComputationContext<int, int>(m_Store, 16, Allocator.Temp))
            using (var processList = new NativeList<Hash128>(16, Allocator.Temp))
            {
                // Simulate BlobAsset operations with GO0
                processList.Add(k1);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k1));
                
                processList.Add(k2);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k2));
                
                processList.Add(k3);
                Assert.IsTrue(context.NeedToComputeBlobAsset(k3));
                var blobAsset = BlobAssetReference<int>.Create(3);
                context.AddComputedBlobAsset(k3, blobAsset);

                // Simulate BlobAsset operations with GO1
                processList.Add(k3);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k3));
                
                processList.Add(k4);
                Assert.IsFalse(context.NeedToComputeBlobAsset(k4));
                
                // Associate the BlobAssets with GO0
                context.AssociateBlobAssetWithGameObject(k1, go0);
                context.AssociateBlobAssetWithGameObject(k2, go0);
                context.AssociateBlobAssetWithGameObject(k3, go0);
                
                // Associate the BlobAssets with GO1
                context.AssociateBlobAssetWithGameObject(k3, go1);
                context.AssociateBlobAssetWithGameObject(k4, go1);

                // Check BlobAsset are retrieved correctly
                var replayIndex = 0;
                context.GetBlobAsset(processList[replayIndex++], out var res);
                Assert.AreEqual(1, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(2, res.Value);
                
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(3, res.Value);
                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(3, res.Value);

                context.GetBlobAsset(processList[replayIndex++], out res);
                Assert.AreEqual(4, res.Value);
            }

            // Check the GO-BlobAsset associations
            m_Store.GetBlobAssetsOfGameObject(go0, Allocator.Temp, out hashes);
            Assert.AreEqual(3, hashes.Length);
            Assert.IsTrue(hashes.Contains(k1));
            Assert.IsTrue(hashes.Contains(k2));
            Assert.IsTrue(hashes.Contains(k3));
            hashes.Dispose();

            m_Store.GetBlobAssetsOfGameObject(go1, Allocator.Temp, out hashes);
            Assert.AreEqual(2, hashes.Length);
            Assert.IsTrue(hashes.Contains(k3));
            Assert.IsTrue(hashes.Contains(k4));
            hashes.Dispose();

            // 0, 1, 1, 2, 1 as expected RefCounter for k0...4
            Assert.AreEqual(0, m_Store.GetBlobAssetRefCounter(k0));
            Assert.AreEqual(1, m_Store.GetBlobAssetRefCounter(k1));
            Assert.AreEqual(1, m_Store.GetBlobAssetRefCounter(k2));
            Assert.AreEqual(2, m_Store.GetBlobAssetRefCounter(k3));
            Assert.AreEqual(1, m_Store.GetBlobAssetRefCounter(k4));
            
            // BlobAsset of k0 is not used by any UnityObject anymore, is should have been removed from the store
            Assert.IsFalse(m_Store.Contains<int>(k0));
            
            // Cleanup
            Object.DestroyImmediate(go0);
        }
    }
}
