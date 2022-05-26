using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct GameInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    /// <summary>
    /// Generates a bunch of plants.
    /// 
    /// Used for testing growing behavior.
    /// </summary>
    private void DebugGeneratePlants(EntityCommandBuffer ecb, Allocator allocator, Entity plantPrefab)
    {
        int2 size = new int2(25, 25);

        var plants = CollectionHelper.CreateNativeArray<Entity>(size.x * size.y, allocator);
        //var plants = new NativeArray<Entity>(size.x * size.y, allocator);
        ecb.Instantiate(plantPrefab, plants);
     
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                ecb.SetComponent(plants[x + (size.x * y)], new Translation { 
                    Value = new float3(x, .2f, y) 
                });
            }
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var config = SystemAPI.GetSingleton<GameConfig>();

        GroundUtilities.GenerateGroundAndRocks(state.EntityManager, config, allocator);

        // Initial Farmer

        //DebugGeneratePlants(ecb, allocator, config.PlantPrefab);

        // This system should only run once at startup. So it disables itself after one update.
        // @TODO: Nic - should we also flag some component as "game ready" so systems relying on game setup know not to run?
        state.Enabled = false;
    }
}
