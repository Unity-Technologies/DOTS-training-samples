using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(TransitionSystemGroup))]
class DoorsOpenTransitionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    EntityQuery m_OpeningDoorQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_OpeningDoorQuery = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<DOORS_OPEN>(),
                    ComponentType.ReadWrite<TimerComponent>(),
                    ComponentType.ReadOnly<TimeInterval>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ApplyDoorOpenTransition { commandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent()};
        var handle = job.Schedule(m_OpeningDoorQuery, inputDeps);
        m_CommandBufferSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    struct ApplyDoorOpenTransition : IJobForEachWithEntity<TimerComponent, TimeInterval>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex, ref TimerComponent timer, [ReadOnly] ref TimeInterval timeInterval)
        {
            if (timer.value >= timeInterval.value )
            {
                timer.value = 0;
                commandBuffer.RemoveComponent<DOORS_OPEN>(jobIndex, entity);
                commandBuffer.AddComponent<UNLOADING>(jobIndex, entity);
            }
        }
    }
}
