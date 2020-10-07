using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MoveBucketToBotSystem : SystemBase
{
    private EntityQuery botQuery;
    
    protected override void OnCreate()
    {
        botQuery = GetEntityQuery(ComponentType.ReadOnly<BotTag>(), ComponentType.ReadOnly<Pos>());
    }
    
    protected override void OnUpdate()
    {
        NativeArray<Pos> botPositions = botQuery.ToComponentDataArrayAsync<Pos>(Allocator.TempJob, out JobHandle j1);
        NativeArray<Entity> botEntities = botQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle j2);
        Dependency = JobHandle.CombineDependencies(Dependency, j1, j2);

        Entities
            .WithName("SyncBucketToBot")
            .WithDisposeOnCompletion(botPositions)
            .WithDisposeOnCompletion(botEntities)
            .ForEach((ref Pos pos, in BucketOwner owner) =>
            {
                pos.Value = GetOwnerPos(owner.Entity, botPositions, botEntities);
            }).ScheduleParallel();
    }

    static float2 GetOwnerPos(in Entity ownerEntity, in NativeArray<Pos> positions, in NativeArray<Entity> entities)
    {
        // This is a lot of searching :/ Could I make this into a NativeDictionary?
        for (int i = 0; i < entities.Length; i++)
        {
            if (ownerEntity == entities[i])
            {
                return positions[i].Value;
            }
        }
        
        Debug.LogWarning($"Bucket could not find owner to sync position to!");
        return float2.zero;
    }
}