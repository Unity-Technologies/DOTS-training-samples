using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities.Tests
{
    [TestFixture]
    class EntityQueryTests : ECSTestsFixture
    {
            ArchetypeChunk[] CreateEntitiesAndReturnChunks(EntityArchetype archetype, int entityCount, Action<Entity> action = null)
        {
            var entities = new NativeArray<Entity>(entityCount, Allocator.Temp);
            m_Manager.CreateEntity(archetype, entities);
#if UNITY_DOTSPLAYER
            var managedEntities = new Entity[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                managedEntities[i] = entities[i];
            }
#else
            var managedEntities = entities.ToArray();
#endif
            entities.Dispose();

            if(action != null)
                foreach(var e in managedEntities)
                    action(e);

            return managedEntities.Select(e => m_Manager.GetChunk(e)).Distinct().ToArray();
        }

        [Test]
        public void CreateArchetypeChunkArray()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData2));
            var archetype12 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));

            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, 5000);
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, 5000);
            var createdChunks12 = CreateEntitiesAndReturnChunks(archetype12, 5000);

            var allCreatedChunks = createdChunks1.Concat(createdChunks2).Concat(createdChunks12);

            var group1 = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            var group12 = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2));

            var queriedChunks1 = group1.CreateArchetypeChunkArray(Allocator.TempJob);
            var queriedChunks12 = group12.CreateArchetypeChunkArray(Allocator.TempJob);
            var queriedChunksAll = m_Manager.GetAllChunks(Allocator.TempJob);

            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks12), queriedChunks1);
            CollectionAssert.AreEquivalent(createdChunks12, queriedChunks12);
            CollectionAssert.AreEquivalent(allCreatedChunks,queriedChunksAll);

            queriedChunks1.Dispose();
            queriedChunks12.Dispose();
            queriedChunksAll.Dispose();
        }

        void SetShared(Entity e, int i)
        {
            m_Manager.SetSharedComponentData(e, new EcsTestSharedComp(i));
        }

        [Test]
        public void CreateArchetypeChunkArray_FiltersSharedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData2), typeof(EcsTestSharedComp));

            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 1));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 1));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 2));
            var createdChunks4 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 2));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestSharedComp));

            group.SetSharedComponentFilter(new EcsTestSharedComp(1));

            var queriedChunks1 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetSharedComponentFilter(new EcsTestSharedComp(2));

            var queriedChunks2 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks2), queriedChunks1);
            CollectionAssert.AreEquivalent(createdChunks3.Concat(createdChunks4), queriedChunks2);

            group.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
        }

        void SetShared(Entity e, int i, int j)
        {
            m_Manager.SetSharedComponentData(e, new EcsTestSharedComp(i));
            m_Manager.SetSharedComponentData(e, new EcsTestSharedComp2(j));
        }

        [Test]
        public void CreateArchetypeChunkArray_FiltersTwoSharedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData2), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));

            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 1,7));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 1,7));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 2,7));
            var createdChunks4 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 2,7));
            var createdChunks5 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 1,8));
            var createdChunks6 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 1,8));
            var createdChunks7 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetShared(e, 2,8));
            var createdChunks8 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetShared(e, 2,8));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));

            group.SetSharedComponentFilter(new EcsTestSharedComp(1), new EcsTestSharedComp2(7));
            var queriedChunks1 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetSharedComponentFilter(new EcsTestSharedComp(2), new EcsTestSharedComp2(7));
            var queriedChunks2 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetSharedComponentFilter(new EcsTestSharedComp(1), new EcsTestSharedComp2(8));
            var queriedChunks3 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetSharedComponentFilter(new EcsTestSharedComp(2), new EcsTestSharedComp2(8));
            var queriedChunks4 = group.CreateArchetypeChunkArray(Allocator.TempJob);


            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks2), queriedChunks1);
            CollectionAssert.AreEquivalent(createdChunks3.Concat(createdChunks4), queriedChunks2);
            CollectionAssert.AreEquivalent(createdChunks5.Concat(createdChunks6), queriedChunks3);
            CollectionAssert.AreEquivalent(createdChunks7.Concat(createdChunks8), queriedChunks4);

            group.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }

        void SetData(Entity e, int i)
        {
            m_Manager.SetComponentData(e, new EcsTestData(i));
        }

        [Test]
        public void CreateArchetypeChunkArray_FiltersChangeVersions()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            var archetype3 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3));

            m_ManagerDebug.SetGlobalSystemVersion(20);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetData(e, 1));
            m_ManagerDebug.SetGlobalSystemVersion(30);
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetData(e, 2));
            m_ManagerDebug.SetGlobalSystemVersion(40);
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype3, 5000, e => SetData(e, 3));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));

            group.SetChangedVersionFilter(typeof(EcsTestData));

            group.SetChangedFilterRequiredVersion(10);
            var queriedChunks1 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetChangedFilterRequiredVersion(20);
            var queriedChunks2 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetChangedFilterRequiredVersion(30);
            var queriedChunks3 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            group.SetChangedFilterRequiredVersion(40);
            var queriedChunks4 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks2).Concat(createdChunks3), queriedChunks1);
            CollectionAssert.AreEquivalent(createdChunks2.Concat(createdChunks3), queriedChunks2);
            CollectionAssert.AreEquivalent(createdChunks3, queriedChunks3);

            Assert.AreEqual(0, queriedChunks4.Length);

            group.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }

        void SetData(Entity e, int i, int j)
        {
            m_Manager.SetComponentData(e, new EcsTestData(i));
            m_Manager.SetComponentData(e, new EcsTestData2(j));
        }

        [Test]
        public void CreateArchetypeChunkArray_FiltersTwoChangeVersions()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3));
            var archetype3 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData4));

            m_ManagerDebug.SetGlobalSystemVersion(20);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, 5000, e => SetData(e, 1, 4));
            m_ManagerDebug.SetGlobalSystemVersion(30);
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, 5000, e => SetData(e, 2, 5));
            m_ManagerDebug.SetGlobalSystemVersion(40);
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype3, 5000, e => SetData(e, 3, 6));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2));

            group.SetChangedVersionFilter(new ComponentType[]{typeof(EcsTestData), typeof(EcsTestData2)});

            group.SetChangedFilterRequiredVersion(30);

            var testType1 = m_Manager.GetArchetypeChunkComponentType<EcsTestData>(false);
            var testType2 = m_Manager.GetArchetypeChunkComponentType<EcsTestData2>(false);

            var queriedChunks1 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            foreach (var chunk in createdChunks1)
            {
                var array = chunk.GetNativeArray(testType1);
                array[0] = new EcsTestData(7);
            }

            var queriedChunks2 = group.CreateArchetypeChunkArray(Allocator.TempJob);

            foreach (var chunk in createdChunks2)
            {
                var array = chunk.GetNativeArray(testType2);
                array[0] = new EcsTestData2(7);
            }

            var queriedChunks3 = group.CreateArchetypeChunkArray(Allocator.TempJob);


            CollectionAssert.AreEquivalent(createdChunks3, queriedChunks1);
            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks3), queriedChunks2);

            group.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
        }

        void SetDataAndShared(Entity e, int data, int shared)
        {
            SetData(e, data);
            SetShared(e, shared);
        }
        
        [Test]
        public void CreateArchetypeChunkArray_FiltersOneSharedOneChangeVersion()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));

            // 9 chunks
                // 3 of archetype1 with 1 shared value
                // 3 of archetype2 with 1 shared value
                // 3 of archetype1 with 2 shared value
            m_ManagerDebug.SetGlobalSystemVersion(10);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 1, 1));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, archetype2.ChunkCapacity * 3, e => SetDataAndShared(e, 2, 1));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 3, 2));

            // query matches all three
            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp));
            
            query.AddChangedVersionFilter(typeof(EcsTestData));
            query.AddSharedComponentFilter(new EcsTestSharedComp{value = 1});

            var queriedChunks1 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            query.SetChangedFilterRequiredVersion(10);
            var queriedChunks2 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData1 for createdChunks1
            m_ManagerDebug.SetGlobalSystemVersion(20);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>()); 
                array[0] = new EcsTestData{value = 10};
            }
            var queriedChunks3 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData2
            query.SetChangedFilterRequiredVersion(20);
            m_ManagerDebug.SetGlobalSystemVersion(30);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData2>());
                array[0] = new EcsTestData2{value1 = 10, value0 = 10};
            }
            var queriedChunks4 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks2), queriedChunks1); // query 1 = created 1,2
            Assert.AreEqual(0, queriedChunks2.Length); // query 2 is empty
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks3); // query 3 = created 1 (version # was bumped)
            Assert.AreEqual(0, queriedChunks4.Length); // query 4 is empty (version # of type we're not change tracking was bumped)

            query.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }
        
        [Test]
        public void CreateArchetypeChunkArray_FiltersOneSharedTwoChangeVersion()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3), typeof(EcsTestSharedComp));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));

            // 9 chunks
                // 3 of archetype1 with 1 shared value
                // 3 of archetype2 with 1 shared value
                // 3 of archetype1 with 2 shared value
            m_ManagerDebug.SetGlobalSystemVersion(10);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 1, 1));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, archetype2.ChunkCapacity * 3, e => SetDataAndShared(e, 2, 1));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 3, 2));

            // query matches all three
            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            
            query.AddChangedVersionFilter(typeof(EcsTestData));
            query.AddChangedVersionFilter(typeof(EcsTestData2));
            query.AddSharedComponentFilter(new EcsTestSharedComp{value = 1});

            var queriedChunks1 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            query.SetChangedFilterRequiredVersion(10);
            var queriedChunks2 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData1 for createdChunks1
            m_ManagerDebug.SetGlobalSystemVersion(20);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>()); 
                array[0] = new EcsTestData{value = 10};
            }
            var queriedChunks3 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData2
            query.SetChangedFilterRequiredVersion(20);
            m_ManagerDebug.SetGlobalSystemVersion(30);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData2>());
                array[0] = new EcsTestData2{value1 = 10, value0 = 10};
            }
            var queriedChunks4 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            CollectionAssert.AreEquivalent(createdChunks1.Concat(createdChunks2), queriedChunks1); // query 1 = created 1,2
            Assert.AreEqual(0, queriedChunks2.Length); // query 2 is empty
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks3); // query 3 = created 1 (version # of type1 was bumped)
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks4); // query 4 = created 1 (version # of type2 was bumped)

            query.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }
        
        void SetDataAndShared(Entity e, int data, int shared1, int shared2)
        {
            SetData(e, data);
            SetShared(e, shared1, shared2);
        }
        
        [Test]
        public void CreateArchetypeChunkArray_FiltersTwoSharedOneChangeVersion()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));

            // 9 chunks
                // 3 of archetype1 with 1 shared value1, 3,3 shared value2
                // 3 of archetype2 with 1 shared value1, 4,4 shared value2
                // 3 of archetype1 with 2 shared value1, 3,3 shared value2
            m_ManagerDebug.SetGlobalSystemVersion(10);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 1, 1, 3));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, archetype2.ChunkCapacity * 3, e => SetDataAndShared(e, 2, 1, 4));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 3, 2, 3));

            // query matches all three
            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));
            
            query.AddChangedVersionFilter(typeof(EcsTestData));
            query.AddSharedComponentFilter(new EcsTestSharedComp{value = 1});
            query.AddSharedComponentFilter(new EcsTestSharedComp2{value0 = 3, value1 = 3});

            var queriedChunks1 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            query.SetChangedFilterRequiredVersion(10);
            var queriedChunks2 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData1 for createdChunks1 and createdChunks2
            m_ManagerDebug.SetGlobalSystemVersion(20);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                {
                    var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>());
                    array[0] = new EcsTestData {value = 10};
                }
                {
                    var array = createdChunks3[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>());
                    array[0] = new EcsTestData {value = 10};
                }
            }
            var queriedChunks3 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData2 for createdChunks1
            query.SetChangedFilterRequiredVersion(20);
            m_ManagerDebug.SetGlobalSystemVersion(30);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData2>());
                array[0] = new EcsTestData2{value1 = 10, value0 = 10};
            }
            var queriedChunks4 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks1); // query 1 = created 1
            Assert.AreEqual(0, queriedChunks2.Length); // query 2 is empty
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks3); // query 3 = created 1 (version # was bumped and we're filtering out created2)
            Assert.AreEqual(0, queriedChunks4.Length); // query 4 is empty (version # of type we're not change tracking was bumped)

            query.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }
        
        [Test]
        public void CreateArchetypeChunkArray_FiltersTwoSharedTwoChangeVersion()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestData3), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));

            // 9 chunks
                // 3 of archetype1 with 1 shared value1, 3,3 shared value2
                // 3 of archetype2 with 1 shared value1, 4,4 shared value2
                // 3 of archetype1 with 2 shared value1, 3,3 shared value2
            m_ManagerDebug.SetGlobalSystemVersion(10);
            var createdChunks1 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 1, 1, 3));
            var createdChunks2 = CreateEntitiesAndReturnChunks(archetype2, archetype2.ChunkCapacity * 3, e => SetDataAndShared(e, 2, 1, 4));
            var createdChunks3 = CreateEntitiesAndReturnChunks(archetype1, archetype1.ChunkCapacity * 3, e => SetDataAndShared(e, 3, 2, 3));

            // query matches all three
            var query = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp), typeof(EcsTestSharedComp2));
            
            query.AddChangedVersionFilter(typeof(EcsTestData));
            query.AddChangedVersionFilter(typeof(EcsTestData2));
            query.AddSharedComponentFilter(new EcsTestSharedComp{value = 1});
            query.AddSharedComponentFilter(new EcsTestSharedComp2{value0 = 3, value1 = 3});

            var queriedChunks1 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            query.SetChangedFilterRequiredVersion(10);
            var queriedChunks2 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData1 for createdChunks1 and createdChunks2
            m_ManagerDebug.SetGlobalSystemVersion(20);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                {
                    var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>());
                    array[0] = new EcsTestData {value = 10};
                }
                {
                    var array = createdChunks3[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData>());
                    array[0] = new EcsTestData {value = 10};
                }
            }
            var queriedChunks3 = query.CreateArchetypeChunkArray(Allocator.TempJob);

            // bumps the version number for TestData2 for createdChunks1
            query.SetChangedFilterRequiredVersion(20);
            m_ManagerDebug.SetGlobalSystemVersion(30);
            for (int i = 0; i < createdChunks1.Length; ++i)
            {
                var array = createdChunks1[i].GetNativeArray(EmptySystem.GetArchetypeChunkComponentType<EcsTestData2>());
                array[0] = new EcsTestData2{value1 = 10, value0 = 10};
            }
            var queriedChunks4 = query.CreateArchetypeChunkArray(Allocator.TempJob);
            
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks1); // query 1 = created 1
            Assert.AreEqual(0, queriedChunks2.Length); // query 2 is empty
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks3); // query 3 = created 1 (version # was bumped and we're filtering out created2)
            CollectionAssert.AreEquivalent(createdChunks1, queriedChunks4); // query 4 = created 1 (version # of type2 was bumped)

            query.Dispose();
            queriedChunks1.Dispose();
            queriedChunks2.Dispose();
            queriedChunks3.Dispose();
            queriedChunks4.Dispose();
        }

        // https://github.com/Unity-Technologies/dots/issues/1098
        [Test]
        public void TestIssue1098()
        {
            m_Manager.CreateEntity(typeof(EcsTestData));

            using
            (
                var group = m_Manager.CreateEntityQuery
                (
                    new EntityQueryDesc
                    {
                        All = new ComponentType[] {typeof(EcsTestData)}
                    }
                )
            )
            // NB: EcsTestData != EcsTestData2
            Assert.Throws<InvalidOperationException>(() => group.ToComponentDataArray<EcsTestData2>(Allocator.TempJob));
        }

#if !UNITY_DOTSPLAYER

        [AlwaysUpdateSystem]
        public class WriteEcsTestDataSystem : JobComponentSystem
        {
            private struct WriteJob : IJobForEach<EcsTestData>
            {
                public void Execute(ref EcsTestData c0) {}
            }

            protected override JobHandle OnUpdate(JobHandle input)
            {
                var job = new WriteJob() {};
                return job.Schedule(this, input);
            }
        }

        [Test]
        public unsafe void CreateArchetypeChunkArray_SyncsChangeFilterTypes()
        {
            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            group.SetChangedVersionFilter(typeof(EcsTestData));
            var ws1 = World.GetOrCreateSystem<WriteEcsTestDataSystem>();
            ws1.Update();
            var safetyHandle = m_Manager.ComponentJobSafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<EcsTestData>(), false);

            Assert.Throws<InvalidOperationException>(() => AtomicSafetyHandle.CheckWriteAndThrow(safetyHandle));
            var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob);
            AtomicSafetyHandle.CheckWriteAndThrow(safetyHandle);

            chunks.Dispose();
            group.Dispose();
        }

        [Test]
        public unsafe void CalculateEntityCount_SyncsChangeFilterTypes()
        {
            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            group.SetChangedVersionFilter(typeof(EcsTestData));
            var ws1 = World.GetOrCreateSystem<WriteEcsTestDataSystem>();
            ws1.Update();
            var safetyHandle = m_Manager.ComponentJobSafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<EcsTestData>(), false);

            Assert.Throws<InvalidOperationException>(() => AtomicSafetyHandle.CheckWriteAndThrow(safetyHandle));
            group.CalculateEntityCount();
            AtomicSafetyHandle.CheckWriteAndThrow(safetyHandle);

            group.Dispose();
        }
#endif

        [Test]
        public void ToEntityArrayOnFilteredGroup()
        {
            // Note - test is setup so that each entity is in its own chunk, this checks that entity indices are correct
            var a = m_Manager.CreateEntity(typeof(EcsTestSharedComp), typeof(EcsTestData));
            var b = m_Manager.CreateEntity(typeof(EcsTestSharedComp), typeof(EcsTestData2));
            var c = m_Manager.CreateEntity(typeof(EcsTestSharedComp), typeof(EcsTestData3));

            m_Manager.SetSharedComponentData(a, new EcsTestSharedComp {value = 123});
            m_Manager.SetSharedComponentData(b, new EcsTestSharedComp {value = 456});
            m_Manager.SetSharedComponentData(c, new EcsTestSharedComp {value = 123});

            using (var group = m_Manager.CreateEntityQuery(typeof(EcsTestSharedComp)))
            {
                group.SetSharedComponentFilter(new EcsTestSharedComp {value = 123});
                using (var entities = group.ToEntityArray(Allocator.TempJob))
                {
                    CollectionAssert.AreEquivalent(new[] {a, c}, entities);
                }
            }

            using (var group = m_Manager.CreateEntityQuery(typeof(EcsTestSharedComp)))
            {
                group.SetSharedComponentFilter(new EcsTestSharedComp {value = 456});
                using (var entities = group.ToEntityArray(Allocator.TempJob))
                {
                    CollectionAssert.AreEquivalent(new[] {b}, entities);
                }
            }
        }
        
        [Test]
        public void CalculateEntityCount()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            var entityA = m_Manager.CreateEntity(archetype);
            var entityB = m_Manager.CreateEntity(archetype);
            
            m_Manager.SetSharedComponentData(entityA, new EcsTestSharedComp{value = 10});

            var query = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp));
            
            var entityCountBeforeFilter = query.CalculateChunkCount();
            
            query.SetSharedComponentFilter(new EcsTestSharedComp{value = 10});
            var entityCountAfterSetFilter = query.CalculateChunkCount();
            
            var entityCountUnfilteredAfterSetFilter = query.CalculateChunkCountWithoutFiltering();

            Assert.AreEqual(2, entityCountBeforeFilter);
            Assert.AreEqual(1, entityCountAfterSetFilter);
            Assert.AreEqual(2, entityCountUnfilteredAfterSetFilter);
        }
        
        [Test]
        public void CalculateChunkCount()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));
            var entityA = m_Manager.CreateEntity(archetype);
            var entityB = m_Manager.CreateEntity(archetype);
            
            m_Manager.SetSharedComponentData(entityA, new EcsTestSharedComp{value = 10});

            var query = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp));
            
            var chunkCountBeforeFilter = query.CalculateChunkCount();
            
            query.SetSharedComponentFilter(new EcsTestSharedComp{value = 10});
            var chunkCountAfterSetFilter = query.CalculateChunkCount();
            
            var chunkCountUnfilteredAfterSetFilter = query.CalculateChunkCountWithoutFiltering();

            Assert.AreEqual(2, chunkCountBeforeFilter);
            Assert.AreEqual(1, chunkCountAfterSetFilter);
            Assert.AreEqual(2, chunkCountUnfilteredAfterSetFilter);
        }

        private struct TestTag0 : IComponentData{}
        private struct TestTag1 : IComponentData{}
        private struct TestTag2 : IComponentData{}
        private struct TestTag3 : IComponentData{}
        private struct TestTag4 : IComponentData{}
        private struct TestTag5 : IComponentData{}
        private struct TestTag6 : IComponentData{}
        private struct TestTag7 : IComponentData{}
        private struct TestTag8 : IComponentData{}
        private struct TestTag9 : IComponentData{}
        private struct TestTag10 : IComponentData{}
        private struct TestTag11 : IComponentData{}
        private struct TestTag12 : IComponentData{}
        private struct TestTag13 : IComponentData{}
        private struct TestTag14 : IComponentData{}
        private struct TestTag15 : IComponentData{}
        private struct TestTag16 : IComponentData{}
        private struct TestTag17 : IComponentData{}

        private struct TestDefaultData : IComponentData
        {
            private int value;
        }
        
        private void MakeExtraQueries(int size)
        {
            var TagTypes = new Type[]
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
                typeof(TestTag17)
            };
            
            for (int i = 0; i < size; i++)
            {
                var typeCount = CollectionHelper.Log2Ceil(i);
                var typeList = new List<ComponentType>();
                for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    if ((i & (1 << typeIndex)) != 0)
                        typeList.Add(TagTypes[typeIndex]);
                }

                typeList.Add(typeof(TestDefaultData));

                var types = typeList.ToArray();
                var archetype = m_Manager.CreateArchetype(types);
                
                m_Manager.CreateEntity(archetype);
                var query = EmptySystem.GetEntityQuery(types);
                m_Manager.GetEntityQueryMask(query);
            }
        }

        [Test]
        public void GetEntityQueryMaskThrowsOnOverflow()
        {
            Assert.That(() => MakeExtraQueries(1200),
                Throws.Exception.With.Message.Matches("You have reached the limit of 1024 unique EntityQueryMasks, and cannot generate any more."));
        }

        [Test]
        public void GetEntityQueryMaskThrowsOnFilter()
        {
            var queryMatches = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            queryMatches.SetSharedComponentFilter(new EcsTestSharedComp(42));
            
            Assert.That(() => m_Manager.GetEntityQueryMask(queryMatches), 
                Throws.Exception.With.Message.Matches("GetEntityQueryMask can only be called on an EntityQuery without a filter applied to it."
                                                      + "  You can call EntityQuery.ResetFilter to remove the filters from an EntityQuery."));
        }

        [Test]
        public unsafe void GetEntityQueryMaskReturnsCachedMask()
        {
            var queryMatches = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            var queryMaskMatches = m_Manager.GetEntityQueryMask(queryMatches);
            
            var queryMaskMatches2 = m_Manager.GetEntityQueryMask(queryMatches);
            
            Assert.True(queryMaskMatches.Mask == queryMaskMatches2.Mask &&
                        queryMaskMatches.Index == queryMaskMatches2.Index &&
                        queryMaskMatches.EntityComponentStore == queryMaskMatches2.EntityComponentStore);
        }
        
        [Test]
        public void Matches()
        {
            var archetypeMatches = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            var archetypeDoesntMatch = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3), typeof(EcsTestSharedComp));
            
            var entity = m_Manager.CreateEntity(archetypeMatches);
            var entityOnlyNeededToPopulateArchetype = m_Manager.CreateEntity(archetypeDoesntMatch);
            
            var queryMatches = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            var queryDoesntMatch = EmptySystem.GetEntityQuery(typeof(EcsTestData), typeof(EcsTestData3), typeof(EcsTestSharedComp));

            var queryMaskMatches = m_Manager.GetEntityQueryMask(queryMatches);
            
            var queryMaskDoesntMatch = m_Manager.GetEntityQueryMask(queryDoesntMatch);
            
            Assert.True(queryMaskMatches.Matches(entity));
            Assert.False(queryMaskDoesntMatch.Matches(entity));
        }

        [Test]
        public void MatchesArchetypeAddedAfterMaskCreation()
        {
            var archetypeBefore = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            var query = EmptySystem.GetEntityQuery(typeof(EcsTestData));
            var queryMask = m_Manager.GetEntityQueryMask(query);

            var archetypeAfter = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData3));
            var entity = m_Manager.CreateEntity(archetypeAfter);
            
            Assert.True(queryMask.Matches(entity));
        }
    }
}
