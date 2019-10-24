using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

class DestinationSystem: JobComponentSystem
{
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var handle = new TestDestinationJob
        {
            commandBuffer = m_ECBSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDeps);
        m_ECBSystem.AddJobHandleForProducer(handle);
        return handle;
    }

    struct TestDestinationJob : IJobForEachWithEntity<TargetPosition, Translation, Speed, DistanceToTarget>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        public void Execute(Entity entity, int jobIndex,
            [ReadOnly] ref TargetPosition target, ref Translation translation, ref Speed speed, [ReadOnly] ref DistanceToTarget distance)
        {
            if (distance.value < 0.1f)
            {
                speed.value = 0.0f;
                translation.Value = target.value;
                commandBuffer.RemoveComponent<TargetPosition>(jobIndex, entity);
                commandBuffer.RemoveComponent<DistanceToTarget>(jobIndex, entity);
            }
        }
    }
}
