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
        var elapsedTime = Time.ElapsedTime;
        
        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref TrainMovement movement, in LineIndex lineIndex) =>
        {
            const float maxSpeed = 0.05f; //from global settings singleton

            ref var points = ref splineData.Value.splineBlobAssets[lineIndex.Index].points;
            ref var platformPositions = ref splineData.Value.splineBlobAssets[lineIndex.Index].platformPositions;

            // TODO: platforms are positioned 0..1 range (relative to total track length). currently our train
            // position is absolute in terms of track length, e.g. 0..25 
            var trackRelativePosition = movement.position / points.Length;
            if (IsApproachingPlatform(trackRelativePosition, ref platformPositions))
            {
                Debug.Log($"Train on line #{lineIndex.Index} is approaching platform, slowing down!");
                movement.state = TrainMovemementStates.Stopping;
            }

            switch (movement.state)
            {
                case TrainMovemementStates.Starting:
                    movement.speed += maxSpeed / 100;
                    if (movement.speed >= maxSpeed)
                    {
                        movement.state = TrainMovemementStates.Running;
                    }
                    break;
                
                case TrainMovemementStates.Stopping:
                    movement.speed -= maxSpeed / 100;
                    if (movement.speed <= 0.0f)
                    {
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
                    if (elapsedTime > movement.timeWhenStoppedAtPlatform + 1)
                    {
                        movement.state = TrainMovemementStates.Starting;
                    }
                    break;

                case TrainMovemementStates.Running:
                    movement.speed = maxSpeed;
                    break;
            }

            movement.position += movement.speed;
            if (movement.position > points.Length - 1)
            {
                movement.position = 0;
            }

            (float3 lerpedPosition, _) = TrackPositionToWorldPosition(movement.position, ref points);
            translation.Value = lerpedPosition;
        }).ScheduleParallel();
    }

    static bool IsApproachingPlatform(float position, ref BlobArray<float> platformPositions)
    {
        // platformPositions contains ends of platforms, in range 0..1 of whole track
        for (int i = 0; i < platformPositions.Length; i++)
        {
            var platformPosition = platformPositions[i];
            if (position > platformPosition - 0.05 && position < platformPosition)
            {
                return true;
            }
        }

        return false;
    }

    public static (float3, Quaternion) TrackPositionToWorldPosition(float trackPosition, ref BlobArray<float3> points)
    {
        var floor = (int)math.floor(trackPosition);
        var ceil = (int)math.ceil(trackPosition);
        if (floor == ceil)
        {
            ceil += 1;
            
            // check for overflow
            if (ceil >= points.Length)
            {
                ceil = 0;
            }
        }

        float3 from = points[floor];
        float3 to = points[ceil];

        float t = trackPosition - math.floor(trackPosition);

        float3 lerpedPosition = math.lerp(from, to, t);
        
        float3 trainDirection = math.normalize(from - to);
        var vector = new Vector3(trainDirection.x, trainDirection.y, trainDirection.z);
        Quaternion rotation = Quaternion.LookRotation(vector);
        
        return (lerpedPosition, rotation);
    }
}
