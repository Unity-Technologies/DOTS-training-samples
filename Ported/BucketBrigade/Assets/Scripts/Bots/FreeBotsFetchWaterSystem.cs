using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class FreeBotsFetchWaterSystem : SystemBase
{
    private EntityQuery bucketQuery;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new [] {ComponentType.ReadOnly<BucketTag>(), ComponentType.ReadOnly<Pos>()},
            None = new [] {ComponentType.ReadOnly<BucketOwner>(), ComponentType.ReadOnly<ChainPosition>()}
        };
        bucketQuery = GetEntityQuery(queryDesc);
    }
    
    protected override void OnUpdate()
    {
        // Make both of these Pos instead of Translation
        NativeArray<Pos> bucketPositions = bucketQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> bucketEntities = bucketQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        Dependency = JobHandle.CombineDependencies(Dependency, j1, j2);
        
        Entities
            .WithName("FreeBotsFetchWater")
            .WithNone<ChainPosition>()
            .WithAll<BotTag>()
            .WithDisposeOnCompletion(bucketPositions)
            .WithDisposeOnCompletion(bucketEntities)
            .ForEach((ref HasBucket hasBucket, ref Target target,
                in Pos pos) =>
            {
                if (!hasBucket.PickedUp)
                {
                    hasBucket.PickingUpBucket = true;
                    target.Position = Utility.GetNearestPos(pos.Value, bucketPositions, out int nearestIndex);
                    target.Entity = bucketEntities[nearestIndex];
                }
            }).ScheduleParallel();
    }
}