using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in InitCounts initCounts) =>
            {
                ecb.DestroyEntity(entity);

                for (int row = 0; row < initCounts.GridSize; ++ row)
                {
                    for (int col = 0; col < initCounts.GridSize; ++col)
                    {
                        var instance = ecb.Instantiate(initCounts.CellPrefab);
                        var translation = new Translation 
                        { 
                            Value = new float3(col, 0, row) 
                        };
                        ecb.SetComponent(instance, translation);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        this.Enabled = false;  
    }
}
