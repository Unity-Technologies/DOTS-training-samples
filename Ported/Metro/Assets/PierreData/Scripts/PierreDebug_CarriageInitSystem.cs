using Unity.Burst;
using Unity.Entities;

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
        foreach (var (queue, queueState) in SystemAPI.Query<Queue, RefRW<QueueState>>())
        {
            foreach (var (carriage, carriageEntity) in SystemAPI.Query<Carriage>().WithEntityAccess())
            {
                if (queue.FacingCarriageNumber == carriage.CarriageNumber)
                {
                    queueState.ValueRW.FacingCarriage = carriageEntity;
                }
            }
        }

        state.Enabled = false;
    }
}
