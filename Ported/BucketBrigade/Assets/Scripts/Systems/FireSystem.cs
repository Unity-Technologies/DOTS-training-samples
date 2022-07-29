using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Numerics;
using UnityEngine;
using Unity.Rendering;

[BurstCompile]
[UpdateAfter(typeof(BucketFillerFetcherSystem))]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
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
        float heatTransferRate = Config.HeatTransferRate * Time.deltaTime;

        float threshold = Config.FireThreshold;
        int gridSize = TerrainConfig.GridSize;

        
        for (int cellIndex = 0; cellIndex < HeatMap.Length; cellIndex++)
        {
            float tempChange = 0f;
            float currentTemperature = HeatMap[cellIndex].Value;


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

                            float neighbourTemperature = HeatMap[(currentRow * gridSize) + currentColumn].Value;
                            if (neighbourTemperature >= threshold)
                            {
                                tempChange += neighbourTemperature * heatTransferRate;
                            }
                        }
                    }
                }
            }

            float newTemperature = Mathf.Clamp(currentTemperature + tempChange, -1f, 1f);
            HeatMap[cellIndex] = new Temperature { Value = newTemperature };
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                var pos = new float2(hit.point.x, hit.point.z);
                pos += gridSize * TerrainConfig.CellSize * 0.5f;
                int index =  (int)(math.floor(pos.x / (TerrainConfig.CellSize)) % gridSize + math.floor(pos.y / (TerrainConfig.CellSize)) * gridSize);
                HeatMap.ElementAt(index).Value = 0.90f;
            }
        }

        var fireJob = new FireJob
        {
            HeatMap = HeatMap,
            Config = Config,
            TerrainConfig = TerrainConfig,
            time = Time.time,
        };

        fireJob.Schedule();
    }
}

[BurstCompile]
partial struct FireJob : IJobEntity
{
    public DynamicBuffer<Temperature> HeatMap;
    public Config Config;
    public TerrainCellConfig TerrainConfig;
    public float time;

    void Execute(ref URPMaterialPropertyBaseColor color, ref Translation translation, in Fire fire)
    {
        float4 colorValue = new float4(TerrainConfig.NeutralCol.r, TerrainConfig.NeutralCol.g, TerrainConfig.NeutralCol.b, 1.0f);
        var temperature = HeatMap.ElementAt(fire.Index).Value;
        var height = -TerrainConfig.CellSize;

        if (temperature >= Config.FireThreshold)
        {
            var coolFact = (1 - temperature) / (1 - Config.FireThreshold);
            var hotFact = 1 - coolFact;

            colorValue = new float4(coolFact * TerrainConfig.CoolCol.r + hotFact * TerrainConfig.HotCol.r,
                                    coolFact * TerrainConfig.CoolCol.g + hotFact * TerrainConfig.HotCol.g,
                                    coolFact * TerrainConfig.CoolCol.b + hotFact * TerrainConfig.HotCol.b,
                                    1.0f);

            var gap = 1 - ((1 - temperature) / (1 - Config.FireThreshold));
            height = -TerrainConfig.CellSize + 2 * TerrainConfig.CellSize * gap - TerrainConfig.FlickerRange;
            height += (TerrainConfig.FlickerRange * 0.5f) + Mathf.PerlinNoise((time - fire.Index) * TerrainConfig.FlickerRate - temperature, temperature) * TerrainConfig.FlickerRange;
        }

        translation.Value.y = height;
        color.Value = colorValue;

    }
}