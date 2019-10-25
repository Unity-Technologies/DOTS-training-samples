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
                    ComponentType.ReadOnly<TimeInterval>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ApplyDoorCloseTransition();
        var handle = job.Schedule(m_ClosingDoorQuery, inputDeps);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    struct ApplyDoorCloseTransition : IJobForEachWithEntity<TimerComponent, TimeInterval>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, ref TimerComponent timer, [ReadOnly] ref TimeInterval timeInterval)
        {
            if (timer.value >= timeInterval.value)
            {
                timer.value = 0;
                commandBuffer.RemoveComponent<DOORS_CLOSE>(jobIndex, entity);
                commandBuffer.AddComponent<EN_ROUTE>(jobIndex, entity);
            }
        }
    }
}
