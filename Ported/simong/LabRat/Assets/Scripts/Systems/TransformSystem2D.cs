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
        // Handle entities with rotations and a non uniform scale component
        Entities
            .WithChangeFilter<Position2D, Rotation2D>()
            .WithNone<CompositeScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in Rotation2D rotation2D, in NonUniformScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var rotation4x4 = float4x4.RotateY(rotation2D.Value);
                var scale4x4 = float4x4.Scale(scale.Value);
                
                localToWorld.Value = math.mul(math.mul(trans4x4, rotation4x4), scale4x4);
            }).ScheduleParallel();

        // Also handle entities with only positions and a non uniform scale component 
        Entities
            .WithChangeFilter<Position2D>()
            .WithNone<Rotation2D>()
            .WithNone<CompositeScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in NonUniformScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var scale4x4 = float4x4.Scale(scale.Value);
                
                localToWorld.Value = math.mul(trans4x4, scale4x4);
            }).ScheduleParallel();
        
        // Handle entities with rotations and a non composite scale component
        Entities
            .WithChangeFilter<Position2D, Rotation2D>()
            .WithNone<NonUniformScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in Rotation2D rotation2D, in CompositeScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var rotation4x4 = float4x4.RotateY(rotation2D.Value);
                var scale4x4 = scale.Value;
                
                localToWorld.Value = math.mul(math.mul(trans4x4, rotation4x4), scale4x4);
            }).ScheduleParallel();

        // Also handle entities with only positions and a non uniform scale component 
        Entities
            .WithChangeFilter<Position2D>()
            .WithNone<Rotation2D>()
            .WithNone<NonUniformScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in CompositeScale scale) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var scale4x4 = scale.Value;
                
                localToWorld.Value = math.mul(trans4x4, scale4x4);
            }).ScheduleParallel();
        
        // Entities with a rotations but without any scale component
        Entities
            .WithChangeFilter<Position2D, Rotation2D>()
            .WithNone<NonUniformScale>()
            .WithNone<CompositeScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D, in Rotation2D rotation2D) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                var rotation4x4 = float4x4.RotateY(rotation2D.Value);
                
                localToWorld.Value = math.mul(trans4x4, rotation4x4);
            }).ScheduleParallel();

        // Entities without any rotation or scale component
        Entities
            .WithChangeFilter<Position2D>()
            .WithNone<Rotation2D>()
            .WithNone<NonUniformScale>()
            .WithNone<CompositeScale>()
            .ForEach((ref LocalToWorld localToWorld, in Position2D position2D) =>
            {
                var trans4x4 = GetTranslationMatrix(position2D);
                
                localToWorld.Value = trans4x4;
            }).ScheduleParallel();
    }
}