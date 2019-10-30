using System;
using NUnit.Framework;
using Unity.Collections;
using System.Collections.Generic;

namespace Unity.Entities.Tests
{
	class IterationTests : ECSTestsFixture
	{
		[Test]
		public void CreateEntityQuery()
		{
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2));
            Assert.AreEqual(0, group.CalculateEntityCount());

            var entity = m_Manager.CreateEntity(archetype);
            m_Manager.SetComponentData(entity, new EcsTestData(42));
            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(42, arr[0].value);

            arr.Dispose();
            m_Manager.DestroyEntity(entity);
        }

        struct TempComponentNeverInstantiated : IComponentData
	    {
	        private int m_Internal;
	    }
	    
		[Test]
		public void IterateEmptyArchetype()
		{
			var group = m_Manager.CreateEntityQuery(typeof(TempComponentNeverInstantiated));
			Assert.AreEqual(0, group.CalculateEntityCount());

			var archetype = m_Manager.CreateArchetype(typeof(TempComponentNeverInstantiated));
			Assert.AreEqual(0, group.CalculateEntityCount());

			Entity ent = m_Manager.CreateEntity(archetype);
			Assert.AreEqual(1, group.CalculateEntityCount());
			m_Manager.DestroyEntity(ent);
			Assert.AreEqual(0, group.CalculateEntityCount());
		}

        [Test]
		public void IterateChunkedEntityQuery()
		{
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(entities.Length, arr.Length);
            HashSet<int> values = new HashSet<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);
            }

            arr.Dispose();
            for (int i = 0; i < entities.Length; i++)
                m_Manager.DestroyEntity(entities[i]);
        }

        [Test]
		public void IterateChunkedEntityQueryBackwards()
		{
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(entities.Length, arr.Length);
            HashSet<int> values = new HashSet<int>();
            for (int i = 0; i < arr.Length; ++i)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);
            }

            arr.Dispose();
            for (int i = 0; i < entities.Length; i++)
                m_Manager.DestroyEntity(entities[i]);
        }

        [Test]
		public void IterateChunkedEntityQueryAfterDestroy()
		{
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
            }
            for (int i = 0; i < entities.Length; i++)
            {
                if (i % 2 != 0)
                {
                    m_Manager.DestroyEntity(entities[i]);
                }
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(entities.Length / 2, arr.Length);
            HashSet<int> values = new HashSet<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val % 2 == 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);
            }

            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                if (i % 2 == 0)
                    m_Manager.RemoveComponent<EcsTestData>(entities[i]);
            }
            arr.Dispose();
            arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(entities.Length / 4, arr.Length);
            values = new HashSet<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val % 2 == 0);
                Assert.IsTrue(val < entities.Length / 2);
                values.Add(i);
            }

            for (int i = 0; i < entities.Length; i++)
            {
                if (i % 2 == 0)
                    m_Manager.DestroyEntity(entities[i]);
            }
            arr.Dispose();
        }

        [Test]
        public void GroupCopyFromNativeArray()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            var entities = new NativeArray<Entity>(10, Allocator.Persistent);
            m_Manager.CreateEntity(archetype, entities);

            var dataToCopyA = new NativeArray<EcsTestData>(10, Allocator.Persistent);
            var dataToCopyB = new NativeArray<EcsTestData>(5, Allocator.Persistent);

            for (int i = 0; i < dataToCopyA.Length; ++i)
            {
                dataToCopyA[i] = new EcsTestData { value = 2 };
            }

            for (int i = 0; i < dataToCopyB.Length; ++i)
            {
                dataToCopyA[i] = new EcsTestData { value = 3 };

            }

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            group.CopyFromComponentDataArray(dataToCopyA);

            for (int i = 0; i < dataToCopyA.Length; ++i)
            {
                Assert.AreEqual(m_Manager.GetComponentData<EcsTestData>(entities[i]).value, dataToCopyA[i].value);
            }

            Assert.Throws<ArgumentException>(() => { group.CopyFromComponentDataArray(dataToCopyB); });

            group.Dispose();
            entities.Dispose();
            dataToCopyA.Dispose();
            dataToCopyB.Dispose();
        }

        [Test]
        [StandaloneFixme]
        public void EntityQueryFilteredEntityIndexWithMultipleArchetypes()
        {
            var archetypeA = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));
            var archetypeB = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestSharedComp));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp));

            var entity1A = m_Manager.CreateEntity(archetypeA);
            var entity2A = m_Manager.CreateEntity(archetypeA);
            var entityB  = m_Manager.CreateEntity(archetypeB);

            m_Manager.SetSharedComponentData(entity1A, new EcsTestSharedComp{ value = 1});
            m_Manager.SetSharedComponentData(entity2A, new EcsTestSharedComp{ value = 2});

            m_Manager.SetSharedComponentData(entityB, new EcsTestSharedComp{ value = 1});

            group.SetSharedComponentFilter(new EcsTestSharedComp{value = 1});

            var iterator = group.GetArchetypeChunkIterator();
            iterator.MoveNext();
            iterator.MoveNext();
            var begin = iterator.CurrentChunkFirstEntityIndex;

            Assert.AreEqual(1, begin); // 1 is index of entity in filtered EntityQuery
            Assert.AreEqual(archetypeB, iterator.CurrentArchetypeChunk.Archetype);

            group.Dispose();
        }
        
        [Test]
        [StandaloneFixme]
        public void EntityQueryFilteredChunkCount()
        {
            var archetypeA = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestSharedComp));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestSharedComp));

            for (int i = 0; i < archetypeA.ChunkCapacity * 2; ++i)
            {
                var entityA = m_Manager.CreateEntity(archetypeA);
                m_Manager.SetSharedComponentData(entityA, new EcsTestSharedComp{ value = 1});
            }

            var entityB  = m_Manager.CreateEntity(archetypeA);
            m_Manager.SetSharedComponentData(entityB, new EcsTestSharedComp{ value = 2});
            
            group.SetSharedComponentFilter(new EcsTestSharedComp{value = 1});

            {
                var iterator = group.GetArchetypeChunkIterator();
                iterator.MoveNext();
                var begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(0, begin);

                iterator.MoveNext();
                begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(archetypeA.ChunkCapacity, begin);

                iterator.MoveNext();
                Assert.Throws<InvalidOperationException>(() => { begin = iterator.CurrentChunkFirstEntityIndex; });

            }

            group.SetSharedComponentFilter(new EcsTestSharedComp{value = 2});
            {
                var iterator = group.GetArchetypeChunkIterator();
                iterator.MoveNext();
                var begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(0, begin);

                iterator.MoveNext();
                Assert.Throws<InvalidOperationException>(() => { begin = iterator.CurrentChunkFirstEntityIndex; });
            }

            group.Dispose();
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        [Test]
        public void CreateEntityQuery_ManagedComponents()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent));
            Assert.AreEqual(0, group.CalculateEntityCount());

            var entity = m_Manager.CreateEntity(archetype);

            m_Manager.SetComponentData(entity, new EcsTestData(42));
            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            Assert.AreEqual(1, arr.Length);
            Assert.AreEqual(42, arr[0].value);

            m_Manager.SetComponentData(entity, new EcsTestManagedComponent() { value = "SomeString" });
            var classArr = group.ToComponentDataArray<EcsTestManagedComponent>();
            Assert.AreEqual(1, classArr.Length);
            Assert.AreEqual("SomeString", classArr[0].value);

            arr.Dispose();
            m_Manager.DestroyEntity(entity);
        }

        [Test]
        public void IterateChunkedEntityQuery_ManagedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            var classArr = group.ToComponentDataArray<EcsTestManagedComponent>();
            Assert.AreEqual(entities.Length, arr.Length);
            Assert.AreEqual(entities.Length, classArr.Length);
            HashSet<int> values = new HashSet<int>();
            HashSet<string> classValues = new HashSet<string>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);

                string classVal = classArr[i].value;
                Assert.IsFalse(classValues.Contains(i.ToString()));
                Assert.IsTrue(classVal != null);
                Assert.IsTrue(classVal != new EcsTestManagedComponent().value);
                classValues.Add(i.ToString());
            }

            arr.Dispose();
            for (int i = 0; i < entities.Length; i++)
                m_Manager.DestroyEntity(entities[i]);
        }

        [Test]
        public void IterateChunkedEntityQueryBackwards_ManagedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            var classArr = group.ToComponentDataArray<EcsTestManagedComponent>();
            Assert.AreEqual(entities.Length, arr.Length);
            Assert.AreEqual(entities.Length, classArr.Length);
            HashSet<int> values = new HashSet<int>();
            HashSet<string> classValues = new HashSet<string>();
            for (int i = 0; i < arr.Length; ++i)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);

                string classVal = classArr[i].value;
                Assert.IsFalse(classValues.Contains(i.ToString()));
                Assert.IsTrue(classVal != null);
                Assert.IsTrue(classVal != new EcsTestManagedComponent().value);
                classValues.Add(i.ToString());
            }

            arr.Dispose();
            for (int i = 0; i < entities.Length; i++)
                m_Manager.DestroyEntity(entities[i]);
        }

        [Test]
        public void IterateChunkedEntityQueryAfterDestroy_ManagedComponents()
        {
            var archetype1 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            var archetype2 = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestManagedComponent));
            Assert.AreEqual(0, group.CalculateEntityCount());

            Entity[] entities = new Entity[10000];
            for (int i = 0; i < entities.Length / 2; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype1);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }
            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                entities[i] = m_Manager.CreateEntity(archetype2);
                m_Manager.SetComponentData(entities[i], new EcsTestData(i));
                m_Manager.SetComponentData(entities[i], new EcsTestManagedComponent() { value = i.ToString() });
            }
            for (int i = 0; i < entities.Length; i++)
            {
                if (i % 2 != 0)
                {
                    m_Manager.DestroyEntity(entities[i]);
                }
            }

            var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            var classArr = group.ToComponentDataArray<EcsTestManagedComponent>();
            Assert.AreEqual(entities.Length / 2, arr.Length);
            Assert.AreEqual(entities.Length / 2, classArr.Length);

            HashSet<int> values = new HashSet<int>();
            HashSet<string> classValues = new HashSet<string>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val % 2 == 0);
                Assert.IsTrue(val < entities.Length);
                values.Add(i);

                string classVal = classArr[i].value;
                Assert.IsFalse(classValues.Contains(i.ToString()));
                Assert.IsTrue(classVal != null);
                Assert.IsTrue(classVal != new EcsTestManagedComponent().value);
                classValues.Add(i.ToString());
            }

            for (int i = entities.Length / 2; i < entities.Length; i++)
            {
                if (i % 2 == 0)
                    m_Manager.RemoveComponent<EcsTestData>(entities[i]);
            }
            arr.Dispose();
            arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            classArr = group.ToComponentDataArray<EcsTestManagedComponent>();
            Assert.AreEqual(entities.Length / 4, arr.Length);
            Assert.AreEqual(entities.Length / 4, classArr.Length);
            values = new HashSet<int>();
            classValues = new HashSet<string>();
            for (int i = 0; i < arr.Length; i++)
            {
                int val = arr[i].value;
                Assert.IsFalse(values.Contains(i));
                Assert.IsTrue(val >= 0);
                Assert.IsTrue(val % 2 == 0);
                Assert.IsTrue(val < entities.Length / 2);
                values.Add(i);

                string classVal = classArr[i].value;
                Assert.IsFalse(classValues.Contains(i.ToString()));
                Assert.IsTrue(classVal != null);
                Assert.IsTrue(classVal != new EcsTestManagedComponent().value);
                classValues.Add(i.ToString());
            }

            for (int i = 0; i < entities.Length; i++)
            {
                if (i % 2 == 0)
                    m_Manager.DestroyEntity(entities[i]);
            }
            arr.Dispose();
        }

        [Test]
        [StandaloneFixme]
        public void EntityQueryFilteredChunkCount_ManagedComponents()
        {
            var archetypeA = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2), typeof(EcsTestManagedComponent), typeof(EcsTestSharedComp));

            var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestManagedComponent), typeof(EcsTestSharedComp));

            for (int i = 0; i < archetypeA.ChunkCapacity * 2; ++i)
            {
                var entityA = m_Manager.CreateEntity(archetypeA);
                m_Manager.SetSharedComponentData(entityA, new EcsTestSharedComp { value = 1 });
            }

            var entityB = m_Manager.CreateEntity(archetypeA);
            m_Manager.SetSharedComponentData(entityB, new EcsTestSharedComp { value = 2 });

            group.SetSharedComponentFilter(new EcsTestSharedComp{value = 1});
            {
                var iterator = group.GetArchetypeChunkIterator();
                iterator.MoveNext();
                var begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(0, begin);

                iterator.MoveNext();
                begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(archetypeA.ChunkCapacity, begin);

                iterator.MoveNext();
                Assert.Throws<InvalidOperationException>(() => { begin = iterator.CurrentChunkFirstEntityIndex; });

            }

            group.SetSharedComponentFilter(new EcsTestSharedComp{value = 2});
            {
                var iterator = group.GetArchetypeChunkIterator();
                iterator.MoveNext();
                var begin = iterator.CurrentChunkFirstEntityIndex;
                Assert.AreEqual(0, begin);

                iterator.MoveNext();
                Assert.Throws<InvalidOperationException>(() => { begin = iterator.CurrentChunkFirstEntityIndex; });
            }

            group.Dispose();
        }
#endif
    }
}
