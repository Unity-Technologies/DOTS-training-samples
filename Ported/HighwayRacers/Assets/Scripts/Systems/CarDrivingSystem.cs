using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(CarCollisionSystem))]
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
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        var updateVelocityJob = new UpdateVelocityJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            LaneLength = globalSettings.LengthLanes

        };
        updateVelocityJob.ScheduleParallel();
    }
}

[BurstCompile]
partial struct UpdateVelocityJob : IJobEntity
{
    public float DeltaTime;
    public float LaneLength;
    
    [BurstCompile]
    void Execute(ref CarVelocity velocity, ref CarPositionInLane positionInLane, in CarOvertakeState overtakeState,
        in CarCollision collision, in CarDefaultValues defaults)
    {
        if (collision.Front)
        {
            velocity.VelY = collision.FrontVelocity * (collision.FrontDistance > 0.1f ? 0 : 1) ; //math.clamp(collision.FrontVelocity - collision.FrontDistance, 0.0f, defaults.DefaultVelY);
        }
        else
        {
            velocity.VelY = defaults.DefaultVelY; //default
        }

        velocity.VelX = overtakeState.ChangingLane ? 5f * math.sign(positionInLane.LaneIndex - overtakeState.OriginalLane) : 0;

        //Convert setting

        positionInLane.Position = (positionInLane.Position + velocity.VelY * DeltaTime) % LaneLength;
        positionInLane.LanePosition += velocity.VelX * DeltaTime;
    }
}