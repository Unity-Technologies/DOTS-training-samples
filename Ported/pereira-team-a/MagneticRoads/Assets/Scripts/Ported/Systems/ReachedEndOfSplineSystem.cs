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
    public NativeQueue<Entity> queue;
    private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<SplineComponent>() },
            None = new[] { ComponentType.ReadOnly<ReachedEndOfSplineComponent>() }
        });

        queue = new NativeQueue<Entity>(Allocator.Persistent);

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        queue.Dispose();
    }

    //With queue now this job is Burst compatible
    [BurstCompile]
    struct AssignFindTargetJob : IJobForEachWithEntity<Translation, SplineComponent>
    {
        public NativeQueue<Entity>.ParallelWriter queue;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SplineComponent targetSplineComponent)
        {
            //check if reaches the target and add find new target component (via a queue)
            bool hasReachedTarget = math.distancesq(translation.Value, targetSplineComponent.Spline.EndPosition) < 0.001f;
            if (hasReachedTarget)
            {
                queue.Enqueue(entity);
            }
        }
    }

    struct DequeueEnityJob : IJob
    {
        public NativeQueue<Entity> queue;
        public EntityCommandBuffer commandBuffer;

        public void Execute()
        {
            for (int i = 0; i < queue.Count; i++)
            {
                Entity entity = queue.Dequeue();
                commandBuffer.AddComponent<ReachedEndOfSplineComponent>(entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!RoadGenerator.ready || !RoadGenerator.useECS)
            return inputDeps;
        
        //1. get the direction
        //2. move to the Position
        var job = new AssignFindTargetJob()
        {
            queue = queue.AsParallelWriter()
        };
        var jobHandle = job.Schedule(m_Query, inputDeps);

        var jobDequeue = new DequeueEnityJob()
        {
            queue = queue,
            commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
        };
        var jobHandleDequeue = jobDequeue.Schedule(jobHandle);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandleDequeue);

        return jobHandle;
    }
}
