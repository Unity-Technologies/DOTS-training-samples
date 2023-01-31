using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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
        var updateVelocityJob = new UpdateVelocityJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        updateVelocityJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct UpdateVelocityJob : IJobEntity
{
    
    public float DeltaTime;

    [BurstCompile]
    void Execute(ref CarVelocity velocity, ref CarPositionInLane positionInLane, in CarCollision collision, in CarDefaultValues defaults)
    {
        if (collision.Front)
        {

            velocity.VelY =
                collision.FrontVelocity; //math.clamp(collision.FrontVelocity - collision.FrontDistance, 0.0f, defaults.DefaultVelY);
        }
        else
        {
            velocity.VelY = defaults.DefaultVelY;//default
        }
        
        positionInLane.Position += velocity.VelY * DeltaTime;
        positionInLane.Lane += velocity.VelX * DeltaTime;
    }
} 