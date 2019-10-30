using NUnit.Framework;
using Unity.Mathematics;
using Unity.Transforms;

namespace Unity.Entities.Determinism.Tests
{
    [TestFixture]
    public class LocalToWorldHash_HashTransforms_Tests
    {
        const int SeedForTesting = 42;

        [Test]
        public void HashLocalToWorld_HashUtilityResult_EqualsRawImplementation()
        {
            using (var world = new World("HashLocalToWorld_Test"))
            {
                var ltw = new LocalToWorld { Value = float4x4.identity };

                var entity = world.EntityManager.CreateEntity();
                world.EntityManager.AddComponent<LocalToWorld>(entity);
                world.EntityManager.SetComponentData(entity, ltw);

                Assert.AreEqual(LocalToWorldHash.HashTransforms(world, SeedForTesting), MimicChunkedLocalToWorldHash(ltw, SeedForTesting));
            }
        }

        [Test]
        public void HashLocalToWorld_WorldWithoutLtwComponents_WillReturnSeed()
        {
            using (var world = new World("HashLocalToWorld_Test"))
            {
                Assert.AreEqual(SeedForTesting, LocalToWorldHash.HashTransforms(world, SeedForTesting));
            }
        }

        static uint MimicChunkedLocalToWorldHash(LocalToWorld ltw, uint seed)
        {
            var h = HashUtility.GetDenseHash(NativeViewUtility.GetView(ref ltw), seed);
            return HashUtility.GetDenseHash(NativeViewUtility.GetView(ref h), seed);
        }
    }
}