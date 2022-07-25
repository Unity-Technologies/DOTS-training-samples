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
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var teamBlueBees = CollectionHelper.CreateNativeArray<Entity>(config.TeamBlueBeeCount, Allocator.Temp);
        ecb.Instantiate(config.BlueBeePrefab, teamBlueBees);
        
        var teamYellowBees = CollectionHelper.CreateNativeArray<Entity>(config.TeamYellowBeeCount, Allocator.Temp);
        ecb.Instantiate(config.YellowBeePrefab, teamYellowBees);

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}