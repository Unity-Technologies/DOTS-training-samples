using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class CitySpawnerSystem : SystemBase
{
    EntityCommandBufferSystem m_CommandBufferSystem;
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var random = new Random(1337);

        Entities
            .ForEach((Entity entity, in CitySpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.NumberOfClusters; ++i)
                {
                    var cluster = ecb.CreateEntity();

                    var position = new Vector3(
                        random.NextFloat(-spawner.CityWidth, spawner.CityWidth),
                        0f,
                        random.NextFloat(-spawner.CityLength, spawner.CityLength));
                    ecb.AddComponent(cluster, new Cluster
                    {
                        Position = position,
                    });
                    ecb.AddComponent(cluster, new ClusterGeneration
                    {
                        NumberOfSubClusters = random.NextInt(1, 5),
                        BarPrefab = spawner.BarPrefab,
                        MinTowerHeight = spawner.MinTowerHeight,
                        MaxTowerHeight = spawner.MaxTowerHeight,
                    });
                    
                    ecb.AddBuffer<Joint>(cluster);
                    ecb.AddBuffer<Connection>(cluster);
                    ecb.AddBuffer<Bar>(cluster);
                    
                    ecb.AddComponent<InitializeJointNeighbours>(cluster);
                    ecb.AddBuffer<JointNeighbours>(cluster);
                }
            }).Schedule();

        ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();
        
        Entities
            .WithAll<ClusterGeneration>()
            .ForEach((Entity entity, 
                int entityInQueryIndex,
                DynamicBuffer<Joint> joints,
                DynamicBuffer<Connection> connections, 
                DynamicBuffer<Bar> bars, 
                in ClusterGeneration clusterData,
                in Cluster cluster) =>
            {
                var clusterPosition = cluster.Position;
                float spacing = 2f;
                float clusterSize = clusterData.NumberOfSubClusters * spacing;
                int structureStartIndex = 0;
                for (int c = 0; c < clusterData.NumberOfSubClusters; ++c)
                {
                    int height = random.NextInt(clusterData.MinTowerHeight, clusterData.MaxTowerHeight);
                    var pos = clusterPosition + new float3(
                        random.NextFloat(-clusterSize, clusterSize), 
                        0f, 
                        random.NextFloat(-clusterSize, clusterSize));
                    for (int h = 0; h < height; ++h)
                    {
                        var pos1 = new float3(pos.x + spacing, h * spacing, pos.z - spacing);
                        joints.Add(new Joint
                        {
                            Value = pos1,
                            OldPos = pos1,
                            IsAnchored = h == 0
                        });
                        var pos2 = new float3(pos.x - spacing, h * spacing, pos.z - spacing);
                        joints.Add(new Joint
                        {
                            Value = pos2,
                            OldPos = pos2,
                            IsAnchored = h == 0
                        });
                        
                        var pos3 = new float3(pos.x, h * spacing, pos.z + spacing);
                        joints.Add(new Joint
                        {
                            Value = pos3,
                            OldPos = pos3,
                            IsAnchored = h == 0
                        });
                    }
                
                    // horizontal bars
                    for (int j = structureStartIndex; j < joints.Length; j += 3)
                    {
                        CreateConnection(j, j + 1, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 1, j + 2, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 2, j, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                    }
                    // vertical bars
                    for (int j = structureStartIndex; j < joints.Length - 3; ++j)
                    {
                        CreateConnection(j, j + 3, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                    }
                    // cross bars
                    for (int j = structureStartIndex + 3; j < joints.Length; j += 3)
                    {
                        CreateConnection(j, j - 2, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j, j - 1, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 1, j - 3, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 1, j - 1, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 2, j - 3, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                        CreateConnection(j + 2, j - 2, joints, connections, parallelWriter, entityInQueryIndex, clusterData.BarPrefab, entity);
                    }

                    structureStartIndex = joints.Length;
                }
                
                parallelWriter.RemoveComponent<ClusterGeneration>(entityInQueryIndex, entity);
                parallelWriter.AddComponent<BarAssignColor>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        Entities
            .WithAll<InitializeJointNeighbours>()
            .ForEach((Entity entity, 
                int entityInQueryIndex,
                ref DynamicBuffer<JointNeighbours> neighbours, 
                in DynamicBuffer<Joint> joints,
                in DynamicBuffer<Connection> connections) =>
            {
                neighbours.ResizeUninitialized(joints.Length);
                for (int i = 0; i < neighbours.Length; ++i)
                {
                    neighbours[i] = new JointNeighbours { Value = 0 };
                }
                for (int c = 0; c < connections.Length; ++c)
                {
                    var connection = connections[c];
                    {
                        var jointNeighbours = neighbours[connection.J1];
                        jointNeighbours.Value++;
                        neighbours[connection.J1] = jointNeighbours;
                    }
                    {
                        var jointNeighbours = neighbours[connection.J2];
                        jointNeighbours.Value++;
                        neighbours[connection.J2] = jointNeighbours;
                    }
                }
                
                parallelWriter.RemoveComponent<InitializeJointNeighbours>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    static void CreateConnection(
        int j1, int j2, DynamicBuffer<Joint> joints, DynamicBuffer<Connection> connections,
        EntityCommandBuffer.ParallelWriter parallelWriter, int key, Entity barPrefab, Entity me)
    {
        connections.Add(new Connection
        {
            J1 = j1, J2 = j2, OriginalLength = math.length(joints[j1].Value - joints[j2].Value),
        });
        
        var bar = parallelWriter.Instantiate(key, barPrefab);
        parallelWriter.AddComponent<BarVisualizer>(key, bar);
        parallelWriter.AppendToBuffer(key, me, new Bar {Value=bar});
    }
}