using System;
using NUnit.Framework;
using Unity.Collections;

namespace Unity.Entities.Tests
{
    class MutableForEachTests : EntityQueryBuilderTestFixture
    {
        EntityQueryBuilder MutableEntities => TestSystem.Entities;

        struct ExtractTestDataFromEntityManager<T> : IDisposable
            where T: struct, IComponentData
        {
            EntityManager m_mgr;
            public NativeArray<T> Values;

            public ExtractTestDataFromEntityManager(EntityManager mgr)
            {
                m_mgr = mgr;

                using (var group = m_mgr.CreateEntityQuery(typeof(T)))
                {
                    Values = group.ToComponentDataArray<T>(Allocator.TempJob);
                }
            }

            public void Sort<U>(U comparer) where U: System.Collections.Generic.IComparer<T>
            {
                Values.Sort(comparer);
            }

            public void Dispose()
            {
                Values.Dispose();
            }
        }

        struct ExtractTestSharedDataFromEntityManager<T> : IDisposable
            where T: struct, ISharedComponentData
        {
            EntityManager m_mgr;
            public NativeArray<T> Values;

            public ExtractTestSharedDataFromEntityManager(EntityManager mgr)
            {
                m_mgr = mgr;
                int count = 0;
                using (var group = m_mgr.CreateEntityQuery(typeof(T)))
                {
                    Values = new NativeArray<T>(group.CalculateEntityCount(), Allocator.TempJob);
                    using(var chunks = group.CreateArchetypeChunkArray(Allocator.TempJob))
                        for (int i = 0; i < chunks.Length; ++i)
                        {
                            var chunk = chunks[i];
                            var shared = chunk.GetSharedComponentData(m_mgr.GetArchetypeChunkSharedComponentType<T>(), m_mgr);
                            Values[count++] = shared;
                        }
                    Assert.AreEqual(group.CalculateEntityCount(), count);
                }
            }

            public void Sort<U>(U comparer) where U: System.Collections.Generic.IComparer<T>
            {
                Values.Sort(comparer);
            }

            public void Dispose()
            {
                Values.Dispose();
            }
        }

        class EcsTestDataComparer : System.Collections.Generic.IComparer<EcsTestData>
        {
            public int Compare(EcsTestData lhs, EcsTestData rhs)
            {
                if (lhs.value < rhs.value) return -1;
                if (lhs.value > rhs.value) return +1;
                return 0;
            }
        }

        class EcsTestData2Comparer : System.Collections.Generic.IComparer<EcsTestData2>
        {
            public int Compare(EcsTestData2 lhs, EcsTestData2 rhs)
            {
                if (lhs.value0 < rhs.value0) return -1;
                if (lhs.value0 > rhs.value0) return +1;
                return 0;
            }
        }

        class EcsTestSharedDataComparer: System.Collections.Generic.IComparer<EcsTestSharedComp>
        {
            public int Compare(EcsTestSharedComp lhs, EcsTestSharedComp rhs)
            {
                if (lhs.value< rhs.value) return -1;
                if (lhs.value > rhs.value) return +1;
                return 0;
            }
        }

        [Test]
        public void AddComponentData_IsNotIteratedOver()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(1));
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(2));
            int count = 0;
            MutableEntities.ForEach((Entity e, ref EcsTestData d) =>
            {
                ++count;
                m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(12));
            });
            Assert.AreEqual(2, count);
        }

        [Test]
        public void DestroyEntity_EMHasTheRightNumberOfEntities()
        {
            const int kRepeat = 3*4; // Make a multiple of 3 for easy math

            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            m_Manager.SetComponentData(m_Manager.CreateEntity(archetype), new EcsTestData(12));
            for (int i = 0; i < kRepeat; i++)
                m_Manager.SetComponentData(m_Manager.CreateEntity(archetype), new EcsTestData(-i));
            MutableEntities.ForEach((Entity entity, ref EcsTestData ed) =>
            {
                if ((ed.value % 3) == 0)
                {
                    m_Manager.DestroyEntity(entity);
                }
            });

            using (var group = m_Manager.CreateEntityQuery(typeof(Entity), typeof(EcsTestData)))
            using (var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob))
            {
                Assert.AreEqual(kRepeat - kRepeat / 3, arr.Length);
            }
        }

        [Test]
        public void DestroyEntity_OfAForwardEntity_CanBeIterated()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            Entity e0 = m_Manager.CreateEntity(archetype);
            m_Manager.SetComponentData(e0, new EcsTestData(12));
            Entity eLast = Entity.Null;
            for (int i = 0; i < 10; i++)
            {
                eLast = m_Manager.CreateEntity(archetype);
                m_Manager.SetComponentData(eLast, new EcsTestData(-i));
            }
            m_Manager.AddComponentData(e0, new EcsTestDataEntity(-12, eLast));

            MutableEntities.ForEach((Entity entity, ref EcsTestData ed) =>
            {
                if (m_Manager.HasComponent<EcsTestDataEntity>(entity))
                {
                    Entity elast = m_Manager.GetComponentData<EcsTestDataEntity>(entity).value1;
                    m_Manager.DestroyEntity(elast);
                }
            });

            using(var group = m_Manager.CreateEntityQuery(typeof(Entity), typeof(EcsTestData)))
            using (var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob))
            {
                Assert.AreEqual(10, arr.Length);
            }
        }

        [Test]
        public void RemoveComponent_OfAForwardEntity_CanBeIterated()
        {
            var archetype = m_Manager.CreateArchetype(typeof(EcsTestData));
            Entity e0 = m_Manager.CreateEntity(archetype);
            m_Manager.SetComponentData(e0, new EcsTestData(12));
            Entity eLast = Entity.Null;
            for (int i = 0; i < 10; i++)
            {
                eLast = m_Manager.CreateEntity(archetype);
                m_Manager.SetComponentData(eLast, new EcsTestData(-i));
            }

            m_Manager.AddComponentData(e0, new EcsTestDataEntity(-12, eLast));

            MutableEntities.ForEach((Entity entity, ref EcsTestData ed) =>
            {
                if (m_Manager.HasComponent<EcsTestDataEntity>(entity))
                {
                    m_Manager.RemoveComponent<EcsTestDataEntity>(entity);
                }
            });

            using (var group = m_Manager.CreateEntityQuery(typeof(Entity), typeof(EcsTestData)))
            using (var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob))
            {
                Assert.AreEqual(10 + 1, arr.Length);
            }
        }

        [Test]
        public void Instantiate_HasGetComponent_VisibleFromInsideForEach()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(5));

            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                Assert.AreEqual(5, testData.value);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData2>(e));
                Entity newe1 = m_Manager.Instantiate(e);
                m_Manager.AddComponentData(e, new EcsTestData2() {value0 = 1, value1 = 3});
                {
                    EcsTestData2 ed2 = m_Manager.GetComponentData<EcsTestData2>(e);
                    Assert.AreEqual(3, ed2.value1);
                    Assert.AreEqual(1, ed2.value0);
                }

                Entity deferred = m_Manager.CreateEntity();
                m_Manager.AddComponentData(deferred, testData);
                {
                    var ed = m_Manager.GetComponentData<EcsTestData>(deferred);
                    Assert.AreEqual(testData.value, ed.value);
                }
                Entity newe2 = m_Manager.Instantiate(e);

                Assert.IsFalse(m_Manager.HasComponent<EcsTestData2>(newe1));
                {
                    EcsTestData ed = m_Manager.GetComponentData<EcsTestData>(newe1);
                    Assert.AreEqual(5, ed.value);
                }
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData2>(e));
                {
                    EcsTestData2 ed2 = m_Manager.GetComponentData<EcsTestData2>(newe2);
                    Assert.AreEqual(3, ed2.value1);
                    Assert.AreEqual(1, ed2.value0);
                }
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(newe1));
                m_Manager.RemoveComponent<EcsTestData>(newe1);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(newe1));
            });

            using(var allEntities = m_Manager.GetAllEntities())
                Assert.AreEqual(4, allEntities.Length);

            using(var group = new ExtractTestDataFromEntityManager<EcsTestData>(m_Manager))
            {
                Assert.AreEqual(3, group.Values.Length);
                Assert.AreEqual(5, group.Values[0].value); // e
                Assert.AreEqual(5, group.Values[1].value); // deferred
                Assert.AreEqual(5, group.Values[2].value); // newe2
            }

            using(var group = new ExtractTestDataFromEntityManager<EcsTestData2>(m_Manager))
            {
                Assert.AreEqual(2, group.Values.Length); // (e && newe2)
                Assert.AreEqual(3, group.Values[0].value1);
                Assert.AreEqual(1, group.Values[0].value0);
                Assert.AreEqual(3, group.Values[1].value1);
                Assert.AreEqual(1, group.Values[1].value0);
            }
        }

        [Test]
        public void Instiate_BasicOperations_VisibleFromInsideForEach()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData() {value = 3});
            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                Entity e0 = m_Manager.Instantiate(e);
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(e));
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(e0));
                Entity e1 = m_Manager.Instantiate(e0);
                m_Manager.SetComponentData(e1, new EcsTestData() {value = 12});
                Entity e2 = m_Manager.Instantiate(e1);
                Assert.AreEqual(3, m_Manager.GetComponentData<EcsTestData>(e).value);
                Assert.AreEqual(3, m_Manager.GetComponentData<EcsTestData>(e0).value);
                Assert.AreEqual(12, m_Manager.GetComponentData<EcsTestData>(e1).value);
                Assert.AreEqual(12, m_Manager.GetComponentData<EcsTestData>(e2).value);
            });
        }

        [Test]
        [StandaloneFixme] // ISharedComponentData
        public void SharedComponent_ModifiedEntities_VisibleFromInsideForEach()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestData(10));

            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                m_Manager.AddSharedComponentData(e, new EcsTestSharedComp(10));
                m_Manager.SetSharedComponentData(e, new EcsTestSharedComp(20));

                Assert.AreEqual(20, m_Manager.GetSharedComponentData<EcsTestSharedComp>(e).value);
                Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedComp>(e));
            });
            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                Assert.AreEqual(20, m_Manager.GetSharedComponentData<EcsTestSharedComp>(e).value);
            });

            Assert.AreEqual(20, m_Manager.GetSharedComponentData<EcsTestSharedComp>(entity).value);
        }

        [Test]
        public void Buffer_ModifiedEntities_VisibleFromInsideForEach()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestData(10));

            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                {
                    var buffer = m_Manager.AddBuffer<EcsIntElement>(e);
                    for (int i = 0; i < 189; ++i)
                        buffer.Add(i);
                }
                {
                    var buffer = m_Manager.GetBuffer<EcsIntElement>(e);
                    for (int i = 0; i < 189; ++i)
                    {
                        Assert.AreEqual(i, buffer[i].Value);
                        buffer[i] = i * 2;
                    }
                }
                {
                    var buffer = m_Manager.GetBuffer<EcsIntElement>(e);
                    for (int i = 0; i < 189; ++i)
                        Assert.AreEqual(i * 2, buffer[i].Value);
                }

            });
            var finalbuffer = m_Manager.GetBuffer<EcsIntElement>(entity);
            for (int i = 0; i < 189; ++i)
                Assert.AreEqual(i * 2, finalbuffer[i].Value);
        }

        [Test]
        public void DestroyEntity_EntityOperations_ShouldThrowWhenRequired()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestData(10));

            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                m_Manager.DestroyEntity(e);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(e));
                Assert.Throws<ArgumentException>(() => m_Manager.AddComponentData(e, new EcsTestData2(22)));
                Assert.Throws<ArgumentException>(() => m_Manager.Instantiate(e));
                Assert.Throws<ArgumentException>(() => m_Manager.SetComponentData(e, new EcsTestData(1)));
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(e));
                Assert.Throws<ArgumentException>(() =>
                    m_Manager.AddSharedComponentData(e, new EcsTestSharedComp(1)));
                Assert.Throws<ArgumentException>(() => m_Manager.GetSharedComponentData<EcsTestSharedComp>(e));
                Assert.Throws<ArgumentException>(() =>
                    m_Manager.SetSharedComponentData(e, new EcsTestSharedComp(1)));
                Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedComp>(e));

                Assert.Throws<ArgumentException>(() => m_Manager.AddBuffer<EcsIntElement>(e));
                Assert.IsFalse(m_Manager.HasComponent<EcsIntElement>(e));
                Assert.Throws<ArgumentException>(() => m_Manager.Instantiate(e));
                m_Manager.RemoveComponent<EcsIntElement>(e);
                m_Manager.RemoveComponent<EcsTestSharedComp>(e);
                m_Manager.RemoveComponent<EcsIntElement>(e);
                Assert.IsFalse(m_Manager.Exists(e));

            });
        }

        [Test]
        [StandaloneFixme] // ISharedComponentData
        public void RemoveSharedComponent_ModifiedEntity_VisibleFromInsideForEach()
        {
            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new EcsTestSharedComp(5));
            MutableEntities.ForEach((Entity e, EcsTestSharedComp testData) =>
            {
                Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedComp>(e));
                m_Manager.RemoveComponent<EcsTestSharedComp>(e);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedComp>(e));
            });
            using (var group = new ExtractTestSharedDataFromEntityManager<EcsTestSharedComp>(m_Manager))
            {
                Assert.AreEqual(0, group.Values.Length);
            }
        }

        [Test]
        [StandaloneFixme] // ISharedComponentData
        public void RemoveComponent_ModifiedEntity_VisibleFromInsideForEach()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(5));
            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(e));
                m_Manager.RemoveComponent<EcsTestData>(e);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(e));
                m_Manager.AddComponentData(e, new EcsTestData(123));
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(e));
                {
                    EcsTestData d = m_Manager.GetComponentData<EcsTestData>(e);
                    Assert.AreEqual(123, d.value);
                    testData.value = 123;
                }

                m_Manager.AddSharedComponentData(e, new EcsTestSharedComp(22));
                Assert.IsTrue(m_Manager.HasComponent<EcsTestSharedComp>(e));
                m_Manager.RemoveComponent<EcsTestSharedComp>(e);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestSharedComp>(e));

                Entity c = m_Manager.CreateEntity();
                m_Manager.AddComponentData(c, new EcsTestData(-22));
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(c));
                m_Manager.RemoveComponent<EcsTestData>(c);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(c));
                m_Manager.AddComponentData(c, new EcsTestData(-123));

                m_Manager.RemoveComponent<EcsTestData>(c);
            });

            using(var group = m_Manager.CreateEntityQuery(typeof(EcsTestData)))
            using (var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob))
            {
                Assert.AreEqual(1, arr.Length); // (e)
                Assert.AreEqual(123, arr[0].value);
            }

        }

        [Test]
        [StandaloneFixme] // ISharedComponentData
        public void RemoveComponent_GetOrSetOfRemovedComponent_Throws()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(5));
            MutableEntities.ForEach((Entity e, ref EcsTestData testData) =>
            {
                Assert.IsTrue(m_Manager.HasComponent<EcsTestData>(e));
                m_Manager.RemoveComponent<EcsTestData>(e);
                Assert.IsFalse(m_Manager.HasComponent<EcsTestData>(e));
                Assert.Throws<ArgumentException>(() => m_Manager.GetComponentData<EcsTestData>(e));
                Assert.Throws<ArgumentException>(() => m_Manager.SetComponentData(e, new EcsTestData(12)));

                m_Manager.AddSharedComponentData(e, new EcsTestSharedComp(22));
                m_Manager.RemoveComponent<EcsTestSharedComp>(e);
                Assert.Throws<ArgumentException>(() => m_Manager.GetSharedComponentData<EcsTestSharedComp>(e));
                Assert.Throws<ArgumentException>(() =>
                    m_Manager.SetSharedComponentData(e, new EcsTestSharedComp(-22)));
            });
        }

        [Test]
        public void NestedForEach_ArchetypeCreatedInsideForEach_CanExecuteNestedForEach()
        {
            Entity e = m_Manager.CreateEntity();
            m_Manager.AddComponentData(e, new EcsTestData(5));
            m_Manager.AddComponentData(e, new EcsTestData2(12));

            MutableEntities.ForEach((Entity e1, ref EcsTestData testData) =>
            {
                if (m_Manager.HasComponent<EcsTestData2>(e1))
                    MutableEntities.ForEach((Entity e2, ref EcsTestData ed, ref EcsTestData2 ed2) =>
                    {
                        m_Manager.SetComponentData(m_Manager.Instantiate(e2),
                            new EcsTestData2(-ed.value));
                    });
            });

            using (var group = m_Manager.CreateEntityQuery(typeof(EcsTestData), typeof(EcsTestData2)))
            {
                using (var arr = group.ToComponentDataArray<EcsTestData>(Allocator.TempJob))
                {
                    Assert.AreEqual(2, arr.Length); // (e)
                    Assert.AreEqual(5, arr[0].value);
                    Assert.AreEqual(5, arr[1].value);
                }
            }

            using(var group = new ExtractTestDataFromEntityManager<EcsTestData2>(m_Manager))
            {
                Assert.AreEqual(2, group.Values.Length);
                group.Sort(new EcsTestData2Comparer());

                Assert.AreEqual(-5, group.Values[0].value0);
                Assert.AreEqual(12, group.Values[1].value0);
            }
        }

        [Test]
        public void RemoveComponent_WhenArchetypeModifiedInsideForEach_CanModifySafely()
        {
            var arch = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            using (var ents = new NativeArray<Entity>(10, Allocator.Persistent))
            {
                m_Manager.CreateEntity(arch, ents);
                for (int i = 0; i < 10; i++)
                {
                    m_Manager.SetComponentData(ents[i], new EcsTestData(i + 1));
                }

                MutableEntities.ForEach((Entity entity, ref EcsTestData c0, ref EcsTestData2 c1) =>
                {
                    m_Manager.RemoveComponent<EcsTestData>(entity);
                    c1.value0 = -c0.value;
                });

                using (var group = new ExtractTestDataFromEntityManager<EcsTestData>(m_Manager))
                {
                    Assert.AreEqual(0, group.Values.Length);
                }

                using (var group = new ExtractTestDataFromEntityManager<EcsTestData2>(m_Manager))
                {
                    group.Sort(new EcsTestData2Comparer());
                    Assert.AreEqual(10, group.Values.Length);
                    for (int i = 0; i < 10; i++)
                        Assert.AreEqual(-10 +i, group.Values[i].value0);
                }
            }
        }

        [Test]
        public void SetComponentData_WhenBothSetAndRefAreModified_RefWins()
        {
            var archA = m_Manager.CreateArchetype(typeof(EcsTestData), typeof(EcsTestData2));
            using (var ents = new NativeArray<Entity>(4, Allocator.Persistent))
            {
                m_Manager.CreateEntity(archA, ents);
                int count = 0;
                MutableEntities.ForEach((Entity entity, ref EcsTestData ca, ref EcsTestData2 ca2) =>
                {
                    switch (count++)
                    {
                        case 0:
                            m_Manager.SetComponentData(entity, new EcsTestData2(count * 100));
                            break;
                        case 1:
                            m_Manager.SetComponentData(entity, new EcsTestData2(count * 100));
                            ca2.value0 = 1;
                            break;
                        case 2:
                            m_Manager.SetComponentData(entity, new EcsTestData2(count * 100));
                            ca2.value0 = -count * 100;
                            break;
                        case 3:
                            ca2.value0 = -count * 100;
                            break;
                    }
                });
                Assert.AreEqual(ents.Length, count);
                using (var group = new ExtractTestDataFromEntityManager<EcsTestData2>(m_Manager))
                {
                    Assert.AreEqual(4, group.Values.Length);
                    Assert.AreEqual(100,  group.Values[0].value0);    // case 0
                    Assert.AreEqual(1,    group.Values[1].value0);    // case 1
                    Assert.AreEqual(-300, group.Values[2].value0);    // case 2
                    Assert.AreEqual(-400, group.Values[3].value0);    // case 3
                }
            }
        }
    }
}
