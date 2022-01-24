using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class CitySpawnerSystem : SystemBase
{
    static readonly int s_MinTowerHeight = 1;
    static readonly int s_MaxTowerHeight = 30;
    NativeArray<float4> m_Colors;
    
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Random(1234);
        var colors = m_Colors;

        Entities
            .ForEach((Entity entity, in CitySpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.NumberOfTowers; ++i)
                {
                    int height = random.NextInt(s_MinTowerHeight, s_MaxTowerHeight);
                    var pos = new Vector3(random.NextFloat(-45f,45f), 0f, random.NextFloat(-45f,45f));
                    float spacing = 2f;
                    for (int j = 0; j < height; j++)
                    {
                        Spawn(ecb, spawner.BarPrefab, new float3(pos.x + spacing, j * spacing, pos.z - spacing), j == 0);
                        Spawn(ecb, spawner.BarPrefab, new float3(pos.x - spacing, j * spacing, pos.z - spacing), j == 0);
                        Spawn(ecb, spawner.BarPrefab, new float3(pos.x, j * spacing, pos.z + spacing), j == 0);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static void Spawn(EntityCommandBuffer ecb, Entity barPrefab, float3 pos, bool anchor)
    {
        var pointEntity = ecb.Instantiate(barPrefab);
        ecb.AddComponent(pointEntity, new Translation
        {
            Value = pos,
        });
        if (anchor)
        {
            ecb.AddComponent<AnchorPoint>(pointEntity);
        }
        else
        {
            ecb.AddComponent<Point>(pointEntity);
        }
    }
}