using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class FreeBotsFetchWaterSystem : SystemBase
{
    private EntityQuery bucketQuery;
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDesc = new EntityQueryDesc
        {
            All = new [] {ComponentType.ReadOnly<BucketTag>(), ComponentType.ReadOnly<Pos>()},
            None = new [] {ComponentType.ReadOnly<BucketOwner>()}
        };
        bucketQuery = GetEntityQuery(queryDesc);

        waterQuery = GetEntityQuery(ComponentType.ReadOnly<WaterTag>(), ComponentType.ReadOnly<Pos>());
    }
    
    protected override void OnUpdate()
    {
        // Make both of these Pos instead of Translation
        NativeArray<Pos> bucketPositions = bucketQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> bucketEntities = bucketQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        NativeArray<Pos> waterPositions = waterQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j3);
        NativeArray<Entity> waterEntities = waterQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j4);
        Dependency = JobHandle.CombineDependencies(Dependency, JobHandle.CombineDependencies(j1, j2, j3), j4);
        
        Entities
            .WithName("FreeBotsFetchWater")
            .WithNone<ChainPosition>()
            .WithAll<BotTag>()
            .WithDisposeOnCompletion(bucketPositions)
            .WithDisposeOnCompletion(bucketEntities)
            .WithDisposeOnCompletion(waterPositions)
            .WithDisposeOnCompletion(waterEntities)
            .ForEach((ref HasBucket hasBucket, ref FillingBucket fillingBucket, ref Target target,
                in Pos pos) =>
            {
                if (!hasBucket.PickedUp)
                {
                    hasBucket.PickingUpBucket = true;
                    target.Position = Utility.GetNearestPos(pos.Value, bucketPositions, out int nearestIndex);
                    target.Entity = bucketEntities[nearestIndex];
                }
                else if (!fillingBucket.Full)
                {
                    fillingBucket.Filling = true;
                    target.Position = Utility.GetNearestPos(pos.Value, waterPositions, out int nearestIndex);
                    fillingBucket.Entity = target.Entity = waterEntities[nearestIndex];
                }
            }).ScheduleParallel();
    }
}