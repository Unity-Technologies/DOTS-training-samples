using Magneto.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Magneto.Track.Jobs
{
#if !DISABLE_BURST
    [BurstCompile]
#endif
    public struct BuildVoxelMapJob : IJob
    {
        private const int IterationCount = 50000;

        [ReadOnly] public NativeArray<int3> R_CachedNeighbourIndexOffsets;
        [ReadOnly] public NativeArray<int3> R_LimitedCachedNeighbourIndexOffsets;

        public NativeList<IntersectionData> RW_Intersections;
        public NativeStrideGridArray<bool> RW_TrackVoxels;

        [WriteOnly] public NativeStrideGridArray<int> W_IntersectionsGrid;

        public void Execute()
        {
            var middle = TrackManager.VOXEL_COUNT / 2;

            // TODO Make random:)
            var random = Random.CreateFromIndex(0);

            // Create our temp allocated
            var activeVoxels = new NativeList<int3>(Allocator.Temp) {new int3(middle, middle, middle)};

            RW_TrackVoxels[middle, middle, middle] = true;

            for (var i = 0; i < IterationCount; i++)
            {
                if (activeVoxels.Length == 0) break;

                var index = random.NextInt(0, activeVoxels.Length);

                var currentPosition = activeVoxels[index];
                var randomDirection = R_CachedNeighbourIndexOffsets[random.NextInt(0, TrackManager.DIRECTIONS_LENGTH)];

                var newPosition = currentPosition + randomDirection;

                // Evaluate our neighbour position for an active voxel
                if (!GetVoxel(newPosition) && CountNeighbors(newPosition, true) < 3)
                {
                    activeVoxels.Add(newPosition);
                    RW_TrackVoxels[newPosition] = true;
                }

                if (CountNeighbors(currentPosition) >= 3)
                {
                    activeVoxels.RemoveAt(index);
                    var intersectionData = new IntersectionData
                    {
                        Index = RW_Intersections.Length,
                        Position = currentPosition
                    };

                    RW_Intersections.Add(intersectionData);
                    W_IntersectionsGrid[currentPosition] = RW_Intersections.Length - 1;

                    activeVoxels.RemoveAt(index);
                }
            }

            activeVoxels.Dispose();
        }

        private bool GetVoxel(int x, int y, int z)
        {
            return GetVoxel(new int3(x, y, z));
        }


        private bool GetVoxel(int3 position, bool outOfBoundsReturns = true)
        {
            if (position.x >= 0 && position.x < TrackManager.VOXEL_COUNT &&
                position.y >= 0 && position.y < TrackManager.VOXEL_COUNT &&
                position.z >= 0 && position.z < TrackManager.VOXEL_COUNT)
                return RW_TrackVoxels[position.x, position.y, position.z];

            return outOfBoundsReturns;
        }

        private int CountNeighbors(int3 position, bool includeDiagonal = false)
        {
            var neighborCount = 0;
            var x = position.x;
            var y = position.y;
            var z = position.z;


            if (includeDiagonal)
                for (var k = 0; k < TrackManager.DIRECTIONS_LENGTH; k++)
                {
                    var dir = R_CachedNeighbourIndexOffsets[k];

                    if (GetVoxel(x + dir.x, y + dir.y, z + dir.z)) neighborCount++;
                }
            else
                for (var k = 0; k < TrackManager.LIMITED_DIRECTIONS_LENGTH; k++)
                {
                    var dir = R_LimitedCachedNeighbourIndexOffsets[k];
                    if (GetVoxel(x + dir.x, y + dir.y, z + dir.z)) neighborCount++;
                }


            return neighborCount;
        }
    }
}