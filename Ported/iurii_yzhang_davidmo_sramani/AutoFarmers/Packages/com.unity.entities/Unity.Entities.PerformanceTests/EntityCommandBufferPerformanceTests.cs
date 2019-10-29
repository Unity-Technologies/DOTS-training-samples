using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.PerformanceTesting;

namespace Unity.Entities.PerformanceTests
{
    [TestFixture]
    [Category("Performance")]
    public sealed class EntityCommandBufferPerformanceTests : EntityPerformanceTestFixture
    {
        public struct EcsTestData : IComponentData
        {
            public int value;
        }

        void FillWithEcsTestData(EntityCommandBuffer cmds, int repeat)
        {
            for (int i = repeat; i != 0; --i)
            {
                var e = cmds.CreateEntity();
                cmds.AddComponent(e, new EcsTestData {value = i});
            }
        }

        [Test, Performance]
        public void EntityCommandBuffer_512SimpleEntities()
        {
            const int kCreateLoopCount = 512;
            const int kPlaybackLoopCount = 1000;

            var ecbs = new List<EntityCommandBuffer>(kPlaybackLoopCount);
            Measure.Method(
                () =>
                {
                    for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                    {
                        var cmds = new EntityCommandBuffer(Allocator.TempJob);
                        FillWithEcsTestData(cmds, kCreateLoopCount);
                        ecbs.Add(cmds);
                    }
                })
                .Definition("CreateEntities")
                .WarmupCount(0)
                .MeasurementCount(1)
                .Run();

            Measure.Method(
                () =>
                {
                    for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                    {
                        ecbs[repeat].Playback(m_Manager);
                    }
                })
                .Definition("Playback")
                .WarmupCount(0)
                .MeasurementCount(1)
                .CleanUp(() =>
                {
                })
                .Run();

            foreach (var ecb in ecbs)
            {
                ecb.Dispose();
            }
        }

        struct EcsTestDataWithEntity : IComponentData
        {
            public int value;
            public Entity entity;
        }

        void FillWithEcsTestDataWithEntity(EntityCommandBuffer cmds, int repeat)
        {
            for (int i = repeat; i != 0; --i)
            {
                var e = cmds.CreateEntity();
                cmds.AddComponent(e, new EcsTestDataWithEntity {value = i});
            }
        }

        [Test, Performance]
        public void EntityCommandBuffer_512EntitiesWithEmbeddedEntity()
        {
            const int kCreateLoopCount = 512;
            const int kPlaybackLoopCount = 1000;

            var ecbs = new List<EntityCommandBuffer>(kPlaybackLoopCount);
            Measure.Method(
                    () =>
                    {
                        for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                        {
                            var cmds = new EntityCommandBuffer(Allocator.TempJob);
                            FillWithEcsTestDataWithEntity(cmds, kCreateLoopCount);
                            ecbs.Add(cmds);
                        }
                    })
                .Definition("CreateEntities")
                .WarmupCount(0)
                .MeasurementCount(1)
                .Run();
            Measure.Method(
                    () =>
                    {
                        for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                        {
                            ecbs[repeat].Playback(m_Manager);
                        }
                    })
                .Definition("Playback")
                .WarmupCount(0)
                .MeasurementCount(1)
                .Run();
            foreach (var ecb in ecbs)
            {
                ecb.Dispose();
            }
        }

        [Test, Performance]
        public void EntityCommandBuffer_OneEntityWithEmbeddedEntityAnd512SimpleEntities()
        {
            // This test should not be any slower than EntityCommandBuffer_SimpleEntities_512x1000
            // It shows that adding one component that needs fix up will not make the fast
            // path any slower

            const int kCreateLoopCount = 512;
            const int kPlaybackLoopCount = 1000;


            var ecbs = new List<EntityCommandBuffer>(kPlaybackLoopCount);
            Measure.Method(
                    () =>
                    {
                        for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                        {
                            var cmds = new EntityCommandBuffer(Allocator.TempJob);
                            Entity e0 = cmds.CreateEntity();
                            cmds.AddComponent(e0, new EcsTestDataWithEntity {value = -1, entity = e0 });
                            FillWithEcsTestData(cmds, kCreateLoopCount);
                            ecbs.Add(cmds);
                        }
                    })
                .Definition("CreateEntities")
                .WarmupCount(0)
                .MeasurementCount(1)
                .Run();
            Measure.Method(
                    () =>
                    {
                        for (int repeat = 0; repeat < kPlaybackLoopCount; ++repeat)
                            ecbs[repeat].Playback(m_Manager);
                    })
                .Definition("Playback")
                .WarmupCount(0)
                .MeasurementCount(1)
                .Run();
            foreach (var ecb in ecbs)
            {
                ecb.Dispose();
            }
        }
    }
}

