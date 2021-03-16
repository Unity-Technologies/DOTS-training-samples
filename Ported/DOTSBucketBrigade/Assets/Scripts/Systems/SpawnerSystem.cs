using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity heatMapEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent<HeatMapTag>(heatMapEntity);
        DynamicBuffer<HeatMap> heatMap = EntityManager.AddBuffer<HeatMap>(heatMapEntity);
        
        Entities
            .ForEach((Entity entity, in InitCounts initCounts) =>
            {
                heatMap.Length = initCounts.GridSize * initCounts.GridSize;
                ecb.DestroyEntity(entity);

                int fireCount = 0;
                int i = 0;
                
                for (int row = 0; row < initCounts.GridSize; ++row)
                {
                    for (int col = 0; col < initCounts.GridSize; ++col)
                    {
                        var instance = ecb.Instantiate(initCounts.CellPrefab);
                        var translation = new Translation 
                        { 
                            Value = new float3(col, 0, row) 
                        };
                        heatMap[i] = new HeatMap {Value = 0.0f};
                        
                        if (fireCount < 5)
                        {
                            heatMap[i] = new HeatMap {Value = 1.0f};
                            fireCount++;
                        }
                        ecb.SetComponent(instance, translation);
                        i++;
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        this.Enabled = false;  
    }
}
