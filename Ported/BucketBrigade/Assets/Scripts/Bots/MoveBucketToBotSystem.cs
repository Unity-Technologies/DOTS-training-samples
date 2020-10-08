using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class MoveBucketToBotSystem : SystemBase
{
    private EntityQuery botQuery;
    
    protected override void OnCreate()
    {
        botQuery = GetEntityQuery(ComponentType.ReadOnly<BotTag>(), ComponentType.ReadOnly<Pos>());
    }
    
    protected override void OnUpdate()
    {
        NativeHashMap<Entity, float2> posMap = new NativeHashMap<Entity, float2>(100, Allocator.TempJob);
        
        Entities
            .WithName("ParseBotEntityPosition")
            .WithAll<BotTag>()
            .ForEach((Entity entity, in Pos pos) =>
            {
                posMap[entity] = pos.Value;
            }).Schedule();
        
        Entities
            .WithName("SyncBucketToBot")
            .WithReadOnly(posMap)
            .WithDisposeOnCompletion(posMap)
            .ForEach((ref Pos pos, in BucketOwner owner) =>
            {
                pos.Value = posMap[owner.Entity];
            }).ScheduleParallel();
    }
}