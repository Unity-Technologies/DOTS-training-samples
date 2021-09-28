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
        
        Entities.WithAll<TrainMovement>().ForEach((ref Translation translation, ref Rotation rotation, ref TrainMovement movement, in LineIndex lineIndex) =>
        {
            float speed = 0.05f; //from global settings singleton

            movement.position += speed;

            if (movement.position > splineData.Value.splineBlobAssets[lineIndex.Index].points.Length - 1)
            {
                movement.position = 0;
            }

            (float3 lerpedPosition, _) = TrackPositionToWorldPosition(movement.position, ref splineData.Value.splineBlobAssets[lineIndex.Index].points);
            translation.Value = lerpedPosition;
        }).ScheduleParallel();
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
