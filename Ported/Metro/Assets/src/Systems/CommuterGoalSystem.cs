using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterGoalSystem : SystemBase
{
    private int nextQueueIndex = 0;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithName("commuter_goal_assignment")
            .WithStructuralChanges()
            .WithAll<CommuterTask_Idle>()
            .ForEach((Entity commuter, ref CommuterOnPlatform platform, in Translation translation) =>
            {
                Entity currentPlatform = platform.Value;
                WalkwayPoints currentWalkways = GetComponent<WalkwayPoints>(currentPlatform);

                var sameStationPlatforms = GetBufferFromEntity<SameStationPlatformBufferElementData>()[currentPlatform];
                Entity nextPlatform = sameStationPlatforms[nextQueueIndex % sameStationPlatforms.Length].Value;
                WalkwayPoints nextWalkways = GetComponent<WalkwayPoints>(nextPlatform);
                System.Threading.Interlocked.Increment(ref nextQueueIndex);

                Entity adjacentPlatform = GetComponent<AdjacentPlatform>(currentPlatform).Value;
                if (nextPlatform == adjacentPlatform)
                {
                    platform.Value = nextPlatform;

                    EntityManager.RemoveComponent<CommuterTask_Idle>(commuter);

                    var numQueues = GetComponent<Queues>(platform.Value).CarriageCount;
                    var queues = GetBufferFromEntity<EntityBufferElementData>()[platform.Value];

                    var nextQueue = queues[nextQueueIndex % numQueues].Value;
                    System.Threading.Interlocked.Increment(ref nextQueueIndex);

                    float3 nextPoint = GetComponent<LocalToWorld>(nextQueue).Position;
                    EntityManager.AddComponentData(commuter, new TargetPoint() { CurrentTarget = nextPoint });

                    EntityManager.AddComponent<CommuterTask_MoveToQueue>(commuter);
                }
                else
                {
                    var navPoints = EntityManager.AddBuffer<NavPointBufferElementData>(commuter);

                    if (math.distancesq(translation.Value, currentWalkways.WalkwayFrontBottom) <= math.distancesq(translation.Value, currentWalkways.WalkwayBackBottom))
                    {
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = currentWalkways.WalkwayFrontBottom });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = currentWalkways.WalkwayFrontTop });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = nextWalkways.WalkwayFrontTop });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = nextWalkways.WalkwayFrontBottom });
                    }
                    else
                    {
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = currentWalkways.WalkwayBackBottom });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = currentWalkways.WalkwayBackTop });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = nextWalkways.WalkwayBackTop });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = nextWalkways.WalkwayBackBottom });
                    }

                    EntityManager.RemoveComponent<CommuterTask_Idle>(commuter);
                    EntityManager.AddComponentData(commuter, new CommuterTask_MoveToPlatform() { TargetPlatform = nextPlatform });
                }
            }).Run();
    }
}
