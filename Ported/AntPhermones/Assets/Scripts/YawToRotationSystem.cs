using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class YawToRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, in Yaw yaw) =>
            {
                rotation.Value = quaternion.RotateY(yaw.Value);
            }
        ).Run();
    }
}
