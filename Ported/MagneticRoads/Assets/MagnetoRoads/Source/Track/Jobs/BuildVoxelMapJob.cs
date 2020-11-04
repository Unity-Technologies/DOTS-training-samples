using Magneto.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = UnityEngine.Random;

namespace Magneto.Track.Jobs
{
#if !DISABLE_BURST
    [BurstCompile]
#endif
    public struct BuildVoxelMapJob : IJob
    {
        private const int IterationCount = 50000;

        [ReadOnly] public NativeArray<int3> CachedNeighbourIndexOffsets;
        [ReadOnly] public NativeArray<int3> LimitedCachedNeighbourIndexOffsets;
        
        public NativeStrideGridArray<bool> TrackVoxels;
        
        public NativeList<IntersectionData> Intersections;
        [WriteOnly] public NativeStrideGridArray<int> IntersectionsGrid;
        

        public void Execute()
        {
            int middle = TrackManager.VOXEL_COUNT / 2;
            
            // TODO Make random:)
            var random = Unity.Mathematics.Random.CreateFromIndex(0);
            
            // Create our temp allocated
            var activeVoxels = new NativeList<int3>(Allocator.Temp) {new int3(middle, middle, middle)};

            TrackVoxels[middle, middle, middle] = true;

            for (int i = 0; i < IterationCount; i++)
            {
                if (activeVoxels.Length == 0) break;
                
                int index = random.NextInt(0, activeVoxels.Length);
                
                int3 currentPosition = activeVoxels[index];
                int3 randomDirection = CachedNeighbourIndexOffsets[random.NextInt(0, TrackManager.DIRECTIONS_LENGTH)];
                
                int3 newPosition = currentPosition + randomDirection;

                // Evaluate our neighbour position for an active voxel
                if (!GetVoxel(newPosition) && CountNeighbors(newPosition, true) < 3)
                {
                    activeVoxels.Add(newPosition);
                    TrackVoxels[newPosition] = true;
                }

                if (CountNeighbors(currentPosition) >= 3)
                {
                    activeVoxels.RemoveAt(index);
                    var intersectionData = new IntersectionData
                    {
                        Index = Intersections.Length
                    };

                    Intersections.Add(intersectionData);
                    IntersectionsGrid[currentPosition] = Intersections.Length - 1;
                    
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
                position.z >= 0 && position.z < TrackManager.VOXEL_COUNT) {
                return TrackVoxels[position.x,position.y, position.z];
            }

            return outOfBoundsReturns;
        }
        
        int CountNeighbors(int3 position, bool includeDiagonal = false) 
        {
            int neighborCount = 0;
            int x = position.x;
            int y = position.y;
            int z = position.z;

            
            if (includeDiagonal)
            {
                for (int k = 0; k < TrackManager.DIRECTIONS_LENGTH; k++) {
                    int3 dir = CachedNeighbourIndexOffsets[k];
                    
                    if (GetVoxel(x + dir.x,y + dir.y,z + dir.z)) {
                        neighborCount++;
                    }
                }
            }
            else
            {
                for (int k = 0; k < TrackManager.LIMITED_DIRECTIONS_LENGTH; k++) {
                    int3 dir = LimitedCachedNeighbourIndexOffsets[k];
                    if (GetVoxel(x + dir.x,y + dir.y,z + dir.z)) {
                        neighborCount++;
                    }
                }
            }

            
            return neighborCount;
        }
    }
}