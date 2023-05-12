using Components;
using Metro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PassengerMovingOnTrainSystem : ISystem
{
    public float3 Y;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();

        Y = new float3(0, 1f, 0);
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        foreach (var (transform, passengerInfo, passenger) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<PassengerComponent>>()
                     .WithAll<PassengerWalkingToSeat>()
                     .WithEntityAccess())
        {
            var toDst = passengerInfo.ValueRO.SeatPosition - passengerInfo.ValueRO.RelativePosition;
            var dist = math.length(toDst);
        
            if (dist < 0.01f)
            {
                state.EntityManager.SetComponentEnabled<PassengerWalkingToSeat>(passenger, false);
            }
            else
            {
                var toDstDirection = math.normalize(toDst);
                passengerInfo.ValueRW.RelativePosition += toDstDirection * config.PassengerSpeed * SystemAPI.Time.DeltaTime;
        
                var rotationAngle = math.acos(math.dot(Y, toDstDirection));
                transform.ValueRW.Rotation = quaternion.RotateY(rotationAngle);
            }
        }
        
        foreach (var (transform, passengerInfo, passenger) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<PassengerComponent>>()
                     .WithAll<PassengerWalkingToDoor>()
                     .WithEntityAccess())
        {
            var toDst = passengerInfo.ValueRO.ExitPosition - passengerInfo.ValueRO.RelativePosition;
            var dist = math.length(toDst);
        
            if (dist < 0.01f)
            {
                state.EntityManager.SetComponentEnabled<PassengerWalkingToDoor>(passenger, false);
                state.EntityManager.SetComponentEnabled<PassengerWaitingToExit>(passenger, true);
            }
            else
            {
                var toDstDirection = math.normalize(toDst);
                passengerInfo.ValueRW.RelativePosition += toDstDirection * config.PassengerSpeed * SystemAPI.Time.DeltaTime;
        
                var rotationAngle = math.acos(math.dot(Y, toDstDirection));
                transform.ValueRW.Rotation = quaternion.RotateY(rotationAngle);
            }
        }
    }
}
