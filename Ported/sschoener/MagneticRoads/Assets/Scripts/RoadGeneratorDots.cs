using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGeneratorDots : MonoBehaviour
{
    public int voxelCount = 60;
    public float voxelSize = 1f;
    public int trisPerMesh = 4000;
    public Material roadMaterial;
    public Mesh intersectionMesh;
    public Mesh intersectionPreviewMesh;
    public Mesh carMesh;
    public Material carMaterial;
    public float carSpeed = 2f;

    NativeArray<bool> m_TrackVoxels;
    List<Intersection> m_Intersections;
    List<TrackSpline> m_TrackSplines;
    Intersection[,,] m_IntersectionsGrid;
    List<Car> m_Cars;

    List<List<Matrix4x4>> m_CarMatrices;

    NativeArray<int3> m_Dirs;
    NativeArray<int3> m_FullDirs;

    public const float intersectionSize = .5f;
    public const float trackRadius = .2f;
    public const float trackThickness = .05f;
    public const int splineResolution = 20;
    public const float carSpacing = .13f;

    const int k_InstancesPerBatch = 1023;

    // intersection pair:  two 32-bit IDs, packed together
    HashSet<long> m_IntersectionPairs;

    List<Mesh> m_RoadMeshes;
    List<List<Matrix4x4>> m_IntersectionMatrices;

    MaterialPropertyBlock m_CarMatProps;
    List<List<Vector4>> m_CarColors;

    long HashIntersectionPair(Intersection a, Intersection b)
    {
        // pack two intersections' IDs into one int64
        int id1 = a.id;
        int id2 = b.id;

        return ((long)Mathf.Min(id1, id2) << 32) + Mathf.Max(id1, id2);
    }

    bool GetVoxel(int3 pos)
    {
        if (math.all((pos >= 0) & (pos < voxelCount)))
        {
            int idx = voxelCount * (pos.z * voxelCount + pos.y) + pos.x;
            return m_TrackVoxels[idx];
        }
        return false;
    }

    Vector3Int ToV3(int3 v) => new Vector3Int(v.x, v.y, v.z);
    
    Intersection FindFirstIntersection(int3 pos, int3 dir, out int3 otherDirection)
    {
        // step along our voxel paths (before splines have been spawned),
        // starting at one intersection, and stopping when we reach another intersection
        while (true)
        {
            pos += dir;
            if (m_IntersectionsGrid[pos.x, pos.y, pos.z] != null)
            {
                otherDirection = dir * -1;
                return m_IntersectionsGrid[pos.x, pos.y, pos.z];
            }

            if (!GetVoxel(pos + dir))
            {
                bool foundTurn = false;
                for (int i = 0; i < m_Dirs.Length; i++)
                {
                    if (math.any(m_Dirs[i] != dir) && math.any(m_Dirs[i] != (dir * -1)))
                    {
                        if (GetVoxel(pos + m_Dirs[i]))
                        {
                            dir = m_Dirs[i];
                            foundTurn = true;
                            break;
                        }
                    }
                }

                if (foundTurn == false)
                {
                    // dead end
                    otherDirection = new int3();
                    return null;
                }
            }
        }
    }

    void Start()
    {
        // cardinal directions:
        m_Dirs = new NativeArray<int3>(new int3[]
        {
            new int3(1, 0, 0),
            new int3(-1, 0, 0),
            new int3(0, 1, 0),
            new int3(0, -1, 0),
            new int3(0, 0, 1),
            new int3(0, 0, -1)
        }, Allocator.Persistent);

        // cardinal directions + diagonals in 3D:
        m_FullDirs = new NativeArray<int3>(26, Allocator.Persistent);
        int dirIndex = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x != 0 || y != 0 || z != 0)
                    {
                        m_FullDirs[dirIndex] = new int3(x, y, z);
                        dirIndex++;
                    }
                }
            }
        }

        StartCoroutine(SpawnRoads());
    }

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

    void OnDestroy()
    {
        if (m_TrackVoxels.IsCreated)
            m_TrackVoxels.Dispose();
        if (m_Dirs.IsCreated)
            m_Dirs.Dispose();
        if (m_FullDirs.IsCreated)
            m_FullDirs.Dispose();
    }

    int3 FromV3(Vector3Int v) => new int3(v.x, v.y, v.z);
    
    IEnumerator SpawnRoads()
    {
        // first generation pass: plan roads as basic voxel data only
        m_TrackVoxels = new NativeArray<bool>(voxelCount * voxelCount * voxelCount, Allocator.Persistent);

        // after voxel generation, we'll convert our network into non-voxels
        m_Intersections = new List<Intersection>();
        m_IntersectionsGrid = new Intersection[voxelCount, voxelCount, voxelCount];
        m_IntersectionPairs = new HashSet<long>();
        m_TrackSplines = new List<TrackSpline>();
        m_RoadMeshes = new List<Mesh>();
        m_IntersectionMatrices = new List<List<Matrix4x4>>();

        m_Cars = new List<Car>();
        m_CarMatrices = new List<List<Matrix4x4>>();
        m_CarMatrices.Add(new List<Matrix4x4>());
        m_CarColors = new List<List<Vector4>>();
        m_CarColors.Add(new List<Vector4>());
        m_CarMatProps = new MaterialPropertyBlock();
        m_CarMatProps.SetVectorArray("_Color", new Vector4[k_InstancesPerBatch]);

        // plan roads broadly: first, as a grid of true/false voxels
        {
            using (var activeVoxels = new NativeList<int3>(Allocator.TempJob))
            using (var outputIntersections = new NativeList<int3>(Allocator.TempJob))
            {
                activeVoxels.Add(new int3(voxelCount / 2));

                m_TrackVoxels[voxelCount / 2 * (voxelCount * voxelCount + voxelCount + 1)] = true;

                new GenerateVoxelsJob()
                {
                    Rng = new Unity.Mathematics.Random(12345),
                    VoxelCount = voxelCount,
                    OutputIntersections = outputIntersections,
                    ActiveVoxels = activeVoxels,
                    Voxels = m_TrackVoxels,
                    Directions = m_Dirs,
                    FullDirections = m_FullDirs
                }.Run();

                for (int i = 0; i < outputIntersections.Length; i++)
                {
                    var pos = ToV3(outputIntersections[i]);
                    var intersection = new Intersection(
                        pos,
                        (Vector3)pos * voxelSize,
                        new float3());
                    intersection.id = i;
                    m_Intersections.Add(intersection);
                    m_IntersectionsGrid[pos.x, pos.y, pos.z] = intersection;
                }
            }
        }
        Debug.Log(m_Intersections.Count + " intersections");

        // at this point, we've generated our full layout, but everything
        // is voxels, and we've identified which voxels are intersections.
        // next, we'll reinterpret our voxels as a network of intersections:
        // we'll find all "neighboring intersections" in our voxel map
        // (neighboring intersections are connected by a chain of voxels,
        // which we'll replace with splines)

        for (int i = 0; i < m_Intersections.Count; i++)
        {
            Intersection intersection = m_Intersections[i];
            int3 axesWithNeighbors = new int3();
            for (int j = 0; j < m_Dirs.Length; j++)
            {
                if (GetVoxel(FromV3(intersection.index) + m_Dirs[j]))
                {
                    axesWithNeighbors += math.abs(m_Dirs[j]);

                    Intersection neighbor = FindFirstIntersection(FromV3(intersection.index), m_Dirs[j], out var connectDir);
                    if (neighbor != null && neighbor != intersection)
                    {
                        // make sure we haven't already added the reverse-version of this spline
                        long hash = HashIntersectionPair(intersection, neighbor);
                        if (m_IntersectionPairs.Contains(hash) == false)
                        {
                            m_IntersectionPairs.Add(hash);

                            TrackSpline spline = new TrackSpline(intersection, ToV3(m_Dirs[j]), neighbor, ToV3(connectDir));
                            m_TrackSplines.Add(spline);

                            intersection.neighbors.Add(neighbor);
                            intersection.neighborSplines.Add(spline);
                            neighbor.neighbors.Add(intersection);
                            neighbor.neighborSplines.Add(spline);
                        }
                    }
                }
            }

            // find this intersection's normal - it's the one axis
            // along which we have no neighbors
            for (int j = 0; j < 3; j++)
            {
                if (axesWithNeighbors[j] == 0)
                {
                    if (intersection.normal.Equals(new float3()))
                    {
                        intersection.normal = new float3();
                        intersection.normal[j] = -1 + Random.Range(0, 2) * 2;

                        //Debug.DrawRay(intersection.position,(Vector3)intersection.normal * .5f,Color.red,1000f);
                    }
                    else
                    {
                        Debug.LogError("a straight line has been marked as an intersection!");
                    }
                }
            }

            if (intersection.normal.Equals(new float3()))
            {
                Debug.LogError("nonplanar intersections are not allowed!");
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

            if (i % 20 == 0)
            {
                yield return null;
            }
        }

        Debug.Log(m_TrackSplines.Count + " road splines");

        // generate road meshes

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triCount = 0;

        for (int i = 0; i < m_TrackSplines.Count; i++)
        {
            var ts = m_TrackSplines[i];
            ts.geometry.startNormal = (Vector3)ts.startIntersection.normal;
            ts.geometry.endNormal = (Vector3)ts.endIntersection.normal;
            ts.twistMode = TrackSpline.SelectTwistMode(ts.bezier, ts.geometry);
            TrackSpline.GenerateMesh(ts.bezier, ts.geometry, ts.twistMode, vertices, uvs, triangles);

            if (triangles.Count / 3 > trisPerMesh || i == m_TrackSplines.Count - 1)
            {
                // our current mesh data is ready to go!
                if (triangles.Count > 0)
                {
                    Mesh mesh = new Mesh();
                    mesh.name = "Generated Road Mesh";
                    mesh.SetVertices(vertices);
                    mesh.SetUVs(0, uvs);
                    mesh.SetTriangles(triangles, 0);
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();
                    m_RoadMeshes.Add(mesh);
                    triCount += triangles.Count / 3;
                }

                vertices.Clear();
                uvs.Clear();
                triangles.Clear();
            }

            if (i % 10 == 0)
            {
                yield return null;
            }
        }

        // generate intersection matrices for batch-rendering
        int batch = 0;
        m_IntersectionMatrices.Add(new List<Matrix4x4>());
        for (int i = 0; i < m_Intersections.Count; i++)
        {
            m_IntersectionMatrices[batch].Add(m_Intersections[i].GetMatrix());
            if (m_IntersectionMatrices[batch].Count == k_InstancesPerBatch)
            {
                batch++;
                m_IntersectionMatrices.Add(new List<Matrix4x4>());
            }
        }

        Debug.Log(triCount + " road triangles (" + m_RoadMeshes.Count + " meshes)");

        // spawn cars

        batch = 0;
        for (int i = 0; i < 4000; i++)
        {
            int splineSide = -1 + Random.Range(0, 2) * 2;
            int splineDirection = -1 + Random.Range(0, 2) * 2;
            var roadSpline = m_TrackSplines[Random.Range(0, m_TrackSplines.Count)];
            Car car = new Car(splineSide, splineDirection, carSpeed, roadSpline);
            roadSpline.GetQueue(splineDirection, splineSide).Add(car);

            m_Cars.Add(car);
            m_CarMatrices[batch].Add(Matrix4x4.identity);
            m_CarColors[batch].Add(Random.ColorHSV());
            if (m_CarMatrices[batch].Count == k_InstancesPerBatch)
            {
                m_CarMatrices.Add(new List<Matrix4x4>());
                m_CarColors.Add(new List<Vector4>());
                batch++;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < m_Cars.Count; i++)
        {
            m_Cars[i].Update();
            m_CarMatrices[i / k_InstancesPerBatch][i % k_InstancesPerBatch] = m_Cars[i].matrix;
        }

        for (int i = 0; i < m_RoadMeshes.Count; i++)
        {
            Graphics.DrawMesh(m_RoadMeshes[i], Matrix4x4.identity, roadMaterial, 0);
        }

        for (int i = 0; i < m_IntersectionMatrices.Count; i++)
        {
            Graphics.DrawMeshInstanced(intersectionMesh, 0, roadMaterial, m_IntersectionMatrices[i]);
        }

        for (int i = 0; i < m_CarMatrices.Count; i++)
        {
            if (m_CarMatrices[i].Count > 0)
            {
                m_CarMatProps.SetVectorArray("_Color", m_CarColors[i]);
                Graphics.DrawMeshInstanced(carMesh, 0, carMaterial, m_CarMatrices[i], m_CarMatProps);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_TrackVoxels.IsCreated && m_IntersectionPairs.Count == 0)
        {
            // visualize voxel generation during generation
            for (int x = 0; x < voxelCount; x++)
            {
                for (int y = 0; y < voxelCount; y++)
                {
                    for (int z = 0; z < voxelCount; z++)
                    {
                        if (GetVoxel(new int3(x, y, z)))
                        {
                            Gizmos.DrawWireCube(new Vector3(x, y, z) * voxelSize, new Vector3(.9f, .9f, .9f) * voxelSize);
                        }
                    }
                }
            }
        }

        if (m_RoadMeshes != null && m_RoadMeshes.Count == 0)
        {
            // visualize splines before road meshes have spawned
            if (m_Intersections != null)
            {
                Gizmos.color = new Color(.2f, .2f, 1f);
                for (int i = 0; i < m_Intersections.Count; i++)
                {
                    if (!m_Intersections[i].normal.Equals(new float3()))
                    {
                        Gizmos.DrawWireMesh(intersectionPreviewMesh, 0, m_Intersections[i].position, Quaternion.LookRotation(m_Intersections[i].normal), new Vector3(intersectionSize, intersectionSize, 0f));
                    }
                }
            }

            if (m_TrackSplines != null)
            {
                for (int i = 0; i < m_TrackSplines.Count; i++)
                {
                    m_TrackSplines[i].DrawGizmos();
                }
            }
        }
    }
}
