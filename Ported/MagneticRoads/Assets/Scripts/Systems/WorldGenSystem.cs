using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Util;

namespace Systems
{
    [BurstCompile]
    partial struct WorldGenSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            NativeArray<int3> tempVoxels = new NativeArray<int3>(3, Allocator.Temp);
            tempVoxels[0] = new int3(1, -1, 0);
            tempVoxels[1] = new int3(0, 1, 0);
            tempVoxels[0] = new int3(-1, 0, 0);

            int3 sumOfVoxels = new int3();
            foreach (var voxelSet in tempVoxels)
            {
                sumOfVoxels += math.abs(voxelSet);
            }

            var normal = new int3();

            if (sumOfVoxels.x == 0)
                normal = new int3(1, 0, 0);
            else if (sumOfVoxels.y == 0)
                normal = new int3(0, 1, 0);
            else if (sumOfVoxels.z == 0)
                normal = new int3(0, 1, 0);
        }
    }
}