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

        static void PopulateAllDirections()
        {
            int index = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x != 0 || y != 0 || z != 0)
                        {
                            AllDirections[index] = new int3(x, y, z);
                            index++;
                        }
                    }
                }
            }
        }

        public static bool[] GenerateVoxels()
        {
            var random = new Random();
            
            // [ SETUP ]
            PopulateAllDirections();
            // flattening 3-dimensional voxels
            var voxelCount = WorldSize * WorldSize * WorldSize;
            // true indicates presence of an intersection
            var voxels = new bool[voxelCount];
            // active voxels will be considered to have new neighboring intersections
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
            int3 pos;
            pos.x = index % WorldSize;
            index /= WorldSize;
            pos.y = WorldSize;
            index /= WorldSize;
            pos.z = index;
            return pos;
        }

        public static NativeList<int3> GetCardinalNeighbors(int3 pos)
        {
            var neighbors = new NativeList<int3>();
            foreach (var direction in CardinalDirections)
            {
                var possibleNeighbor = pos + direction;
                if (possibleNeighbor.x > 0 && possibleNeighbor.x < WorldSize
                    && possibleNeighbor.y > 0 && possibleNeighbor.y < WorldSize
                    && possibleNeighbor.z > 0 && possibleNeighbor.z < WorldSize)
                {
                    neighbors.Add(possibleNeighbor);
                }
            }

            return neighbors;
        }
    }
}