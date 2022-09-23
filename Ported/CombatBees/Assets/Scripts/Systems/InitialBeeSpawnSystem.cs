using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

// [BurstCompile]
[UpdateBefore(typeof(BeeConstructionSystem))]
partial struct InitialBeeSpawningSystem : ISystem
{
    
    public static Entity BeePrefab;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();

        if (BeePrefab == Entity.Null)
        {
            BeePrefab = config.beePrefab;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var blueBees  = CollectionHelper.CreateNativeArray<Entity>(config.startBeeCount / 2, Allocator.Temp);
        var yellowBees  = CollectionHelper.CreateNativeArray<Entity>(config.startBeeCount / 2, Allocator.Temp);

        float3 blueSpawn = new float3 { x = -40f, y = 0f, z = 0f };
        float3 yellowSpawn = new float3 { x = 40f, y = 0f, z = 0f };
        BeeSpawnHelper.SpawnBees(BeePrefab, ref ecb, ref blueBees, BeeTeam.Blue, in blueSpawn);
        BeeSpawnHelper.SpawnBees(BeePrefab, ref ecb, ref yellowBees, BeeTeam.Yellow, in yellowSpawn);

        state.Enabled = false;
    }
}
