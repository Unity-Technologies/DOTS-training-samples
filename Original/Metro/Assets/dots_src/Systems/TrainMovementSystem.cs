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
            float maxSpeed = settings.MaxSpeed * deltaTime / splineData.Value.splineBlobAssets[lineIndex.Index].length * 1000;

            ref var points = ref splineData.Value.splineBlobAssets[lineIndex.Index].points;
            ref var platformPositions = ref splineData.Value.splineBlobAssets[lineIndex.Index].platformPositions;

            // TODO: platforms are positioned 0..1 range (relative to total track length). currently our train
            // position is absolute in terms of track length, e.g. 0..25 
            var trackRelativePosition = movement.position / points.Length;
            if (movement.state == TrainMovemementStates.Running
                && IsApproachingPlatform(trackRelativePosition, ref platformPositions))
            {
                // Debug.Log($"Train on line #{lineIndex.Index} is approaching platform, slowing down!");
                movement.state = TrainMovemementStates.Stopping;
            }

            switch (movement.state)
            {
                case TrainMovemementStates.Starting:
                    movement.speed += maxSpeed / 10;
                    if (movement.speed >= maxSpeed)
                    {
                        movement.state = TrainMovemementStates.Running;
                    }
                    break;
                
                case TrainMovemementStates.Stopping:
                    movement.speed -= maxSpeed / 10;
                    if (movement.speed <= 0.0f)
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
        // }).WithoutBurst().Run(); // for debugging
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

    public static (float3, quaternion) TrackPositionToWorldPosition(float trackPosition, ref BlobArray<float3> points)
    {
        var floor = (int)math.floor(trackPosition%points.Length);
        var ceil = (floor+1)%points.Length;
        
        return (math.lerp(points[floor], points[ceil], math.frac(trackPosition)), 
                quaternion.LookRotation(points[floor] - points[ceil], math.up()));
    }
}
