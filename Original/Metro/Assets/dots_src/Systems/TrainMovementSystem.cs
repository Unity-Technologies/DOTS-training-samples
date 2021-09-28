using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TrainMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        
        Entities.WithAll<TrainMovement>().ForEach((ref Translation translation, ref Rotation rotation, ref TrainMovement movement) =>
        {
            float speed = 0.05f; //from global settings singleton

            movement.position += speed;

            if (movement.position > splineData.Value.points.Length - 1)
            {
                movement.position = 0;
            }

            float3 from = splineData.Value.points[(int)math.floor(movement.position)];
            float3 to = splineData.Value.points[(int)math.ceil(movement.position)];

            float t = movement.position - math.floor(movement.position);

            float3 lerpedPosition = math.lerp(from, to, t); 

            translation.Value = lerpedPosition;

            float3 trainDirection = math.normalize(from - to);

            rotation.Value = Quaternion.LookRotation(new Vector3(trainDirection.x, trainDirection.y, trainDirection.z));

        }).ScheduleParallel();
    }
}
