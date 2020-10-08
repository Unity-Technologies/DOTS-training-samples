using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CommuterGoalSystem : SystemBase
{
    private struct CommuterToQueue
    {
        public Entity Commuter;
        public Entity Queue;
    }
    private NativeList<CommuterToQueue> m_CommuterToQueues;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_CommuterToQueues = new NativeList<CommuterToQueue>(0, Allocator.Persistent);
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        m_CommuterToQueues.Dispose();

        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Random random = new Random(12345);

        Entities
            .WithName("commuter_goal_assignment")
            .WithNone<CommuterTask_MoveToPlatform, CommuterTask_MoveToQueue, CommuterTask_WaitOnQueue>()
            .ForEach((Entity commuter, int entityInQueryIndex, ref CommuterOnPlatform platform, in Translation translation) =>
            {
                Entity currentPlatform = platform.Value;

                int targetPlatformIndex = new Random((uint)commuter.Index).NextInt() % 2;
                if (targetPlatformIndex == 0)
                {
                    ecb.AddComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, commuter);
                }
                else
                {
                    WalkwayPoints currentWalkways = GetComponent<WalkwayPoints>(currentPlatform);

                    var sameStationPlatforms = GetBufferFromEntity<SameStationPlatformBufferElementData>()[currentPlatform];
                    int nextQueueIndex = random.NextInt(sameStationPlatforms.Length);
                    Entity nextPlatform = sameStationPlatforms[nextQueueIndex].Value;
                    WalkwayPoints nextWalkways = GetComponent<WalkwayPoints>(nextPlatform);

                    Entity adjacentPlatform = GetComponent<AdjacentPlatform>(currentPlatform).Value;
                    if (nextPlatform == adjacentPlatform)
                    {
                        platform.Value = nextPlatform;

                        //EntityManager.RemoveComponent<CommuterTask_Idle>(commuter);

                        //var numQueues = GetComponent<Queues>(platform.Value).CarriageCount;
                        //var queues = GetBufferFromEntity<EntityBufferElementData>()[platform.Value];

                        //var nextQueue = queues[nextQueueIndex % numQueues].Value;
                        //System.Threading.Interlocked.Increment(ref nextQueueIndex);

                        //float3 nextPoint = GetComponent<LocalToWorld>(nextQueue).Position;
                        //EntityManager.AddComponentData(commuter, new TargetPoint() { CurrentTarget = nextPoint });

                        ecb.AddComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, commuter);
                    }
                    else
                    {
                        var navPoints = ecb.AddBuffer<NavPointBufferElementData>(entityInQueryIndex, commuter);

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

                        ecb.AddComponent(entityInQueryIndex, commuter, new CommuterTask_MoveToPlatform() { TargetPlatform = nextPlatform });
                    }
                }

            }).Schedule();

        //BufferFromEntity<EntityBufferElementData> buffers = GetBufferFromEntity<EntityBufferElementData>(true);

        var comQueues = m_CommuterToQueues.AsParallelWriter();

        Entities
            .WithName("commuter_arriving_platform")
            .WithNone<TargetPoint>()
            .WithAll<CommuterTask_MoveToQueue>()
            //.WithReadOnly(buffers)
            .ForEach((Entity entity, int entityInQueryIndex, in CommuterOnPlatform commuterOnPlatform, in Translation translation) =>
            {
                float minDistSq = float.MaxValue;
                float3 targetPosition = float3.zero;
                Entity targetQueue = Entity.Null;
                //var queueBuffer = buffers[commuterOnPlatform.Value];
                var queueBuffer = GetBuffer<EntityBufferElementData>(commuterOnPlatform.Value);
                Queues platform = GetComponent<Queues>(commuterOnPlatform.Value);
                for (int i = 0; i < platform.CarriageCount; ++i)
                {
                    var queue = queueBuffer[i];
                    float3 queueTranslation = GetComponent<LocalToWorld>(queue.Value).Position;
                    float distSq = math.distancesq(translation.Value, queueTranslation);
                    if (distSq < minDistSq)
                    {
                        minDistSq = distSq;
                        targetPosition = queueTranslation;
                        targetQueue = queue.Value;
                    }
                }
                if (targetQueue != Entity.Null)
                {
                    //comQueues.Add(new CommuterToQueue
                    //{
                    //    Commuter = entity,
                    //    Queue = targetQueue
                    //});

                    var commutersBuffer = GetBuffer<EntityBufferElementData>(targetQueue);
                    int n = commutersBuffer.Add(new EntityBufferElementData
                    {
                        Value = entity
                    });
                    targetPosition.z += (n - 1) * 0.5f;

                    ecb.AddComponent(entityInQueryIndex, entity, new TargetPoint
                    {
                        CurrentTarget = targetPosition
                    });

                    ecb.RemoveComponent<CommuterTask_MoveToQueue>(entityInQueryIndex, entity);
                    ecb.AddComponent<CommuterTask_WaitOnQueue>(entityInQueryIndex, entity);
                } // todo: else error?
                  //}).ScheduleParallel();
            }).Schedule();

        //Job.WithCode(() =>
        //    {
        //        while(!m_CommuterToQueues.IsEmpty())
        //        {
        //            comQueues.
        //            Entity commuter = commuterToQueue.Commuter;
        //            Entity queue = commuterToQueue.Queue;
        //            Translation translation = GetComponent<Translation>(queue);
        //            var commutersBuffer = GetBuffer<EntityBufferElementData>(queue);
        //            int n = commutersBuffer.Add(new EntityBufferElementData
        //            {
        //                Value = commuter
        //            });
        //            float3 targetPosition = translation.Value;
        //            targetPosition.z += (n - 1) * 0.2f;
        //            ecb.AddComponent(commuter.Index, commuter, new TargetPoint
        //            {
        //                CurrentTarget = targetPosition
        //            });
        //        }
        //    }).Schedule();


        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
