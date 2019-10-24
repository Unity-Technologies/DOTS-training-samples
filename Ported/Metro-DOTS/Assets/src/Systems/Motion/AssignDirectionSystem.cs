using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

class AssignDirectionSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_Query;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_Query = GetEntityQuery(
            new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<TargetPosition>(),
                    ComponentType.ReadOnly<Translation>()
                },
                None = new []
                {
                    ComponentType.ReadOnly<Direction>(),
                    ComponentType.ReadOnly<DistanceToTarget>()
                }
            });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new DirectionJob {commandBuffer = m_ECBSystem.CreateCommandBuffer().ToConcurrent()}.Schedule(m_Query, inputDeps);
    }

    struct DirectionJob : IJobForEachWithEntity<Translation, TargetPosition>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int jobIndex,
            [ReadOnly] ref Translation translation, [ReadOnly] ref TargetPosition target)
        {
            var movementVector = target.value - translation.Value;
            commandBuffer.AddComponent(jobIndex, entity, new Direction { value = math.normalize(movementVector) });
            commandBuffer.AddComponent(jobIndex, entity, new DistanceToTarget { value = math.length(movementVector)});
        }
    }
}
