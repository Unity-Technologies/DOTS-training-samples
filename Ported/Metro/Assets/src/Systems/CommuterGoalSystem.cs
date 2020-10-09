using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterGoalSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Random random = new Random(12345);

        Entities
            .WithName("commuter_goal_assignment")
            .WithNone<CommuterTask_MoveToPlatform, CommuterTask_MoveToQueue, CommuterTask_WaitOnQueue>()
            .WithNone<CommuterTask_BoardTrain, CommuterTask_UnboardTrain>()
            .ForEach((Entity commuter, int entityInQueryIndex, ref CommuterOnPlatform platform, in Translation translation) =>
            {
                Entity currentPlatform = platform.Value;
                WalkwayPoints currentWalkways = GetComponent<WalkwayPoints>(currentPlatform);

                var sameStationPlatforms = GetBufferFromEntity<SameStationPlatformBufferElementData>()[currentPlatform];
                int nextQueueIndex = random.NextInt(sameStationPlatforms.Length);
                Entity nextPlatform = sameStationPlatforms[nextQueueIndex].Value;
                WalkwayPoints nextWalkways = GetComponent<WalkwayPoints>(nextPlatform);

                Entity adjacentPlatform = GetComponent<AdjacentPlatform>(currentPlatform).Value;
                if (nextPlatform == adjacentPlatform)
                {
                    platform.Value = nextPlatform;

                    ecb.AddComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, commuter);
                }
                else
                {
                    var navPoints = ecb.AddBuffer<NavPointBufferElementData>(entityInQueryIndex, commuter);

                    float3 frontBottomPosition = GetComponent<LocalToWorld>(currentWalkways.WalkwayFrontBottom).Position;
                    float3 backBottomPosition = GetComponent<LocalToWorld>(currentWalkways.WalkwayBackBottom).Position;
                    float3 topPoint;

                    if (math.distancesq(translation.Value, frontBottomPosition) <= math.distancesq(translation.Value, backBottomPosition))
                    {
                        topPoint = GetComponent<LocalToWorld>(currentWalkways.WalkwayFrontTop).Position;
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = frontBottomPosition });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = topPoint });
                    }
                    else
                    {
                        topPoint = GetComponent<LocalToWorld>(currentWalkways.WalkwayBackTop).Position;
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = backBottomPosition });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = topPoint });
                    }

                    float3 targetTopFrontPoistion = GetComponent<LocalToWorld>(nextWalkways.WalkwayFrontTop).Position;
                    float3 targetTopBackPosition = GetComponent<LocalToWorld>(nextWalkways.WalkwayBackTop).Position;

                    if (math.distancesq(topPoint, targetTopFrontPoistion) <= math.distancesq(topPoint, targetTopBackPosition))
                    {
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = targetTopFrontPoistion });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = GetComponent<LocalToWorld>(nextWalkways.WalkwayFrontBottom).Position });
                    }
                    else
                    {
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = targetTopBackPosition });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = GetComponent<LocalToWorld>(nextWalkways.WalkwayBackBottom).Position });
                    }

                    ecb.AddComponent(entityInQueryIndex, commuter, new CommuterTask_MoveToPlatform() { TargetPlatform = nextPlatform });
                }
            }).Schedule();

        Entities
            .WithName("commuter_arriving_platform")
            .WithNone<TargetPoint>()
            .WithAll<CommuterTask_MoveToQueue>()
            .ForEach((Entity commuter, int entityInQueryIndex, in CommuterOnPlatform commuterOnPlatform, in Translation translation) =>
            {
                var queueBuffer = GetBuffer<QueueBufferElementData>(commuterOnPlatform.Value);
                Queues platform = GetComponent<Queues>(commuterOnPlatform.Value);
                Entity targetQueue = queueBuffer[random.NextInt(platform.CarriageCount)].Value;
                if (targetQueue != Entity.Null)
                {
                    var commutersBuffer = GetBuffer<CommuterInQueueBufferElementData>(targetQueue);
                    int n = commutersBuffer.Add(new CommuterInQueueBufferElementData
                    {
                        Value = commuter
                    });

                    LocalToWorld queueMatrix = GetComponent<LocalToWorld>(targetQueue);
                    float3 targetPosition = queueMatrix.Position;
                    targetPosition += n * 0.5f * queueMatrix.Forward;

                    ecb.AddComponent(entityInQueryIndex, commuter, new TargetPoint
                    {
                        CurrentTarget = targetPosition
                    });

                    ecb.RemoveComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, commuter);
                    ecb.AddComponent<CommuterTask_WaitOnQueue>(entityInQueryIndex, commuter);
                } // todo: else error?
            }).Schedule();

        Entities
            .WithName("commuter_boarding_train")
            .WithNone<TargetPoint>()
            .ForEach((Entity commuter, int entityInQueryIndex, in CommuterTask_BoardTrain task) =>
            {
                var navPointsForCommuter = GetBufferFromEntity<NavPointBufferElementData>();
                if (navPointsForCommuter.HasComponent(commuter))
                {
                    var navPoints = navPointsForCommuter[commuter];

                    float3 nextPoint = navPoints[0].NavPoint;

                    navPoints.RemoveAt(0);
                    if (navPoints.IsEmpty)
                    {
                        ecb.RemoveComponent<NavPointBufferElementData>(entityInQueryIndex, commuter);
                    }

                    ecb.AddComponent(entityInQueryIndex, commuter, new TargetPoint() { CurrentTarget = nextPoint });
                }
                else
                {
                    ecb.RemoveComponent<CommuterTask_BoardTrain>(entityInQueryIndex, commuter);
                    ecb.AddComponent(entityInQueryIndex, commuter, new Parent { Value = task.Carriage });

                    var carriageTransform = GetComponent<LocalToWorld>(task.Carriage);
                    var commuterTransform = GetComponent<LocalToWorld>(commuter);

                    var localToWorld = math.mul(math.inverse(carriageTransform.Value), commuterTransform.Value);
                    ecb.AddComponent<LocalToParent>(entityInQueryIndex, commuter);
                    ecb.SetComponent(entityInQueryIndex, commuter, new Translation
                    {
                        Value = math.transform(math.inverse(carriageTransform.Value), commuterTransform.Position)
                    });

                    ecb.RemoveComponent<CommuterOnPlatform>(entityInQueryIndex, commuter);
                }
            }).Schedule();

        Entities
            .WithName("commuter_unboarding_train")
            .WithNone<TargetPoint>()
            .ForEach((Entity commuter, int entityInQueryIndex, in CommuterTask_UnboardTrain task) =>
            {
                var navPointsForCommuter = GetBufferFromEntity<NavPointBufferElementData>();
                if (navPointsForCommuter.HasComponent(commuter))
                {
                    var navPoints = navPointsForCommuter[commuter];

                    float3 nextPoint = navPoints[0].NavPoint;

                    navPoints.RemoveAt(0);
                    if (navPoints.IsEmpty)
                    {
                        ecb.RemoveComponent<NavPointBufferElementData>(entityInQueryIndex, commuter);
                    }

                    ecb.AddComponent(entityInQueryIndex, commuter, new TargetPoint() { CurrentTarget = nextPoint });
                }
                else
                {
                    ecb.AddComponent(entityInQueryIndex, commuter, new CommuterOnPlatform { Value = task.Platform });
                    ecb.RemoveComponent<CommuterTask_UnboardTrain>(entityInQueryIndex, commuter);
                }
            }).Schedule();


        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
