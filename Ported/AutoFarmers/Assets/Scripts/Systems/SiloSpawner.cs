using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MapCreationSystem))]
[UpdateAfter(typeof(RockSpawningSystem))]
public partial struct SiloSpawner : ISystem
{
    Random m_random;

    public void OnCreate(ref SystemState state)
    {
        m_random = Random.CreateFromIndex(51);
        state.RequireForUpdate<SiloConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<SiloConfig>();
        var map = SystemAPI.GetSingleton<Map>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var random = m_random;
        var entity = SystemAPI.GetSingletonEntity<Grid>();
        var typeBuffer = SystemAPI.GetSingletonBuffer<CellType>();
        Grid grid = SystemAPI.GetSingleton<Grid>();
        grid.size = map.mapSize;

        var silos = CollectionHelper.CreateNativeArray<Entity>(config.NumberSilos, Allocator.Temp);
        ecb.Instantiate(config.SiloPrefab, silos);
        foreach (var silo in silos)
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


            ecb.SetComponent(silo, new Translation {Value = translation});

            ecb.SetComponent(silo, new CellPosition {cellPosition = position});


                    int idx = position.x + grid.size.x + position.y ;
                    typeBuffer[idx] = new CellType {Value = CellState.Silo};


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