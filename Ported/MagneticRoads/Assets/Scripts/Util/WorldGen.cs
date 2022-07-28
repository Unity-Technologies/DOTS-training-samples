using Unity.Mathematics;
using Unity.Collections;

namespace Util
{
    public static class WorldGen
    {
        public const int WorldSize = 10;
        public const int MaxAttempts = 1000;

        public static int3[] CardinalDirections = new int3[]
        {
            new int3(1,0,0),
            new int3(-1,0,0),
            new int3(0,1,0),
            new int3(0,-1,0),
            new int3(0,0,1),
            new int3(0,0,-1),
        };

        public static int3[] AllDirections = new int3[26];

        public static bool[] GenerateVoxels()
        {
            var random = new Random();
            
            // [ SETUP ]
            // flattening 3-dimensional voxels
            var voxelCount = WorldSize * WorldSize * WorldSize;
            // true indicates presence of an intersection
            var voxels = new bool[voxelCount];
            // active voxels will be considered to have new neighbouring intersections
            var activeVoxels = new NativeList<int>();

            // Setup middle voxel
            var middle = GetVoxelIndex(WorldSize / 2, WorldSize / 2, WorldSize / 2);
            voxels[middle] = true;
            activeVoxels.Add(middle);
            
            // [ DO ALL THE THINGS ]
            var attempts = 0;
            while (attempts < MaxAttempts && activeVoxels.Length > 0)
            {
                attempts++;
                var activeNode = activeVoxels[random.NextInt(activeVoxels.Length)];
            }
                        
            return voxels;
        }

        public static int GetVoxelIndex(int x, int y, int z)
        {
            return x + WorldSize * y + WorldSize * WorldSize * z;
        }

        public static int3 GetVoxelCoords(int index)
        {
            return int3.zero;
        }
    }
}