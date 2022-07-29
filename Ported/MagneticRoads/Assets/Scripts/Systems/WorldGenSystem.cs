using System;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Util;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [BurstCompile]
    partial struct WorldGenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TestSplineConfig>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<TestSplineConfig>();

            int worldScale = 10; // How large a voxel is in Unity world space
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var voxels = WorldGen.GenerateVoxels();
            var random = Random.CreateFromIndex(11111);

            // <index, entity>
            NativeHashMap<int, Entity> IndexToEntityHash =
                new NativeHashMap<int, Entity>(voxels.Length, Allocator.Temp);

            // <index, normal>
            NativeHashMap<int, int3> IndexToNormalHash = new NativeHashMap<int, int3>(voxels.Length, Allocator.Temp);

            // Get norms and create Intersections
            for (int voxelIndex = 0; voxelIndex < voxels.Length; voxelIndex++)
            {
                if (voxels[voxelIndex])
                {
                    int3 coords = WorldGen.GetVoxelCoords(voxelIndex);
                    var neighbors = WorldGen.GetCardinalNeighbors(coords);

                    int3 sumOfVoxels = new int3();
                    var normal = new int3();

                    foreach (var voxelSet in neighbors)
                    {
                        if (voxels[WorldGen.GetVoxelIndex(voxelSet)])
                        {
                            sumOfVoxels += math.abs(voxelSet - coords);
                        }
                    }

                    if (sumOfVoxels.x == 0)
                        normal = new int3(1, 0, 0);
                    else if (sumOfVoxels.y == 0)
                        normal = new int3(0, 1, 0);
                    else if (sumOfVoxels.z == 0)
                        normal = new int3(0, 0, 1);

                    var tangent = normal.x == 0 ? new int3(1, 0, 0) : new int3(0, 1, 0);
                    var quat = quaternion.LookRotationSafe(tangent, normal);

                    var entity = ecb.CreateEntity();
                    ecb.AddComponent<Intersection>(entity);
                    ecb.AddComponent(entity, new Translation {Value = coords * worldScale});
                    ecb.AddComponent(entity, new Rotation {Value = quat});
                    ecb.AddComponent<LocalToWorld>(entity);

                    IndexToEntityHash.Add(voxelIndex, entity);
                    IndexToNormalHash.Add(voxelIndex, normal);
                }
            }

            // Create RoadSegments between Intersections and populate them with cars and lane data
            for (int voxelIndex = 0; voxelIndex < voxels.Length; voxelIndex++)
            {
                if (!voxels[voxelIndex])
                    continue;

                int3 coords = WorldGen.GetVoxelCoords(voxelIndex);
                var neighbors = WorldGen.GetCardinalNeighbors(coords);

                for (int j = 0; j < neighbors.Length; j++)
                {
                    var neighborCoords = neighbors[j];
                    var neighborIndex = WorldGen.GetVoxelIndex(neighborCoords);

                    // We have a pair of touching intersections
                    if (voxels[neighborIndex])
                    {
                        // TODO: dedupe
                        // if (!IndexToEntityHash.ContainsKey(voxelIndex) || !IndexToEntityHash.ContainsKey(neighborIndex))
                            // continue;

                        var startNormal = IndexToNormalHash[voxelIndex];
                        var endNormal = IndexToNormalHash[neighborIndex];

                        // TODO: determine RoadTerminator tangents from direction in voxel array
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

                        var roadEntity = ecb.CreateEntity();

                        // Okay so we need to store the lanes locally in this array so we can assign easier
                        NativeArray<DynamicBuffer<Entity>> lanes = new NativeArray<DynamicBuffer<Entity>>(4, Allocator.Temp);
                        
                        // Create the entities that will store the respective lane buffers on the road
                        var lane1Entity = ecb.CreateEntity();
                        var lane2Entity = ecb.CreateEntity();
                        var lane3Entity = ecb.CreateEntity();
                        var lane4Entity = ecb.CreateEntity();

                        // Add the buffers to the entities we just created
                        // lanes[0] = ecb.AddBuffer<CarDynamicBuffer>(lane1Entity).Reinterpret<Entity>();
                        // lanes[1] = ecb.AddBuffer<CarDynamicBuffer>(lane2Entity).Reinterpret<Entity>();
                        // lanes[2] = ecb.AddBuffer<CarDynamicBuffer>(lane3Entity).Reinterpret<Entity>();
                        // lanes[3] = ecb.AddBuffer<CarDynamicBuffer>(lane4Entity).Reinterpret<Entity>();

                        // Create a new buffer on the road entity that stores each of the lanes above
                        // var laneBuffer = ecb.AddBuffer<LaneDynamicBuffer>(roadEntity).Reinterpret<Entity>();
                        // laneBuffer.Add(lane1Entity);
                        // laneBuffer.Add(lane2Entity);
                        // laneBuffer.Add(lane3Entity);
                        // laneBuffer.Add(lane4Entity);

                        ecb.AddComponent(roadEntity, new RoadSegment
                        {
                            Start = Start,
                            End = End,
                            Length = Spline.EvaluateLength(Start, End),
                            StartIntersection = IndexToEntityHash[voxelIndex],
                            EndIntersection = IndexToEntityHash[neighborIndex]
                        });

                        // Populate Dynamic buffers with random amount of cars
                        // for (int laneNumber = 0; laneNumber < 4; laneNumber++)
                        // {
                        //     for (int carNumber = 0; carNumber < random.NextInt(0, 10); carNumber++)
                        //     {
                        //         var carEntity = ecb.Instantiate(config.CarPrefab);
                        //         ecb.SetComponent(carEntity, new Car {RoadSegment = roadEntity, Speed = 3f, LaneNumber = laneNumber}); 
                        //         lanes[laneNumber].Add(carEntity); // Add cars to the car buffers
                        //     }
                        // }

                        // IndexToEntityHash.Remove(voxelIndex);
                        // IndexToEntityHash.Remove(j);
                    }
                }
            }

            state.Enabled = false;
        }
    }
}
