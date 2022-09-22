using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawnerSystem : ISystem
{
    private EntityQuery NestQuery;
    private EntityQuery MeshRendererQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        NestQuery = SystemAPI.QueryBuilder().WithAll<Faction, Area>().Build();
        MeshRendererQuery = SystemAPI.QueryBuilder().WithAll<RenderMeshArray>().Build();
    
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
    public unsafe void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var combinedJobHandle = new JobHandle();

        for (int i = (int)Factions.Team1; i < (int)Factions.NumFactions; i++)
        {
            NestQuery.SetSharedComponentFilter(new Faction { Value = i });
            var nestEntities = NestQuery.ToEntityArray(Allocator.Temp);

            foreach (var nest in nestEntities)
            {
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                var nestArea = state.EntityManager.GetComponentData<Area>(nest);
                var transform = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.bee);
                var nestBaseColor = state.EntityManager.GetComponentData<URPMaterialPropertyBaseColor>(nest);
                var faction = state.EntityManager.GetSharedComponent<Faction>(nest).Value;
                
                var beeSpawnJob = new BeeSpawnJob
                {
                    Aabb = nestArea.Value,

                    // Note the function call required to get a parallel writer for an EntityCommandBuffer.
                    ECB = ecb.AsParallelWriter(),
                    Prefab = config.bee,
                    InitTransform = transform,
                    InitFaction = faction,
                    InitColor = nestBaseColor.Value,
                    Mask = MeshRendererQuery.GetEntityQueryMask(),
                    InitVel = config.initVel,
                    Aggressivity = config.aggressivity,
                };
                var jobHandle = beeSpawnJob.Schedule(config.beeCount, 64, state.Dependency);
                combinedJobHandle = JobHandle.CombineDependencies(jobHandle, combinedJobHandle);
            }
        }
        
        // establish dependency between the spawn job and the command buffer to ensure the spawn job is completed
        // before the command buffer is played back.
        state.Dependency = combinedJobHandle;

        // force disable system after the first update call
        state.Enabled = false;
    }
}