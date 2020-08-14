using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSplashSystem))]
public class FireUpdateSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private FireConfiguration m_config;
    private float m_timer;
    private NativeArray<float> m_grid;
    private bool m_configValid = false;

    private bool bUseNativeArray = true;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        if (m_grid.Length > 0)
        {
            m_grid.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        m_timer -= Time.DeltaTime;
        if (m_timer >= 0)
            return;
        
        m_timer = m_config.FireSimUpdateRate;

        if (m_configValid == false)
        {
            var fireConfigEntity = GetSingletonEntity<FireConfiguration>();
            m_config = EntityManager.GetComponentData<FireConfiguration>(fireConfigEntity);
            m_configValid = true;
        }
        
        var config = m_config;
        var time = UnityEngine.Time.time;

        if (m_grid.Length == 0)
        {
            var arraySize = config.GridHeight * config.GridWidth;
            m_grid = new NativeArray<float>(arraySize, Allocator.Persistent);
        }

        var temperatureGrid = m_grid;
        
        if (bUseNativeArray)
        {
            var jobHandle1 = Entities
                .WithName("FireTemperatureArray")
                .ForEach((Entity entity, in Temperature temp) =>
                    {
                        temperatureGrid[temp.FireGridIndex] = temp.Value;
                    }
                ).Schedule(Dependency);
            
            var jobHandle2 = Entities
                .WithName("FirePropagationViaArray")
                .ForEach((Entity entity, ref Translation translation, ref Temperature temp, ref AddedTemperature addedTemp, ref FireColor fireColor) =>
                    {
                        // Either apply splash (distinguishing)
                        if (addedTemp.Value < 0)
                        {
                            temp.Value += addedTemp.Value;
                        }
                        // Or apply heat transfer
                        else
                        {
                            int index = temp.FireGridIndex;
                            int y = index / config.GridWidth;
                            int x = index % config.GridWidth;
                            for (int xx = math.max(0, x - config.HeatRadius);
                                xx <= math.min(config.GridWidth - 1, x + config.HeatRadius);
                                ++xx)
                            {
                                for (int yy = math.max(0, y - config.HeatRadius);
                                    yy <= math.min(config.GridHeight - 1, y + config.HeatRadius);
                                    ++yy)
                                {
                                    if (x == xx && y == yy)
                                        continue;

                                    float neighborTemp = temperatureGrid[xx + yy * config.GridWidth];

                                    // On fire?
                                    if (neighborTemp >= config.FlashPoint)
                                    {
                                        // My temperature changes depending on my neighbors
                                        temp.Value += neighborTemp * config.HeatTransferRate;
                                    }
                                }
                            }
                        }

                        addedTemp.Value = 0;

                        // Keep temperature in valid range
                        temp.Value = math.clamp(temp.Value, 0, config.MaxTemperature);

                        // Affect look & feel
                        var newTrans = translation;
                        if (temp.Value > 0)
                        {
                            // Position
                            newTrans.Value.y = temp.Value * config.MaxFireHeight + config.Origin.y;

                            if (temp.Value >= config.FlashPoint)
                                newTrans.Value.y +=
                                    Mathf.PerlinNoise((time - temp.FireGridIndex) * config.FlickerRate - temp.Value,
                                        temp.Value) * config.FlickerRange;

                            // Color
                            if (temp.Value < config.FlashPoint)
                            {
                                fireColor.Value = math.lerp(config.DefaultColor, config.LowFireColor,
                                    math.max(0f, temp.Value / config.FlashPoint));
                            }
                            else
                            {
                                fireColor.Value = math.lerp(config.LowFireColor, config.HigHFireColor,
                                    math.min(1f, (temp.Value - config.FlashPoint) / (1f - config.FlashPoint)));
                            }
                        }
                        else
                        {
                            fireColor.Value = config.DefaultColor;
                            newTrans.Value.y = config.Origin.y;
                        }

                        translation = newTrans;
                    }
                ).ScheduleParallel(jobHandle1);

            Dependency = jobHandle2;
        }
        else
        {
            var temperatures = GetComponentDataFromEntity<Temperature>(true);

            // 1st pass: 
            Entities
                .WithName("FirePropagation")
                .WithReadOnly(temperatures)
                .ForEach((Entity entity, ref AddedTemperature addedTemp,
                    in DynamicBuffer<FireGridNeighbor> neighborBuffer) =>
                {
                    if (addedTemp.Value < 0f)
                        return;

                    float tempChange = 0;
                    for (int i = 0; i < neighborBuffer.Length; ++i)
                    {
                        FireGridNeighbor neigh = neighborBuffer[i];
                        Temperature theirTemp = temperatures[neigh.Entity];

                        // On fire?
                        if (theirTemp.Value >= config.FlashPoint)
                        {
                            // My temperature changes depending on my neighbors
                            tempChange += theirTemp.Value * config.HeatTransferRate;
                        }
                    }

                    // Add neighbors' accumulated temperatures to my own added temperature (next job adds this effectively)
                    addedTemp.Value += tempChange;

                }).ScheduleParallel();

            // 2nd pass: add each cell's accumulated neighbor temperature
            Entities
                .WithName("FireTemperatureUpdates")
                .ForEach((Entity entity, ref Translation translation, ref Temperature temp,
                    ref AddedTemperature addedTemp, ref FireColor fireColor) =>
                {
                    temp.Value = math.max(0, math.min(addedTemp.Value + temp.Value, config.MaxTemperature));
                    addedTemp.Value = 0;

                    var newTrans = translation;

                    // Affect look & feel
                    if (temp.Value > 0)
                    {
                        // Position
                        newTrans.Value.y = temp.Value * config.MaxFireHeight + config.Origin.y;

                        if (temp.Value >= config.FlashPoint)
                            newTrans.Value.y +=
                                Mathf.PerlinNoise((time - temp.FireGridIndex) * config.FlickerRate - temp.Value,
                                    temp.Value) * config.FlickerRange;

                        // Color
                        if (temp.Value < config.FlashPoint)
                        {
                            fireColor.Value = math.lerp(config.DefaultColor, config.LowFireColor,
                                math.max(0f, temp.Value / config.FlashPoint));
                        }
                        else
                        {
                            fireColor.Value = math.lerp(config.LowFireColor, config.HigHFireColor,
                                math.min(1f, (temp.Value - config.FlashPoint) / (1f - config.FlashPoint)));
                        }
                    }
                    else
                    {
                        fireColor.Value = config.DefaultColor;
                        newTrans.Value.y = config.Origin.y;
                    }

                    translation = newTrans;

                }).ScheduleParallel();
        }

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
