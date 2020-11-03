using Magneto.Track.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Magneto.Track
{
    public static class TrackManager
    {
        public const int DIRECTIONS_LENGTH = 26;
        public const int LIMITED_DIRECTIONS_LENGTH = 6;
        
        
        
        public const int JOB_EXECUTION_MAXIMUM_FRAMES = 4;
        public const int VOXEL_COUNT = 60;
        public const float VOXEL_SIZE = 1f;
        public const int TRI_PER_MESH = 4000;
        
        
        public const float intersectionSize = .5f;
        public const float trackRadius = .2f;
        public const float trackThickness = .05f;
        public const int splineResolution=20;
        public const float carSpacing = .13f;

        const int instancesPerBatch=1023;
        
        
        public static TrackGenerationSystem System;
        

        // Grid split based X y sizingf to make voxels

        // build map of grid cells that will used

        // split into sub cells and map
        
        //
        
        public class TrackGenerationSystem
        {
            public bool IsRunning { get; private set; }

            private NativeArray<int3> _cachedNeighbourIndexOffsets;
            private NativeArray<int3> _cachedNeighbourIndexOffsetsLimited;

            public void Init()
            {
                _cachedNeighbourIndexOffsets = new NativeArray<int3>(DIRECTIONS_LENGTH, Allocator.Persistent);
                _cachedNeighbourIndexOffsetsLimited = new NativeArray<int3>(LIMITED_DIRECTIONS_LENGTH, Allocator.Persistent);
            
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(1,0,0);
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(-1,0,0);
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(0,1,0);
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(0,-1,0);
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(0,0,1);
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(0,0,-1);
            }
            
            public void Schedule()
            {
                IsRunning = true;
                
                // Create Cached Directional Offsets
                var cacheNeighbourIndexOffsetsJobHandle = new CachedNeighbourIndexOffsetJob
                {
                    Buffer = _cachedNeighbourIndexOffsets
                }.Schedule();

                // Build Voxel Map
                // TODO: Would love to break out to parallize more
                var buildVoxelMapJobHandle = new BuildVoxelMapJob
                {

                }.Schedule(cacheNeighbourIndexOffsetsJobHandle);



            }

            public void CleanUp()
            {
                _cachedNeighbourIndexOffsets.Dispose();
            }
        }
    }
}
