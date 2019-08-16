using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MovementSystem))]
public class ReachedEndOfSplineSystem : JobComponentSystem
{
    private EntityQuery m_Query;
    private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<SplineComponent>()},
            None = new []{ComponentType.ReadOnly<ReachedEndOfSplineComponent>() }
        });
        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct AssignFindTargetJob : IJobForEachWithEntity<Translation, SplineComponent>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SplineComponent targetSplineComponent)
        {
            //check if reaches the target
            // add find new target component
            bool hasReachedTarget = math.distancesq(translation.Value, targetSplineComponent.Spline.EndPosition) < 0.011f;
            if (hasReachedTarget)
            {
                commandBuffer.AddComponent<ReachedEndOfSplineComponent>(index, entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!RoadGenerator.ready || !RoadGenerator.useECS)
            return inputDeps;
        
        //1. get the direction
        //2. move to the Position
        var job = new AssignFindTargetJob
        {
            commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        
        var jobHandle = job.Schedule(m_Query, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}
