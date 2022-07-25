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

        var teamRedBees = CollectionHelper.CreateNativeArray<Entity>(config.TeamRedBeeCount, Allocator.Temp);
        ecb.Instantiate(config.BeePrefab, teamRedBees);

        var teamBlueBees = CollectionHelper.CreateNativeArray<Entity>(config.TeamBlueBeeCount, Allocator.Temp);
        ecb.Instantiate(config.BeePrefab, teamBlueBees);


        //ComponentDataFromEntity<TeamColor> teamColorLookup = SystemAPI.GetComponentDataFromEntity<TeamColor>();
        //foreach (Entity bee in teamRedBees)
        //{
        //    ecb.SetComponent<Bee>(bee, Bee);
        //}

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}