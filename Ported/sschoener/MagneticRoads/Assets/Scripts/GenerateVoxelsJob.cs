using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct GenerateVoxelsJob : IJob
{
    public int VoxelCount;
    public float VoxelSize;
    public NativeArray<bool> Voxels;
    public NativeList<int3> OutputIntersectionIndices;
    public NativeList<Intersection> OutputIntersections;
    public NativeArray<int> OutputIntersectionGrid;
    public Unity.Mathematics.Random Rng;

    int Idx(int3 p) => (p.z * VoxelCount + p.y) * VoxelCount + p.x;
    bool HasVoxel(int3 p) => math.any((p >= VoxelCount) & (p < 0)) || Voxels[Idx(p)];
    
    bool HasLessNeighborsThan(int3 p, int max, NativeArray<int3> dirList)
    {
        int neighborCount = 0;
        for (int k = 0; k < dirList.Length; k++)
        {
            int3 dir = dirList[k];
            if (HasVoxel(p + dir))
            {
                neighborCount++;
                if (neighborCount >= max)
                    return false;
            }
        }
        return true;
    }

    public void Execute()
    {
        var dirs = new NativeArray<int3>(6, Allocator.Temp);
        dirs[0] = new int3(1, 0, 0);
        dirs[1] = new int3(-1, 0, 0);
        dirs[2] = new int3(0, 1, 0);
        dirs[3] = new int3(0, -1, 0);
        dirs[4] = new int3(0, 0, 1);
        dirs[5] = new int3(0, 0, -1);
        
        var fullDirs = new NativeArray<int3>(26, Allocator.Temp);
        int dirIndex = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0)
                    {
                        fullDirs[dirIndex] = new int3(x, y, z);
                        dirIndex++;
                    }
                }
            }
        }
        
        const int steps = 50000;
        int ticker = 0;
        var activeVoxels = new NativeList<int3>(Allocator.Temp);
        activeVoxels.Add(new int3(VoxelCount / 2));
        Voxels[VoxelCount / 2 * (VoxelCount * VoxelCount + VoxelCount + 1)] = true;
        while (activeVoxels.Length > 0 && ticker < steps)
        {
            ticker++;
            int index = Rng.NextInt(activeVoxels.Length);
            int3 pos = activeVoxels[index];
            int3 dir = dirs[Rng.NextInt(dirs.Length)];
            int3 pos2 = pos + dir;
            if (!HasVoxel(pos2))
            {
                // when placing a new voxel, it must have fewer than three
                // diagonal-or-cardinal neighbors.
                // (this blocks nonplanar intersections from forming)
                if (HasLessNeighborsThan(pos2, 3, fullDirs))
                {
                    activeVoxels.Add(pos2);
                    Voxels[Idx(pos2)] = true;
                }
            }

            if (!HasLessNeighborsThan(pos, 3, dirs))
            {
                // no more than three cardinal neighbors for any voxel (no 4-way intersections allowed)
                // (really, this is to avoid nonplanar intersections)
                OutputIntersectionIndices.Add(pos);
                OutputIntersections.Add(new Intersection
                {
                    Position = (float3) pos * VoxelSize
                });
                OutputIntersectionGrid[Idx(pos)] = OutputIntersectionIndices.Length - 1;
                activeVoxels.RemoveAtSwapBack(index);
            }
        }
    }
}
