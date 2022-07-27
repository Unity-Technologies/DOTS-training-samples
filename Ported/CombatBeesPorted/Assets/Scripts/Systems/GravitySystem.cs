using Unity.Burst;
using Unity.Entities;

[WithAll(typeof(Gravity))]
[BurstCompile]
partial struct GravityJob : IJobEntity
{
    public float DeltaTime;
    public float GravityDown;

    void Execute(ref Velocity vel)
    {
        vel.Value.y -= DeltaTime * GravityDown;
    }
}


[BurstCompile]
public partial struct GravitySystem : ISystem
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
        float gravityDown = SystemAPI.GetSingleton<Config>().GravityDown;
        var gravityJob = new GravityJob
        {
            DeltaTime = state.Time.DeltaTime,
            GravityDown = gravityDown
        };
        gravityJob.ScheduleParallel();
    }
}