using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;
using Unity.Entities.Tests;

namespace Unity.Entities.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    public sealed class EntityQueryPerformanceTests : EntityPerformanceTestFixture
    {
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
                typeList.Add(typeof(EcsTestSharedComp));

                var types = typeList.ToArray();
                archetypes[i] = m_Manager.CreateArchetype(types);
            }

            return archetypes;
        }

        [Test, Performance]
        public void CalculateEntityCount_N_Archetypes_M_ChunksPerArchetype([Values(1, 10, 100)] int archetypeCount, [Values(1, 10, 100)] int chunkCount)
        {
            var archetypes = CreateUniqueArchetypes(archetypeCount);
            for (int i = 0; i < archetypes.Length; ++i)
            {
                var entities =new NativeArray<Entity>(archetypes[i].ChunkCapacity * chunkCount, Allocator.Temp);
                m_Manager.CreateEntity(archetypes[i], entities);
            }
            
            var group = m_Manager.CreateEntityQuery(ComponentType.ReadWrite<EcsTestData>(), ComponentType.ReadWrite<EcsTestSharedComp>());
            
            Measure.Method(
                    () =>
                    {
                        group.CalculateEntityCount();
                    })
                .Definition("CalculateEntityCount")
                .Run();
            
            group.SetSharedComponentFilter(new EcsTestSharedComp{value = archetypeCount + chunkCount});
            
            Measure.Method(
                    () =>
                    {
                        group.CalculateEntityCount();
                    })
                .Definition("CalculateEntityCount with Filtering")
                .Run();
            
            using (var entities = m_Manager.UniversalQuery.ToEntityArray(Allocator.TempJob))
            {
                m_Manager.DestroyEntity(entities);
            }
            archetypes.Dispose();
            group.Dispose();
        }
    }
}

