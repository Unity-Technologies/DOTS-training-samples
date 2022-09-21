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
        NestQuery = state.GetEntityQuery(typeof(Faction), typeof(Area));
        
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
        var nestEntities = NestQuery.ToEntityArray(Allocator.Temp);

        var combinedJobHandle = new JobHandle();
        foreach (var nest in nestEntities)
        {
            var nestFaction = state.EntityManager.GetComponentData<Faction>(nest);
            if (nestFaction.Value == (int)Factions.None)
            {
                continue;
            }
            
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var nestArea = state.EntityManager.GetComponentData<Area>(nest);
            var transform = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.bee);
            var beeSpawnJob = new SpawnJob
            {
                Aabb = nestArea.Value,
            
                // Note the function call required to get a parallel writer for an EntityCommandBuffer.
                ECB = ecb.AsParallelWriter(),
                Prefab = config.bee,
                InitTransform = transform,
                InitFaction = nestFaction.Value
            };
            var jobHandle = beeSpawnJob.Schedule(config.beeCount, 64, state.Dependency);
            combinedJobHandle = JobHandle.CombineDependencies(jobHandle, combinedJobHandle);
        }

        // establish dependency between the spawn job and the command buffer to ensure the spawn job is completed
        // before the command buffer is played back.
        state.Dependency = combinedJobHandle;
        
        // force disable system after the first update call
        state.Enabled = false;
    }
}