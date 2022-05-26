using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class CarMovementSystem : SystemBase
{
    private const float MIN_DIST_BETWEEN_CARS = 2f;

    protected override void OnCreate()
    {
        RequireForUpdate<TrackConfig>();

        base.OnCreate();        
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        TrackConfig track = SystemAPI.GetSingleton<TrackConfig>();

        Entities
            .ForEach((Entity entity, ref TransformAspect transform, ref CarPosition carPos, ref CarSpeed carSpeed, in CarChangingLanes carCC, in CarAICache car, in CarProperties properties) =>
            {
                TrackUtilities.GetCarPosition(track.highwaySize, carPos.distance, carPos.currentLane,
                    out float posX, out float posZ, out float outRotation);

                if (carCC.FromLane != carCC.ToLane)
                {
                    float fromDistance = TrackUtilities.GetEquivalentDistance(track.highwaySize, carPos.distance, carCC.ToLane, carCC.FromLane);
                    TrackUtilities.GetCarPosition(track.highwaySize, fromDistance, carCC.FromLane, out float fromX, out float fromZ, out float fromRot);
                    posX = (1.0f - carCC.Progress) * fromX + carCC.Progress * posX;
                    posZ = (1.0f - carCC.Progress) * fromZ + carCC.Progress * posZ;
                    outRotation = (1.0f - carCC.Progress) * fromRot + carCC.Progress * outRotation;
                }

                var pos = transform.Position;
                transform.Position = new float3(posX, pos.y, posZ);
                transform.Rotation = quaternion.RotateY(outRotation);

                float targetSpeed = properties.desiredSpeed;
                if (car.DistanceAhead < properties.minDistanceInFront)
                {
                    targetSpeed = math.min(properties.desiredSpeed, car.CarInFrontSpeed);
                }

                if (targetSpeed > carSpeed.currentSpeed)
                {
                    carSpeed.currentSpeed = math.min(targetSpeed, carSpeed.currentSpeed + properties.acceleration * dt);
                    carSpeed.currentSpeed = math.min(targetSpeed, carSpeed.currentSpeed);
                }
                else if (targetSpeed < carSpeed.currentSpeed)
                {
                    carSpeed.currentSpeed = math.max(targetSpeed, carSpeed.currentSpeed - properties.braking * dt);
                }

                //Prevent a crash with car in front
                if (car.CarInFront != null && dt > 0)
                {                    
                    if (car.DistanceAhead < 0.1f)
                    {
                        Debug.Log("Oh no a crash");
                    }
                    float maxDistanceDiff = math.max(0, car.DistanceAhead - MIN_DIST_BETWEEN_CARS);
                    carSpeed.currentSpeed = math.min(carSpeed.currentSpeed, maxDistanceDiff / dt);
                }

                carPos.distance += dt * carSpeed.currentSpeed;
            }).ScheduleParallel();
    }
}
