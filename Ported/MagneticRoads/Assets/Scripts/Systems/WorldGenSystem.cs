using System;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Util;

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
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            bool[] voxels = WorldGen.GenerateVoxels();

            // <index, entity>
            NativeHashMap<int, Entity> IndexToEntityHash = new NativeHashMap<int, Entity>(voxels.Length, Allocator.Temp);
            
            // <index, normal>
            NativeHashMap<int, int3> IndexToNormalHash = new NativeHashMap<int, int3>(voxels.Length, Allocator.Temp);

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
                        
                        var entity = ecb.CreateEntity();
                        
                        var startNormal = IndexToNormalHash[i];
                        var endNormal = IndexToNormalHash[j];

                        var startTangent = startNormal.x == 0 ? new float3(0, 1, 0) : new float3(1, 0, 0);
                        var endTangent = endNormal.x == 0 ? new float3(0, 1, 0) : new float3(1, 0, 0);
                        
                        var Start = new Spline.RoadTerminator
                        {
                            Position = coords,
                            Normal = startNormal,
                            Tangent = startTangent
                        };
                        var End = new Spline.RoadTerminator
                        {
                            Position = neighbors[j],
                            Normal = endNormal,
                            Tangent = endTangent,
                        };

                        ecb.AddComponent(entity, new RoadSegment
                        {
                            Start = Start, 
                            End = End, 
                            Length = Spline.EvaluateLength(Start, End),
                            StartIntersection = IndexToEntityHash[i],
                            EndIntersection = IndexToEntityHash[j]
                        });
                        
                        ecb.AddComponent<Lane>(entity);
                        ecb.AddComponent<Lane>(entity);
                        ecb.AddComponent<Lane>(entity);
                        ecb.AddComponent<Lane>(entity);
                        ecb.AddBuffer<CarDynamicBuffer>(entity).Reinterpret<Entity>();
                        ecb.AddBuffer<CarDynamicBuffer>(entity).Reinterpret<Entity>();
                        ecb.AddBuffer<CarDynamicBuffer>(entity).Reinterpret<Entity>();
                        ecb.AddBuffer<CarDynamicBuffer>(entity).Reinterpret<Entity>();

                        IndexToEntityHash.Remove(i);
                        IndexToEntityHash.Remove(j);
                    }
                }
            }
        }
    }
}
