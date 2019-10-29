using NUnit.Framework;

namespace Unity.Entities.Tests
{
    public unsafe class ChunkDataUtilityTests : ECSTestsFixture
    {
        [Test]
        public void AreLayoutCompatibleSameArchetype()
        {
            var a = m_Manager.CreateArchetype();
            Assert.IsTrue(ChunkDataUtility.AreLayoutCompatible(a.Archetype, a.Archetype));
        }

        [Test]
        public void AreLayoutCompatibleDifferentComponents()
        {
            var a = m_Manager.CreateArchetype(typeof(EcsTestFloatData));
            var b = m_Manager.CreateArchetype(typeof(EcsTestData));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(a.Archetype, b.Archetype));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(b.Archetype, a.Archetype));
        }

        [Test]
        public void AreLayoutCompatibleDifferentComponentCount()
        {
            var a = m_Manager.CreateArchetype();
            var b = m_Manager.CreateArchetype(typeof(EcsTestData));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(a.Archetype, b.Archetype));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(b.Archetype, a.Archetype));
        }

        [Test]
        public void AreLayoutCompatibleSharedComponent()
        {
            var a = m_Manager.CreateArchetype();
            var b = m_Manager.CreateArchetype(typeof(EcsTestSharedComp));
            Assert.IsTrue(ChunkDataUtility.AreLayoutCompatible(a.Archetype, b.Archetype));
            Assert.IsTrue(ChunkDataUtility.AreLayoutCompatible(b.Archetype, a.Archetype));
        }

        [MaximumChunkCapacity(1)]
        public struct SharedComponentWithMaximumChunkCapacity1 : ISharedComponentData
        {
            public int Value;
        }

        [Test]
        public void AreLayoutCompatibleSharedComponentWithMaximumChunkCapacity()
        {
            var a = m_Manager.CreateArchetype();
            var b = m_Manager.CreateArchetype(typeof(SharedComponentWithMaximumChunkCapacity1));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(a.Archetype, b.Archetype));
            Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(b.Archetype, a.Archetype));
        }
    }
}
