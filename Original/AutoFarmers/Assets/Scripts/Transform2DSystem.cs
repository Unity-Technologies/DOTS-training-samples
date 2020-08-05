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
            .ForEach((ref LocalToWorld localToWorld, in Position2D translation2D) => // TODO: size and scale components
        {
            var trans = float4x4.Translate(new float3(translation2D.position.x, 0, translation2D.position.y));
            // var scale = float4x4.Scale(new float3(scale2D.Value, 1));
            localToWorld.Value = trans;
        }).ScheduleParallel();
    }
}
