using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[UpdateAfter(typeof(CommuterSpawner))] 
[BurstCompile]
public partial struct PierreDebug_CarriageInitSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (platform, platformEntity) in SystemAPI.Query<RefRO<Platform>>().WithEntityAccess())
        {
            var queues = SystemAPI.GetBuffer<PlatformQueue>(platformEntity);
            foreach (var queueEntity in queues)
            {
                var queue = SystemAPI.GetComponent<Queue>(queueEntity.Queue);
                foreach (var (carriage, parent, carriageEntity) in SystemAPI.Query<RefRW<Carriage>, Parent>().WithEntityAccess())
                {
                    if (parent.Value == platform.ValueRO.ParkedTrain &&
                        carriage.ValueRO.CarriageNumber == queue.FacingCarriageNumber)
                    {
                        carriage.ValueRW.CurrentPlatform = platformEntity;
                        SystemAPI.SetComponent(queueEntity.Queue, new QueueState()
                        {
                            FacingCarriage = carriageEntity
                        });
                    }
                }
            }
        }

        state.Enabled = false;
    }
}
