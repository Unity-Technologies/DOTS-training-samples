using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

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

                for (int buckets = 0; buckets < initCounts.InitialBucketCount; buckets++)
                {
                    var instance = ecb.Instantiate(initCounts.BucketPrefab);
                    var translation = new Translation 
                    { 
                        Value = new float3(Random.Range(1,initCounts.GridSize-1), 0.0f, Random.Range(1,initCounts.GridSize-1)) 
                    };
                    ecb.SetComponent(instance, translation);
                }
                
                for (int waters = 0; waters < initCounts.WaterSourceCount; waters++)
                {
                    var instance = ecb.Instantiate(initCounts.WaterPrefab);
                    var translation = new Translation 
                    { 
                        Value = new float3(Random.Range(1.0f,initCounts.GridSize-1.0f), 0.0f, Random.Range(-6.0f, -2.0f)) 
                    };
                    ecb.SetComponent(instance, translation);
                }
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        this.Enabled = false;  
    }
}
