using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // The default world should not be used because the targeted EntityManager
        // may not be part of it.
        // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var config = SystemAPI.GetSingleton<Config>();

        //Instantiate our two bee teams...
        NativeArray<Entity> blueBees = state.EntityManager.Instantiate(config.BlueBeePrefab, config.TeamBlueBeeCount, Allocator.Temp);
        NativeArray<Entity> yellowBees = state.EntityManager.Instantiate(config.YellowBeePrefab, config.TeamYellowBeeCount, Allocator.Temp);

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}