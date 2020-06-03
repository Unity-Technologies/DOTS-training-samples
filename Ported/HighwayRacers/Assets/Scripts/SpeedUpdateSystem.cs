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
            in TrackPosition trackPosition, in CarProperties carProperties, in CarInFront carInFront) =>
        {
            // 1. if carInFront is faster 
                // => accelerate
            // 2. else 
                // t = (V-V0) / a -- time to slow down

             // s = v0 * t + 1/2 * a * t^2 

       //     { // blocked state check
                float v = carInFront.Speed; 
                float v0 = speed.Value;

                float timeToSlowDown = (v-v0) / carProperties.Acceleration;
                float spaceToSlowDown = v0 * timeToSlowDown + 0.5f * carProperties.Acceleration * timeToSlowDown * timeToSlowDown;
            
                float threshold = 1.25f;

                float distanceBetweenCars = carInFront.TrackProgressCarInFront - trackPosition.TrackProgress;
                distanceBetweenCars -= threshold; // XXX temp. replace

                // get distance and take track length into account
                distanceBetweenCars = (distanceBetweenCars + trackProperties.TrackLength) % trackProperties.TrackLength;
       //     }

            if (v > v0 || distanceBetweenCars > spaceToSlowDown) {
                // simple accelerate first
                speed.Value += carProperties.Acceleration * dtime; 

                float maxSpeed = carProperties.DefaultSpeed;
                speed.Value = math.min(v, math.min(speed.Value, maxSpeed));
            } else {
            
                speed.Value -= carProperties.Acceleration * dtime; 
                speed.Value = math.max(v, math.max(speed.Value, 0));
            }
        }).ScheduleParallel();
    }
}