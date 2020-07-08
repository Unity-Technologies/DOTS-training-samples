using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithChangeFilter<Position>()
            .ForEach((ref LocalToWorld localToWorld, in Position position) =>
            {
                var trans = float4x4.Translate(new float3(position.Value, 0));
                localToWorld.Value = trans;
            }).ScheduleParallel();
    }
}
