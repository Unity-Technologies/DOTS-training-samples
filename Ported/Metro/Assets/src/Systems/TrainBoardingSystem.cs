using Unity.Entities;
using Unity.Mathematics;

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
            //.WithNone<TargetPoint>()
            .ForEach((Entity platform, int entityInQueryIndex, in PlatformBoardingTrain platformTrain, in Queues platformCarriageCount) =>
            {
                var queueBuffer = GetBuffer<QueueBufferElementData>(platform);

                for (int i = 0; i < platformCarriageCount.CarriageCount; ++i)
                {
                    var queue = queueBuffer[i].Value;
                    var commutersBuffer = GetBuffer<CommuterInQueueBufferElementData>(queue);
                    int boardingCount = 2;
                    for (int j = 0; j < commutersBuffer.Length; ++j)
                    {
                        var commuter = commutersBuffer[j].Value;
                        if (j < boardingCount)
                        {
                            ecb.AddComponent(commuter.Index, commuter, new TargetPoint
                            {
                                CurrentTarget = new float3(0, 0, 0)
                            });
                            ecb.AddComponent<CommuterTask_BoardTrain>(commuter.Index, commuter);
                        }
                        else
                        {
                            ecb.RemoveComponent<TargetPoint>(commuter.Index, commuter);
                            ecb.AddComponent<CommuterTask_MoveToQueue>(commuter.Index, commuter);
                        }
                        ecb.RemoveComponent<CommuterTask_WaitOnQueue>(commuter.Index, commuter);
                    }
                    commutersBuffer.Clear();
                }

                ecb.RemoveComponent<PlatformBoardingTrain>(entityInQueryIndex, platform);
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
