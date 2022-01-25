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
        
        var up = new float3(0, 1, 0);
        var left = new float3(1, 0, 0);

        Entities
            .ForEach((Entity entity, in CitySpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < spawner.NumberOfTowers; ++i)
                {
                    int height = random.NextInt(s_MinTowerHeight, s_MaxTowerHeight);
                    var joints = new NativeArray<Entity>(height * 3, Allocator.Temp);
                    var jointPosition = new NativeArray<float3>(height * 3, Allocator.Temp);
                    int towerIndex = 0;
                    var pos = new Vector3(random.NextFloat(-45f,45f), 0f, random.NextFloat(-45f,45f));
                    float spacing = 2f;
                    for (int j = 0; j < height; j++)
                    {
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x + spacing, j * spacing, pos.z - spacing), j == 0);
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x - spacing, j * spacing, pos.z - spacing), j == 0);
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x, j * spacing, pos.z + spacing), j == 0);
                    }

                    for (int j = 0; j < joints.Length; j += 3)
                    {
                        SpawnBar(ecb, spawner.BarPrefab, joints[j], jointPosition[j], joints[j+1], jointPosition[j+1], up);
                        SpawnBar(ecb, spawner.BarPrefab, joints[j+1], jointPosition[j+1], joints[j+2], jointPosition[j+2], up);
                        SpawnBar(ecb, spawner.BarPrefab, joints[j+2], jointPosition[j+2], joints[j], jointPosition[j], up);
                    }
                    for (int j = 0; j < joints.Length - 3; ++j)
                    {
                        SpawnBar(ecb, spawner.BarPrefab, joints[j], jointPosition[j], joints[j+3], jointPosition[j+3], left);
                    }

                    joints.Dispose();
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        
    }
    
    static void SpawnBar(EntityCommandBuffer ecb, in Entity barPrefab, Entity j1, float3 pos1, Entity j2, float3 pos2, float3 up) 
    {
        var bar = ecb.Instantiate(barPrefab);
        var delta = pos2 - pos1;
        var length = math.length(delta);
        delta /= length;
        ecb.AddComponent(bar, new BarConnection
        {
            Joint1 = j1, 
            Joint2 = j2, 
            Length = length,
        });
        ecb.SetComponent(bar, new Translation
        {
            Value = (pos1 + pos2) / 2f,
        });
        ecb.SetComponent(bar, new Rotation
        {
            Value = quaternion.LookRotation(delta, up),
        });
        ecb.AddComponent(bar, new NonUniformScale
        {
            Value = new float3(0.3f, 0.3f, length),
        });
    }

    static Entity SpawnJoint(EntityCommandBuffer ecb, float3 pos, bool anchor)
    {
        var entity = ecb.CreateEntity();
        
        ecb.AddComponent(entity, new Translation
        {
            Value = pos,
        });
        if (anchor)
        {
            ecb.AddComponent<AnchorPoint>(entity);
        }
        else
        {
            ecb.AddComponent<Point>(entity);
        }

        return entity;
    }
}