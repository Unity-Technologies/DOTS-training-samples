using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem2D : SystemBase
{
    static float4x4 GetTranslationMatrix(in Position2D position2D)
    {
        return float4x4.Translate(new float3(position2D.Value.x, 0, position2D.Value.y));
    }

    protected override void OnUpdate()
    {
        // Handle entities with rotations
        Entities
            .WithChangeFilter<Position2D, Rotation2D>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in Rotation2D rotation2D, in NonUniformScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var rotation4x4 = float4x4.RotateY(rotation2D.Value);
                var scale4x4 = float4x4.Scale(scale.Value);
                localToWorld.Value = math.mul(math.mul(trans4x4, rotation4x4), scale4x4);
            }).ScheduleParallel();

        // Also handle entities with only positions
        Entities
            .WithChangeFilter<Position2D>()
            .WithNone<Rotation2D>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in NonUniformScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var scale4x4 = float4x4.Scale(scale.Value);
                localToWorld.Value = math.mul(trans4x4, scale4x4);
            }).ScheduleParallel();
    }
}