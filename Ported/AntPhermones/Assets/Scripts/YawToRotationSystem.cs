using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TRSToLocalToWorldSystem))]
public class YawToRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, in Translation translation, in Yaw yaw) =>
            {
                rotation.Value = quaternion.RotateY(yaw.Value + math.sin(translation.Value.z) + math.cos(translation.Value.x));
            }
        ).Run();
    }
}
