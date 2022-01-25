using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class CitySpawnerSystem : SystemBase
{
    static readonly float3 k_Up = new float3(0, 1, 0);
    static readonly float3 k_Left = new float3(-1, 0, 0);
    
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1337);

        Entities
            .ForEach((Entity entity, in CitySpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (int i = 0; i < spawner.NumberOfTowers; ++i)
                {
                    int height = random.NextInt(spawner.MinTowerHeight, spawner.MaxTowerHeight);
                    var joints = new NativeArray<Entity>(height * 3, Allocator.Temp);
                    var jointPosition = new NativeArray<float3>(height * 3, Allocator.Temp);
                    int towerIndex = 0;
                    var pos = new Vector3(
                        random.NextFloat(-spawner.CityWidth, spawner.CityWidth), 
                        0f, 
                        random.NextFloat(-spawner.CityLength, spawner.CityLength));
                    float spacing = 2f;
                    for (int j = 0; j < height; j++)
                    {
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x + spacing, j * spacing, pos.z - spacing), j == 0);
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x - spacing, j * spacing, pos.z - spacing), j == 0);
                        joints[towerIndex] = SpawnJoint(ecb, jointPosition[towerIndex++] = new float3(pos.x, j * spacing, pos.z + spacing), j == 0);
                    }

                    for (int j = 0; j < joints.Length; j += 3)
                    {
                        SpawnBar(ecb, spawner.BarPrefab, joints[j], jointPosition[j], joints[j+1], jointPosition[j+1], k_Up);
                        SpawnBar(ecb, spawner.BarPrefab, joints[j+1], jointPosition[j+1], joints[j+2], jointPosition[j+2], k_Up);
                        SpawnBar(ecb, spawner.BarPrefab, joints[j+2], jointPosition[j+2], joints[j], jointPosition[j], k_Up);
                    }
                    for (int j = 0; j < joints.Length - 3; ++j)
                    {
                        SpawnBar(ecb, spawner.BarPrefab, joints[j], jointPosition[j], joints[j+3], jointPosition[j+3], k_Left);
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