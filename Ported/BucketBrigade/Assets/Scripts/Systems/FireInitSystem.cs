using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireInitSystem : SystemBase
{
    Random m_random = new Random(54321);
    
    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
            return;
        
        Entity tuningDataEntity = GetSingletonEntity<TuningData>();
        TuningData tuningData = EntityManager.GetComponentData<TuningData>(tuningDataEntity);

        var random = m_random;
        float preFireOdds = 0.05f;//tuningDataEntity.OddsOfInitFire;
        float maxFire = tuningData.MaxValue;

        float cellSize = 0.5f;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity spawnerEntity, in InitData spawner, in TuningData data, in Translation spawnerTranslation) =>
        {
            for (int y = 0; y < data.GridSize.y; ++y)
            {
                for (int x = 0; x < data.GridSize.x; ++x)
                {
                    var instance = ecb.Instantiate(spawner.FirePrefab);
                    float3 translation = spawnerTranslation.Value;
                    translation.x += (x - (data.GridSize.x - 1) / 2f) * cellSize;
                    translation.z += (y - (data.GridSize.y - 1) / 2f) * cellSize;
                    ecb.SetComponent(instance, new Translation {Value = translation});
                    ecb.SetComponent(instance, new GridIndex(){Index = new int2(x,y)});
                    
                    
                    
                    if (random.NextFloat(0,1) < preFireOdds)
                    {
                        ecb.SetComponent(instance, new ValueComponent(){Value = maxFire * 0.8f });
                    }

                }
            }
            ecb.RemoveComponent<InitData>(spawnerEntity);
        }).Run();
        
        ecb.Playback(EntityManager);
        m_random = random;
    }
}
