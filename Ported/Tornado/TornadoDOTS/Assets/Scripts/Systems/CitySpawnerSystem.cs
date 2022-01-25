using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class CitySpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1337);

        Entities
            .ForEach((Entity entity, in CitySpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.NumberOfClusters; ++i)
                {
                    var cluster = ecb.CreateEntity();

                    // TODO: create a grid based on number of clusters -> assign grid mid position + radius to cluster so we don't have overlapping buildings
                    var position = new Vector3(
                        random.NextFloat(-spawner.CityWidth, spawner.CityWidth),
                        0f,
                        random.NextFloat(-spawner.CityLength, spawner.CityLength));
                    ecb.AddComponent(cluster, new Cluster
                    {
                        Position = position,
                        NumberOfSubClusters = random.NextInt(1, 5),
                        MinTowerHeight = spawner.MinTowerHeight,
                        MaxTowerHeight = spawner.MaxTowerHeight,
                    });
                    ecb.AddBuffer<Joint>(cluster);
                    ecb.AddBuffer<Connection>(cluster);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        Entities
            .ForEach((Entity entity, 
                DynamicBuffer<Joint> joints,
                DynamicBuffer<Connection> connections, 
                in Cluster cluster) =>
            {
                if (!joints.IsEmpty || !connections.IsEmpty)
                {
                    return; // TODO: instead of this, we could have a tag component on cluster to signal that it has been generated, and then filter on that
                }
                var clusterPosition = cluster.Position;
                float spacing = 2f;
                float clusterSize = cluster.NumberOfSubClusters * spacing;
                for (int c = 0; c < cluster.NumberOfSubClusters; ++c)
                {
                    int height = random.NextInt(cluster.MinTowerHeight, cluster.MaxTowerHeight);
                    var pos = clusterPosition + new float3(
                        random.NextFloat(-clusterSize, clusterSize), 
                        0f, 
                        random.NextFloat(-clusterSize, clusterSize));
                    for (int h = 0; h < height; ++h)
                    {
                        joints.Add(new Joint
                        {
                            Value = new float3(pos.x + spacing, h * spacing, pos.z - spacing), IsAnchored = h == 0
                        });
                        joints.Add(new Joint
                        {
                            Value = new float3(pos.x - spacing, h * spacing, pos.z - spacing), IsAnchored = h == 0
                        });
                        joints.Add(new Joint
                        {
                            Value = new float3(pos.x, h * spacing, pos.z + spacing), IsAnchored = h == 0
                        });
                    }
                
                    // horizontal bars
                    for (int j = 0; j < joints.Length; j += 3)
                    {
                        connections.Add(CreateConnection(j, j+1, joints));
                        connections.Add(CreateConnection(j+1, j+2, joints));
                        connections.Add(CreateConnection(j+2, j, joints));
                    }
                    // vertical bars
                    for (int j = 0; j < joints.Length - 3; ++j)
                    {
                        connections.Add(CreateConnection(j, j+3, joints));
                    }
                    // cross bars
                    for (int j = 3; j < joints.Length; j += 3)
                    {
                        connections.Add(CreateConnection(j, j-2, joints));
                        connections.Add(CreateConnection(j, j-1, joints));
                        connections.Add(CreateConnection(j+1, j-3, joints));
                        connections.Add(CreateConnection(j+1, j-1, joints));
                        connections.Add(CreateConnection(j+2, j-3, joints));
                        connections.Add(CreateConnection(j+2, j-2, joints));
                    }
                }
            }).ScheduleParallel();
    }

    static Connection CreateConnection(int j1, int j2, DynamicBuffer<Joint> joints)
    {
        return new Connection
        {
            J1 = j1, J2 = j2, OriginalLength = math.length(joints[j1].Value - joints[j2].Value),
        };
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