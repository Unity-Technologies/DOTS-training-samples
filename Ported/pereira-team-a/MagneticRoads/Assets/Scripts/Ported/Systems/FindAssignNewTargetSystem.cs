using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MovementSystem))]
public class FindAssignNewTargetSystem : JobComponentSystem
{
    private EntityQuery m_Query;
    private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<TargetPosition>()},
            None = new []{ComponentType.ReadOnly<FindTarget>() }
        });
        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    struct AssignFindTargetJob : IJobForEachWithEntity<Translation, TargetPosition>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref TargetPosition target)
        {
            //check if reaches the target
            // add find new target component
            bool hasReachedTarget = math.distancesq(translation.Value, target.Value) < 0.1f;
            if (hasReachedTarget)
            {
                commandBuffer.AddComponent<FindTarget>(index, entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
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
