using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
struct heatmm
{
    public NativeArray<float> HeatValues;
}

[BurstCompile]
partial struct FireFighterLineSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        float fireThreshold = config.FireThreshold;
        var terrainConfig = SystemAPI.GetSingleton<TerrainCellConfig>();
        int gridSize = terrainConfig.GridSize; //20

        DynamicBuffer<Temperature> heatMap = SystemAPI.GetSingletonBuffer<Temperature>();

        var searcher = new FireSearcherJob
        {
            HeatMap = heatMap,
            Config = config,
            TerrainConfig = terrainConfig,
        };
        searcher.ScheduleParallel();
       
    }
}

[BurstCompile]
partial struct FireSearcherJob : IJobEntity
{
    [ReadOnly] public DynamicBuffer<Temperature> HeatMap;
    public Config Config;
    public TerrainCellConfig TerrainConfig;

    void Execute(ref FireFighterLine fireFighterLine)
    {
        var closestPoint = new float2(999999, 999999);

        for (int cellIndex = 0; cellIndex < HeatMap.Length; cellIndex++)
        {
            float currentTemperature = HeatMap[cellIndex].Value;
            if (currentTemperature >= Config.FireThreshold)
            {
                    int cellColumnIndex = Mathf.FloorToInt(cellIndex / TerrainConfig.GridSize);
                    int cellRowIndex = cellIndex % TerrainConfig.GridSize;
                    
                    var offset = new float2(-TerrainConfig.GridSize * TerrainConfig.CellSize * 0.5f + TerrainConfig.CellSize * 0.5f, -TerrainConfig.GridSize * TerrainConfig.CellSize * 0.5f + TerrainConfig.CellSize * 0.5f);
                    
                    var newPoint = new float2(cellRowIndex * TerrainConfig.CellSize, cellColumnIndex * TerrainConfig.CellSize) + offset;
                    
                    
                    if (math.distance(fireFighterLine.StartPosition, closestPoint) >
                        math.distance(fireFighterLine.StartPosition, newPoint))
                    {
                        closestPoint = newPoint;
                    }
                
            }
        }

        fireFighterLine.EndPosition = closestPoint;
    }
}