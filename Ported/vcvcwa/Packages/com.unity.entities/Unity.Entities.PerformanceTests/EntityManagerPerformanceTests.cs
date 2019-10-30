#define ENABLE_ADD_REMOVE_TEST_100
// #define ENABLE_ADD_REMOVE_TEST_1000
// #define ENABLE_ADD_REMOVE_TEST_10000

using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.Tests;
using Unity.PerformanceTesting;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Unity.Entities.PerformanceTests
{
    [Category("Performance")]
    public sealed class EntityManagerPerformanceTests : EntityPerformanceTestFixture
    {
        EntityArchetype archetype1;
        EntityArchetype archetype2;
        EntityArchetype archetype3;
        EntityArchetype archetype1WithSharedComponent;
        EntityArchetype archetype2WithSharedComponent;
        EntityArchetype archetype3WithSharedComponent;
        NativeArray<Entity> entities1;
        NativeArray<Entity> entities2;
        NativeArray<Entity> entities3;
        EntityQuery group;

        const int count = 1024 * 128;

        struct TestTag0 : IComponentData
        {
        }

        struct TestTag1 : IComponentData
        {
        }

        struct TestTag2 : IComponentData
        {
        }

        struct TestTag3 : IComponentData
        {
        }

        struct TestTag4 : IComponentData
        {
        }

        struct TestTag5 : IComponentData
        {
        }

        struct TestTag6 : IComponentData
        {
        }

        struct TestTag7 : IComponentData
        {
        }

        struct TestTag8 : IComponentData
        {
        }

        struct TestTag9 : IComponentData
        {
        }

        struct TestTag10 : IComponentData
        {
        }

        struct TestTag11 : IComponentData
        {
        }

        struct TestTag12 : IComponentData
        {
        }

        struct TestTag13 : IComponentData
        {
        }

        struct TestTag14 : IComponentData
        {
        }

        struct TestTag15 : IComponentData
        {
        }

        struct TestTag16 : IComponentData
        {
        }

        struct TestTag17 : IComponentData
        {
        }

        Type[] TagTypes = 
        {
            typeof(TestTag0),
            typeof(TestTag1),
            typeof(TestTag2),
            typeof(TestTag3),
            typeof(TestTag4),
            typeof(TestTag5),
            typeof(TestTag6),
            typeof(TestTag7),
            typeof(TestTag8),
            typeof(TestTag9),
            typeof(TestTag10),
            typeof(TestTag11),
            typeof(TestTag12),
            typeof(TestTag13),
            typeof(TestTag14),
            typeof(TestTag15),
            typeof(TestTag16),
            typeof(TestTag17),
        };

        public override void Setup()
        {
            base.Setup();

            archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3), typeof(TestTag0));
            archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            archetype3 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3));
            archetype1WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            archetype2WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            archetype3WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            entities1 = new NativeArray<Entity>(count, Allocator.Persistent);
            entities2 = new NativeArray<Entity>(count, Allocator.Persistent);
            entities3 = new NativeArray<Entity>(count, Allocator.Persistent);
            group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
        }

        [TearDown]
        public override void TearDown()
        {
            if (m_Manager != null)
            {
                entities1.Dispose();
                entities2.Dispose();
                entities3.Dispose();
                group.Dispose();
            }
            base.TearDown();
        }

        void CreateEntities()
        {
            m_Manager.CreateEntity(archetype1, entities1);
            m_Manager.CreateEntity(archetype2, entities2);
            m_Manager.CreateEntity(archetype3, entities3);
        }

        void DestroyEntities()
        {
            m_Manager.DestroyEntity(entities1);
            m_Manager.DestroyEntity(entities2);
            m_Manager.DestroyEntity(entities3);
        }

        NativeArray<Entity> CreateUniqueEntities(int size, ComponentType additionalComponentType)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));
                typeList.Add(additionalComponentType);

                var types = typeList.ToArray();
                var archetype = m_Manager.CreateArchetype(types);
                entities[i] = m_Manager.CreateEntity(archetype);
            }

            return entities;
        }

        NativeArray<Entity> CreateSameEntities(int size, ComponentType additionalComponentType)
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), additionalComponentType);
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            m_Manager.CreateEntity(archetype, entities);
            return entities;
        }

        NativeArray<Entity> CreateUniqueEntities(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));
                var types = typeList.ToArray();
                var archetype = m_Manager.CreateArchetype(types);
                entities[i] = m_Manager.CreateEntity(archetype);
            }

            return entities;
        }

        NativeArray<Entity> CreateUniqueEntitiesWithSharedComponent(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));
                typeList.Add(ComponentType.ReadWrite<EcsTestSharedComp>());

                var types = typeList.ToArray();
                var archetype = m_Manager.CreateArchetype(types);
                entities[i] = m_Manager.CreateEntity(archetype);
            }

            return entities;
        }

        NativeArray<Entity> CreateUniqueEntitiesWithChunkComponent(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));
                typeList.Add(ComponentType.ChunkComponent<EcsTestDataEntity>());

                var types = typeList.ToArray();
                var archetype = m_Manager.CreateArchetype(types);
                entities[i] = m_Manager.CreateEntity(archetype);
            }

            return entities;
        }

        ComponentType[][] CreateUniqueArchetypeTypes(int size)
        {
            var types = new ComponentType[size][];

            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));

                types[i] = typeList.ToArray();
            }

            return types;
        }

        NativeArray<EntityArchetype> CreateUniqueArchetypes(int size)
        {
            var archetypes = new NativeArray<EntityArchetype>(size, Allocator.Persistent);

            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(EcsTestData));

                var types = typeList.ToArray();
                archetypes[i] = m_Manager.CreateArchetype(types);
            }

            return archetypes;
        }

        NativeArray<Entity> CreateSameEntities(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            m_Manager.CreateEntity(archetype1, entities);
            return entities;
        }

        NativeArray<Entity> CreateSameEntitiesNoTag(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData2), typeof(EcsTestData3));
            m_Manager.CreateEntity(archetype, entities);
            return entities;
        }

        [Test, Performance]
        public void AddComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddComponent(group, typeof(EcsTestData4)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void AddTagComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddComponent(group, typeof(EcsTestTag)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void AddSharedComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddSharedComponentData(group, new EcsTestSharedComp(7)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void AddChunkComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddChunkComponentData(group, new EcsTestData4(7)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void RemoveComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.RemoveComponent(group, typeof(EcsTestData4)); })
                .SetUp(() =>
                {
                    CreateEntities();
                    m_Manager.AddComponent(group, typeof(EcsTestData4));
                })
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void RemoveTagComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.RemoveComponent(group, typeof(EcsTestTag)); })
                .SetUp(() =>
                {
                    CreateEntities();
                    m_Manager.AddComponent(group, typeof(EcsTestTag));
                })
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void RemoveSharedComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.RemoveComponent(group, typeof(EcsTestSharedComp)); })
                .SetUp(() =>
                {
                    CreateEntities();
                    m_Manager.AddSharedComponentData(group, new EcsTestSharedComp(7));
                })
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void RemoveChunkComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.RemoveChunkComponentData<EcsTestData4>(group); })
                .SetUp(() =>
                {
                    CreateEntities();
                    m_Manager.AddChunkComponentData(group, new EcsTestData4(7));
                })
                .CleanUp(DestroyEntities)
                .Run();
        }

        [Test, Performance]
        public void AddSharedComponentWithGroupIncompatibleLayout()
        {
            Measure.Method(() =>
                {
                    m_Manager.AddSharedComponentData(group, new EcsTestSharedCompWithMaxChunkCapacity(7));
                })
                .SetUp(() =>
                {
                    unsafe
                    {
                        Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype1.Archetype,
                            archetype1WithSharedComponent.Archetype));
                        Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype2.Archetype,
                            archetype2WithSharedComponent.Archetype));
                        Assert.IsFalse(ChunkDataUtility.AreLayoutCompatible(archetype3.Archetype,
                            archetype3WithSharedComponent.Archetype));
                    }

                    CreateEntities();
                })
                .CleanUp(DestroyEntities)
                .Run();
        }

        // Test Conditions:
        //   * +/- SharedComponent
        //   * Few/Many Archetypes
        //        
        // EntityManagerCreateDestroyEntities:
        //   [x] public Entity CreateEntity()
        //   [x] public Entity CreateEntity(EntityArchetype archetype)
        //   [x] public void CreateEntity(EntityArchetype archetype, NativeArray<Entity> entities)
        //   [x] public Entity CreateEntity(params ComponentType[] types)
        //   [x] public Entity Instantiate(Entity srcEntity)
        //   [x] public void Instantiate(Entity srcEntity, NativeArray<Entity> outputEntities)
        //   [ ] public void CreateChunk(EntityArchetype archetype, NativeArray<ArchetypeChunk> chunks, int entityCount)
        //   [x] public void DestroyEntity(Entity entity)
        //   [ ] public void DestroyEntity(NativeSlice<Entity> entities)
        //   [x] public void DestroyEntity(NativeArray<Entity> entities)
        //   [x] public void DestroyEntity(EntityQuery entityQuery)
        //
        // EntityManagerCreateArchetype:
        //   [x] public EntityArchetype CreateArchetype(params ComponentType[] types)
        //
        // EntityManagerChangeArchetype:
        //   [ ] public void AddComponent(Entity entity, ComponentType componentType)
        //   [x] public void AddComponent<T>(Entity entity)
        //   [ ] public void AddComponent(EntityQuery entityQuery, ComponentType componentType)
        //   [x] public void AddComponent<T>(EntityQuery entityQuery)
        //   [ ] public void AddComponent(NativeArray<Entity> entities, ComponentType componentType)
        //   [x] public void AddComponent<T>(NativeArray<Entity> entities)
        //   [x] public void AddComponents(Entity entity, ComponentTypes types)
        //   [ ] public void AddComponentData<T>(EntityQuery entityQuery, NativeArray<T> componentArray) where T : struct, IComponentData
        //   [x] public void AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        //   [x] public void AddChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        //   [x] public void AddChunkComponentData<T>(EntityQuery entityQuery, T componentData) where T : struct, IComponentData
        //   [ ] public void AddSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        //   [ ] public void AddSharedComponentData<T>(EntityQuery entityQuery, T componentData) where T : struct, ISharedComponentDats
        //   [x] public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        //   [ ] public void AddComponentObject(Entity entity, object componentData)
        //   [x] public void RemoveComponent<T>(NativeArray<Entity> entities)
        //   [ ] public void RemoveComponent(NativeArray<Entity> entities, ComponentType componentType)
        //   [x] public void RemoveComponent<T>(Entity entity)
        //   [ ] public void RemoveComponent(Entity entity, ComponentType type)
        //   [ ] public void RemoveComponent(EntityQuery entityQuery, ComponentType componentType)
        //   [ ] public void RemoveComponent(EntityQuery entityQuery, ComponentTypes types)
        //   [x] public void RemoveComponent<T>(EntityQuery entityQuery)
        //   [x] public void RemoveChunkComponent<T>(Entity entity)
        //   [x] public void RemoveChunkComponentData<T>(EntityQuery entityQuery)
        //   [ ] public void SetArchetype(Entity entity, EntityArchetype archetype)
        //   [ ] public void SetEnabled(Entity entity, bool enabled)

        [Test, Performance]
        public void CreateEntity([Values(1, 10, 1000, 10000)] int size)
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.CreateEntity();
                })
                .SetUp(() => { })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void CreateEntityArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.CreateEntity(archetype1);
                })
                .SetUp(() => { })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void CreateEntitiesArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.CreateEntity(archetype1, entities); })
                .SetUp(() => { entities = new NativeArray<Entity>(size, Allocator.Persistent); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void CreateEntityArchetypeUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var archetypes = default(NativeArray<EntityArchetype>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.CreateEntity(archetypes[i]);
                })
                .SetUp(() => { archetypes = CreateUniqueArchetypes(size); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }

                    archetypes.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void CreateEntityArchetypeByTypesUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var types = new ComponentType[size][];

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.CreateEntity(types[i]);
                })
                .SetUp(() => { types = CreateUniqueArchetypeTypes(size); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntitySame([Values(1, 10, 1000, 10000)] int size)
        {
            var sourceEntity = default(Entity);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.Instantiate(sourceEntity);
                })
                .SetUp(() => { sourceEntity = m_Manager.CreateEntity(archetype1); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntitySameWithSharedComponent([Values(1, 10, 1000, 10000)] int size)
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            var sourceEntity = default(Entity);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.Instantiate(sourceEntity);
                })
                .SetUp(() => { sourceEntity = m_Manager.CreateEntity(archetype); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntityUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var sourceEntities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.Instantiate(sourceEntities[i]);
                })
                .SetUp(() => { sourceEntities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }

                    sourceEntities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntityUniqueWithSharedComponent([Values(1, 10, 1000, 10000)] int size)
        {
            var sourceEntities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.Instantiate(sourceEntities[i]);
                })
                .SetUp(() => { sourceEntities = CreateUniqueEntitiesWithSharedComponent(size); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }

                    sourceEntities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntitiesSame([Values(1, 10, 1000, 10000)] int size)
        {
            var sourceEntity = default(Entity);
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.Instantiate(sourceEntity, entities); })
                .SetUp(() =>
                {
                    entities = new NativeArray<Entity>(size, Allocator.Persistent);
                    sourceEntity = m_Manager.CreateEntity(archetype1);
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void InstantiateEntitiesSameWithSharedComponent([Values(1, 10, 1000, 10000)] int size)
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            var sourceEntity = default(Entity);
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.Instantiate(sourceEntity, entities); })
                .SetUp(() =>
                {
                    entities = new NativeArray<Entity>(size, Allocator.Persistent);
                    sourceEntity = m_Manager.CreateEntity(archetype);
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntityArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.DestroyEntity(entities[i]);
                })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntityArchetypeUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.DestroyEntity(entities[i]);
                })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntitiesArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntitiesArchetypeUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntitiesArchetypeSameByQuery([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(m_Manager.UniversalQuery); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void DestroyEntitiesArchetypeUniqueByQuery([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(m_Manager.UniversalQuery); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void CreateArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var types = new ComponentType[size][];

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.CreateArchetype(types[i]);
                })
                .SetUp(() => { types = CreateUniqueArchetypeTypes(size); })
                .CleanUp(() =>
                {
                    using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
                    {
                        m_Manager.DestroyEntity(entities);
                    }
                })
                .WarmupCount(1)
                .Run();
        }


        [Test, Performance]
        public void AddComponentDataWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddComponentData(entities[i], new EcsTestFloatData {Value = 1.0f});
                })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void AddComponentDataWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddComponentData(entities[i], new EcsTestFloatData {Value = 1.0f});
                })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


        [Test, Performance]
        public void AddChunkComponentDataWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.AddChunkComponentData(query, new EcsTestFloatData {Value = 1.0f}); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

        [Test, Performance]
        public void AddChunkComponentDataWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.AddChunkComponentData(query, new EcsTestFloatData {Value = 1.0f}); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


        public enum TestComponentVariation
        {
            Component,
            ComponentTag,
            SharedComponent,
            Buffer
        }

        public enum TestArchetypeVariation
        {
            AllSame,
            AllUnique
        }

        public enum TestTypeVariation
        {
            Query,
            Entity,
            EntityArray
        };

        public struct WobbleParcelBatch : ISharedComponentData
        {
            public int Value;

            public WobbleParcelBatch(int value)
            {
                Value = value;
            }
        }

        public struct WobbleParcel : IComponentData
        {
            public int Value;
        }

        public struct WobbleParcelBuffer : IBufferElementData
        {
            public int Value;
        }

        public struct WobbleParcelTag : IComponentData
        {
        }

        public struct WobbleParcelShared : ISharedComponentData, IEquatable<WobbleParcelShared>
        {
            public string Value;

            public bool Equals(WobbleParcelShared other)
            {
                return Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }


        delegate void TestQuery(EntityQuery query, ComponentType componentType);

        delegate void TestEntity(Entity entity, ComponentType componentType);

        delegate void TestEntityArray(NativeArray<Entity> entities, ComponentType componentType);

        void AddRemoveComponentTest(
            int entityCount,
            int batchSize,
            TestTypeVariation testTypeVariation,
            TestComponentVariation componentVariation,
            bool addComponentVariation,
            TestArchetypeVariation archetypeVariation,
            TestQuery testQuery,
            TestEntity testEntity,
            TestEntityArray testEntityArray)
        {
            ComponentType additionalComponentType;

            if (componentVariation == TestComponentVariation.Component)
                additionalComponentType = typeof(WobbleParcel);
            else if (componentVariation == TestComponentVariation.ComponentTag)
                additionalComponentType = typeof(WobbleParcelTag);
            else if (componentVariation == TestComponentVariation.SharedComponent)
                additionalComponentType = typeof(WobbleParcelShared);
            else // if (componentVariation == TestComponentVariation.Buffer)
                additionalComponentType = typeof(WobbleParcelBuffer);

            if (testTypeVariation == TestTypeVariation.Query)
            {
                var entities = default(NativeArray<Entity>);
                var queries = new EntityQuery[0];

                Measure.Method(() =>
                    {
                        for (int i = 0; i < queries.Length; i++)
                            testQuery(queries[i], additionalComponentType);
                    })
                    .SetUp(() =>
                    {
                        if (addComponentVariation)
                        {
                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount, additionalComponentType);
                            else
                                entities = CreateSameEntities(entityCount, additionalComponentType);
                        }
                        else
                        {

                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount);
                            else
                                entities = CreateSameEntities(entityCount);
                        }

                        if (batchSize == entityCount)
                            queries = new EntityQuery[] {m_Manager.UniversalQuery};
                        else
                        {
                            var queryCount = (entityCount + (batchSize - 1)) / batchSize;
                            queries = new EntityQuery[queryCount];
                            var queryIndex = 0;
                            for (int i = 0; i < entityCount; i += batchSize)
                            {
                                int remaining = math.min(entityCount - i, batchSize);
                                var filterBy = new WobbleParcelBatch(i);
                                queries[queryIndex] =
                                    m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(WobbleParcelBatch));
                                queries[queryIndex].SetSharedComponentFilter(filterBy);
                                for (int j = i; j < i + remaining; j++)
                                    m_Manager.AddSharedComponentData(entities[j], filterBy);
                                queryIndex++;
                            }
                        }
                    })
                    .CleanUp(() =>
                    {
                        m_Manager.DestroyEntity(entities);

                        for (int i = 0; i < queries.Length; i++)
                            queries[i].Dispose();

                        entities.Dispose();
                    })
                    .WarmupCount(1)
                    .Run();
            }
            else if (testTypeVariation == TestTypeVariation.EntityArray)
            {
                var entities = default(NativeArray<Entity>);
                var entityBatches = new NativeArray<Entity>[0];

                Measure.Method(() =>
                    {
                        for (int i = 0; i < entityBatches.Length; i++)
                            testEntityArray(entityBatches[i], additionalComponentType);
                    })
                    .SetUp(() =>
                    {
                        if (addComponentVariation)
                        {
                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount, additionalComponentType);
                            else
                                entities = CreateSameEntities(entityCount, additionalComponentType);
                        }
                        else
                        {

                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount);
                            else
                                entities = CreateSameEntities(entityCount);
                        }

                        if (batchSize == entityCount)
                        {
                            var batchAll = new NativeArray<Entity>(entities.Length, Allocator.Persistent);
                            batchAll.CopyFrom(entities);
                            entityBatches = new NativeArray<Entity>[] {batchAll};
                        }
                        else
                        {
                            var queryCount = (entityCount + (batchSize - 1)) / batchSize;
                            entityBatches = new NativeArray<Entity>[queryCount];
                            var queryIndex = 0;
                            for (int i = 0; i < entityCount; i += batchSize)
                            {
                                int remaining = math.min(entityCount - i, batchSize);
                                var filterBy = new WobbleParcelBatch(i);
                                var query =
                                    m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(WobbleParcelBatch));
                                query.SetSharedComponentFilter(filterBy);
                                for (int j = i; j < i + remaining; j++)
                                    m_Manager.AddSharedComponentData(entities[j], filterBy);
                                entityBatches[queryIndex] = query.ToEntityArray(Allocator.Persistent);
                                queryIndex++;
                            }
                        }
                    })
                    .CleanUp(() =>
                    {
                        m_Manager.DestroyEntity(entities);

                        for (int i = 0; i < entityBatches.Length; i++)
                            entityBatches[i].Dispose();

                        entities.Dispose();
                    })
                    .WarmupCount(1)
                    .Run();
            }
            else if (testTypeVariation == TestTypeVariation.Entity)
            {
                var entities = default(NativeArray<Entity>);
                var entityBatches = new NativeArray<Entity>[0];

                Measure.Method(() =>
                    {
                        for (int i = 0; i < entityBatches.Length; i++)
                        for (int j = 0; j < entityBatches[i].Length; j++)
                            testEntity(entityBatches[i][j], additionalComponentType);
                    })
                    .SetUp(() =>
                    {
                        if (addComponentVariation)
                        {
                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount, additionalComponentType);
                            else
                                entities = CreateSameEntities(entityCount, additionalComponentType);
                        }
                        else
                        {

                            if (archetypeVariation == TestArchetypeVariation.AllUnique)
                                entities = CreateUniqueEntities(entityCount);
                            else
                                entities = CreateSameEntities(entityCount);
                        }

                        if (batchSize == entityCount)
                        {
                            var batchAll = new NativeArray<Entity>(entities.Length, Allocator.Persistent);
                            batchAll.CopyFrom(entities);
                            entityBatches = new NativeArray<Entity>[] {batchAll};
                        }
                        else
                        {
                            var queryCount = (entityCount + (batchSize - 1)) / batchSize;
                            entityBatches = new NativeArray<Entity>[queryCount];
                            var queryIndex = 0;
                            for (int i = 0; i < entityCount; i += batchSize)
                            {
                                int remaining = math.min(entityCount - i, batchSize);
                                var filterBy = new WobbleParcelBatch(i);
                                var query =
                                    m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(WobbleParcelBatch));
                                query.SetSharedComponentFilter(filterBy);
                                for (int j = i; j < i + remaining; j++)
                                    m_Manager.AddSharedComponentData(entities[j], filterBy);
                                entityBatches[queryIndex] = query.ToEntityArray(Allocator.Persistent);
                                queryIndex++;
                            }
                        }
                    })
                    .CleanUp(() =>
                    {
                        m_Manager.DestroyEntity(entities);

                        for (int i = 0; i < entityBatches.Length; i++)
                            entityBatches[i].Dispose();

                        entities.Dispose();
                    })
                    .WarmupCount(1)
                    .Run();
            }
        }

#if ENABLE_ADD_REMOVE_TEST_100

        [Test, Performance]
        public void RemoveComponent100(
            [Values(1, 10, 100)] int batchSize, [Values(100)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)]
            TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag,
                TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)]
            TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)]
            TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, true,
                archetypeVariation,
                (entityQuery, componentType) => { m_Manager.RemoveComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.RemoveComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.RemoveComponent(entities, componentType); }
            );

        [Test, Performance]
        public void AddComponent100(
            [Values(1, 10, 100)] int batchSize, [Values(100)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)]
            TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag,
                TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)]
            TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)]
            TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, false,
                archetypeVariation,
                (entityQuery, componentType) => { m_Manager.AddComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.AddComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.AddComponent(entities, componentType); }
            );
#endif
 
#if ENABLE_ADD_REMOVE_TEST_1000

        [Test, Performance]
        public void RemoveComponent1000(
            [Values(1, 10, 1000)] int batchSize, [Values(1000)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)]
            TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag,
                TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)]
            TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)]
            TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, true,
                archetypeVariation,
                (entityQuery, componentType) => { m_Manager.RemoveComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.RemoveComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.RemoveComponent(entities, componentType); }
            );

        [Test, Performance]
        public void AddComponent1000(
            [Values(1, 10, 1000)] int batchSize, [Values(1000)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)]
            TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag,
                TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)]
            TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)]
            TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, false,
                archetypeVariation,
                (entityQuery, componentType) => { m_Manager.AddComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.AddComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.AddComponent(entities, componentType); }
            );
#endif 
    
#if ENABLE_ADD_REMOVE_TEST_10000

        [Test, Performance]
        public void RemoveComponent1000(
            [Values(10, 1000, 10000)] int batchSize, [Values(10000)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)] TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag, TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)] TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)] TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, true, archetypeVariation,
                (entityQuery, componentType) => { m_Manager.RemoveComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.RemoveComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.RemoveComponent(entities, componentType); }
                );
        
        [Test, Performance]
        public void AddComponent10000(
            [Values(10, 1000, 10000)] int batchSize, [Values(10000)] int entityCount,
            [Values(TestTypeVariation.Entity, TestTypeVariation.EntityArray, TestTypeVariation.Query)] TestTypeVariation testTypeVariation,
            [Values(TestComponentVariation.Component, TestComponentVariation.ComponentTag, TestComponentVariation.SharedComponent, TestComponentVariation.Buffer)] TestComponentVariation componentVariation,
            [Values(TestArchetypeVariation.AllSame, TestArchetypeVariation.AllUnique)] TestArchetypeVariation archetypeVariation)
            => AddRemoveComponentTest(entityCount, batchSize, testTypeVariation, componentVariation, false, archetypeVariation,
                (entityQuery, componentType) => { m_Manager.AddComponent(entityQuery, componentType); },
                (entity, componentType) => { m_Manager.AddComponent(entity, componentType); },
                (entities, componentType) => { m_Manager.AddComponent(entities, componentType); }
            );
#endif 
    }
}
