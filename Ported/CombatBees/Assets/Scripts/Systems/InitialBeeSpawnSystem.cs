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

        var bees  = CollectionHelper.CreateNativeArray<Entity>(config.startBeeCount, Allocator.Temp);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        ecb.Instantiate(config.beePrefab, bees);

        foreach (var bee in bees)
        {
            ecb.AddComponent<IsAttacking>(bee);
            ecb.AddComponent<IsHolding>(bee);
        }
        
        state.Enabled = false;
    }
}
