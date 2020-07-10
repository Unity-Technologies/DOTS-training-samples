using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public class TransformSystem2D : SystemBase
{
    protected override void OnUpdate()
    {
        float3 firefighterScale = new float3(0.2f, 0.5f, 0.2f);

        // These queries will be specialized for object type and assign correct 3D translation and 3D,
        // so this system will effectively be presentation layer logic and will need a different name
        
        // Firefighters
        Entities
            .WithAll<Firefighter>()
            .WithChangeFilter<Translation2D>()
            .ForEach((ref LocalToWorld localToWorld, in Translation2D translation2D) =>
        {
            var trans = float4x4.Translate(new float3(translation2D.Value.x, 0, translation2D.Value.y));
            var scale = float4x4.Scale(firefighterScale);
            localToWorld.Value = math.mul(trans, scale);
        }).ScheduleParallel();


        // Buckets
        float raisedBucketHeight = 0.5f;
        float3 bucketScale = new float3(0.3f, 0.3f, 0.3f);
        BaseColor colorEmpty = new BaseColor { Value = new float4(255.0f/255.0f, 105.0f/255.0f, 117.0f/255.0f, 1.0f)};
        BaseColor colorFull = new BaseColor { Value = new float4(0.0f/255.0f, 250.0f/255.0f, 255.0f/255.0f, 1.0f)};

        Entities
            .WithChangeFilter<Translation2D>()
            .ForEach((Entity entity, ref LocalToWorld localToWorld, ref BaseColor baseColor, in WaterBucket waterBucket, in Translation2D translation2D) =>
        {
            var trans = float4x4.Translate(new float3(translation2D.Value.x, raisedBucketHeight, translation2D.Value.y));
            var scale = float4x4.Scale(bucketScale * math.lerp(0.3f, 1.0f, waterBucket.Value));
            localToWorld.Value = math.mul(trans, scale);

            // BaseColor color = waterBucket.Value > 0 ? colorFull : colorEmpty;
            // SetComponent(entity, color);
            baseColor = waterBucket.Value > 0 ? colorFull : colorEmpty;
        }).ScheduleParallel();
    }
}
