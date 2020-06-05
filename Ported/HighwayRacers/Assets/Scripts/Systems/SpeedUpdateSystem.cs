using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpeedUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        CarConfigurations carConfig = GetSingleton<CarConfigurations>();
        float dtime = Time.DeltaTime;

        var trackLength = trackProperties.TrackLength;
        var minDistanceToFront = carConfig.MinDistanceToFront;
        var decollisionDeceleration = carConfig.DecollisionDeceleration;

        // decelerate in case the car is blocked
        Entities.WithAll<BlockedState>().ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            speed.Value -= carConfig.Deceleration * dtime;            
            speed.Value = math.max(carInFront.Speed, math.max(speed.Value, 0));

            var distanceToFront = TrackPosition.GetLoopedDistanceInFront(trackPosition.TrackProgress, carInFront.TrackProgressCarInFront, trackLength);
            speed.Value -= math.step(distanceToFront, minDistanceToFront) * decollisionDeceleration;
            speed.Value = math.max(speed.Value, 0);
        }).ScheduleParallel();

        // Accelerate in case the car is not blocked
        Entities
            // .WithoutBurst()
            .WithNone<BlockedState>().ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            speed.Value += carConfig.Acceleration * dtime; 

            float maxSpeed = carProperties.DefaultSpeed;
            speed.Value = math.min(speed.Value, maxSpeed);
        }).ScheduleParallel();
        // }).Run();
    }
}