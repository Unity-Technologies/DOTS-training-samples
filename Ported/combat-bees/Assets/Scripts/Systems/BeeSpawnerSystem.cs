using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
    
[BurstCompile]
partial struct BeeSpawnerSystem : ISystem
{
    private EntityQuery BeeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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

        var beeSpawnJob = new SpawnJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            ECB = ecb.AsParallelWriter(),
            Prefab = config.bee,
            InitTM = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.bee)
        };

        JobHandle jobHandle = beeSpawnJob.Schedule(config.beeCount, 64);
        jobHandle.Complete();
        
        // force disable system after the first update call
        state.Enabled = false;
    }
}