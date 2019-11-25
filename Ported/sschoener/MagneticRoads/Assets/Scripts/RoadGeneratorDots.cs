using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
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
    int[,,] m_IntersectionsGrid;
    List<Car> m_Cars;

    List<List<Matrix4x4>> m_CarMatrices;

    NativeArray<int3> m_Dirs;
    NativeArray<int3> m_FullDirs;

    public const float intersectionSize = .5f;
    public const float trackRadius = .2f;
    public const float trackThickness = .05f;
    public const int splineResolution = 20;
    const float k_CarSpacing = .13f;
    const int k_InstancesPerBatch = 1023;

    // intersection pair:  two 32-bit IDs, packed together
    HashSet<long> m_IntersectionPairs;

    List<Mesh> m_RoadMeshes;
    List<List<Matrix4x4>> m_IntersectionMatrices;

    MaterialPropertyBlock m_CarMatProps;
    List<List<Vector4>> m_CarColors;
    static readonly int k_Color = Shader.PropertyToID("_Color");

    static long HashIntersectionPair(int a, int b)
    {
        int u = math.min(a, b);
        int v = math.max(a, b);
        return ((long)u << 32) | v;
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

    int FindFirstIntersection(int3 pos, int3 dir, out int3 otherDirection)
    {
        // step along our voxel paths (before splines have been spawned),
        // starting at one intersection, and stopping when we reach another intersection
        while (true)
        {
            pos += dir;
            if (m_IntersectionsGrid[pos.x, pos.y, pos.z] >= 0)
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
                    return -1;
                }
            }
        }
    }

    void Start()
    {
        Random.InitState(1);
        // cardinal directions:
        m_Dirs = new NativeArray<int3>(new[]
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

        SpawnRoads();
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

    struct TrackSplineCtorData
    {
        public float3 tangent1;
        public float3 tangent2;
        public int startIntersection;
        public int endIntersection;
    }
    
    void SpawnRoads()
    {
        // first generation pass: plan roads as basic voxel data only
        m_TrackVoxels = new NativeArray<bool>(voxelCount * voxelCount * voxelCount, Allocator.Persistent);

        // after voxel generation, we'll convert our network into non-voxels
        m_IntersectionsGrid = new int[voxelCount, voxelCount, voxelCount];
        unsafe
        {
            fixed (int* g = m_IntersectionsGrid)
            {
                UnsafeUtility.MemSet(g, 0xFF, sizeof(int) * m_IntersectionsGrid.Length);
            }
        }

        m_IntersectionPairs = new HashSet<long>();
        m_RoadMeshes = new List<Mesh>();
        m_IntersectionMatrices = new List<List<Matrix4x4>>();

        m_Cars = new List<Car>();
        m_CarMatrices = new List<List<Matrix4x4>>();
        m_CarMatrices.Add(new List<Matrix4x4>());
        m_CarColors = new List<List<Vector4>>();
        m_CarColors.Add(new List<Vector4>());
        m_CarMatProps = new MaterialPropertyBlock();
        m_CarMatProps.SetVectorArray(k_Color, new Vector4[k_InstancesPerBatch]);

        // plan roads broadly: first, as a grid of true/false voxels
        using (new ProfilerMarker("VoxelGeneration").Auto())
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

                Intersections.Init(outputIntersections.Length);
                
                for (int i = 0; i < outputIntersections.Length; i++)
                {
                    var pos = outputIntersections[i];
                    Intersections.Index[i] = pos;
                    Intersections.Position[i] = (float3)pos * voxelSize;
                    Intersections.Normal[i] = new float3();
                    m_IntersectionsGrid[pos.x, pos.y, pos.z] = i;
                }
            }
        }

        Debug.Log(Intersections.Count + " intersections");

        // at this point, we've generated our full layout, but everything
        // is voxels, and we've identified which voxels are intersections.
        // next, we'll reinterpret our voxels as a network of intersections:
        // we'll find all "neighboring intersections" in our voxel map
        // (neighboring intersections are connected by a chain of voxels,
        // which we'll replace with splines)

        using (new ProfilerMarker("FindNeighbors").Auto())
        {
            var trackSplineList = new List<TrackSplineCtorData>();
            for (int i = 0; i < Intersections.Count; i++)
            {
                int intersection = i;
                int3 axesWithNeighbors = new int3();
                for (int j = 0; j < m_Dirs.Length; j++)
                {
                    if (GetVoxel(Intersections.Index[intersection] + m_Dirs[j]))
                    {
                        axesWithNeighbors += math.abs(m_Dirs[j]);

                        int neighbor = FindFirstIntersection(
                            Intersections.Index[intersection],
                            m_Dirs[j],
                            out var connectDir);
                        if (neighbor >= 0 && neighbor != intersection)
                        {
                            // make sure we haven't already added the reverse-version of this spline
                            long hash = HashIntersectionPair(intersection, neighbor);
                            if (m_IntersectionPairs.Add(hash))
                            {
                                int splineIdx = trackSplineList.Count;
                                trackSplineList.Add(new TrackSplineCtorData
                                {
                                    startIntersection = intersection,
                                    endIntersection = neighbor,
                                    tangent1 = m_Dirs[j],
                                    tangent2 = connectDir
                                });

                                Intersections.Neighbors[intersection].Add(neighbor);
                                Intersections.Neighbors[neighbor].Add(intersection);
                                Intersections.NeighborSplines[intersection].Add(splineIdx);
                                Intersections.NeighborSplines[neighbor].Add(splineIdx);
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
                        if (Intersections.Normal[intersection].Equals(new float3()))
                        {
                            Intersections.Normal[intersection][j] = -1 + Random.Range(0, 2) * 2;

                            //Debug.DrawRay(intersection.position,(Vector3)intersection.normal * .5f,Color.red,1000f);
                        }
                        else
                        {
                            Debug.LogError("a straight line has been marked as an intersection!");
                        }
                    }
                }

                if (Intersections.Normal[intersection].Equals(new float3()))
                {
                    Debug.LogError("non-planar intersections are not allowed!");
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

            {
                int n = TrackSplines.Count = trackSplineList.Count;
                TrackSplines.bezier = new CubicBezier[n];
                TrackSplines.geometry = new TrackGeometry[n];
                TrackSplines.endIntersection = new int[n];
                TrackSplines.measuredLength = new float[n];
                TrackSplines.startIntersection = new int[n];
                TrackSplines.twistMode = new int[n];
                TrackSplines.waitingQueues = new List<Car>[n][];
                TrackSplines.carQueueSize = new float[n];
                TrackSplines.maxCarCount = new int[n];

                for (int i = 0; i < trackSplineList.Count; i++)
                {
                    int start = TrackSplines.startIntersection[i] = trackSplineList[i].startIntersection;
                    int end = TrackSplines.endIntersection[i] = trackSplineList[i].endIntersection;
                    var startP = TrackSplines.bezier[i].start = Intersections.Position[start] + .5f *  intersectionSize * trackSplineList[i].tangent1;
                    var endP = TrackSplines.bezier[i].end = Intersections.Position[end] + .5f *  intersectionSize * trackSplineList[i].tangent2;

                    float dist = math.length(startP - endP);
                    TrackSplines.bezier[i].anchor1 = startP + .5f * dist * trackSplineList[i].tangent1;
                    TrackSplines.bezier[i].anchor2 = endP + .5f * dist * trackSplineList[i].tangent2;
                    TrackSplines.geometry[i].startTangent = math.round(trackSplineList[i].tangent1);
                    TrackSplines.geometry[i].endTangent = math.round(trackSplineList[i].tangent2);

                    var measuredLength = TrackSplines.measuredLength[i] = TrackSplines.bezier[i].MeasureLength(splineResolution);
                    var maxCarCount = TrackSplines.maxCarCount[i] = (int)math.ceil(measuredLength / k_CarSpacing);
                    TrackSplines.carQueueSize[i] = 1f / maxCarCount;
                    
                    var queues = TrackSplines.waitingQueues[i] = new List<Car>[4];
                    for (int j = 0; j < 4; j++)
                        queues[j] = new List<Car>();
                }
            }
        }

        Debug.Log(TrackSplines.Count + " road splines");

        // generate road meshes

        using(new ProfilerMarker("Generate Meshes").Auto()) {
            int numSplines = TrackSplines.Count;
            TrackUtils.SizeOfMeshData(splineResolution, out int verticesPerSpline, out int indicesPerSpline);
            var vertices = new NativeArray<float3>(verticesPerSpline * numSplines, Allocator.TempJob);
            var uvs = new NativeArray<float2>(verticesPerSpline * numSplines, Allocator.TempJob);
            var triangles = new NativeArray<int>(indicesPerSpline * numSplines, Allocator.TempJob);
            var twistMode = new NativeArray<int>(numSplines, Allocator.TempJob);
            using (vertices)
            using (uvs)
            using (triangles)
            using(twistMode)
            {
                var bezier = new NativeArray<CubicBezier>(numSplines, Allocator.TempJob);
                var geometry = new NativeArray<TrackGeometry>(numSplines, Allocator.TempJob);
                for (int i = 0; i < numSplines; i++)
                {
                    TrackSplines.geometry[i].startNormal = Intersections.Normal[TrackSplines.startIntersection[i]];
                    TrackSplines.geometry[i].endNormal = Intersections.Normal[TrackSplines.endIntersection[i]];
                    geometry[i] = TrackSplines.geometry[i];
                    bezier[i] = TrackSplines.bezier[i];
                }

                int splinesPerMesh = (3 * trisPerMesh) / indicesPerSpline;
                int numMeshes = numSplines / splinesPerMesh;
                int remaining = numSplines % splinesPerMesh;
                numMeshes += remaining != 0 ? 1 : 0;

                var job = new GenerateTrackMeshes()
                {
                    VerticesPerSpline = verticesPerSpline,
                    IndicesPerSpline = indicesPerSpline,
                    Bezier = bezier,
                    Geometry = geometry,
                    OutVertices = vertices,
                    OutUVs = uvs,
                    OutTriangles = triangles,
                    OutTwistMode = twistMode,
                    SplinesPerMesh = splinesPerMesh,
                };
                job.Setup(splineResolution, trackRadius, trackThickness);
                job.Schedule(numSplines, 16).Complete();

                for (int i = 0; i < numSplines; i++)
                    TrackSplines.twistMode[i] = twistMode[i];

                using (new ProfilerMarker("create mesh").Auto())
                {
                    for (int i = 0; i < numMeshes; i++)
                    {
                        int splines = i < numMeshes - 1 ? splinesPerMesh : remaining;
                        Mesh mesh = new Mesh();
                        mesh.name = "Generated Road Mesh";
                        mesh.SetVertices(vertices, i * splinesPerMesh * verticesPerSpline, splines * verticesPerSpline);
                        mesh.SetUVs(0, uvs, i * splinesPerMesh * verticesPerSpline, splines * verticesPerSpline);
                        mesh.SetIndices(triangles, i * splinesPerMesh * indicesPerSpline, splines * indicesPerSpline, MeshTopology.Triangles, 0);
                        mesh.RecalculateNormals();
                        mesh.RecalculateBounds();
                        m_RoadMeshes.Add(mesh);
                    }
                }

                Debug.Log(triangles.Length + " road triangles");
                Debug.Log(m_RoadMeshes.Count + " road meshes");
            }
        }

        // generate intersection matrices for batch-rendering
        {
            int batch = 0;
            m_IntersectionMatrices.Add(new List<Matrix4x4>());
            for (int i = 0; i < Intersections.Count; i++)
            {
                m_IntersectionMatrices[batch].Add(Intersections.GetMatrix(i));
                if (m_IntersectionMatrices[batch].Count == k_InstancesPerBatch)
                {
                    batch++;
                    m_IntersectionMatrices.Add(new List<Matrix4x4>());
                }
            }
        }
        

        // spawn cars

        {
            int batch = 0;
            for (int i = 0; i < 4000; i++)
            {
                int splineSide = -1 + Random.Range(0, 2) * 2;
                int splineDirection = -1 + Random.Range(0, 2) * 2;
                var roadSpline = Random.Range(0, TrackSplines.Count);
                Car car = new Car(splineSide, splineDirection, carSpeed, roadSpline);
                TrackSplines.GetQueue(roadSpline, splineDirection, splineSide).Add(car);

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
    }

    void Update()
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
                m_CarMatProps.SetVectorArray(k_Color, m_CarColors[i]);
                Graphics.DrawMeshInstanced(carMesh, 0, carMaterial, m_CarMatrices[i], m_CarMatProps);
            }
        }
    }

    void OnDrawGizmos()
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
                            Gizmos.DrawWireCube(new Vector3(x, y, z) * voxelSize,
                                new Vector3(.9f, .9f, .9f) * voxelSize);
                        }
                    }
                }
            }
        }

        if (m_RoadMeshes != null && m_RoadMeshes.Count == 0)
        {
            // visualize splines before road meshes have spawned
            if (Intersections.Count > 0)
            {
                Gizmos.color = new Color(.2f, .2f, 1f);
                for (int i = 0; i < Intersections.Count; i++)
                {
                    if (!Intersections.Normal[i].Equals(new float3()))
                    {
                        Gizmos.DrawWireMesh(intersectionPreviewMesh, 0, Intersections.Position[i],
                            Quaternion.LookRotation(Intersections.Normal[i]),
                            new Vector3(intersectionSize, intersectionSize, 0f));
                    }
                }
            }

            for (int i = 0; i < TrackSplines.Count; i++)
            {
                TrackSplines.DrawGizmos(i);
            }
        }
    }
}
