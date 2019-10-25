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

        public void Execute(Entity entity, int index, [ReadOnly] DynamicBuffer<StationData> b0, [ReadOnly] ref PlatformId c1, [ReadOnly] ref BezierTOffset c2, ref Speed speed)
        {
            var nextStation = b0[0];
            for (int i = 0; i < b0.Length; i++)
            {
                if (c1.value == b0[i].platformId)
                {
                    if (i == b0.Length - 1)
                        nextStation = b0[0];
                    else
                        nextStation = b0[i];
                }
            }

            if (c2.offset > nextStation.start)
            {
                speed.value = 0;
                commandBuffer.AddComponent(index, entity, new ARRIVING());
                commandBuffer.RemoveComponent(index, entity, typeof(EN_ROUTE));
            }
        }
    }
}
