using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public partial class AimSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var up = new float3(0, 1, 0);

        Entities
            .WithNone<Gravity, Decay>()
            .ForEach((ref Velocity velocity, in Translation translation, in Goal goal) =>
        {
            velocity.Value = 4.0f * math.normalize(velocity.Value + math.normalize(goal.target - translation.Value) * 0.2f);
        }).Schedule();

        Entities
            .WithNone<Gravity, Decay>()
            .ForEach((ref Rotation rotation, in Velocity velocity) =>
        {
            rotation.Value = Quaternion.FromToRotation(Vector3.up, math.normalize(velocity.Value));
        }).Schedule();
    }
}
