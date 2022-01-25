using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class RenderBucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gameConstants = GetSingleton<GameConstants>();

        Entities
            .ForEach((ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color, in Bucket bucket) =>
            {
                scale.Value = math.clamp(bucket.Volume, 0.2f, 0.4f);
                color.Value = math.lerp(gameConstants.BucketEmpty, gameConstants.BucketFilled, bucket.Volume);
            }).Schedule();
    }
}