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
                        var entity = ecb.CreateEntity();

                        var Start = new Spline.RoadTerminator
                        {
                            Position = coords,
                            Normal = new float3(0, 1, 0),
                            Tangent = new float3(0, 0, 1)
                        };
                        var End = new Spline.RoadTerminator
                        {
                            Position = neighbors[j],
                            Normal = new float3(0, 1, 0),
                            Tangent = new float3(0, 0, 1),
                        };

                        ecb.AddComponent(entity, new RoadSegment
                        {
                            Start = Start, 
                            End = End, 
                            Length = Spline.EvaluateLength(Start, End)
                        });
                    }
                }
            }
        }
    }
}
