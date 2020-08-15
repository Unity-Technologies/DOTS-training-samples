using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FireSimSystem : SystemBase
{
    float m_TimeUntilFireUpdate = 0;
    private EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var fireSpreadSettings = GetSingleton<FireSpreadSettings>();
        var tileSpawner = GetSingleton<TileSpawner>();
        int columns = tileSpawner.XSize;
        int rows = tileSpawner.YSize;
       
        m_TimeUntilFireUpdate -= deltaTime;
        if (m_TimeUntilFireUpdate <= 0)
        {
            m_TimeUntilFireUpdate = fireSpreadSettings.fireSimUpdateRate;
            int tileCount = rows * columns;
            NativeArray<float> temperatures = new NativeArray<float>(tileCount, Allocator.TempJob); 
            
            var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

            // Get all of the temperatures in the expected spatial order.
            Entities
                .WithName("FireSimRead")
                .ForEach((in Temperature temperature, in Tile tile) =>
                {
                    temperatures[tile.Id] = temperature.Value;
                }).ScheduleParallel();
            
            // Spread temperatures.
            Entities
                .WithName("FireSimSpread")
                .ForEach((
                    int entityInQueryIndex, 
                    Entity tileEntity, 
                    ref Temperature temperature, 
                    in Tile tile) =>
                {
                    float sumTemperature = temperatures[tile.Id];
                    int cellRowIndex = tile.Id / columns;
                    int cellColumnIndex = tile.Id % columns;

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
                                        sumTemperature += neighborTemperature * fireSpreadSettings.heatTransferRate;
                                    }
                                }
                            }
                        }
                    }
                    temperatures[tile.Id] = math.clamp(sumTemperature, -1f, 1f);
                    temperature.Value = math.clamp(sumTemperature, -1f, 1f);
                    if (temperature.Value > fireSpreadSettings.flashpoint)
                    {
                        // TODO: We need a system that Removes the OnFire component.
                        ecb.AddComponent<OnFire>(entityInQueryIndex, tileEntity);
                    }

                }).ScheduleParallel();

            // Register a dependency for the EntityCommandBufferSystem.
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}