using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpeedUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        float dtime = Time.DeltaTime;

        // deccelerate in case the car is blocked
        Entities.WithAll<BlockedState>().ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            speed.Value -= carProperties.Acceleration * dtime; 
            // speed.Value = math.max(carInFront.Speed, math.max(speed.Value, 0));
            speed.Value = math.max(speed.Value, 0);
        }).ScheduleParallel();

        // Accelerate in case the car is not blocked
        Entities
            // .WithoutBurst()
            .WithNone<BlockedState>().ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            speed.Value += carProperties.Acceleration * dtime; 

            float maxSpeed = carProperties.DefaultSpeed;
            speed.Value = math.min(speed.Value, maxSpeed);
        }).ScheduleParallel();
        // }).Run();
    }
}