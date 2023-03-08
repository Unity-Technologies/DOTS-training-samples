using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateAfter(typeof(FireGridSpawnerSystem))]
    public partial struct FireGridHeatSetterSystem : ISystem
    {
        float timeUntilFireUpdate;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
            state.RequireForUpdate<ConfigAuthoring.FlameHeat>();
            timeUntilFireUpdate = 0;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timeUntilFireUpdate -= SystemAPI.Time.DeltaTime;
            
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
            var heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
            
            var previousHeatMap = new NativeList<ConfigAuthoring.FlameHeat> (heatMap.Length, Allocator.Temp);
            previousHeatMap.AddRange(heatMap.AsNativeArray());
            
            // Debug.Log($"Prev Heat Map Length {previousHeatMap.Length}");
            // int countFire = 0;
            // for (int i = 0; i < previousHeatMap.Length; i++)
            // {
            //     if (previousHeatMap[i].Value != 0)
            //     {
            //         Debug.Log($"Valid Heat Map {i}");
            //         countFire++;
            //     }
            // }    
            // Debug.Log($"Count Fire {countFire}");
            var index = 0;

            Random random = new Random(123);
            foreach (var (transform, color) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<FlameCell>())
            {
                
                //setting colour based on heat
                var heat = previousHeatMap[index];
                /*if (heat.Value != 0f && heat.Value != 1f)
                {
                    Debug.Log($"heat {heat.Value}");
                }*/

                if (heat.Value < config.flashpoint)
                {
                    transform.ValueRW.Position.y = -(config.maxFlameHeight * 0.5f) + random.NextFloat(0.01f,0.02f);
                    color.ValueRW.Value = config.fireNeutralColor;
                }
                else
                {
                    float yPos = (-config.maxFlameHeight*0.5f + (heat.Value * config.maxFlameHeight)) - config.flickerRange;
                    yPos += (config.flickerRange * 0.5f) + noise.cnoise(new float2(((float)SystemAPI.Time.ElapsedTime - index) * config.flickerRate - heat.Value, heat.Value)) * config.flickerRange;
                    
                    // float noiseValue = noise.cnoise(new float2(Time.time - index * config.flickerRate - heat.Value, heat.Value));
                    transform.ValueRW.Position.y = yPos;//-(config.maxFlameHeight * 0.5f) + heat.Value + noiseValue;
                    color.ValueRW.Value =
                        math.lerp(config.fireCoolColor, config.fireHotColor, heat.Value);
                }
                
                //updating fire
                if (timeUntilFireUpdate < 0)
                {
                    float tempChange = 0f;

                    int cellRowIndex = Mathf.FloorToInt((float) index / config.numColumns);
                    int cellColumnIndex = index % config.numColumns;
                    int heatRadius = config.heatRadius;
                    
                    // Debug.Log($"Cell {cellRowIndex} {cellColumnIndex}");
                    
                    for (int rowIndex = -heatRadius; rowIndex <= heatRadius; rowIndex++)
                    {
                        int currentRow = cellRowIndex - rowIndex;
                        if (currentRow >= 0 && currentRow < config.numRows)
                        {
                            for (int columnIndex = -heatRadius; columnIndex <= heatRadius; columnIndex++)
                            {
                                int currentColumn = cellColumnIndex + columnIndex;
                                if (currentColumn >= 0 && currentColumn < config.numColumns)
                                {
                                    int neighbourIndex = currentRow * config.numColumns + currentColumn;
                                    float neighbourHeat = previousHeatMap[neighbourIndex].Value;
                                    // Debug.Log($"neighbourHeat {neighbourHeat}");
                                    if (neighbourHeat > config.flashpoint)
                                    {
                                        tempChange += neighbourHeat * config.heatTransferRate;
                                        // Debug.Log($"Changing temp {tempChange}");
                                    }

                                    // FlameCell _neighbour = flameCells[(currentRow * columns) + currentColumn];
                                    // if (_neighbour.onFire)
                                    // {
                                    //     currentCell.neighbourOnFire = true;
                                    //     tempChange += _neighbour.temperature * heatTransferRate;
                                    // }
                                }
                            }
                        }
                    }

                    float newHeat = math.clamp(previousHeatMap[index].Value + tempChange,-1f,1f);
                    heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();
                    heatMap[index] = new ConfigAuthoring.FlameHeat {Value = newHeat};

                    // if (previousHeatMap[index].Value < config.flashpoint && heatMap[index].Value > config.flashpoint)
                    // {
                    //     Debug.Log($"Burn!! {index}");
                    // }
                }
                
                
                index++;
            }

            if (timeUntilFireUpdate < 0)
            {
                timeUntilFireUpdate = config.fireSimUpdateRate;
            }
        }
    }
}
