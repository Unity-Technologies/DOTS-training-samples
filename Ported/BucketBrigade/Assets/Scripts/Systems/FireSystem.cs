using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Numerics;
using UnityEngine;
using Unity.Rendering;

[BurstCompile]
partial struct FireSystem : ISystem
{
    public float updateTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Temperature>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        DynamicBuffer<Temperature> HeatMap = SystemAPI.GetSingletonBuffer<Temperature>();
        Config Config = SystemAPI.GetSingleton<Config>();
        TerrainCellConfig TerrainConfig = SystemAPI.GetSingleton<TerrainCellConfig>();

        int heatRadius = 1;
        float heatTransferRate = Config.HeatTransferRate * Time.deltaTime;

        float threshold = Config.FireThreshold;
        int gridSize = TerrainConfig.GridSize;

        
         
        for (int cellIndex = 0; cellIndex < HeatMap.Length; cellIndex++)
        {
            float tempChange = 0f;
            float currentTemperature = HeatMap.ElementAt(cellIndex).Value; // currentCell


            int cellRowIndex = Mathf.FloorToInt(cellIndex / gridSize);
            int cellColumnIndex = cellIndex % gridSize;

            for (int rowIndex = -heatRadius; rowIndex <= heatRadius; rowIndex++)
            {
                int currentRow = cellRowIndex - rowIndex;
                if (currentRow >= 0 && currentRow < gridSize)
                {
                    for (int columnIndex = -heatRadius; columnIndex <= heatRadius; columnIndex++)
                    {
                        int currentColumn = cellColumnIndex + columnIndex;
                        if (currentColumn >= 0 && currentColumn < gridSize)
                        {

                            float neighbourTemperature = HeatMap.ElementAt((currentRow * gridSize) + currentColumn).Value;
                            if (neighbourTemperature >= threshold) // neighbour in fire
                            {
                                tempChange += neighbourTemperature * heatTransferRate;
                            }
                        }
                    }
                }
            }

            float newTemperature = Mathf.Clamp(currentTemperature + tempChange, -1f, 1f);
            HeatMap.ElementAt(cellIndex).Value = newTemperature;

        
        }

        foreach (var(color, translation,  fire) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<Translation>,RefRO<Fire>>())
        {
            float4 colorValue = new float4(TerrainConfig.NeutralCol.r, TerrainConfig.NeutralCol.g, TerrainConfig.NeutralCol.b, 1.0f);
            var temperature = HeatMap.ElementAt(fire.ValueRO.Index).Value;
            
            if (temperature >= threshold)
            {
                var coolFact = (1 - temperature) / (1 - threshold);
                var hotFact = 1 - coolFact;

                colorValue = new float4(coolFact * TerrainConfig.CoolCol.r + hotFact * TerrainConfig.HotCol.r,
                                        coolFact * TerrainConfig.CoolCol.g + hotFact * TerrainConfig.HotCol.g,
                                        coolFact * TerrainConfig.CoolCol.b + hotFact * TerrainConfig.HotCol.b, 
                                        1.0f);

                var gap = 1 - ((1 - temperature) / (1 - threshold));
                var height = -TerrainConfig.CellSize + 2 * TerrainConfig.CellSize * gap;
                translation.ValueRW.Value.y = height;

            }

            color.ValueRW.Value = colorValue;
        }

    }
}