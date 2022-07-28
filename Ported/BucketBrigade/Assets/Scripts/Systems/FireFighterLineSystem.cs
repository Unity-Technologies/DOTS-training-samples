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

        foreach (var fireFighterLine in SystemAPI.Query<RefRW<FireFighterLine>>())
        {
            var closestPoint = new float2(999999, 999999);

            DynamicBuffer<Temperature> HeatMap = SystemAPI.GetSingletonBuffer<Temperature>();
            for (int cellIndex = 0; cellIndex < HeatMap.Length; cellIndex++)
            {
                float currentTemperature = HeatMap.ElementAt(cellIndex).Value;
                if (currentTemperature >= fireThreshold)
                {
                    int cellRowIndex = Mathf.FloorToInt(cellIndex / gridSize);
                    int cellColumnIndex = cellIndex % gridSize;
                    var newPoint = new float2(cellRowIndex, cellColumnIndex);
                    if (math.distancesq(fireFighterLine.ValueRO.StartPosition, closestPoint) >
                        math.distancesq(fireFighterLine.ValueRO.StartPosition, newPoint))
                    {
                        closestPoint = newPoint;
                    }
                }
            }

            fireFighterLine.ValueRW.EndPosition = closestPoint;
        }
    }
}