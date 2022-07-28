using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct MovementJob : IJobEntity
{
    public float DeltaTime;
    public float3 Bounds;

    void Execute(in Velocity vel, ref Translation pos)
    {
        pos.Value += DeltaTime * vel.Value;
        if (pos.Value.x < -Bounds.x) pos.Value.x = -Bounds.x;
        else if (pos.Value.x > Bounds.x) pos.Value.x = Bounds.x;

        if (pos.Value.y < -Bounds.y) pos.Value.y = -Bounds.y;
        else if (pos.Value.y > Bounds.y) pos.Value.y = Bounds.y;

        if (pos.Value.z < -Bounds.z) pos.Value.z = -Bounds.z;
        else if (pos.Value.z > Bounds.z) pos.Value.z = Bounds.z;
    }
}


[BurstCompile]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var movementJob = new MovementJob
        {
            DeltaTime = state.Time.DeltaTime,
            Bounds = config.PlayVolume
        };
        movementJob.ScheduleParallel();
    }
}