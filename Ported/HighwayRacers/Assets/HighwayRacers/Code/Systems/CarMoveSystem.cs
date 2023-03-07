using Unity.Burst;
using Unity.Entities;
using Jobs;
using Unity.Jobs;

[BurstCompile]
public partial struct CarMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
     
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (state.Dependency != null)
        {
            state.Dependency.Complete();
        }

        var config = SystemAPI.GetSingleton<Config>();
        var testJob = new CarMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        JobHandle jobHandle = testJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;        
    }
}