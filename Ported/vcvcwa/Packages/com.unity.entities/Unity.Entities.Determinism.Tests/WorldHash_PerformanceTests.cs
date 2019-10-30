using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.Serialization;
using Unity.PerformanceTesting;

namespace Unity.Entities.Determinism.Tests
{
    [TestFixture]
    public class WorldHash_PerformanceTests : PaddingMasks_TestFixture
    {
        const uint SeedForTesting = 0x32;
        const int EntityCount = 1024 * 1024;
        
        const string worldName = "WorldHash_PerformanceTests";
        
        World World;

        protected override void OnFixtureSetUp()
        {
            World = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, EntityCount);
        }

        protected override void OnFixtureTearDown()
        {
            World.Dispose();
        }


        static int GetChunkCount(World world) => world.EntityManager.UniversalQuery.CalculateChunkCount();
        static int GetUnfilteredChunkCount(World world) => world.EntityManager.UniversalQuery.CalculateChunkCountWithoutFiltering();
        
        [Test]
        public void WorldHash_UniversalQueryFilteredAndWithoutFiltering_HaveIdenticalChunkCount()
        {
            Assert.AreEqual(GetUnfilteredChunkCount(World), GetChunkCount(World));
        }
        
        [Test, Performance]
        public void WorldHash_Performance_RunUniversalQueryUnfiltered()
        {
            Measure.Method(() =>
                   {
                       using (var array = new NativeArray<byte>(GetUnfilteredChunkCount(World), Allocator.Temp))
                       {
                           Assert.That(array.Length > 0);
                       }
                   })
                   .Definition($"WorldHashPerformance_RunUniversalQuery_{GetUnfilteredChunkCount(World)}_Chunks")
                   .MeasurementCount(50)
                   .Run();
        }

        [Test, Performance]
        public void WorldHash_Performance_RunUniversalQuery()
        {
            Measure.Method(() =>
                   {
                       using (var array = new NativeArray<byte>(GetChunkCount(World), Allocator.Temp))
                       {
                           Assert.That(array.Length > 0);
                       }
                   })
                   .Definition($"WorldHashPerformance_RunUniversalQuery_{GetChunkCount(World)}_Chunks")
                   .MeasurementCount(50)
                   .Run();
        }
        
        [Test, Performance]
        public void WorldHash_Performance_UseLocalToWorldHash()
        {
            Measure.Method(() => LocalToWorldHash.HashTransforms(World, SeedForTesting) )
                .Definition("WorldHashPerformance_LocalToWorldHash_64MB_RawSOL")
                .MeasurementCount(10)
                .Run();
        }
        
        [Test, Performance]
        public void WorldHash_Performance_GetWorldHashesParallelFor_MaskedHasher()
        {
            Measure.Method(() =>
                   {
                       HashUtility.GetWorldHashes_ParallelFor(World, MaskedHasher, SeedForTesting, Allocator.TempJob).Dispose();
                   })
                .Definition("WorldHash_Performance_GetWorldHashesParallelFor_MaskedHasher_72MB")
                .MeasurementCount(10)
                .Run();
        }
                     
        [Test, Performance]
        public void WorldHash_Performance_GetWorldHashesIJobChunk_MaskedHasher()
        {
            Measure.Method(() =>
                   {
                       HashUtility.GetWorldHashes_IJobChunk(World, MaskedHasher, SeedForTesting, Allocator.TempJob).Dispose();
                   })
                .Definition("WorldHash_Performance_GetWorldHashesIJobChunk_MaskedHasher_72MB")
                .MeasurementCount(10)
                .Run();
        }
                
        [Test, Performance]
        public void WorldHash_Performance_GetWorldHashesParallelFor_DenseHasher()
        {
            Measure.Method(() =>
                   {
                       HashUtility.GetWorldHashes_ParallelFor(World, DenseHasher, SeedForTesting, Allocator.TempJob).Dispose();
                   })
                   .Definition("WorldHash_Performance_GetWorldHashesParallelFor_DenseHasher_72MB")
                   .MeasurementCount(10)
                   .Run();
        }
                     
        [Test, Performance]
        public void WorldHash_Performance_GetWorldHashesIJobChunk_DenseHasher()
        {
            Measure.Method(() =>
                   {
                       HashUtility.GetWorldHashes_IJobChunk(World, DenseHasher, SeedForTesting, Allocator.TempJob).Dispose();
                   })
                   .Definition("WorldHash_Performance_GetWorldHashesIJobChunk_DenseHasher_72MB")
                   .MeasurementCount(10)
                   .Run();
        }

        [Test, Performance]
        public void WorldHash_Performance_BinaryWorldMemorySerialization()
        {
            Measure.Method(() =>
                {
                    using (var writer = new MemoryBinaryWriter())
                    {
                        SerializeUtility.SerializeWorld(World.EntityManager, writer);
                        HashUtility.GetDenseHash(InternallyExposedPerformanceTestExtensions.GetContentAsNativeArray(writer), SeedForTesting);
                    }
                })
                .Definition("WorldHashPerformance_WorldSerializationBinaryMemory_72MB")
                .MeasurementCount(3)
                .Run();
        }
    }
}