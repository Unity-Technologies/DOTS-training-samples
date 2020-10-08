using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterTask_MoveToPlatform_System : SystemBase
{
    private int nextQueueIndex = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithName("commuter_task_movetoplatform")
            .WithStructuralChanges()
            .WithNone<TargetPoint>()
            .ForEach((Entity commuter, ref CommuterOnPlatform platform, in CommuterTask_MoveToPlatform task) =>
            {
                var navPointsForCommuter = GetBufferFromEntity<NavPointBufferElementData>();
                if (navPointsForCommuter.HasComponent(commuter))
                {
                    var navPoints = navPointsForCommuter[commuter];

                    float3 nextPoint = navPoints[0].NavPoint;

                    navPoints.RemoveAt(0);
                    if (navPoints.IsEmpty)
                    {
                        EntityManager.RemoveComponent<NavPointBufferElementData>(commuter);
                    }

                    EntityManager.AddComponentData(commuter, new TargetPoint() { CurrentTarget = nextPoint });
                }
                else
                {
                    platform.Value = task.TargetPlatform;

                    EntityManager.RemoveComponent<CommuterTask_MoveToPlatform>(commuter);

                    //var numQueues = GetComponent<Queues>(platform.Value).CarriageCount;
                    //var queues = GetBufferFromEntity<EntityBufferElementData>()[platform.Value];

                    //var nextQueue = queues[nextQueueIndex % numQueues].Value;
                    //System.Threading.Interlocked.Increment(ref nextQueueIndex);
                    
                    //float3 nextPoint = GetComponent<LocalToWorld>(nextQueue).Position;
                    //EntityManager.AddComponentData(commuter, new TargetPoint() { CurrentTarget = nextPoint });

                    EntityManager.AddComponent<CommuterTask_MoveToQueue>(commuter);
                }
            }).Run();
    }
}
