using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithName("L2WFromPositionXZ")
            .WithChangeFilter<PositionXZ>()
            .ForEach((ref LocalToWorld l2w, in PositionXZ pos) => l2w.Value = float4x4.Translate(new float3(pos.Value.x, 0f, pos.Value.y)))
            .ScheduleParallel();
    }
}
