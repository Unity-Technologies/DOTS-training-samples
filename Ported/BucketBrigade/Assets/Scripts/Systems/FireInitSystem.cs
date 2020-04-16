using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity spawnerEntity, in InitData spawner, in TuningData data, in Translation spawnerTranslation) =>
        {
            for (int y = 0; y < data.GridSize.y; ++y)
            {
                for (int x = 0; x < data.GridSize.x; ++x)
                {
                    var instance = ecb.Instantiate(spawner.FirePrefab);
                    float3 translation = spawnerTranslation.Value;
                    translation.x += x - (data.GridSize.x - 1) / 2f;
                    translation.z += y - (data.GridSize.y - 1) / 2f;
                    ecb.SetComponent(instance, new Translation {Value = translation});
                    ecb.SetComponent(instance, new GridIndex(){Index = new int2(x,y)});
                    
                    if ((x * y) % 17 == 1)
                    {
                        float v = data.ValueThreshold + (x * y * 5) % (data.ValueThreshold * 0.1f);
                        ecb.SetComponent(instance, new ValueComponent(){Value = v });
                    }

                }
            }
            ecb.RemoveComponent<InitData>(spawnerEntity);
        }).Run();
        
        ecb.Playback(EntityManager);
    }
}
