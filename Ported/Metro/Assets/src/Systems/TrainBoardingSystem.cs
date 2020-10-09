using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainBoardingSystem : SystemBase
{
    private const int UNBOARD_PERCENTAGE = 40;

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

                    var carriageCenter = GetComponent<CarriageCenterSeat>(carriage);
                    var carriageSeats = GetBuffer<CarriageSeat>(carriage);

                    int numValidSeats = math.min(carriageCommuterBuffer.Length, carriageSeats.Length);

                    // Get as many commuters as possible in
                    int queueCommuterIndex = 0;
                    for (int j = 0; j < numValidSeats && queueCommuterIndex < queueCommuterBuffer.Length; j++)
                    {
                        if(carriageCommuterBuffer[j].Value != Entity.Null)
                            continue;

                        var commuter = queueCommuterBuffer[queueCommuterIndex].Value;
                        queueCommuterIndex++;

                        carriageCommuterBuffer[j] = new CommuterInCarriageBufferElementData { Value = commuter };
                        
                        ecb.RemoveComponent<CommuterTask_WaitOnQueue>(commuter.Index, commuter);
                        ecb.AddComponent(commuter.Index, commuter, new CommuterTask_BoardTrain { Carriage = carriage, SeatIndex = j });

                        var navPoints = ecb.AddBuffer<NavPointBufferElementData>(commuter.Index, commuter);
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = GetComponent<LocalToWorld>(carriageCenter.Value).Position });
                        navPoints.Add(new NavPointBufferElementData() { NavPoint = GetComponent<LocalToWorld>(carriageSeats[j].Value).Position });
                    }

                    // Return the rest to MoveToQueue so they queue again
                    // Move commuters into the train
                    for (int j = queueCommuterIndex; j < queueCommuterBuffer.Length; ++j)
                    {
                        var commuter = queueCommuterBuffer[j].Value;

                        ecb.RemoveComponent<CommuterTask_WaitOnQueue>(commuter.Index, commuter);
                        ecb.AddComponent<CommuterTask_MoveToQueue>(commuter.Index, commuter);
                        
                        ecb.RemoveComponent<TargetPoint>(commuter.Index, commuter);
                    }

                    queueCommuterBuffer.Clear();
                }

                ecb.RemoveComponent<PlatformBoardingTrain>(entityInQueryIndex, platform);
            }).Schedule();

        uint timeOffset = (uint)System.DateTime.Now.Millisecond;

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
                    var carriageCenter = GetComponent<CarriageCenterSeat>(carriage);

                    var random = new Random((uint)carriage.Index + timeOffset);

                    for (int j = 0; j < carriageCommuterBuffer.Length; ++j)
                    {
                        var commuter = carriageCommuterBuffer[j].Value;
                        if (commuter != Entity.Null)
                        {
                            if(random.NextInt(100) < UNBOARD_PERCENTAGE)
                            {
                                carriageCommuterBuffer[j] = new CommuterInCarriageBufferElementData() { Value = Entity.Null };

                                ecb.RemoveComponent<CommuterTask_WaitInCarriage>(commuter.Index, commuter);
                                ecb.AddComponent(commuter.Index, commuter, new CommuterTask_UnboardTrain { Platform = platform });

                                var navPoints = ecb.AddBuffer<NavPointBufferElementData>(commuter.Index, commuter);
                                navPoints.Add(new NavPointBufferElementData() { NavPoint = GetComponent<LocalToWorld>(carriageCenter.Value).Position });
                                navPoints.Add(new NavPointBufferElementData() { NavPoint = queueTransform.Position });
                            }
                        }
                        carriageCommuterBuffer[j] = new CommuterInCarriageBufferElementData { Value = Entity.Null };
                    }
                }

                ecb.RemoveComponent<PlatformUnboardingTrain>(entityInQueryIndex, platform);
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
