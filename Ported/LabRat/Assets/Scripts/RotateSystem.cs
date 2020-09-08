using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithAll<Rotate>().ForEach((ref Rotation rotation, in Rotate rotate) =>
        {
            var delta = quaternion.RotateY(deltaTime * rotate.RotationSpeed);
            rotation.Value = math.normalize(math.mul(rotation.Value, delta));
        }).ScheduleParallel();
    }
 }