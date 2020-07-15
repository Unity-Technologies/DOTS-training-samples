using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in Rotate rotate) =>
        {
            quaternion delta = quaternion.RotateY(dt * rotate.RotationSpeed);
            rotation.Value = math.normalize(math.mul(rotation.Value, delta));
        }).ScheduleParallel();
    }
}


