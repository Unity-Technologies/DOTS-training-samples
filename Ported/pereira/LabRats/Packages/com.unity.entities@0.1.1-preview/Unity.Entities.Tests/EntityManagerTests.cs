using System;
using Unity.Collections;
using NUnit.Framework;
using UnityEngine;

namespace Unity.Entities.Tests
{
    interface IEcsFooInterface
    {
        int value { get; set; }

    }
    public struct EcsFooTest : IComponentData, IEcsFooInterface
    {
        public int value { get; set; }

        public EcsFooTest(int inValue) { value = inValue; }
    }

    interface IEcsNotUsedInterface
    {
        int value { get; set; }

    }

    class EntityManagerTests : ECSTestsFixture
    {
#if UNITY_EDITOR
        [Test]
        public void NameEntities()
        {
            WordStorage.Setup();
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var count = 1024;
            var array = new NativeArray<Entity>(count, Allocator.Temp);
            m_Manager.CreateEntity(archetype, array);
            for (int i = 0; i < count; i++)
            {
                m_Manager.SetName(array[i], "Name" + i);
            }

            for (int i = 0; i < count; ++i)
            {
                Assert.AreEqual(m_Manager.GetName(array[i]), "Name" + i);
            }

            // even though we've made 1024 entities, the string table should contain only two entries:
            // "", and "Name"
            Assert.IsTrue(WordStorage.Instance.Entries == 2);
            array.Dispose();
        }

        [Test]
        public void InstantiateKeepsName()
        {
            WordStorage.Setup();
            var entity = m_Manager.CreateEntity();
            m_Manager.SetName(entity, "Blah");

            var instance = m_Manager.Instantiate(entity);
            Assert.AreEqual("Blah", m_Manager.GetName(instance));
        }
#endif

        [Test]
        public void IncreaseEntityCapacity()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var count = 1024;
            var array = new NativeArray<Entity>(count, Allocator.Temp);
            m_Manager.CreateEntity(archetype, array);
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, array[i].Index);
            }

            array.Dispose();
        }

        [Test]
        public void AddComponentEmptyNativeArray()
        {
            var array = new NativeArray<Entity>(0, Allocator.Temp);
            m_Manager.AddComponent(array, typeof(EcsTestData));
            array.Dispose();
        }

        unsafe public bool IndexInChunkIsValid(Entity entity)
        {
            var entityInChunk = m_Manager.EntityComponentStore->GetEntityInChunk(entity);
            return entityInChunk.IndexInChunk < entityInChunk.Chunk->Count;
        }

        [Test]
        unsafe public void AddComponentNativeArrayCorrectChunkIndexAfterPacking()
        {
            // This test checks for the bug revealed here https://github.com/Unity-Technologies/dots/issues/2133
            // When packing was done, it was possible for the packed entities to have an incorrect
            // EntityInChunk.IndexInChunk.  A catastrophic case was when IndexInChunk was larger than Chunk.Count.
            var types = new[] {ComponentType.ReadWrite<EcsTestData>()};
            var archetype = m_Manager.CreateArchetype(types);
            var entities = new NativeArray<Entity>(2, Allocator.TempJob);

            // Create four entities so that we create two holes in the chunk when we add a new
            // component to two of them.
            entities[0] = m_Manager.CreateEntity(archetype);
            var checkEntity1 = m_Manager.CreateEntity(archetype);
            entities[1] = m_Manager.CreateEntity(archetype);
            var checkEntity2 = m_Manager.CreateEntity(archetype);

            m_Manager.AddComponent(entities, typeof(EcsTestData2));

            Assert.IsTrue(IndexInChunkIsValid(checkEntity1));
            Assert.IsTrue(IndexInChunkIsValid(checkEntity2));
            entities.Dispose();
        }

        [Test]
        public void FoundComponentInterface()
        {
            var fooTypes = m_Manager.GetAssignableComponentTypes(typeof(IEcsFooInterface));
            Assert.AreEqual(1, fooTypes.Count);
            Assert.AreEqual(typeof(EcsFooTest), fooTypes[0]);

            var barTypes = m_Manager.GetAssignableComponentTypes(typeof(IEcsNotUsedInterface));
            Assert.AreEqual(0, barTypes.Count);
        }

        [Test]
        public void VersionIsConsistent()
        {
            Assert.AreEqual(0, m_Manager.Version);

            var entity = m_Manager.CreateEntity(typeof(EcsTestData));
            Assert.AreEqual(1, m_Manager.Version);

            m_Manager.AddComponentData(entity, new EcsTestData2(0));
            Assert.AreEqual(3, m_Manager.Version);

            m_Manager.SetComponentData(entity, new EcsTestData2(5));
            Assert.AreEqual(3, m_Manager.Version); // Shouldn't change when just setting data

            m_Manager.RemoveComponent<EcsTestData2>(entity);
            Assert.AreEqual(5, m_Manager.Version);

            m_Manager.DestroyEntity(entity);
            Assert.AreEqual(6, m_Manager.Version);
        }

        [Test]
        public void GetChunkVersions_ReflectsChange()
        {
            m_Manager.Debug.SetGlobalSystemVersion(1);

            var entity = m_Manager.CreateEntity(typeof(EcsTestData));

            m_Manager.Debug.SetGlobalSystemVersion(2);

            var version = m_Manager.GetChunkVersionHash(entity);

            m_Manager.SetComponentData(entity, new EcsTestData());

            var version2 = m_Manager.GetChunkVersionHash(entity);

            Assert.AreNotEqual(version, version2);
        }


        interface TestInterface
        {
        }

        struct TestInterfaceComponent : TestInterface, IComponentData
        {
            public int Value;
        }

        [Test]
        [StandaloneFixme]
        public void GetComponentBoxedSupportsInterface()
        {
            var entity = m_Manager.CreateEntity();

            m_Manager.AddComponentData(entity, new TestInterfaceComponent {Value = 5});
            var obj = m_Manager.Debug.GetComponentBoxed(entity, typeof(TestInterface));

            Assert.AreEqual(typeof(TestInterfaceComponent), obj.GetType());
            Assert.AreEqual(5, ((TestInterfaceComponent) obj).Value);
        }

        [Test]
        [StandaloneFixme]
        public void GetComponentBoxedThrowsWhenInterfaceNotFound()
        {
            var entity = m_Manager.CreateEntity();
            Assert.Throws<ArgumentException>(() => m_Manager.Debug.GetComponentBoxed(entity, typeof(TestInterface)));
        }

        [Test]
        [Ignore("NOT IMPLEMENTED")]
        public void UsingComponentGroupOrArchetypeorEntityFromDifferentEntityManagerGivesExceptions()
        {
        }

        [Test]
        public unsafe void ComponentsWithBool()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestComponentWithBool));
            var count = 128;
            var array = new NativeArray<Entity>(count, Allocator.Temp);
            m_Manager.CreateEntity(archetype, array);

            var hash = new NativeHashMap<Entity, bool>(count, Allocator.Temp);

            var cg = m_Manager.CreateEntityQuery(ComponentType.ReadWrite<EcsTestComponentWithBool>());
            using (var chunks = cg.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                var boolsType = m_Manager.GetArchetypeChunkComponentType<EcsTestComponentWithBool>(false);
                var entsType = m_Manager.GetArchetypeChunkEntityType();

                foreach (var chunk in chunks)
                {
                    var bools = chunk.GetNativeArray(boolsType);
                    var entities = chunk.GetNativeArray(entsType);

                    for (var i = 0; i < chunk.Count; ++i)
                    {
                        bools[i] = new EcsTestComponentWithBool {value = (entities[i].Index & 1) == 1};
                        Assert.IsTrue(hash.TryAdd(entities[i], bools[i].value));
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                var data = m_Manager.GetComponentData<EcsTestComponentWithBool>(array[i]);
                Assert.AreEqual((array[i].Index & 1) == 1, data.value);
            }

            array.Dispose();
            hash.Dispose();
        }


        struct BigComponentWithAlign1A : IComponentData
        {
            NativeString4096 bs;
            unsafe fixed byte val[3];
        }

        struct ComponentWithAlign8 : IComponentData
        {
            double val;
        }

        struct BigComponentWithAlign1B : IComponentData
        {
            NativeString4096 bs;
            unsafe fixed byte val[3];
        }

        [Test]
        public unsafe void ChunkComponentRunIsAligned()
        {
            // We need to make sure that the stricter-alignment component (WithAlign8) comes after the simpler one
            Type oneByteAlignmentType = typeof(BigComponentWithAlign1A);
            int bigTypeIndex = TypeManager.GetTypeIndex<ComponentWithAlign8>();
            if ((TypeManager.GetTypeIndex<BigComponentWithAlign1A>() & TypeManager.ClearFlagsMask) >
                (bigTypeIndex & TypeManager.ClearFlagsMask))
            {
                // must be the other one
                oneByteAlignmentType = typeof(BigComponentWithAlign1B);
            }

            var oneByteInfo = TypeManager.GetTypeInfo(TypeManager.GetTypeIndex(oneByteAlignmentType));
            int bigAlignment = TypeManager.GetTypeInfo<ComponentWithAlign8>().AlignmentInBytes;

            //Assert.AreEqual(4, TypeManager.GetTypeInfo(TypeManager.GetTypeIndex(oneByteAlignmentType)).AlignmentInBytes);
            Assert.AreEqual(8, bigAlignment);

            // Create an entity
            var archetype = m_Manager.CreateArchetype(oneByteAlignmentType, typeof(ComponentWithAlign8));
            var entity = m_Manager.CreateEntity(archetype);
            // Get a pointer to the first bigger-aligned component
            var p2 = m_Manager.GetComponentDataRawRW(entity, bigTypeIndex);

            // p2 needs to be aligned properly
            Assert.AreEqual(0, (long) p2 & (bigAlignment - 1));

            // But let's verify that we didn't get lucky.  If you see this assertion fire due to a change,
            // it's because chunk layout chanked such that the 8-byte-aligned chunk would naturally fall
            // on its proper alignment.  Play with the BigComponent sizes (by adding/removing members) above
            // until you get this to pass.
            Assert.AreNotEqual(0, (archetype.Archetype->ChunkCapacity * oneByteInfo.SizeInChunk) % 8);
        }

        void CreateEntitiesWithDataToRemove(NativeArray<Entity> entities)
        {
            var count = entities.Length;
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3), typeof(EcsTestDataEntity), typeof(EcsIntElement));
            
            m_Manager.CreateEntity(archetype, entities);

            for (int i = 0; i < count; i++)
            {
                m_Manager.SetComponentData(entities[i], new EcsTestData {value = i});
                m_Manager.SetComponentData(entities[i], new EcsTestData2 {value0 = 20000 + i, value1 = 40000 + i});
                m_Manager.SetComponentData(entities[i],
                    new EcsTestData3 {value0 = 50000 + i, value1 = 60000 + i, value2 = 70000 + i});
                m_Manager.SetComponentData(entities[i], new EcsTestDataEntity {value0 = i, value1 = entities[i]});
                var buffer = m_Manager.GetBuffer<EcsIntElement>(entities[i]);
                buffer.Add(new EcsIntElement {Value = i});
            }
        }

        void ValidateRemoveComponents(NativeArray<Entity> entities, NativeArray<Entity> entitiesToRemoveData)
        {
            var count = entities.Length;
            var group0 = m_Manager.CreateEntityQuery(typeof(EcsTestDataEntity));
            var entities0 = group0.ToEntityArray(Allocator.Persistent);

            Assert.AreEqual(entities0.Length, count);

            for (int i = 0; i < count; i++)
            {
                var entity = entities[i];
                var testDataEntity = m_Manager.GetComponentData<EcsTestDataEntity>(entity);
                var testBuffer = m_Manager.GetBuffer<EcsIntElement>(entity);
                Assert.AreEqual(testDataEntity.value1, entity);
                Assert.AreEqual(testDataEntity.value0, testBuffer[0].Value);
            }

            entities0.Dispose();
            group0.Dispose();

            for (int i = 0; i < entitiesToRemoveData.Length; i++)
            {
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(entitiesToRemoveData[i]));
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData2>(entitiesToRemoveData[i]));
            }
        }

        [Test]
        public void BatchRemoveComponents()
        {
            var count = 1024;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);

            CreateEntitiesWithDataToRemove(entities);
            
            var entitiesToRemoveData = new NativeArray<Entity>(count / 2, Allocator.Persistent);
            for (int i = 0; i < count / 2; i++)
                entitiesToRemoveData[i] = entities[i * 2];

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));

            var group0 = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            var group1 = m_Manager.CreateEntityQuery(typeof(EcsTestData2), typeof(EcsTestData3));

            var entities0 = group0.ToEntityArray(Allocator.Persistent);
            var entities1 = group1.ToEntityArray(Allocator.Persistent);

            Assert.AreEqual(entities0.Length, count / 2);
            Assert.AreEqual(entities1.Length, count);

            entities0.Dispose();
            entities1.Dispose();

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData2));

            var entities1b = group1.ToEntityArray(Allocator.Persistent);

            Assert.AreEqual(entities1b.Length, count / 2);

            entities1b.Dispose();

            entities.Dispose();
            entitiesToRemoveData.Dispose();

            group0.Dispose();
            group1.Dispose();
        }

        [Test]
        public void BatchRemoveComponentsValuesSimplified()
        {
            var count = 4;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);
            
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestDataEntity), typeof(EcsIntElement));
            
            m_Manager.CreateEntity(archetype, entities);

            for (int i = 0; i < count; i++)
            {
                m_Manager.SetComponentData(entities[i], new EcsTestData {value = i});
                m_Manager.SetComponentData(entities[i], new EcsTestDataEntity {value0 = i, value1 = entities[i]});
                var buffer = m_Manager.GetBuffer<EcsIntElement>(entities[i]);
                buffer.Add(new EcsIntElement {Value = i});
            }

            var entitiesToRemoveData = new NativeArray<Entity>(1, Allocator.Persistent);
            entitiesToRemoveData[0] = entities[0];
            
            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));

            ValidateRemoveComponents(entities, entitiesToRemoveData);
            
            entities.Dispose();
            entitiesToRemoveData.Dispose();
        }
        
        [Test]
        public void BatchRemoveComponentsValues()
        {
            var count = 1024;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);
            
            CreateEntitiesWithDataToRemove(entities);

            var entitiesToRemoveData = new NativeArray<Entity>(count / 2, Allocator.Persistent);
            for (int i = 0; i < count / 2; i++)
            {
               entitiesToRemoveData[i] = entities[i * 2];
            }

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));
            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData2));

            ValidateRemoveComponents(entities, entitiesToRemoveData);
            
            entities.Dispose();
            entitiesToRemoveData.Dispose();
        }
        
        [Test]
        public void BatchRemoveComponentsValuesWholeChunks()
        {
            var count = 1024;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);

            CreateEntitiesWithDataToRemove(entities);

            var entitiesToRemoveData = new NativeArray<Entity>(count / 2, Allocator.Persistent);
            for (int i = 0; i < count / 2; i++)
                entitiesToRemoveData[i] = entities[i];

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));
            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData2));

            ValidateRemoveComponents(entities, entitiesToRemoveData);
            
            entities.Dispose();
            entitiesToRemoveData.Dispose();
        }
        
        [Test]
        [Ignore("TODO")]
        public void BatchRemoveComponentsValuesDuplicates()
        {
            var count = 1024;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);

            CreateEntitiesWithDataToRemove(entities);

            var entitiesToRemoveData = new NativeArray<Entity>(count / 2, Allocator.Persistent);
            for (int i = 0; i < count / 2; i++)
                entitiesToRemoveData[i] = entities[i%16];

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));
            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData2));

            ValidateRemoveComponents(entities, entitiesToRemoveData);
            
            entities.Dispose();
            entitiesToRemoveData.Dispose();
        }
        
        [Test]
        public void BatchRemoveComponentsValuesBatches()
        {
            var count = 1024;
            var entities = new NativeArray<Entity>(count, Allocator.Persistent);

            CreateEntitiesWithDataToRemove(entities);

            var entitiesToRemoveData = new NativeArray<Entity>(count / 2, Allocator.Persistent);
            for (int i = 0; i < count / 2; i++)
            {
                var skipOffset = (i >> 5) * 40;
                var offset = i & 0x1f;
                entitiesToRemoveData[i] = entities[skipOffset + offset];
            }

            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData));
            m_Manager.RemoveComponent(entitiesToRemoveData, typeof(EcsTestData2));

            ValidateRemoveComponents(entities, entitiesToRemoveData);
            
            for (int i = 0; i < entitiesToRemoveData.Length; i++)
            {
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(entitiesToRemoveData[i]));
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData2>(entitiesToRemoveData[i]));
            }

            entities.Dispose();
            entitiesToRemoveData.Dispose();
        }
        
        [Test]
        public void AddComponentQueryWithArray()
        {
            m_Manager.CreateEntity(typeof(EcsTestData2));
            m_Manager.CreateEntity();
            m_Manager.CreateEntity();
            m_Manager.CreateEntity();

            var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob);
            var data = new NativeArray<EcsTestData>(entities.Length, Allocator.Temp);
            for (int i = 0; i != data.Length; i++)
                data[i] = new EcsTestData(entities[i].Index);
            
            m_Manager.AddComponentData(m_Manager.UniversalQuery, data);

            for (int i = 0; i != data.Length; i++)
                Assert.AreEqual(entities[i].Index, data[i].value);
            Assert.AreEqual(4, entities.Length);
                
            data.Dispose();
            entities.Dispose();
        }
    }
}
