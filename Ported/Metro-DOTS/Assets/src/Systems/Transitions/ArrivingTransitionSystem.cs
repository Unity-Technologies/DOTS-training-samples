using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class ArrivingTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ApplyArrivingTransition
        {
            commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var handle = job.Schedule(this, inputDeps);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    [RequireComponentTag(typeof(EN_ROUTE))]
    struct ApplyArrivingTransition : IJobForEachWithEntity_EBCCC<StationData, PlatformId, BezierTOffset, Speed>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int index,
            [ReadOnly] DynamicBuffer<StationData> stations, [ReadOnly] ref PlatformId id, [ReadOnly] ref BezierTOffset t, ref Speed speed)
        {
            var nextStation = stations[0];
            for (var i = 0; i < stations.Length; i++)
            {
                if (id.value == stations[i].platformId)
                {
                    if (i != stations.Length - 1)
                        nextStation = stations[i];
                }
            }

            if (t.offset > nextStation.start)
            {
                speed.value = 0;
                commandBuffer.AddComponent(index, entity, new ARRIVING());
                commandBuffer.RemoveComponent(index, entity, typeof(EN_ROUTE));
            }
        }
    }
}
