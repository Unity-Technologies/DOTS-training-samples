using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.Tests;
using Unity.PerformanceTesting;
using System.Collections.Generic;
using Unity.Entities.Properties.Tests;

namespace Unity.Entities.PerformanceTests
{
    [Category("Performance")]
    public sealed class EntityManagerPerformanceTests
    {
        World m_PreviousWorld;
        World m_World;
        EntityManager m_Manager;
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

        Type[] TagTypes;

        [SetUp]
        public void Setup()
        {
            m_PreviousWorld = World.Active;
            m_World = World.Active = new World("Test World");
            m_Manager = m_World.EntityManager;
            archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3),
                typeof(TestTag0));
            archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            archetype3 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3));
            archetype1WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2),
                typeof(EcsTestData3), typeof(EcsTestSharedCompWithMaxChunkCapacity));
            archetype2WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2),
                typeof(EcsTestSharedCompWithMaxChunkCapacity));
            archetype3WithSharedComponent = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3),
                typeof(EcsTestSharedCompWithMaxChunkCapacity));
            entities1 = new NativeArray<Entity>(count, Allocator.Persistent);
            entities2 = new NativeArray<Entity>(count, Allocator.Persistent);
            entities3 = new NativeArray<Entity>(count, Allocator.Persistent);
            group = m_Manager.CreateEntityQuery(typeof(EcsTestData));

            TagTypes = new Type[]
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
        }

        [TearDown]
        public void TearDown()
        {
            if (m_Manager != null)
            {
                entities1.Dispose();
                entities2.Dispose();
                entities3.Dispose();
                group.Dispose();

                m_World.Dispose();
                m_World = null;

                World.Active = m_PreviousWorld;
                m_PreviousWorld = null;
                m_Manager = null;
            }
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

        ComponentTypes[] CreateUniqueArchetypeComponentTypes(int size)
        {
            var types = CreateUniqueArchetypeTypes(size);
            var componentTypes = new ComponentTypes[types.Length];
            for (int i = 0; i < types.Length; i++)
                componentTypes[i] = new ComponentTypes(types[i]);
            return componentTypes;
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

        NativeArray<Entity> CreateSameEntitiesWithSharedComponent(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            m_Manager.CreateEntity(archetype, entities);
            return entities;
        }

        NativeArray<Entity> CreateSameEntitiesWithChunkComponent(int size)
        {
            var entities = new NativeArray<Entity>(size, Allocator.Persistent);
            var archetype =
                m_Manager.CreateArchetype(typeof(EcsTestData), ComponentType.ChunkComponent<EcsTestDataEntity>());
            m_Manager.CreateEntity(archetype, entities);
            return entities;
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddComponent(group, typeof(EcsTestData4)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddTagComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddComponent(group, typeof(EcsTestTag)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddSharedComponentData(group, new EcsTestSharedComp(7)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddChunkComponentWithGroup()
        {
            Measure.Method(() => { m_Manager.AddChunkComponentData(group, new EcsTestData4(7)); })
                .SetUp(CreateEntities)
                .CleanUp(DestroyEntities)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void DestroyEntityArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.DestroyEntity(entities[i]);
                })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose();})
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void DestroyEntitiesArchetypeSame([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void DestroyEntitiesArchetypeUnique([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void DestroyEntitiesArchetypeSameByQuery([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(m_Manager.UniversalQuery); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void DestroyEntitiesArchetypeUniqueByQuery([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.DestroyEntity(m_Manager.UniversalQuery); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() => { entities.Dispose(); })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestData4>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTagWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestTag>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestSharedComp2>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestData4>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTagWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestTag>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponent<EcsTestSharedComp2>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithQuerySameArchetype()
        {
            var size = 10000;
            var entities = default(NativeArray<Entity>);
            var query = default(EntityQuery);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestData5>(query); })
                .SetUp(() =>
                {
                    entities = CreateSameEntities(size);
                    query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                    query.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTagWithQuerySameArchetype()
        {
            var size = 10000;
            var entities = default(NativeArray<Entity>);
            var query = default(EntityQuery);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestTag>(query); })
                .SetUp(() =>
                {
                    entities = CreateSameEntities(size);
                    query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                    query.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithQuerySameArchetype()
        {
            var size = 10000;
            var entities = default(NativeArray<Entity>);
            var query = default(EntityQuery);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestSharedComp2>(query); })
                .SetUp(() =>
                {
                    entities = CreateSameEntities(size);
                    query = m_Manager.CreateEntityQuery(typeof(EcsTestData));
                })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                    query.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestData4>(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTagWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestTag>(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestSharedComp2>(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestData4>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTagWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestTag>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddSharedComponentWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.AddComponent<EcsTestSharedComp2>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTypesWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var componentTypes = CreateUniqueArchetypeComponentTypes(size);

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponents(entities[i], componentTypes[i]);
                })
                .SetUp(() => { entities = CreateSameEntitiesNoTag(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTypesWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var componentTypes = new ComponentTypes(new ComponentType[]
                {typeof(TestTag0), typeof(TestTag1), typeof(TestTag2), typeof(TestTag3)});

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponents(entities[i], componentTypes);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddComponentTypesWithEntitySameArchetypeWithSharedComponent([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var componentTypes = new ComponentTypes(new ComponentType[]
                {typeof(TestTag0), typeof(TestTag1), typeof(TestTag2), typeof(TestTag3), typeof(TestSharedComponent)});

            Measure.Method(() =>
                {
                    for (int i = 0; i < size; i++)
                        m_Manager.AddComponents(entities[i], componentTypes);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddChunkComponentWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddChunkComponentData<EcsTestFloatData>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddChunkComponentWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddChunkComponentData<EcsTestFloatData>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddBufferWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddBuffer<TestBufferElementData>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddBufferWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.AddBuffer<TestBufferElementData>(entities[i]);
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddBufferAndAddWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var buffer = m_Manager.AddBuffer<TestBufferElementData>(entities[i]);
                        buffer.Add(new TestBufferElementData());
                    }
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void AddBufferAndAdddWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var buffer = m_Manager.AddBuffer<TestBufferElementData>(entities[i]);
                        buffer.Add(new TestBufferElementData());
                    }
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

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<TestTag0>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestData>(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<TestTag0>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<TestTag0>(entities); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithEntitiesUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestSharedComp>(entities); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithEntitiesSameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestSharedComp>(entities); })
                .SetUp(() => { entities = CreateSameEntitiesWithSharedComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<EcsTestData>(entities[i]);
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<EcsTestData>(entities[i]);
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<TestTag0>(entities[i]);
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<TestTag0>(entities[i]);
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<EcsTestSharedComp>(entities[i]);
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


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveComponent<EcsTestSharedComp>(entities[i]);
                })
                .SetUp(() => { entities = CreateSameEntitiesWithSharedComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }

#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestData>(query); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestData>(query); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<TestTag0>(query); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveComponentTagWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<TestTag0>(query); })
                .SetUp(() => { entities = CreateSameEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestSharedComp>(query); })
                .SetUp(() => { entities = CreateUniqueEntities(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveSharedComponentWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveComponent<EcsTestSharedComp>(query); })
                .SetUp(() => { entities = CreateSameEntitiesWithSharedComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveChunkomponentWithEntityUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveChunkComponent<EcsTestData>(entities[i]);
                })
                .SetUp(() => { entities = CreateUniqueEntitiesWithChunkComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveChunkComponentWithEntitySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);

            Measure.Method(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                        m_Manager.RemoveChunkComponent<EcsTestDataEntity>(entities[i]);
                })
                .SetUp(() => { entities = CreateSameEntitiesWithChunkComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }
#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveChunkComponentWithQueryUniqueArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveChunkComponentData<EcsTestDataEntity>(query); })
                .SetUp(() => { entities = CreateUniqueEntitiesWithChunkComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }


#if UNITY_2019_2_OR_NEWER
        [Test, Performance]
#else
        [PerformanceTest]
#endif
        public void RemoveChunkComponentWithQuerySameArchetype([Values(1, 10, 1000, 10000)] int size)
        {
            var entities = default(NativeArray<Entity>);
            var query = m_Manager.UniversalQuery;

            Measure.Method(() => { m_Manager.RemoveChunkComponentData<EcsTestDataEntity>(query); })
                .SetUp(() => { entities = CreateSameEntitiesWithChunkComponent(size); })
                .CleanUp(() =>
                {
                    m_Manager.DestroyEntity(entities);
                    entities.Dispose();
                })
                .WarmupCount(1)
                .Run();
        }
    }
}