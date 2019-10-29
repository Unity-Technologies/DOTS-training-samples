using System;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    struct SharedData1 : ISharedComponentData
    {
        public int value;

        public SharedData1(int val) { value = val; }
    }

    struct SharedData2 : ISharedComponentData
    {
        public int value;

        public SharedData2(int val) { value = val; }
    }
    
    struct SharedData3 : ISharedComponentData
    {
        public int value;

        public SharedData3(int val) { value = val; }
    }
    
    struct SharedData4 : ISharedComponentData
    {
        public int value;

        public SharedData4(int val) { value = val; }
    }
    
    struct SharedData5 : ISharedComponentData
    {
        public int value;

        public SharedData5(int val) { value = val; }
    }
    
    struct SharedData6 : ISharedComponentData
    {
        public int value;

        public SharedData6(int val) { value = val; }
    }
    
    struct SharedData7 : ISharedComponentData
    {
        public int value;

        public SharedData7(int val) { value = val; }
    }
    
    struct SharedData8 : ISharedComponentData
    {
        public int value;

        public SharedData8(int val) { value = val; }
    }
    
    struct SharedData9 : ISharedComponentData
    {
        public int value;

        public SharedData9(int val) { value = val; }
    }


    class SharedComponentDataTests : ECSTestsFixture
    {
        //@TODO: No tests for invalid shared components / destroyed shared component data
        //@TODO: No tests for if we leak shared data when last entity is destroyed...
        //@TODO: No tests for invalid shared component type?

        [Test]
        public void SetSharedComponent()
        {
            var archetype = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData), typeof(SharedData2));

            var group1 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(SharedData1));
            var group2 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(SharedData2));
            var group12 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(SharedData2), typeof(SharedData1));

            var group1_filter_0 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(SharedData1));
            group1_filter_0.SetSharedComponentFilter(new SharedData1(0));
            var group1_filter_20 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(SharedData1));
            group1_filter_20.SetSharedComponentFilter(new SharedData1(20));

            Assert.AreEqual(0, group1.CalculateEntityCount());
            Assert.AreEqual(0, group2.CalculateEntityCount());
            Assert.AreEqual(0, group12.CalculateEntityCount());

            Assert.AreEqual(0, group1_filter_0.CalculateEntityCount());
            Assert.AreEqual(0, group1_filter_20.CalculateEntityCount());

            Entity e1 = m_Manager.CreateEntity(archetype);
            m_Manager.SetComponentData(e1, new EcsTestData(117));
            Entity e2 = m_Manager.CreateEntity(archetype);
            m_Manager.SetComponentData(e2, new EcsTestData(243));

            var group1_filter0_data = group1_filter_0.ToComponentDataArray<EcsTestData>(Allocator.TempJob);

            Assert.AreEqual(2, group1_filter_0.CalculateEntityCount());
            Assert.AreEqual(0, group1_filter_20.CalculateEntityCount());
            Assert.AreEqual(117, group1_filter0_data[0].value);
            Assert.AreEqual(243, group1_filter0_data[1].value);

            m_Manager.SetSharedComponentData(e1, new SharedData1(20));
            
            group1_filter0_data.Dispose();
            group1_filter0_data = group1_filter_0.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            var group1_filter20_data = group1_filter_20.ToComponentDataArray<EcsTestData>(Allocator.TempJob);

            Assert.AreEqual(1, group1_filter_0.CalculateEntityCount());
            Assert.AreEqual(1, group1_filter_20.CalculateEntityCount());
            Assert.AreEqual(117, group1_filter20_data[0].value);
            Assert.AreEqual(243, group1_filter0_data[0].value);

            m_Manager.SetSharedComponentData(e2, new SharedData1(20));
            
            group1_filter20_data.Dispose();
            group1_filter20_data = group1_filter_20.ToComponentDataArray<EcsTestData>(Allocator.TempJob);

            Assert.AreEqual(0, group1_filter_0.CalculateEntityCount());
            Assert.AreEqual(2, group1_filter_20.CalculateEntityCount());
            Assert.AreEqual(117, group1_filter20_data[0].value);
            Assert.AreEqual(243, group1_filter20_data[1].value);

            group1.Dispose();
            group2.Dispose();
            group12.Dispose();
            group1_filter_0.Dispose();
            group1_filter_20.Dispose();
            
            group1_filter0_data.Dispose();
            group1_filter20_data.Dispose();
        }

        [Test]
        public void GetAllUniqueSharedComponents()
        {
            var unique = new List<SharedData1>(0);
            m_Manager.GetAllUniqueSharedComponentData(unique);

            Assert.AreEqual(1, unique.Count);
            Assert.AreEqual(default(SharedData1).value, unique[0].value);

            var archetype = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);
            m_Manager.SetSharedComponentData(e, new SharedData1(17));

            unique.Clear();
            m_Manager.GetAllUniqueSharedComponentData(unique);

            Assert.AreEqual(2, unique.Count);
            Assert.AreEqual(default(SharedData1).value, unique[0].value);
            Assert.AreEqual(17, unique[1].value);

            m_Manager.SetSharedComponentData(e, new SharedData1(34));

            unique.Clear();
            m_Manager.GetAllUniqueSharedComponentData(unique);

            Assert.AreEqual(2, unique.Count);
            Assert.AreEqual(default(SharedData1).value, unique[0].value);
            Assert.AreEqual(34, unique[1].value);

            m_Manager.DestroyEntity(e);

            unique.Clear();
            m_Manager.GetAllUniqueSharedComponentData(unique);

            Assert.AreEqual(1, unique.Count);
            Assert.AreEqual(default(SharedData1).value, unique[0].value);
        }

        [Test]
        public void GetSharedComponentData()
        {
            var archetype = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            Assert.AreEqual(0, m_Manager.GetSharedComponentData<SharedData1>(e).value);

            m_Manager.SetSharedComponentData(e, new SharedData1(17));

            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);
        }

        [Test]
        public void GetSharedComponentDataAfterArchetypeChange()
        {
            var archetype = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            Assert.AreEqual(0, m_Manager.GetSharedComponentData<SharedData1>(e).value);

            m_Manager.SetSharedComponentData(e, new SharedData1(17));
            m_Manager.AddComponentData(e, new EcsTestData2 {value0 = 1, value1 = 2});

            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);
        }

        [Test]
        public void NonExistingSharedComponentDataThrows()
        {
            Entity e = m_Manager.CreateEntity(typeof(EcsTestData));

            Assert.Throws<ArgumentException>(() => { m_Manager.GetSharedComponentData<SharedData1>(e); });
            Assert.Throws<ArgumentException>(() => { m_Manager.SetSharedComponentData(e, new SharedData1()); });
        }

        [Test]
        public void AddSharedComponent()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            Assert.IsFalse(m_Manager.HasComponent<SharedData1>(e));
            Assert.IsFalse(m_Manager.HasComponent<SharedData2>(e));

            m_Manager.AddSharedComponentData(e, new SharedData1(17));

            Assert.IsTrue(m_Manager.HasComponent<SharedData1>(e));
            Assert.IsFalse(m_Manager.HasComponent<SharedData2>(e));
            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);

            m_Manager.AddSharedComponentData(e, new SharedData2(34));
            Assert.IsTrue(m_Manager.HasComponent<SharedData1>(e));
            Assert.IsTrue(m_Manager.HasComponent<SharedData2>(e));
            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);
            Assert.AreEqual(34, m_Manager.GetSharedComponentData<SharedData2>(e).value);
        }

        [Test]
        public void AddSharedComponentCompatibleChunkLayouts()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetypeWithShared = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(SharedData1));
            unsafe
            {
                Assert.IsTrue(ChunkDataUtility.AreLayoutCompatible(archetype.Archetype, archetypeWithShared.Archetype));
            }

            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            Assert.IsFalse(m_Manager.HasComponent<SharedData1>(e));

            m_Manager.AddSharedComponentData(query, new SharedData1(17));

            Assert.IsTrue(m_Manager.HasComponent<SharedData1>(e));
            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);
        }

        [Test]
        public void AddSharedComponentIncompatibleChunkLayouts()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetypeWithShared = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            unsafe
            {
                Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype.Archetype, archetypeWithShared.Archetype));
            }

            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(e));

            m_Manager.AddSharedComponentData(query, new EcsTestSharedCompWithMaxChunkCapacity(17));

            Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(e));
            Assert.AreEqual(17, m_Manager.GetSharedComponentData<EcsTestSharedCompWithMaxChunkCapacity>(e).Value);
        }

        [Test]
        public void AddSharedComponentToMultipleEntitiesIncompatibleChunkLayouts()
        {
            // The goal of this test is to verify that the moved IComponentData keeps the same values from
            // before the addition of the shared component.
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetypeWithShared = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            unsafe
            {
                Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype.Archetype, archetypeWithShared.Archetype));
            }

            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            const int numEntities = 5000;
            using (var entities = new NativeArray<Entity>(numEntities, Allocator.Persistent))
            {
                m_Manager.CreateEntity(archetype, entities);

                for (int i = 0; i < entities.Length; ++i)
                {
                    m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                }

                foreach (var e in entities)
                {
                    Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(e));
                }

                m_Manager.AddSharedComponentData(query, new EcsTestSharedCompWithMaxChunkCapacity(17));
                var chunk = m_Manager.GetChunk(entities[0]);
                int maxChunkCapacity = TypeManager.GetTypeInfo<EcsTestSharedCompWithMaxChunkCapacity>().MaximumChunkCapacity;
                int expectedChunkCount = (numEntities + maxChunkCapacity - 1) / maxChunkCapacity;
                Assert.AreEqual(expectedChunkCount, chunk.Archetype.ChunkCount);

                // Ensure that the moved components have the correct values.
                for (int i = 0; i < entities.Length; ++i)
                {
                    Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(entities[i]));
                    Assert.AreEqual(17, m_Manager.GetSharedComponentData<EcsTestSharedCompWithMaxChunkCapacity>(entities[i]).Value);
                    Assert.AreEqual(i, m_Manager.GetComponentData<EcsTestData>(entities[i]).value);
                }
            }
        }

        [Test]
        public void AddSharedComponentViaAddComponentWithIncompatibleChunkLayouts()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetypeWithShared = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            unsafe
            {
                Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype.Archetype, archetypeWithShared.Archetype));
            }

            using (var entities = new NativeArray<Entity>(1, Allocator.TempJob))
            {
                m_Manager.CreateEntity(archetype, entities);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(entities[0]));

                m_Manager.AddComponent(entities, typeof(EcsTestSharedCompWithMaxChunkCapacity));

                Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedCompWithMaxChunkCapacity>(entities[0]));
                Assert.AreEqual(0, m_Manager.GetSharedComponentData<EcsTestSharedCompWithMaxChunkCapacity>(entities[0]).Value);
            }
        }

        [Test]
        public void RemoveSharedComponent()
        {
            Entity e = m_Manager.CreateEntity();

            m_Manager.AddComponentData(e, new EcsTestData(42));
            m_Manager.AddSharedComponentData(e, new SharedData1(17));
            m_Manager.AddSharedComponentData(e, new SharedData2(34));

            Assert.IsTrue(m_Manager.HasComponent<SharedData1>(e));
            Assert.IsTrue(m_Manager.HasComponent<SharedData2>(e));
            Assert.AreEqual(17, m_Manager.GetSharedComponentData<SharedData1>(e).value);
            Assert.AreEqual(34, m_Manager.GetSharedComponentData<SharedData2>(e).value);

            m_Manager.RemoveComponent<SharedData1>(e);
            Assert.IsFalse(m_Manager.HasComponent<SharedData1>(e));
            Assert.AreEqual(34, m_Manager.GetSharedComponentData<SharedData2>(e).value);

            m_Manager.RemoveComponent<SharedData2>(e);
            Assert.IsFalse(m_Manager.HasComponent<SharedData2>(e));

            Assert.AreEqual(42, m_Manager.GetComponentData<EcsTestData>(e).value);
        }

        [Test]
        public void SCG_DoesNotMatchRemovedSharedComponentInEntityQuery()
        {
            var archetype0 = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            var archetype1 = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData), typeof(SharedData2));

            var group0 = m_Manager.CreateEntityQuery(typeof(SharedData1));
            var group1 = m_Manager.CreateEntityQuery(typeof(SharedData2));

            m_Manager.CreateEntity(archetype0);
            var entity1 = m_Manager.CreateEntity(archetype1);

            Assert.AreEqual(2, group0.CalculateEntityCount());
            Assert.AreEqual(1, group1.CalculateEntityCount());

            m_Manager.RemoveComponent<SharedData2>(entity1);

            Assert.AreEqual(2, group0.CalculateEntityCount());
            Assert.AreEqual(0, group1.CalculateEntityCount());

            group0.Dispose();
            group1.Dispose();
        }

        [Test]
        public void SCG_DoesNotMatchRemovedSharedComponentInChunkQuery()
        {
            var archetype0 = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            var archetype1 = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData), typeof(SharedData2));

            var group0 = m_Manager.CreateEntityQuery(ComponentType.ReadWrite<SharedData1>());
            var group1 = m_Manager.CreateEntityQuery(ComponentType.ReadWrite<SharedData2>());

            m_Manager.CreateEntity(archetype0);
            var entity1 = m_Manager.CreateEntity(archetype1);

            var preChunks0 = group0.CreateArchetypeChunkArray(Allocator.TempJob);
            var preChunks1 = group1.CreateArchetypeChunkArray(Allocator.TempJob);

            Assert.AreEqual(2, ArchetypeChunkArray.CalculateEntityCount(preChunks0));
            Assert.AreEqual(1, ArchetypeChunkArray.CalculateEntityCount(preChunks1));

            m_Manager.RemoveComponent<SharedData2>(entity1);

            var postChunks0 = group0.CreateArchetypeChunkArray(Allocator.TempJob);
            var postChunks1 = group1.CreateArchetypeChunkArray(Allocator.TempJob);

            Assert.AreEqual(2, ArchetypeChunkArray.CalculateEntityCount(postChunks0));
            Assert.AreEqual(0, ArchetypeChunkArray.CalculateEntityCount(postChunks1));

            group0.Dispose();
            group1.Dispose();
            preChunks0.Dispose();
            preChunks1.Dispose();
            postChunks0.Dispose();
            postChunks1.Dispose();
        }
        
        [Test]
        public void SCG_SetSharedComponentDataWithQuery()
        {
            var noShared = m_Manager.CreateEntity(typeof(EcsTestData));
            
            var e0 = m_Manager.CreateEntity(typeof(SharedData1), typeof(EcsTestData));
            var e1 = m_Manager.CreateEntity(typeof(SharedData1));
            var e2 = m_Manager.CreateEntity(typeof(SharedData1), typeof(EcsTestData));

            m_Manager.SetSharedComponentData(e0, new SharedData1 {value = 0});
            m_Manager.SetSharedComponentData(e1, new SharedData1 {value = 1});
            m_Manager.SetSharedComponentData(e2, new SharedData1 {value = 2});

            var c0 = m_Manager.GetChunk(e0);
            var c1 = m_Manager.GetChunk(e1);
            var c2 = m_Manager.GetChunk(e2);
            var query = m_Manager.CreateEntityQuery(ComponentType.ReadWrite<SharedData1>(), ComponentType.ReadWrite<EcsTestData>());
            m_Manager.SetSharedComponentData(query, new SharedData1 {value = 10});
            
            Assert.AreEqual(10, m_Manager.GetSharedComponentData<SharedData1>(e0).value);
            Assert.AreEqual(1, m_Manager.GetSharedComponentData<SharedData1>(e1).value);
            Assert.AreEqual(10, m_Manager.GetSharedComponentData<SharedData1>(e2).value);
            Assert.IsFalse(m_Manager.HasComponent<SharedData1>(noShared));
            
            // This is not required but describes current behaviour,
            // Query based shared component does not reorder or merge chunks. (Even though in this case e0 & e2 could be in the same chunk)
            Assert.AreEqual(c0, m_Manager.GetChunk(e0));
            Assert.AreEqual(c1, m_Manager.GetChunk(e1));
            Assert.AreEqual(c2, m_Manager.GetChunk(e2));
            Assert.AreNotEqual(c0, c2);

            query.Dispose();
        }

        [Test]
        public void TooManySharedComponentsEntity()
        {
            var archetype = m_Manager.CreateArchetype(
                typeof(EcsTestData),
                typeof(SharedData1),
                typeof(SharedData2),
                typeof(SharedData3),
                typeof(SharedData4),
                typeof(SharedData5),
                typeof(SharedData6),
                typeof(SharedData7),
                typeof(SharedData8));
            
            Entity e = m_Manager.CreateEntity(archetype);
            Assert.Throws<InvalidOperationException>(() => m_Manager.AddComponent<SharedData9>(e) );
        }
        
        [Test]
        public void TooManySharedComponentsQuery()
        {
            var archetype = m_Manager.CreateArchetype(
                typeof(EcsTestData),
                typeof(SharedData1),
                typeof(SharedData2),
                typeof(SharedData3),
                typeof(SharedData4),
                typeof(SharedData5),
                typeof(SharedData6),
                typeof(SharedData7),
                typeof(SharedData8));
            
            Entity e = m_Manager.CreateEntity(archetype);
            EntityQuery q = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Assert.Throws<InvalidOperationException>(() => m_Manager.AddComponent<SharedData9>(q) );
            q.Dispose();
        }
        
        [Test]
        public void TooManySharedComponentsEntityArray()
        {
            var archetype = m_Manager.CreateArchetype(
                typeof(EcsTestData),
                typeof(SharedData1),
                typeof(SharedData2),
                typeof(SharedData3),
                typeof(SharedData4),
                typeof(SharedData5),
                typeof(SharedData6),
                typeof(SharedData7),
                typeof(SharedData8));
            
            var entities = new NativeArray<Entity>(1024, Allocator.Persistent);
            m_Manager.CreateEntity(archetype, entities); 
            Assert.Throws<InvalidOperationException>(() => m_Manager.AddComponent<SharedData9>(entities) );
            entities.Dispose();
        }

#if !NET_DOTS
        [Test]
        public void GetSharedComponentDataWithTypeIndex()
        {
            var archetype = m_Manager.CreateArchetype(typeof(SharedData1), typeof(EcsTestData));
            Entity e = m_Manager.CreateEntity(archetype);

            int typeIndex = TypeManager.GetTypeIndex<SharedData1>();

            object sharedComponentValue = m_Manager.GetSharedComponentData(e, typeIndex);
            Assert.AreEqual(typeof(SharedData1), sharedComponentValue.GetType());
            Assert.AreEqual(0, ((SharedData1)sharedComponentValue).value);

            m_Manager.SetSharedComponentData(e, new SharedData1(17));

            sharedComponentValue = m_Manager.GetSharedComponentData(e, typeIndex);
            Assert.AreEqual(typeof(SharedData1), sharedComponentValue.GetType());
            Assert.AreEqual(17, ((SharedData1)sharedComponentValue).value);
        }

        [Test]
        public void Case1085730()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsStringSharedComponent), typeof(EcsTestData));

            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new EcsStringSharedComponent { Value = "1" });
            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new EcsStringSharedComponent { Value = 1.ToString() });

            List<EcsStringSharedComponent> uniques = new List<EcsStringSharedComponent>();
            m_Manager.GetAllUniqueSharedComponentData(uniques);

            Assert.AreEqual(2, uniques.Count);
        }

        [Test]
        public void Case1085730_HashCode()
        {
            var a = new EcsStringSharedComponent { Value = "1" };
            var b = new EcsStringSharedComponent { Value = 1.ToString() };
            int ahash = TypeManager.GetHashCode(ref a);
            int bhash = TypeManager.GetHashCode(ref b);

            Assert.AreEqual(ahash, bhash);
        }

        [Test]
        public void Case1085730_Equals()
        {
            var a = new EcsStringSharedComponent { Value = "1" };
            var b = new EcsStringSharedComponent { Value = 1.ToString() };
            bool iseq = TypeManager.Equals(ref a, ref b);

            Assert.IsTrue(iseq);
        }

        public struct CustomEquality : ISharedComponentData, IEquatable<CustomEquality>
        {
            public int Foo;

            public bool Equals(CustomEquality other)
            {
                return (Foo & 0xff) == (other.Foo & 0xff);
            }

            public override int GetHashCode()
            {
                return Foo & 0xff;
            }
        }

        [Test]
        public void BlittableComponentCustomEquality()
        {
            var archetype = m_Manager.CreateArchetype(typeof(CustomEquality), typeof(EcsTestData));

            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new CustomEquality { Foo = 0x01 });
            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new CustomEquality { Foo = 0x2201 });
            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new CustomEquality { Foo = 0x3201 });

            List<CustomEquality> uniques = new List<CustomEquality>();
            m_Manager.GetAllUniqueSharedComponentData(uniques);

            Assert.AreEqual(2, uniques.Count);
        }
#endif
    }
}
