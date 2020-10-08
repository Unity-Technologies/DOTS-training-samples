using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainBoardingSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithName("train_start_boarding")
            .ForEach((Entity platform, int entityInQueryIndex, in PlatformBoardingTrain platformTrain) =>
            {
                var queueBuffer = GetBuffer<QueueBufferElementData>(platform);
                var carriageBuffer = GetBuffer<BufferCarriage>(platformTrain.Train);

                int carriageCount = carriageBuffer.Length;
                for (int i = 0; i < carriageCount; ++i)
                {
                    var queue = queueBuffer[i].Value;
                    var queueCommuterBuffer = GetBuffer<CommuterInQueueBufferElementData>(queue);

                    var carriage = carriageBuffer[i].Value;
                    var carriageCommuterBuffer = GetBuffer<CommuterInCarriageBufferElementData>(carriage);
                    
                    var carriageTransform = GetComponent<LocalToWorld>(carriage);

                    // Count empty seats
                    int boardingCount = 0;
                    for (int j = 0; j < carriageCommuterBuffer.Length; ++j)
                        if (carriageCommuterBuffer[j].Value == Entity.Null)
                            boardingCount++;

                    // Move commuters into the train
                    for (int j = 0; j < queueCommuterBuffer.Length; ++j)
                    {
                        var commuter = queueCommuterBuffer[j].Value;

                        if (j < boardingCount)
                        {
                            carriageCommuterBuffer[j] = new CommuterInCarriageBufferElementData { Value = commuter };

                            ecb.AddComponent(commuter.Index, commuter, new TargetPoint
                            {
                                // todo: select an empty spot and choose that position
                                CurrentTarget = carriageTransform.Position
                            });
                            ecb.AddComponent(commuter.Index, commuter, new CommuterTask_BoardTrain { Carriage = carriage });
                        }
                        else
                        {
                            ecb.RemoveComponent<TargetPoint>(commuter.Index, commuter);
                            ecb.AddComponent<CommuterTask_MoveToQueue>(commuter.Index, commuter);
                        }

                        ecb.RemoveComponent<CommuterTask_WaitOnQueue>(commuter.Index, commuter);
                    }

                    queueCommuterBuffer.Clear();
                }

                ecb.RemoveComponent<PlatformBoardingTrain>(entityInQueryIndex, platform);
            }).Schedule();

        Entities
            .WithName("train_start_unboarding")
            .ForEach((Entity platform, int entityInQueryIndex, in PlatformUnboardingTrain platformTrain) =>
            {
                var queueBuffer = GetBuffer<QueueBufferElementData>(platform);
                var carriageBuffer = GetBuffer<BufferCarriage>(platformTrain.Train);

                int carriageCount = carriageBuffer.Length;
                for (int i = 0; i < carriageCount; ++i)
                {
                    var queue = queueBuffer[i].Value;
                    var queueTransform = GetComponent<LocalToWorld>(queue);

                    var carriage = carriageBuffer[i].Value;
                    var carriageCommuterBuffer = GetBuffer<CommuterInCarriageBufferElementData>(carriage);

                    for (int j = 0; j < carriageCommuterBuffer.Length; ++j)
                    {
                        // todo: decide which ones unboard
                        var commuter = carriageCommuterBuffer[j].Value;
                        if (commuter != Entity.Null)
                        {
                            ecb.AddComponent(commuter.Index, commuter, new TargetPoint { CurrentTarget = queueTransform.Position });
                            ecb.RemoveComponent<CommuterTask_WaitInCarriage>(commuter.Index, commuter);
                            ecb.AddComponent(commuter.Index, commuter, new CommuterTask_UnboardTrain { Platform = platform });

                            carriageCommuterBuffer[j] = new CommuterInCarriageBufferElementData() { Value = Entity.Null };
                        }
                    }
                }

                ecb.RemoveComponent<PlatformUnboardingTrain>(entityInQueryIndex, platform);
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
