using System;
using NUnit.Framework;

namespace Unity.Entities.Tests.ForEach
{
    class ForEachBasicTests : EntityQueryBuilderTestFixture
    {
        [SetUp]
        public void CreateTestEntities()
        {
            m_Manager.AddComponentData(m_Manager.CreateEntity(), new EcsTestData(5));
            m_Manager.CreateEntity(typeof(EcsTestTag));
            m_Manager.AddSharedComponentData(m_Manager.CreateEntity(), new SharedData1(7));
            m_Manager.CreateEntity(typeof(EcsIntElement));
        }

        [Test, Repeat(2)]
        public void Ensure_CacheClearedBetweenTests() // sanity check
        {
            Assert.IsNull(TestSystem.EntityQueryCache);

            var counter = 0;
            TestSystem.Entities.ForEach(e => ++counter);
            Assert.AreEqual(4, counter);
        }

        [Test]
        public void Ensure_EachElementOfEntityQueryBuilder_UsedInHashing()
        {
            // re-run should not change hash
            for (var i = 0; i < 2; ++i)
            {
                // variance in 'all' should change hash
                TestSystem.Entities.WithAll<EcsTestData>().ForEach(e => { });
                TestSystem.Entities.WithAll<EcsTestData2>().ForEach(e => { });

                // variance in 'any' should change hash
                TestSystem.Entities.WithAny<EcsTestData>().ForEach(e => { });
                TestSystem.Entities.WithAny<EcsTestData2>().ForEach(e => { });

                // variance in 'none' should change hash
                TestSystem.Entities.WithNone<EcsTestData>().ForEach(e => { });
                TestSystem.Entities.WithNone<EcsTestData2>().ForEach(e => { });

                // variance in delegate params should change hash
                TestSystem.Entities.WithNone<EcsTestData>().ForEach((ref EcsTestData2 d) => { });
                TestSystem.Entities.WithNone<EcsTestData>().ForEach((ref EcsTestData3 d) => { });

                Assert.AreEqual(8, TestSystem.EntityQueryCache.CalcUsedCacheCount());
            }
        }

        [Test]
        public void All()
        {
            var counter = 0;
            TestSystem.Entities.ForEach(entity =>
            {
                Assert.IsTrue(m_Manager.Exists(entity));
                counter++;
            });
            Assert.AreEqual(4, counter);
        }

        [Test]
        public void ComponentData()
        {
            {
                var counter = 0;
                TestSystem.Entities.ForEach((ref EcsTestData testData) =>
                {
                    Assert.AreEqual(5, testData.value);
                    testData.value++;
                    counter++;
                });
                Assert.AreEqual(1, counter);
            }

            {
                var counter = 0;
                TestSystem.Entities.ForEach((Entity entity, ref EcsTestData testData) =>
                {
                    Assert.AreEqual(6, testData.value);
                    testData.value++;

                    // ForEach currently modifies a copy of EcsTestData to provide for safe mutation
                    Assert.AreEqual(6, m_Manager.GetComponentData<EcsTestData>(entity).value);

                    counter++;
                });
                Assert.AreEqual(1, counter);
            }
        }

        [Test]
        public void SharedComponentData()
        {
            var counter = 0;
            TestSystem.Entities.ForEach((SharedData1 testData) =>
            {
                Assert.AreEqual(7, testData.value);
                counter++;
            });
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void DynamicBuffer()
        {
            var counter = 0;
            TestSystem.Entities.ForEach((DynamicBuffer<EcsIntElement> testData) =>
            {
                testData.Add(0);
                testData.Add(1);
                counter++;
            });
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void EmptyComponentData() // "tag"
        {
            var counter = 0;
            TestSystem.Entities.WithAll<EcsTestTag>().ForEach(entity => ++counter);
            Assert.AreEqual(1, counter);
        }
    }

    class ForEachGenericTests : EntityQueryBuilderTestFixture
    {
        // $ currently instantiations of generic types are not discovered by static type registry tooling

#if !UNITY_DOTSPLAYER
        [Test]
        public void GenericComponentData()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestGeneric<int> { value = 10 });
            m_Manager.AddComponentData(entity, new EcsTestData { value = 11 });

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestGeneric<int>>().ForEach(e => ++counter);
                Assert.AreEqual(1, counter);
            }

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestGeneric<float>>().ForEach(e => ++counter);
                Assert.AreEqual(0, counter);
            }

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestData>().ForEach((ref EcsTestGeneric<int> data) =>
                {
                    Assert.AreEqual(10, data.value);
                    ++counter;
                });
                Assert.AreEqual(1, counter);
            }

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestGeneric<int>>().ForEach((ref EcsTestData data) =>
                {
                    Assert.AreEqual(11, data.value);
                    ++counter;
                });
                Assert.AreEqual(1, counter);
            }
        }
#endif // !NET_DOTS

#if !UNITY_DOTSPLAYER
        [Test]
        public void GenericTag()
        {
            m_Manager.CreateEntity(typeof(EcsTestGenericTag<int>));

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestGenericTag<int>>().ForEach(e => ++counter);
                Assert.AreEqual(1, counter);
            }

            {
                var counter = 0;
                TestSystem.Entities.WithAll<EcsTestGenericTag<float>>().ForEach(e => ++counter);
                Assert.AreEqual(0, counter);
            }
        }
#endif // !NET_DOTS

#if !UNITY_DOTSPLAYER
        [Test]
        public void GenericValueMethod()
        {
            void Func<T>(T value) where T : struct
            {
                var entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestGeneric<T> { value = value });

                {
                    var counter = 0;
                    TestSystem.Entities.WithAll<EcsTestGeneric<T>>().ForEach(e => ++counter);
                    Assert.AreEqual(1, counter);
                }

                {
                    var counter = 0;
                    TestSystem.Entities.ForEach((ref EcsTestGeneric<T> data) =>
                    {
                        Assert.AreEqual(value, data.value);
                        ++counter;
                    });
                    Assert.AreEqual(1, counter);
                }
            }

            Func(10);
        }
#endif // !NET_DOTS

        [Test]
        public void GenericTagMethod()
        {
            void Func<T>()
                where T : struct, IComponentData
            {
                var counter = 0;
                TestSystem.Entities.WithAll<T>().ForEach(e => ++counter);
                Assert.AreEqual(1, counter);
            }

            m_Manager.CreateEntity(typeof(EcsTestTag));
            Func<EcsTestTag>();
        }
    }

    class ForEachTests : EntityQueryBuilderTestFixture
    {
        [Test]
        public void Many()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestData(0));
            m_Manager.AddComponentData(entity, new EcsTestData2(1));
            m_Manager.AddComponentData(entity, new EcsTestData3(2));
            m_Manager.AddComponentData(entity, new EcsTestData4(3));
            m_Manager.AddComponentData(entity, new EcsTestData5(4));

            var counter = 0;
            TestSystem.Entities.ForEach((Entity e, ref EcsTestData t0, ref EcsTestData2 t1, ref EcsTestData3 t2, ref EcsTestData4 t3, ref EcsTestData5 t4) =>
            {
                Assert.AreEqual(entity, e);
                Assert.AreEqual(0, t0.value);
                Assert.AreEqual(1, t1.value0);
                Assert.AreEqual(2, t2.value0);
                Assert.AreEqual(3, t3.value0);
                Assert.AreEqual(4, t4.value0);
                counter++;
            });
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void MixingNoneAndAll_Throws()
        {
            Assert.Throws<EntityQueryDescValidationException>(() =>
            {
                TestSystem.Entities.WithNone<EcsTestData2>().ForEach((Entity e, ref EcsTestData2 t1) =>
                {
                    Assert.Fail();
                });
            });
        }

        [Test]
        public void UsingAnyInForEach_Matches()
        {
            m_Manager.CreateEntity(typeof(EcsTestData));
            m_Manager.CreateEntity(typeof(EcsTestData2));
            m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));

            var counter = 0;
            TestSystem.Entities
                .WithAny<EcsTestData, EcsTestData2>()
                .ForEach(e => ++counter);
            Assert.AreEqual(counter, 3);

        }

        [Test]
        public void MixingAnyWithForEachDelegateParams_Throws()
        {
            Assert.Throws<EntityQueryDescValidationException>(() =>
            {
                TestSystem.Entities.WithAny<EcsTestData>().ForEach((ref EcsTestData t1) =>
                {
                    Assert.Fail();
                });
            });
        }

        [Test]
        public void AllowMutationInForEach()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestData(0));

            var counter = 0;
            TestSystem.Entities.ForEach((Entity e, ref EcsTestData t0) =>
            {
                Assert.DoesNotThrow(() => m_Manager.CreateEntity());
                Assert.DoesNotThrow(() => m_Manager.AddComponent(e, typeof(EcsTestData2)));
                Assert.DoesNotThrow(() => m_Manager.RemoveComponent<EcsTestData2>(e));
                Assert.DoesNotThrow(() => m_Manager.DestroyEntity(e));
                
                counter++;
            });
            Assert.AreEqual(1, counter);
            
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            Assert.IsFalse(m_Manager.IsInsideForEach);
            #endif
        }

        [Test]
        public void ForEachOnDifferentAmount([Values(16, 1024, 2048, 4096)]int entityCount)
        {
            for (int i = 0; i < entityCount; i++)
            {
                var entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData(entity.Index));
            }

            var count = 0;
            TestSystem.Entities.ForEach((Entity e, ref EcsTestData t0) =>
            {
                Assert.AreEqual(count, t0.value);
                count++;
            });
            
            Assert.AreEqual(entityCount, count);
        }
        
        [Test]
        // The goal here is to generate many nested call that contains different results
        // So The first level is defining the types to query based on its Entity.Index
        //  then we nest calls by mixing A, B, A, B types
        public void NestedForEach()
        {
            var maxRecursionCall = 10_000;
            var curRecursionCounter = 0;
            
            // Create L0 entities
            var l0Count = 16;
            for (int i = 0; i < l0Count; i++)
            {
                var entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData(entity.Index));
            }

            // Create other nested levels entities
            for (int i = 0; i < 16; i++)
            {
                var entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData2(entity.Index*100));
                
                entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData3(entity.Index*200));
                
                entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData4(entity.Index*300));
                
                entity = m_Manager.CreateEntity();
                m_Manager.AddComponentData(entity, new EcsTestData5(entity.Index*400));
            }

            // The function that does a query on T, then recurse to do a query on U, then T, then U...
            // The factors given allows us to check that we are actually retrieve the data we are expected
            void NestedForEach<T, U>(int factorU, int factorV, int nestedCount) where T : struct, IComponentData, IGetValue where U : struct, IComponentData, IGetValue
            {
                if (++curRecursionCounter >= maxRecursionCall)
                {
                    return;
                }
                
                TestSystem.Entities.ForEach((Entity e, ref T c) =>
                {
                    Assert.AreEqual(e.Index*factorU, c.GetValue());

                    if (nestedCount > 0)
                    {
                        NestedForEach<U, T>(factorV, factorU, nestedCount-1);
                    }
                });
            }

            // Main loop doing a ForEach on level 0...
            var l0ResultCount = 0;
            TestSystem.Entities.ForEach((Entity e0, ref EcsTestData t0) =>
            {
                Assert.AreEqual(e0.Index, t0.value);

                switch (e0.Index % 4)
                {
                    case 0:
                        TestSystem.Entities.ForEach((Entity e1, ref EcsTestData2 t1) =>
                        {
                            NestedForEach<EcsTestData3, EcsTestData4>(200, 300, 4);
                            Assert.AreEqual(e1.Index*100, t1.GetValue());
                        });
                        break;
                    case 1:
                        TestSystem.Entities.ForEach((Entity e1, ref EcsTestData3 t1) =>
                        {
                            NestedForEach<EcsTestData5, EcsTestData2>(400, 100, 4);
                            Assert.AreEqual(e1.Index*200, t1.GetValue());
                        });
                        break;
                    case 2:
                        TestSystem.Entities.ForEach((Entity e1, ref EcsTestData4 t1) =>
                        {
                            NestedForEach<EcsTestData2, EcsTestData3>(100, 200, 4);
                            Assert.AreEqual(e1.Index*300, t1.GetValue());
                        });
                        break;
                    case 3:
                        TestSystem.Entities.ForEach((Entity e1, ref EcsTestData5 t1) =>
                        {
                            NestedForEach<EcsTestData4, EcsTestData2>(300, 100, 4);
                            Assert.AreEqual(e1.Index*400, t1.GetValue());
                        });
                        break;
                }
                
                l0ResultCount++;
            });
            
            Assert.AreEqual(l0Count, l0ResultCount);
        }

        //@TODO: Class iterator test coverage...

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        [Test]
        public void Many_ManagedComponents()
        {
            var entity = m_Manager.CreateEntity();
            m_Manager.AddComponentData(entity, new EcsTestManagedComponent() { value = "SomeString" });
            m_Manager.AddComponentData(entity, new EcsTestManagedComponent2() { value2 = "SomeString2" });
            m_Manager.AddComponentData(entity, new EcsTestManagedComponent3() { value3 = "SomeString3" });
            m_Manager.AddComponentData(entity, new EcsTestManagedComponent4() { value4 = "SomeString4" });

            var counter = 0;
            TestSystem.Entities.ForEach((Entity e, EcsTestManagedComponent t0, EcsTestManagedComponent2 t1, EcsTestManagedComponent3 t2, EcsTestManagedComponent4 t3) =>
            {
                Assert.AreEqual(entity, e);
                Assert.AreEqual("SomeString", t0.value);
                Assert.AreEqual("SomeString2", t1.value2);
                Assert.AreEqual("SomeString3", t2.value3);
                Assert.AreEqual("SomeString4", t3.value4);
                counter++;
            });
            Assert.AreEqual(1, counter);
        }
#endif
    }
}
