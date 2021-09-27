using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TrainMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        
        Entities.WithAll<TrainMovement>().ForEach((ref Translation translation, ref TrainMovement movement) =>
        {
            float speed = 0.01f; //from global settings singleton

            movement.position += speed;

            float3 from = splineData.Value.points[(int)math.floor(movement.position)];
            float3 to = splineData.Value.points[(int)math.ceil(movement.position)];

            float t = movement.position - math.floor(movement.position);

            float3 lerpedPosition = math.lerp(from, to, t); 

            translation.Value = lerpedPosition;

        }).ScheduleParallel();
    }
}
