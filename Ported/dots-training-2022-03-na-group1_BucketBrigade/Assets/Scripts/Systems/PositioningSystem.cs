using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static BucketBrigadeUtility;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class PositioningSystem : SystemBase
{
    private const float EmptyBucketY = 0.05f;
    private const float FullBucketY = 0.55f;

    
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .ForEach((in Position position, in BucketHeld bucket) =>
            {
                if (bucket.Value != Entity.Null)
                {
                    ecb.SetComponent(bucket.Value, position);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Entities
            .WithNone<MyBucketState>()
            .ForEach((ref Translation translation, in Position position) =>
            {
                translation.Value = new float3(position.Value.x, translation.Value.y, position.Value.y);
            }).ScheduleParallel();

        Entities
            .ForEach((ref Translation translation, in Position position, in MyBucketState state) =>
            {
                translation.Value = new float3(position.Value.x, IsBucketCarried(state.Value) ? FullBucketY : EmptyBucketY, position.Value.y);
            }).ScheduleParallel();

        Entities
            .ForEach((ref NonUniformScale scale, in Scale newScale) =>
            {
                scale.Value = newScale.Value;
            }).ScheduleParallel();
    }
}
