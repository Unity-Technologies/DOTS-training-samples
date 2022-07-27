using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Numerics;
using UnityEngine;

[BurstCompile]
partial struct FireSystem : ISystem
{

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
        float heatTransferRate = 0.7f;
        float fireSimUpdateRate = 0.5f;

        float threshold = Config.FireThreshold;
        int gridSize = TerrainConfig.GridSize;

        Config.FireUpdateRate -= Time.deltaTime;
        if (Config.FireUpdateRate <= 0)
        {
            Config.FireUpdateRate = fireSimUpdateRate;

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

                if (newTemperature >= threshold) Burn();
            }
        }
    }

    public void Burn()
    {
        Debug.Log("Burn!");
    }
}