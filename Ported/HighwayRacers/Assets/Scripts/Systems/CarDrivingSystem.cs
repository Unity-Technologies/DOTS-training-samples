using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(CollisionSystem))]
partial struct CarDrivingSystem : ISystem
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
        var updateVelocityJob = new UpdateVelocityJob();
        updateVelocityJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct UpdateVelocityJob : IJobEntity
{
    [BurstCompile]
    void Execute(ref CarVelocity velocity, in CarCollision collision)
    {
        if (collision.Front)
        {
            velocity.VelY = collision.FrontVelocity;
        }
    }
} 