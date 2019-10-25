using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class DoorsCloseTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_ClosingDoorQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_ClosingDoorQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<DOORS_CLOSE>(),
                    ComponentType.ReadWrite<TimerComponent>(),
                    ComponentType.ReadOnly<TimeInterval>(),
                    ComponentType.ReadWrite<Speed>(),
                    ComponentType.ReadWrite<PlatformId>(),
                    ComponentType.ReadOnly<StationData>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ApplyDoorCloseTransition{ commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()};;
        var handle = job.Schedule(m_ClosingDoorQuery, inputDeps);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    struct ApplyDoorCloseTransition : IJobForEachWithEntity_EBCCCC<StationData, TimerComponent, TimeInterval, Speed, PlatformId>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex,[ReadOnly] DynamicBuffer<StationData>stations, ref TimerComponent timer, [ReadOnly] ref TimeInterval timeInterval, ref Speed speed, ref PlatformId id)
        {
            if (timer.value >= timeInterval.value)
            {
                timer.value = 0;
                speed.value = 0.02f;
                commandBuffer.RemoveComponent<DOORS_CLOSE>(jobIndex, entity);
                commandBuffer.AddComponent<EN_ROUTE>(jobIndex, entity);

                var nextStation = stations[0];
                for (var i = 0; i < stations.Length; i++)
                {
                    if (id.value == stations[i].platformId)
                    {
                        if (i != stations.Length - 1)
                            nextStation = stations[i+1];
                    }
                }

                commandBuffer.SetComponent(jobIndex, entity, new PlatformId{ value = nextStation.platformId});
            }
        }
    }
}
