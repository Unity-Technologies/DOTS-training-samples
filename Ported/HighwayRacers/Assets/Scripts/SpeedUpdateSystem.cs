using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpeedUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();
        float dtime = Time.DeltaTime;

        Entities.ForEach((ref Speed speed, 
            in TrackPosition trackPosition, in CarProperties carProperties) =>
        {
            // simple accelerate first
            speed.Value += carProperties.Acceleration * dtime; 

            float maxSpeed = carProperties.DefaultSpeed;
            speed.Value = math.min(speed.Value, maxSpeed);

        }).ScheduleParallel();
    }
}