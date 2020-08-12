using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireSimSystem : SystemBase
{
    float m_TimeUntilFireUpdate = 0;

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var fireSpreadSettings = GetSingleton<FireSpreadSettings>();
        var tileSpawner = GetSingleton<TileSpawner>();
        int columns = tileSpawner.XSize;
        int rows = tileSpawner.YSize;

        m_TimeUntilFireUpdate -= deltaTime;
        if (m_TimeUntilFireUpdate <= fireSpreadSettings.fireSimUpdateRate)
        {
            m_TimeUntilFireUpdate = fireSpreadSettings.fireSimUpdateRate;
            int tileCount = rows * columns;
            NativeArray<float> temperatures = new NativeArray<float>(tileCount, Allocator.TempJob); 
            
            // Get all of the temperatures in the expected spatial order.
            Entities
                .WithName("FireSimRead")
                .ForEach((in Temperature temperature, in Tile tile) =>
                {
                    temperatures[tile.Id] = temperature.Value;
                }).Schedule();
            
            // Spread temperatures.
            Entities
                .WithName("FireSimSpread")
                .ForEach((in Tile tile) =>
                {
                    float temperature = temperatures[tile.Id];
                    int cellRowIndex = tile.Id % columns;
                    int cellColumnIndex = tile.Id / columns;

                    for (int rowIndex = -fireSpreadSettings.heatRadius; rowIndex <= fireSpreadSettings.heatRadius; rowIndex++)
                    {
                        int currentRow = cellRowIndex - rowIndex;
                        if (currentRow >= 0 && currentRow < rows)
                        {
                            for (int columnIndex = -fireSpreadSettings.heatRadius; columnIndex <= fireSpreadSettings.heatRadius; columnIndex++)
                            {
                                int currentColumn = cellColumnIndex + columnIndex;
                                if (currentColumn >= 0 && currentColumn < columns)
                                {
                                    float neighborTemperature = temperatures[(currentRow * columns) + currentColumn];
                                    if (neighborTemperature > fireSpreadSettings.flashpoint)
                                    {
                                        temperature += neighborTemperature * fireSpreadSettings.heatTransferRate;
                                    }
                                }
                            }
                        }
                    }
                    temperatures[tile.Id] = math.clamp(temperature, -1f, 1f);

                }).ScheduleParallel();
            
            // Apply the temporary array
            Entities
                .WithName("FireSimApply")
                .WithDisposeOnCompletion(temperatures)
                .ForEach((ref Temperature temperature, in Tile tile) =>
                {
                    temperature.Value = temperatures[tile.Id];
                }).Schedule();
        }

    }
}