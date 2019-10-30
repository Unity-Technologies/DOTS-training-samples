using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Unity.Entities.Tests
{
    class EntitiesAssertTests : ECSTestsFixture 
    {
        [Test]
        public void IsEmpty_WithEmptyEcs_DoesNotAssert()
        {
            Assert.DoesNotThrow(() =>
                EntitiesAssert.IsEmpty(m_Manager));
        }
        
        [Test]
        public void IsEmpty_WithNonEmptyEcs_Asserts()
        {
            m_Manager.CreateEntity();

            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.IsEmpty(m_Manager));
        }
        
        [Test]
        public void ContainsOnly_WithNoMatchers_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager));
            StringAssert.Contains("Use IsEmpty", ex.Message);
        }

        [Test]
        public void Contains_WithNoMatchers_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                EntitiesAssert.Contains(m_Manager));
            StringAssert.Contains("Use IsEmpty", ex.Message);
        }

        [Test]
        public void ContainsOnly_WithEmptyEcs_Asserts()
        {
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Where(() => "", _ => true)));
        }

        [Test]
        public void ContainsOnly_WithNullTypeMatch_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(null)));
            Assert.Throws<ArgumentNullException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>(null)));
        }

        [Test]
        public void ContainsOnly_WithMultipleEntityMatches_Throws()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(new Entity { Index = 1 }, new Entity { Index = 2 })));
            StringAssert.Contains("multiple Entity", ex.Message);
        }
        
        [Test]
        public void ContainsOnly_WithEmptyEntity()
        {
            var entity = m_Manager.CreateEntity();
            
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Any(entity)));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(entity)));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>(entity)));
        }

        [Test]
        public void ContainsOnly_WithNonEmptyEntity()
        {
            var entity = m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));
            
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Any(entity)));
            
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>(entity)));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(entity)));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2, EcsTestData3>(entity)));
            
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>()));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>()));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2, EcsTestData3>()));
        }

        [Test]
        public void Contains_WithPartialMatch_DoesNotAssert()
        {
            var entity1 = m_Manager.CreateEntity(typeof(EcsTestData));
            var entity2 = m_Manager.CreateEntity();
            
            Assert.DoesNotThrow(() =>
                EntitiesAssert.Contains(m_Manager, EntityMatch.Any(entity1)));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.Contains(m_Manager, EntityMatch.Exact<EcsTestData>()));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.Contains(m_Manager, EntityMatch.Exact(entity2)));
        }

        [Test]
        public void ContainsOnly_WithParamTypeComparison()
        {
            var entity = m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));
            
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(typeof(EcsTestData2), entity, typeof(EcsTestData))));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(typeof(EcsTestData), new[] { typeof(EcsTestData2) })));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(entity, new[] { typeof(EcsTestData), typeof(EcsTestData2) })));
            
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData2>(new[] { typeof(EcsTestData2) }, typeof(EcsTestData))));
        }

        [Test]
        public void ContainsOnly_WithDataComparison()
        {
            var entity = m_Manager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));
            m_Manager.SetComponentData(entity, new EcsTestData2(5));
            
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(entity)));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>()));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>(entity, new EcsTestData2(5))));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData>(new EcsTestData2(5), entity)));

            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(entity, new EcsTestData2(6))));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(new EcsTestData2(6), entity)));
            
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(entity, new EcsTestData2(5))));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsTestData, EcsTestData2>(new EcsTestData2(5), entity)));
        }

        [Test]
        public void ContainsOnly_WithBufferElementData()
        {
            var entity = m_Manager.CreateEntity();
            var buffer = m_Manager.AddBuffer<EcsIntElement>(entity);
            buffer.Add(1);
            buffer.Add(5);
            buffer.Add(9);

            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsIntElement>()));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<EcsIntElement[]>()));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact<List<EcsIntElement>>()));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(new EcsIntElement[] { 1, 5, 9 })));
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(EntityMatch.Component((EcsIntElement[] match) => match.Length == 3))));
            Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Exact(EntityMatch.Component((EcsIntElement[] match) => match.Length == 2))));
        }
        
        [Test]
        public void ContainsOnly_WithMatchingCustomMatcher_DoesNotAssert()
        {
            m_Manager.CreateEntity();

            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Where(() => "Matcher #0", _ => true)));
        }
        
        [Test]
        public void ContainsOnly_WithNonMatchingCustomMatcher_Asserts()
        {
            m_Manager.CreateEntity();

            var ex = Assert.Throws<AssertionException>(() =>
                EntitiesAssert.ContainsOnly(m_Manager, EntityMatch.Where(() => "Matcher #0", _ => false)));
            StringAssert.Contains("Matcher #0", ex.Message);
        }
        
        [Test]
        public void ContainsOnly_WithCustomMatcher_OnlyCalledOnce()
        {
            m_Manager.CreateEntity();
            m_Manager.CreateEntity();

            var (count0, count1) = (0, 10);
            Assert.DoesNotThrow(() =>
                EntitiesAssert.ContainsOnly(m_Manager,
                    EntityMatch.Where(() => "Matcher #0", _ => { ++count0; return true; }),
                    EntityMatch.Where(() => "Matcher #1", _ => { ++count1; return true; })));
            
            Assert.AreEqual(1, count0);
            Assert.AreEqual(11, count1);
        }
    }
}