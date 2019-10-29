
#if false // !UNITY_DOTSPLAYER
using System;
using NUnit.Framework;

namespace Unity.Entities.Tests
{
    class EntityQueryBuilderMemoryTests : EntityQueryBuilderTestFixture
    {
        [Test]
        public void ConstructAndConfigure_DoesNotAlloc()
        {
            ValidateNoGCAllocs(() =>
            {
                TestSystem.Entities
                    .WithAll<EcsTestTag>()
                    .WithAny<EcsTestData, EcsTestData2>()
                    .WithNone<EcsTestData3, EcsTestData4, EcsTestData5>();
                new EntityQueryBuilder(TestSystem)
                    .WithNone<EcsTestData3>()
                    .WithAll<EcsTestTag>()
                    .WithAny<EcsTestData>()
                    .WithNone<EcsTestData4, EcsTestData5>()
                    .WithAny<EcsTestData2>();
            });
        }

        [Test]
        public void EntityQueryBuilder_ShallowEquals_DoesNotAlloc()
        {
            ValidateNoGCAllocs(() =>
            {
                var builder0 = TestSystem.Entities
                    .WithAll<EcsTestTag>()
                    .WithAny<EcsTestData, EcsTestData2>()
                    .WithNone<EcsTestData3, EcsTestData4, EcsTestData5>();
                var builder1 = new EntityQueryBuilder(TestSystem)
                    .WithNone<EcsTestData3>()
                    .WithAll<EcsTestTag>()
                    .WithAny<EcsTestData>()
                    .WithNone<EcsTestData4, EcsTestData5>()
                    .WithAny<EcsTestData2>();

                builder0.ShallowEquals(ref builder1);
            });
        }

        [Test]
        public unsafe void LookupInCache_DoesNotAlloc()
        {
            // insert into cache
            int index;
            {
                var builder = new EntityQueryBuilder().WithAll<EcsTestTag>();
                var delegateTypes = stackalloc int[] { TypeManager.GetTypeIndex<EcsTestData>() };
                index = TestSystem.GetOrCreateEntityQueryCache().CreateCachedQuery(0, new EntityQuery(null, null, null, null), ref builder, delegateTypes, 1);

                Assert.Zero(index); // sanity
            }

            ValidateNoGCAllocs(() =>
            {
                var testBuilder = new EntityQueryBuilder().WithAll<EcsTestTag>();
                var testDelegateTypes = stackalloc int[] { TypeManager.GetTypeIndex<EcsTestData>() };
                TestSystem.EntityQueryCache.ValidateMatchesCache(index, ref testBuilder, testDelegateTypes, 1);
            });
        }

        [Test, Ignore("CreateArchetypeChunkArray allocs GC")] // this is due to safety sentinels; move to release build playmode test
        public void ForEachSecondRun_DoesNotAlloc()
        {
            EntityQueryBuilder.F_ED<EcsTestData> emptyFunc = (Entity e, ref EcsTestData d) => { };

            ValidateNoGCAllocs(() => TestSystem.Entities.ForEach(emptyFunc));
        }
    }
}
#endif // !UNITY_DOTSPLAYER
