using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class MoveToWaterSystem : SystemBase
{
    private EntityQuery waterQuery;

    protected override void OnCreate()
    {
        waterQuery = GetEntityQuery(ComponentType.ReadOnly<WaterTag>(), ComponentType.ReadOnly<Pos>());
    }
    
    protected override void OnUpdate()
    {
        NativeArray<Pos> waterPositions = waterQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> waterEntities = waterQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        Dependency = JobHandle.CombineDependencies(Dependency, j1, j2);
        
        Entities
            .WithName("FreeBotsFetchWater")
            .WithNone<ChainPosition>()
            .WithAll<BotTag>()
            .WithDisposeOnCompletion(waterPositions)
            .WithDisposeOnCompletion(waterEntities)
            .ForEach((ref FillingBucket fillingBucket, ref Target target,
                in Pos pos, in HasBucket hasBucket) =>
            {
                if (hasBucket.PickedUp && !fillingBucket.Full)
                {
                    fillingBucket.Filling = true;
                    target.Position = Utility.GetNearestPos(pos.Value, waterPositions, out int nearestIndex);
                    fillingBucket.Entity = target.Entity = waterEntities[nearestIndex];
                }
            }).ScheduleParallel();
    }
}