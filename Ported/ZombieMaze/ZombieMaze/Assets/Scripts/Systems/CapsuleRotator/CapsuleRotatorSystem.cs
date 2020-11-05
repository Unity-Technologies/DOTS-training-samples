using System.Diagnostics;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CapsuleRotatorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;

        Entities.WithAll<CapsuleRotation>().ForEach((ref CapsuleRotation capsuleRotation, ref Rotation rotation) =>
        {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(5 * (float)time));

        }).Schedule();
    }
}
