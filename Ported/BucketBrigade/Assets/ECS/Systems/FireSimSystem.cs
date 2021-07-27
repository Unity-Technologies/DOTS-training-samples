using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FireSimSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConfigComponent>();
        var rad = config.HeatTrasferRadius;
        var falloff = config.HeatFallOff;
        var grid = config.SimulationSize;
        var threashold = config.FlashPoint;
        Entities.ForEach((DynamicBuffer<HeatMapElement> heatMap) => {
            for (int i = 0; i < heatMap.Length; i++)
            {
                
                int col = i % grid;
                int row = i / grid;

                var heatContribution = 0f;

                for (int j = math.max(row - rad, 0); j <= math.min(row + rad, grid-1); j++)
                {
                    for (int k = math.max(col - rad, 0); k <= math.min(col + rad, grid-1); k++)
                    {
                        var index = k + j * grid;
                        var heat = heatMap[index].temperature;
                        if (index != i)
                        {
                            
                            if (heat >= threashold)
                            {
                                heatContribution += heat * falloff;
                                
                            }
                            
                        }
                        else
                        {
                            heatContribution += heat;
                        }
                        
                    }
                }

                heatMap[i] = new HeatMapElement(){ temperature = math.min(heatContribution,1)};
                
            }
        }).Schedule();
    }
}
