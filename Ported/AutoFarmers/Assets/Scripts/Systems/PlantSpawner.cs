using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MapCreationSystem))]
[UpdateAfter(typeof(RockSpawningSystem))]
[UpdateAfter(typeof(SiloSpawner))]
public partial struct PlantSpawner : ISystem


{
    Random m_random;

    public void OnCreate(ref SystemState state)
    {
        m_random = Random.CreateFromIndex(51);
        state.RequireForUpdate<PlantConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<PlantConfig>();
        var map = SystemAPI.GetSingleton<Map>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var random = m_random;
        var entity = SystemAPI.GetSingletonEntity<Grid>();
        var typeBuffer = SystemAPI.GetSingletonBuffer<CellType>();
        Grid grid = SystemAPI.GetSingleton<Grid>();
        grid.size = map.mapSize;

        var plants = CollectionHelper.CreateNativeArray<Entity>(config.NumberPlants, Allocator.Temp);
        ecb.Instantiate(config.PlantPrefab, plants);
        foreach (var plant in plants)
        {
            int2 position = new int2(new float2(10, 10));

            for (int tries = 0; tries < 100; tries++)
            {
                
                position = random.NextInt2(new int2(0, 0), grid.size );

                if (isEmptyArea(typeBuffer, grid, position))
                {
                    break;
                }
            }

            var translation = new float3(position.x, 0.0f, position.y);


            ecb.SetComponent(plant, new Translation {Value = translation});

            ecb.SetComponent(plant, new CellPosition {cellPosition = position});


                    int idx = position.x + grid.size.x + position.y ;
                    typeBuffer[idx] = new CellType {Value = CellState.Plant};


        }

        state.Enabled = false;
    }

    static bool isEmptyArea(DynamicBuffer<CellType> buffer, Grid grid, int2 position)
    {

                int idx = position.x +  grid.size.x + position.y ;
                if (buffer[idx].Value != CellState.Raw)
                {
                    return false;
                }


        return true;
    }
}