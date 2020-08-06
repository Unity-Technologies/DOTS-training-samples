using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class Transform2DSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // produce LocalToWorld matrix from Position2D and 
        Entities
            .WithChangeFilter<Position2D>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D translation2D) =>
        {
            var trans = float4x4.Translate(new float3(translation2D.position.x, 0, translation2D.position.y));
            // var scale = float4x4.Scale(new float3(scale2D.Value, 1));
            localToWorld.Value = trans;
        }).ScheduleParallel();

        // For the plants
        float t = (float)Time.ElapsedTime;
        Entities
            .ForEach((ref LocalToWorld localToWorld, in Translation translation, in Size size) =>
        {
            var trans = float4x4.Translate(translation.Value);
            var scale = float4x4.Scale(new float3(size.value, size.value, size.value));
            localToWorld.Value = math.mul(trans, scale);
        }).ScheduleParallel();
    }
}
