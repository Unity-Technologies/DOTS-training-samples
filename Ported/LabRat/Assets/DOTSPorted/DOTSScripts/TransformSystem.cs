using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SpawningSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref Translation translation, in Direction direction, in Position position) =>
        {
            translation.Value = new float3(position.position, 0.0f);
            rotation.Value = quaternion.RotateY(math.radians((float)direction.Value * 90));
        }).Schedule();
    }
}
