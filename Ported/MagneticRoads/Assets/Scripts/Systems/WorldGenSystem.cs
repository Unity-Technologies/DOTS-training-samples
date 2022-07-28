using System;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Util;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    partial struct WorldGenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int worldScale = 10; // How large a voxel is in Unity world space
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var voxels = WorldGen.GenerateVoxels();
            var random = Random.CreateFromIndex(11111);

            // <index, entity>
            NativeHashMap<int, Entity> IndexToEntityHash = new NativeHashMap<int, Entity>(voxels.Length, Allocator.Temp);

            // <index, normal>
            NativeHashMap<int, int3> IndexToNormalHash = new NativeHashMap<int, int3>(voxels.Length, Allocator.Temp);

            // Get norms and create Intersections
            for (int i = 0; i < voxels.Length; i++)
            {
                int3 coords = WorldGen.GetVoxelCoords(i);
                var neighbors = WorldGen.GetCardinalNeighbors(coords);

                int3 sumOfVoxels = new int3();
                var normal = new int3();

                foreach (var voxelSet in neighbors)
                {
                    sumOfVoxels += math.abs(voxelSet);
                }

                if (sumOfVoxels.x == 0)
                    normal = new int3(1, 0, 0);
                else if (sumOfVoxels.y == 0)
                    normal = new int3(0, 1, 0);
                else if (sumOfVoxels.z == 0)
                    normal = new int3(0, 1, 0);

                var entity = ecb.CreateEntity();
                ecb.AddComponent<Translation>(entity);
                ecb.AddComponent(entity, new Rotation {Value = quaternion.LookRotation(sumOfVoxels, normal)});
                ecb.AddComponent<LocalToWorld>(entity);
                ecb.AddComponent<Intersection>(entity);

                IndexToEntityHash.Add(i, entity);
                IndexToNormalHash.Add(i, normal);
            }

            // Create RoadSegments between Intersections and populate them with cars and lane data
            for (int i = 0; i < voxels.Length; i++)
            {
                var voxelBeingEvaluated = voxels[i];
                if (!voxelBeingEvaluated)
                    continue;

                int3 coords = WorldGen.GetVoxelCoords(i);
                var neighbors = WorldGen.GetCardinalNeighbors(coords);

                for (int j = 0; j < neighbors.Length; j++)
                {
                    var flatIndexOfNeighbor = WorldGen.GetVoxelIndex(neighbors[j]);

                    if (voxels[flatIndexOfNeighbor])
                    {
                        if (!IndexToEntityHash.ContainsKey(i) || !IndexToEntityHash.ContainsKey(j))
                            continue;

                        var roadEntity = ecb.CreateEntity();

                        var startNormal = IndexToNormalHash[i];
                        var endNormal = IndexToNormalHash[j];

                        var startTangent = startNormal.x == 0 ? new float3(0, 1, 0) : new float3(1, 0, 0);
                        var endTangent = endNormal.x == 0 ? new float3(0, 1, 0) : new float3(1, 0, 0);

                        var Start = new Spline.RoadTerminator
                        {
                            Position = coords * worldScale,
                            Normal = startNormal,
                            Tangent = startTangent
                        };
                        var End = new Spline.RoadTerminator
                        {
                            Position = neighbors[j] * worldScale,
                            Normal = endNormal,
                            Tangent = endTangent,
                        };

                        ecb.AddComponent(roadEntity, new RoadSegment
                        {
                            Start = Start,
                            End = End,
                            Length = Spline.EvaluateLength(Start, End),
                            StartIntersection = IndexToEntityHash[i],
                            EndIntersection = IndexToEntityHash[j]
                        });

                        ecb.AddComponent<Lane>(roadEntity);
                        ecb.AddComponent<Lane>(roadEntity);
                        ecb.AddComponent<Lane>(roadEntity);
                        ecb.AddComponent<Lane>(roadEntity);
                        
                        NativeArray<DynamicBuffer<Entity>> carDynamicBuffers = new NativeArray<DynamicBuffer<Entity>>(4, Allocator.Temp);
                        carDynamicBuffers[0] = ecb.AddBuffer<CarDynamicBuffer>(roadEntity).Reinterpret<Entity>();
                        carDynamicBuffers[1] = ecb.AddBuffer<CarDynamicBuffer>(roadEntity).Reinterpret<Entity>();
                        carDynamicBuffers[2] = ecb.AddBuffer<CarDynamicBuffer>(roadEntity).Reinterpret<Entity>();
                        carDynamicBuffers[3] = ecb.AddBuffer<CarDynamicBuffer>(roadEntity).Reinterpret<Entity>();

                        // Populate Dynamic buffers with random amount of cars
                        for (int k = 0; k < 4; k++)
                        {
                            for (int l = 0; l < random.NextInt(0, 10); l++)
                            {
                                var carEntity = ecb.CreateEntity();
                                ecb.SetComponent(carEntity, new Car {RoadSegment = roadEntity, Speed = 3f, LaneNumber = k}); // Give K as lane number
                                carDynamicBuffers[k].Add(carEntity);
                            }
                        }

                        IndexToEntityHash.Remove(i);
                        IndexToEntityHash.Remove(j);
                    }
                }
            }
        }
    }
}
