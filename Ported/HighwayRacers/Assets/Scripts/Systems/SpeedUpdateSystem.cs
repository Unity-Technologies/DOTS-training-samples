using Unity.Entities;
using Unity.Mathematics;

public class SpeedUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        CarConfigurations carConfig = GetSingleton<CarConfigurations>();
        float dtime = Time.DeltaTime;

        // deccelerate in case the car is blocked
        Entities.WithAll<BlockedState>().ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            speed.Value -= carConfig.Decceleration * dtime; 
            speed.Value = math.max(carInFront.Speed, math.max(speed.Value, 0));
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