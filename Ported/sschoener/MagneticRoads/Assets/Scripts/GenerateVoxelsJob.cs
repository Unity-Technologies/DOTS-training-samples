using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct GenerateVoxelsJob : IJob
{
    [ReadOnly]
    public NativeArray<int3> Directions;

    [ReadOnly]
    public NativeArray<int3> FullDirections;

    public int VoxelCount;
    public NativeArray<bool> Voxels;
    public NativeList<int3> ActiveVoxels;
    public NativeList<int3> OutputIntersections;
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
        const int steps = 50000;
        int ticker = 0;
        while (ActiveVoxels.Length > 0 && ticker < steps)
        {
            ticker++;
            int index = Rng.NextInt(ActiveVoxels.Length);
            int3 pos = ActiveVoxels[index];
            int3 dir = Directions[Rng.NextInt(Directions.Length)];
            int3 pos2 = pos + dir;
            if (!HasVoxel(pos2))
            {
                // when placing a new voxel, it must have fewer than three
                // diagonal-or-cardinal neighbors.
                // (this blocks nonplanar intersections from forming)
                if (HasLessNeighborsThan(pos2, 3, FullDirections))
                {
                    ActiveVoxels.Add(pos2);
                    Voxels[Idx(pos2)] = true;
                }
            }

            if (!HasLessNeighborsThan(pos, 3, Directions))
            {
                // no more than three cardinal neighbors for any voxel (no 4-way intersections allowed)
                // (really, this is to avoid nonplanar intersections)
                OutputIntersections.Add(pos);
                ActiveVoxels.RemoveAtSwapBack(index);
            }
        }
    }
}
