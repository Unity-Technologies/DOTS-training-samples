using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

struct TrackSplineCtorData
{
    public float3 tangent1;
    public float3 tangent2;
    public ushort startIntersection;
    public ushort endIntersection;
}

[BurstCompile]
struct FindSplinesJob : IJob
{
    public NativeList<Intersection> Intersections;
    [ReadOnly]
    public NativeList<int3> IntersectionIndices;
    [ReadOnly]
    public NativeArray<bool> TrackVoxels;
    [ReadOnly]
    public NativeArray<int> IntersectionsGrid;

    public NativeList<TrackSplineCtorData> OutTrackSplinePrototypes;

    public Random Rng;
    public int VoxelCount;

    static int Idx3d(int3 pos, int l)
    {
        if (math.all((pos >= 0) & (pos < l)))
            return l * (pos.z * l + pos.y) + pos.x;
        return -1;
    }

    bool GetVoxel(int3 pos)
    {
        int idx = Idx3d(pos, VoxelCount);
        return idx >= 0 && TrackVoxels[idx];
    }

    int FindFirstIntersection(int3 pos,  NativeArray<int3> directions, int3 dir, out int3 otherDirection)
    {
        // step along our voxel paths (before splines have been spawned),
        // starting at one intersection, and stopping when we reach another intersection
        while (true)
        {
            pos += dir;
            var posIdx = Idx3d(pos, VoxelCount);
            if (IntersectionsGrid[posIdx] >= 0)
            {
                otherDirection = dir * -1;
                return IntersectionsGrid[posIdx];
            }

            if (GetVoxel(pos + dir)) continue;
            bool foundTurn = false;
            for (int i = 0; i < directions.Length; i++)
            {
                if (math.all(directions[i] == dir) || math.all(directions[i] == dir * -1))
                    continue;
                if (!GetVoxel(pos + directions[i]))
                    continue;
                dir = directions[i];
                foundTurn = true;
                break;
            }

            if (!foundTurn)
            {
                // dead end
                otherDirection = new int3();
                return -1;
            }
        }
    }

    static long HashIntersectionPair(int a, int b)
    {
        int u = math.min(a, b);
        int v = math.max(a, b);
        return ((long)u << 32) | v;
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
        
        var intersectionPairs = new NativeHashMap<long, bool>(Intersections.Length, Allocator.Temp);
        for (int i = 0; i < Intersections.Length; i++)
        {
            int3 axesWithNeighbors = new int3();
            for (int j = 0; j < dirs.Length; j++)
            {
                if (!GetVoxel(IntersectionIndices[i] + dirs[j]))
                    continue;
                axesWithNeighbors += math.abs(dirs[j]);

                int neighbor = FindFirstIntersection(
                    IntersectionIndices[i], dirs,
                    dirs[j],
                    out var connectDir);
                if (neighbor < 0 || neighbor == i)
                    continue;

                // make sure we haven't already added the reverse-version of this spline
                long hash = HashIntersectionPair(i, neighbor);
                if (!intersectionPairs.TryAdd(hash, true))
                    continue;
                int splineIdx = OutTrackSplinePrototypes.Length;
                OutTrackSplinePrototypes.Add(new TrackSplineCtorData
                {
                    startIntersection = (ushort)i,
                    endIntersection = (ushort)neighbor,
                    tangent1 = dirs[j],
                    tangent2 = connectDir
                });
                
                {
                    var inter = Intersections[i];
                    inter.Neighbors.Add((ushort)neighbor, (ushort)splineIdx);
                    Intersections[i] = inter;
                }

                {
                    var inter = Intersections[neighbor];
                    inter.Neighbors.Add((ushort)i, (ushort)splineIdx);
                    Intersections[neighbor] = inter;
                }
            }

            // find this intersection's normal - it's the one axis
            // along which we have no neighbors
            for (int j = 0; j < 3; j++)
            {
                if (axesWithNeighbors[j] != 0)
                    continue;
                var inter = Intersections[i];
                inter.Normal[j] = -1 + Rng.NextInt(2) * 2;
                Intersections[i] = inter;
                break;
            }

            // NOTE - if you investigate the above logic, you might be confused about how
            // dead-ends are given normals, since we're assuming that all intersections
            // have two axes with neighbors and only one axis without. dead-ends only have
            // one neighbor-axis...and somehow they still get a normal without a special case.
            //
            // the "gotcha" is that the visible dead-ends in the demo have three
            // neighbors during the voxel phase, with two of their neighbor chains leading
            // to nothing. these "hanging chains" are not included as splines, so the
            // dead-ends that we see are actually "T" shapes with the top two segments hidden.
        }
    }
}