using Components;
using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PassengerSystemGroup))]
// [UpdateAfter(typeof(PassangerOnTrainSystem))]
public partial struct PassengerWalkingToSeatSystem : ISystem
{
    public float3 Y;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();

        Y = new float3(0, 1f, 0);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var movePassengerToSeatJob = new MovePassengerToSeatJob
        {
            PassengerSpeed = SystemAPI.Time.DeltaTime * config.PassengerSpeed,
            Y = Y
        };

        state.Dependency = movePassengerToSeatJob.ScheduleParallel(state.Dependency);

    }
}

[BurstCompile]
public partial struct MovePassengerToSeatJob : IJobEntity
{
    public float PassengerSpeed;
    public float3 Y;

    public void Execute(ref LocalTransform transform, ref PassengerComponent passengerInfo,
        EnabledRefRW<PassengerWalkingToSeat> walkingState)
    {
        var toDst = passengerInfo.SeatPosition - passengerInfo.RelativePosition;
        var dist = math.length(toDst);
        
        if (dist < 0.1f)
        {
            walkingState.ValueRW = false;
        }
        else
        {
            var toDstDirection = math.normalize(toDst);
            passengerInfo.RelativePosition += toDstDirection * PassengerSpeed;
        
            var rotationAngle = math.acos(math.dot(Y, toDstDirection));
            transform.Rotation = quaternion.RotateY(rotationAngle);
        }
    }
}

