using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MapCreationSystem))]
public partial struct RockSpawningSystem : ISystem
{
    private Random m_Random;
    
    public void OnCreate(ref SystemState state)
    {
        m_Random = new Random(40);
        state.RequireForUpdate<rockPrefabCreator>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<rockPrefabCreator>();
        var map = SystemAPI.GetSingleton<Map>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var random = m_Random;


        var entity = SystemAPI.GetSingletonEntity<Grid>();
        var typeBuffer = SystemAPI.GetSingletonBuffer<CellType>();
        Grid grid = SystemAPI.GetSingleton<Grid>();
        grid.size = map.mapSize;
        
        var rocks = CollectionHelper.CreateNativeArray<Entity>(config.NumRocks, Allocator.Temp);
        ecb.Instantiate(config.prefab, rocks);
        foreach (var rock in rocks)
        {
            
            int2 size = new int2();
            int2 position = new int2(new float2(10,10));
         
            for (int tries =  0; tries < 100; tries++)
            {
                size = random.NextInt2(config.RandomSizeMin, config.RandomSizeMax);
                position = random.NextInt2(new int2(0, 0), grid.size - size);
            
                if ( isEmptyArea(typeBuffer, grid, position, size))
                {
                    break;
                }
            }
            var translation = new float3(position.x, 0.0f, position.y);
            var scale = new float3(size.x, 1.0f, size.y);
            translation += scale * 0.5f;
            
          
            scale.x -= 0.2f;
            scale.z -= 0.2f;
            
            ecb.SetComponent(rock, new Translation {Value = translation});
            ecb.AddComponent(rock, new NonUniformScale {Value = scale});
            ecb.SetComponent(rock, new CellPosition {cellPosition = position});
            ecb.SetComponent(rock, new CellSize {Value = size});
            
            float health = random.NextFloat(config.minHeight, config.maxHeight);
            ecb.SetComponent(rock, new Health { Value = health });
        
            for (int j = 0; j < size.y; j++)
            {
                for (int i = 0; i < size.x; i++)
                {
                    int idx = position.x + i * grid.size.x + position.y + j;
                    typeBuffer[idx] = new CellType { Value = CellState.Raw };
                }
            }

        }
      
        state.Enabled = false;
    }


    static bool isEmptyArea(DynamicBuffer<CellType> buffer, Grid grid, int2 position, int2 areaSize)
    {
        for (int j = 0; j < areaSize.y; j++)
        {
            for (int i = 0; i < areaSize.x; i++)
            {
                int idx = position.x + i * grid.size.x + position.y + j;
                if (buffer[idx].Value != CellState.Raw)
                {
                    return false;
                }
            }
        }

        return true;
    }

}