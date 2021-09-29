using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TrainMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settings = GetSingleton<Settings>();
        var elapsedTime = Time.ElapsedTime;
        var deltaTime = Time.DeltaTime;
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref TrainMovement movement, in LineIndex lineIndex) =>
        {
            float lineLength = splineData.Value.splineBlobAssets[lineIndex.Index].length;
            float breakingPoint = 0.05f;

            ref var points = ref splineData.Value.splineBlobAssets[lineIndex.Index].points;
            ref var platformPositions = ref splineData.Value.splineBlobAssets[lineIndex.Index].platformPositions;
            var trackRelativePosition = movement.position / points.Length;

            switch (movement.state)
            {
                case TrainMovemementStates.Starting:
                    movement.speed += settings.MaxSpeed / 100 * 5;
                    if (movement.speed >= settings.MaxSpeed)
                    {
                        movement.state = TrainMovemementStates.Running;
                    }
                    break;
                
                case TrainMovemementStates.Stopping:
                    // following are in meters 
                    float distanceTraveledLastFrame = movement.speed * deltaTime;
                    var distanceToPlatform = DistanceToClosestPlatform(trackRelativePosition, ref platformPositions) * lineLength;
                    float framesUntilStop = distanceToPlatform/ distanceTraveledLastFrame;

                    Debug.Log(distanceToPlatform);
                    var reduceSpeedWith = math.max(0.1f * deltaTime, movement.speed / framesUntilStop); 
                    movement.speed -= reduceSpeedWith;

                    if (movement.speed <= 0.0f || distanceToPlatform < 1f)
                    {
                        movement.speed = 0.0f;
                        movement.state = TrainMovemementStates.Stopped;
                    }
                    break;
                
                case TrainMovemementStates.Stopped:
                    movement.speed = 0.0f;
                    movement.timeWhenStoppedAtPlatform = elapsedTime;
                    movement.state = TrainMovemementStates.Waiting;
                    break;
                
                case TrainMovemementStates.Waiting:
                    movement.speed = 0.0f;
                    if (elapsedTime > movement.timeWhenStoppedAtPlatform + settings.TimeAtStation)
                    {
                        movement.state = TrainMovemementStates.Starting;
                    }
                    break;

                case TrainMovemementStates.Running:
                    movement.speed = settings.MaxSpeed;

                    // are we approaching platform?
                    var relativeDistanceToPlatform = DistanceToClosestPlatform(trackRelativePosition, ref platformPositions);
                    var distanceToPlatformInMeters = relativeDistanceToPlatform * lineLength;

                    if (distanceToPlatformInMeters < 50)
                    {
                        movement.state = TrainMovemementStates.Stopping;
                    }

                    break;
            }

            // convert speed (m/s) to track positions/second
            var trackPositionsSpeed = movement.speed / lineLength * points.Length;
            movement.position += trackPositionsSpeed * deltaTime;
            if (movement.position > points.Length - 1)
            {
                movement.position = 0;
            }

            (float3 lerpedPosition, _) = TrackPositionToWorldPosition(movement.position, ref points);
            translation.Value = lerpedPosition;
        }).WithoutBurst().Run(); // for debugging
        // }).ScheduleParallel();
    }

    /// <summary>
    /// Returns distance to next platform in relative points, i.e. range [0..1[ 
    /// </summary>
    static float DistanceToClosestPlatform(float position, ref BlobArray<float> platformPositions)
    {
        for (int i = 0; i < platformPositions.Length; i++)
        {
            var platformPosition = platformPositions[i];
            if (position < platformPosition)
            {
                return platformPosition - position;
            }
        }

        return 1 - position + platformPositions[0];
    }

    public static (float3, Quaternion) TrackPositionToWorldPosition(float trackPosition, ref BlobArray<float3> points)
    {
        var floor = (int)math.floor(trackPosition);
        var ceil = (int)math.ceil(trackPosition);

        float3 from = points[floor];
        float3 to = points[ceil];

        while (from.Equals(to))
        {
            ceil += 1;
            
            // check for overflow
            if (ceil >= points.Length)
            {
                ceil = 0;
            }
            to = points[ceil];
        }
        
        float t = trackPosition - math.floor(trackPosition);

        float3 lerpedPosition = math.lerp(from, to, t);
        
        float3 trainDirection = math.normalize(from - to);
        var vector = new Vector3(trainDirection.x, trainDirection.y, trainDirection.z);
        Quaternion rotation = Quaternion.LookRotation(vector);
        
        return (lerpedPosition, rotation);
    }
}
