using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

partial struct InitialBeeSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {}

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        if (BeeSpawnHelper.BeePrefab == Entity.Null)
        {
            BeeSpawnHelper.BeePrefab = config.beePrefab;
        }

        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var blueBees  = CollectionHelper.CreateNativeArray<Entity>(config.startBeeCount / 2, Allocator.Temp);
        var yellowBees  = CollectionHelper.CreateNativeArray<Entity>(config.startBeeCount / 2, Allocator.Temp);
        
        BeeSpawnHelper.SpawnBees(ecb, ref blueBees, BeeTeam.Blue, float2.zero);
        BeeSpawnHelper.SpawnBees(ecb, ref yellowBees, BeeTeam.Yellow, float2.zero);

        state.Enabled = false;
    }
}
