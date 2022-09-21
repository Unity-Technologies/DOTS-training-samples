using NUnit.Framework.Constraints;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
    
[BurstCompile]
partial struct BeeSpawnerSystem : ISystem
{
    private EntityQuery NestQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        NestQuery = SystemAPI.QueryBuilder().WithAll<Faction, Area>().Build();
        
        // Only need to update if there are any entities with a SpawnRequestQuery
        state.RequireForUpdate(NestQuery);
        
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var nestEntities = NestQuery.ToEntityArray(Allocator.Temp);

        foreach (var nest in nestEntities)
        {
            var nestFaction = state.EntityManager.GetComponentData<Faction>(nest);
            if (nestFaction.Value == (int)Factions.None)
            {
                continue;
            }
            
            var nestArea = state.EntityManager.GetComponentData<Area>(nest);
            var transform = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.bee);
            var beeSpawnJob = new SpawnJob
            {
                Aabb = nestArea.Value,
            
                // Note the function call required to get a parallel writer for an EntityCommandBuffer.
                ECB = ecb.AsParallelWriter(),
                Prefab = config.bee,
                InitTransform = transform,
                InitFaction = state.EntityManager.GetComponentData<Faction>(nest).Value
            };

            //state.Dependency = beeSpawnJob.Schedule(config.beeCount, 64, state.Dependency);
            JobHandle jobHandle = beeSpawnJob.Schedule(config.beeCount, 64);
            jobHandle.Complete();
        }
        
        // force disable system after the first update call
        state.Enabled = false;
    }
}