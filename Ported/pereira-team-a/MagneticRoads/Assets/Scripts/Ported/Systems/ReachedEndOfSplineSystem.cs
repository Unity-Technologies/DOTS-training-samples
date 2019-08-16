using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/*[UpdateAfter(typeof(MovementSystem))]
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
        [ReadOnly] public DynamicBuffer<SplineBufferElementData> splineBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SplineComponent targetSplineComponent)
        {
            //check if reaches the target and add find new target component (via a queue)
            bool hasReachedTarget = math.distancesq(translation.Value, targetSplineComponent.Spline.EndPosition) < 0.001f;
            if (hasReachedTarget)
            {
                //targetSplineComponent.reachEndOfSpline = true;
                //queue.Enqueue(entity);
                
                if (targetSplineComponent.IsInsideIntersection)
                {
                    var spline = SplineBuffer[targetSplineComponent.TargetSplineId];
                    var splineComponent = new SplineComponent
                    {
                        splineId = exitIntersectionComponent.TargetSplineId,
                        Spline = spline, 
                        IsInsideIntersection = false,
                        t = 0,
                        reachEndOfSpline = false
                    };
                }
                else
                {
                    // Get the intersection object, to which we have arrived
                    var currentIntersection = IntersectionBuffer[currentSplineComponent.Spline.EndIntersectionId];
                    
                    // Select the next spline to travel
                    // TODO: Replace this with a noise/random solution
                    currentIntersection.LastIntersection += 1;
                    IntersectionBuffer[currentSplineComponent.Spline.EndIntersectionId] = currentIntersection;
                    
                    int[] targetSplineIDs = {
                        currentIntersection.SplineId0,
                        currentIntersection.SplineId1,
                        currentIntersection.SplineId2
                    };
                    var targetSplineId = targetSplineIDs[currentIntersection.LastIntersection % currentIntersection.SplineIdCount];
                    var targetSpline = SplineBuffer[targetSplineId];

                    var newSpline = new SplineBufferElementData()
                    {
                        EndIntersectionId = -1,
                        SplineId = 1,

                        StartPosition = currentSplineComponent.Spline.EndPosition,
                        EndPosition = targetSpline.StartPosition,

                        //intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint) * .5f;
                        Anchor1 = (currentIntersection.Position + targetSpline.StartPosition) * .5f,
                        // intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
                        Anchor2 = (currentIntersection.Position + targetSpline.EndPosition) * .5f,
                        // intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
                        StartTangent =
                            math.round(math.normalize(currentIntersection.Position -
                                                      currentSplineComponent.Spline.EndPosition)),
                        // intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
                        EndTangent = math.round(math.normalize(currentIntersection.Position - targetSpline.StartPosition)),
                        // intersectionSpline.startNormal = intersection.normal;
                        StartNormal = currentIntersection.Normal,
                        // intersectionSpline.endNormal = intersection.normal;
                        EndNormal = IntersectionBuffer[targetSpline.EndIntersectionId].Normal
                    };
                    
                    //if (targetSpline.OppositeDirectionSplineId == currentSplineComponent.Spline.SplineId)
                    {
                        newSpline.Anchor1 = newSpline.StartPosition;
                        newSpline.Anchor2 = newSpline.EndPosition;
                        
                        // u-turn - make our intersection spline more rounded than usual
                        // Vector3 perp = Vector3.Cross(intersectionSpline.startTangent, intersectionSpline.startNormal);
                        // intersectionSpline.anchor1 += (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;
                        // intersectionSpline.anchor2 += (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;

                        // intersectionSpline.anchor1 -= perp * RoadGenerator.trackRadius * .5f * intersectionSide;
                        // intersectionSpline.anchor2 += perp * RoadGenerator.trackRadius * .5f * intersectionSide;
                    }
                    

                    CommandBuffer.SetComponent(index, entity, new SplineComponent
                    {
                        Spline = newSpline,
                        IsInsideIntersection = true,
                        t = 0,
                        TargetSplineId = targetSplineId,
                        reachEndOfSpline = false
                    });
                    
                }
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
        if (!RoadGenerator.ready || !RoadGenerator.useECS || true)
            return inputDeps;
        
        var splineBuffer = EntityManager.GetBuffer<SplineBufferElementData>(GetSingletonEntity<SplineBufferElementData>());
        
        
        //1. get the direction
        //2. move to the Position
        var job = new AssignFindTargetJob()
        {
            queue = queue.AsParallelWriter(),
            //var splineBuffer = EntityManager.GetBuffer<SplineBufferElementData>(GetSingletonEntity<SplineBufferElementData>()); 
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
*/