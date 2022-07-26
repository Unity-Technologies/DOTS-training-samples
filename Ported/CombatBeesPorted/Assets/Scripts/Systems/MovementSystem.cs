using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct MovementJob : IJobEntity
{
    public float DeltaTime;

    void Execute(in Velocity vel, ref Translation pos)
    {
        pos.Value += DeltaTime * vel.Value;
    }
}


[BurstCompile]
public partial struct MovementSystem : ISystem
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
        var movementJob = new MovementJob
        {
            DeltaTime = state.Time.DeltaTime
        };
        movementJob.ScheduleParallel();
    }
}