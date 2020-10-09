using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup), OrderLast = true)]
public class SetBotLocalToWorldSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("SetLtWJobScale")
            .WithNone<Translation, Rotation, BucketOwner>()
            .ForEach((ref LocalToWorld ltw, in Pos pos, in MyScale scale) =>
        {
            ltw.Value = new float4x4(
                scale.Value, 0f,          0f,          pos.Value.x,
                0f,          scale.Value, 0f,          0f,
                0f,          0f,          scale.Value, pos.Value.y,
                0f,          0f,          0f,          1f);
        }).ScheduleParallel();
        
        Entities
            .WithName("SetLtWJob")
            .WithNone<Translation, Rotation>()
            .ForEach((ref LocalToWorld ltw, in Pos pos, in MyNonUniformScale scale) =>
            {
                ltw.Value = new float4x4(
                    scale.Value.x, 0f,            0f,            pos.Value.x,
                    0f,            scale.Value.y, 0f,            0f,
                    0f,            0f,            scale.Value.z, pos.Value.y,
                    0f,            0f,            0f,            1f);
            }).ScheduleParallel();
        
        Entities
            .WithName("SetLtWJobScaleBucket")
            .WithAny<BucketOwner, ChainPosition>()
            .WithNone<Translation, Rotation>()
            .ForEach((ref LocalToWorld ltw, in Pos pos, in MyScale scale) =>
            {
                ltw.Value = new float4x4(
                    scale.Value, 0f,          0f,          pos.Value.x,
                    0f,          scale.Value, 0f,          0.5f,
                    0f,          0f,          scale.Value, pos.Value.y,
                    0f,          0f,          0f,          1f);
            }).ScheduleParallel();
    }
}
