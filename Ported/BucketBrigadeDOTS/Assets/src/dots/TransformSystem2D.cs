using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem2D : SystemBase
{
    protected override void OnUpdate()
    {
        float3 firefighterScale = new float3(0.2f, 0.5f, 0.2f);

        Entities
            .WithChangeFilter<Translation2D>()
            .ForEach((ref LocalToWorld localToWorld, in Translation2D translation2D) =>
        {
            var trans = float4x4.Translate(new float3(translation2D.Value.x, 0, translation2D.Value.y));
            var scale = float4x4.Scale(firefighterScale);
            localToWorld.Value = math.mul(trans, scale);
        }).ScheduleParallel();
    }
}
