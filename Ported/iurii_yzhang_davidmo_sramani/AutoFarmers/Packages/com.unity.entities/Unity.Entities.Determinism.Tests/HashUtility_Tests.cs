using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Unity.Entities.Determinism.Tests
{
    struct HashUtilityTest_BufferElement : IBufferElementData
    {
        public bool Offset0;
        public int Offset4;
    }

    [TestFixture]
    public class HashUtility_Tests : PaddingMasks_TestFixture
    {
        const uint SeedForTesting = 0xf00d;
        const string worldName = "HashUtility_JobComparisonTests";

        [Test]
        public void HashUtility_ZeroPadding_HashWorldIJobChunk_EqualsIJobForEach()
        {
            using (var world = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            {
                var ltw = new LocalToWorld { Value = float4x4.identity };

                var entity = world.EntityManager.CreateEntity();
                world.EntityManager.AddComponent<LocalToWorld>(entity);
                world.EntityManager.SetComponentData(entity, ltw);
                
                using (var jobChunk = HashUtility.GetWorldHashes_IJobChunk(world, MaskedHasher, SeedForTesting, Allocator.TempJob))
                using (var parallelFor = HashUtility.GetWorldHashes_ParallelFor(world, MaskedHasher, SeedForTesting, Allocator.TempJob))
                {
                    var hash_JobChunk = HashUtility.GetDenseHash(jobChunk, SeedForTesting);
                    var hash_ParallelFor = HashUtility.GetDenseHash(parallelFor, SeedForTesting);
                    
                    Assert.AreEqual(hash_JobChunk, hash_ParallelFor);
                }
            }
        }

        [Test]
        public void HashUtility_MultipleCoexistingEqualWorlds_YieldSameHash()
        {
            const int kTestEntityCount = 1024;
            using (var world0 = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents("World_Iteration_0", kTestEntityCount))
            using (var world1 = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents("World_Iteration_1", kTestEntityCount))
            {
                var h0 = HashUtility.GetWorldHash(world0, MaskedHasher, SeedForTesting);
                var h1 = HashUtility.GetWorldHash(world1, MaskedHasher, SeedForTesting);
                Assert.AreEqual(h0, h1);
            }
        }
        
        [Test]
        public void HashUtility_MultipleSequentialExistingEqualWorlds_YieldSameHash()
        {
            const int kTestEntityCount = 1024;
            uint h0, h1;
            
            using (var world0 = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents("World_Iteration_0", kTestEntityCount))
            { 
                h0 = HashUtility.GetWorldHash(world0, MaskedHasher, SeedForTesting);
            }
            
            using (var world1 = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents("World_Iteration_1", kTestEntityCount))
            {
                h1 = HashUtility.GetWorldHash(world1, MaskedHasher, SeedForTesting);
            }
            
            Assert.AreEqual(h0, h1);
        }

        [Test]
        public void HashUtility_ZeroPadding_EntityWithBufferEndToEnd_HasExpectedHash()
        {
            using (var world = new World("HashUtilityTests_BufferTestWorld"))
            {
                var bufferElement = new HashUtilityTest_BufferElement { Offset0 = true, Offset4 = 0xBeef };

                var entity = world.EntityManager.CreateEntity();
                var buffer = world.EntityManager.AddBuffer<HashUtilityTest_BufferElement>(entity);

                buffer.Add(bufferElement);

                using (var chunks = world.EntityManager.GetAllChunks(Allocator.Persistent))
                {
                    Assert.AreEqual(chunks.Length, 1);

                    var chunk = chunks[0];
                    using (var types = chunk.GetNonzeroComponentTypes(Allocator.Persistent))
                    {
                        Assert.AreEqual(types.Length, 2 /* Entity, HashUtilityTest_BufferElement */ );

                        using (var _ = new NativeArray<uint>(3, Allocator.Persistent))
                        {
                            var hashes = _;
                            hashes[0] = HashUtility.GetDenseHash(NativeViewUtility.GetView(ref entity), SeedForTesting);

                            var mask = NativeViewUtility.GetReadView( Masks.GetTypeMask<HashUtilityTest_BufferElement>() );
                            var bufferHash = HashUtility.GetMaskedHash(NativeViewUtility.GetView(ref bufferElement), mask, SeedForTesting);
                            hashes[1] = HashUtility.GetDenseHash(NativeViewUtility.GetView(ref bufferHash), SeedForTesting);

                            hashes[2] = HashUtility.GetChunkHeaderHash(chunk, SeedForTesting);

                            using (var expected = HashUtility.GetChunkHashes(chunk, MaskedHasher, SeedForTesting))
                            {
                                Assert.That(hashes, Is.EquivalentTo(expected), DeterminismTestUtility.PrintAll(hashes, expected));
                            }
                        
                        }
                    }
                }
            }
        }

        [Test]
        public void HashUtility_GetWorldHashZeroedPadding_DoesNotTriggerWorldWrites()
        {
            using (var world = DeterminismTestUtility.CreateTestWorld_WithLocalToWorldComponents(worldName, 1))
            using (var context = SynchronizedContext.CreateFromWorld(world))
            {
                // move BeforeStructuralChange() outside of GetWorldHash() as it would otherwise indeed modify the world
                SanityCheckHashValue( HashUtility.GetWorldHash(world, MaskedHasher, SeedForTesting) );
                
                // if the global system version was bumped, the context is invalid:
                Assert.That(context.IsValidIn(world));
            }
        }

        void SanityCheckHashValue(uint result)
        {
            Assert.AreNotEqual(0, result, "SanityCheck failed: Hash is 0");
            Assert.AreNotEqual(SeedForTesting, result, "SanityCheck failed: Hash equals seed.");
        }
    }
}
