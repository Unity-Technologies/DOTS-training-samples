using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : SystemBase
{
    private EntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        this.ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Entities.WithName("L2WFromPositionXZWithSize")
            .WithChangeFilter<PositionXZ, Size>()
            .WithAll<Size>()
            .ForEach((ref LocalToWorld l2w, in PositionXZ pos, in Size size) =>
            {
                var trans = float4x4.Translate(new float3(pos.Value.x, 0f, pos.Value.y));
                var scale = float4x4.Scale(size.Value);
                l2w.Value = math.mul(trans, scale);
            })
            .ScheduleParallel();


        Entities.WithName("L2WFromPositionXZ")
          //  .WithStructuralChanges() TODO(ddebaets) is this needed ??
            .WithChangeFilter<PositionXZ>()
            .WithNone<Size>()
            .ForEach((ref LocalToWorld l2w, in PositionXZ pos) =>
            {
                var trans = float4x4.Translate(new float3(pos.Value.x, 0f, pos.Value.y));
                l2w.Value = trans;
            })
            .ScheduleParallel();
    }
}