using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;


namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class BucketExtinguishFireUpdate : SystemBase
    {
        EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_EndSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var concurrentEcb = ecb.AsParallelWriter();

            Entities.WithBurst()
                .WithName("BucketExtinguishingFire")
                .WithAll<ThrowBucketAtFire, FullBucketTag>()
                .ForEach((int entityInQueryIndex, Entity bucketEntity, ref ThrowBucketAtFire throwBucketAtFire, ref EcsBucket bucket) =>
                {
                    bucket.WaterLevel = 0;
                    concurrentEcb.RemoveComponent<FullBucketTag>(entityInQueryIndex, bucketEntity);
                    concurrentEcb.RemoveComponent<ThrowBucketAtFire>(entityInQueryIndex, bucketEntity);
                }).ScheduleParallel();

            m_EndSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
