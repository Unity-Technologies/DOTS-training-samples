using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using static BucketBrigadeUtility;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class PositioningSystem : SystemBase
{
    protected override void OnUpdate()
    {
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
                translation.Value = new float3(position.Value.x, IsBucketCarried(state.Value) ? 1.2f : 0.2f, position.Value.y);
            }).ScheduleParallel();

        Entities
            .ForEach((ref Scale scale, in MyBucketState state) =>
            {
                switch (state.Value)
                {
                    case BucketState.FullCarried:
                    case BucketState.FullOnGround:
                        scale.Value = new float3(0.3f, 0.3f, 0.3f);
                        break;
                    
                    default:
                        scale.Value = new float3(0.2f, 0.2f, 0.2f);
                        break;
                }
            }).Run();
        
        Entities
            .ForEach((ref NonUniformScale scale, in Scale newScale) =>
            {
                scale.Value = newScale.Value;
            }).ScheduleParallel();
    }
}
