using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct MovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moveMoversJob = new MoveMovers
        {
            dt = state.Time.DeltaTime
        };

        // Schedule execution in a single thread, and do not block main thread.
        state.Dependency = moveMoversJob.ScheduleParallel(state.Dependency);
    }
}


[BurstCompile]
partial struct MoveMovers : IJobEntity
{
    public float dt;

    void Execute(ref MovementAspect mover)
    {
        if (mover.HasDestination)
        {
            if (!mover.AtDesiredLocation)
            {
                var dir = mover.DesiredWorldLocation - mover.WorldPosition;
                mover.WorldPosition += math.normalize(dir) * dt * mover.Speed;
            }
        }
    }
}