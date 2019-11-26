using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;
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

    NativeArray<bool> m_TrackVoxels;
    int[,,] m_IntersectionsGrid;
    Matrix4x4[][] m_CarMatrices;
    MaterialPropertyBlock[] m_CarMatProps;

    NativeArray<int3> m_Dirs;
    NativeArray<int3> m_FullDirs;

    public const float intersectionSize = .5f;
    public const float trackRadius = .2f;
    public const float trackThickness = .05f;
    public const int splineResolution = 20;
    const float k_CarSpacing = .13f;
    const int k_InstancesPerBatch = 1023;

    List<Mesh> m_RoadMeshes;
    List<List<Matrix4x4>> m_IntersectionMatrices;

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
        if (Intersections.Occupied.IsCreated)
            Intersections.Occupied.Dispose();
    }

    struct TrackSplineCtorData
    {
        public float3 tangent1;
        public float3 tangent2;
        public ushort startIntersection;
        public ushort endIntersection;
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

        var intersectionPairs = new HashSet<long>();
        m_RoadMeshes = new List<Mesh>();
        m_IntersectionMatrices = new List<List<Matrix4x4>>();

        Intersection[] intersections;
        int3[] intersectionIndices;

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

                intersections = new Intersection[outputIntersections.Length];
                intersectionIndices = new int3[outputIntersections.Length];

                Intersections.Occupied = new NativeArray<OccupiedSides>(outputIntersections.Length, Allocator.Persistent);

                for (int i = 0; i < outputIntersections.Length; i++)
                {
                    var pos = outputIntersections[i];
                    intersectionIndices[i] = pos;
                    intersections[i].Position = (float3)pos * voxelSize;
                    intersections[i].Normal = new float3();
                    m_IntersectionsGrid[pos.x, pos.y, pos.z] = i;
                }
            }
        }

        Debug.Log(intersections.Length + " intersections");

        // at this point, we've generated our full layout, but everything
        // is voxels, and we've identified which voxels are intersections.
        // next, we'll reinterpret our voxels as a network of intersections:
        // we'll find all "neighboring intersections" in our voxel map
        // (neighboring intersections are connected by a chain of voxels,
        // which we'll replace with splines)

        var trackSplineList = new List<TrackSplineCtorData>();
        using (new ProfilerMarker("FindNeighbors").Auto())
        {
            for (int i = 0; i < intersections.Length; i++)
            {
                int intersection = i;
                int3 axesWithNeighbors = new int3();
                for (int j = 0; j < m_Dirs.Length; j++)
                {
                    if (GetVoxel(intersectionIndices[intersection] + m_Dirs[j]))
                    {
                        axesWithNeighbors += math.abs(m_Dirs[j]);

                        int neighbor = FindFirstIntersection(
                            intersectionIndices[intersection],
                            m_Dirs[j],
                            out var connectDir);
                        if (neighbor >= 0 && neighbor != intersection)
                        {
                            // make sure we haven't already added the reverse-version of this spline
                            long hash = HashIntersectionPair(intersection, neighbor);
                            if (intersectionPairs.Add(hash))
                            {
                                int splineIdx = trackSplineList.Count;
                                trackSplineList.Add(new TrackSplineCtorData
                                {
                                    startIntersection = (ushort)intersection,
                                    endIntersection = (ushort)neighbor,
                                    tangent1 = m_Dirs[j],
                                    tangent2 = connectDir
                                });

                                intersections[intersection].Neighbors.Add((ushort)neighbor, (ushort)splineIdx);
                                intersections[neighbor].Neighbors.Add((ushort)intersection, (ushort)splineIdx);
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
                        if (intersections[intersection].Normal.Equals(new float3()))
                        {
                            intersections[intersection].Normal[j] = -1 + Random.Range(0, 2) * 2;

                            //Debug.DrawRay(intersection.position,(Vector3)intersection.normal * .5f,Color.red,1000f);
                        }
                        else
                        {
                            Debug.LogError("a straight line has been marked as an intersection!");
                        }
                    }
                }

                if (intersections[intersection].Normal.Equals(new float3()))
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
        }

        {
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var intersectionsBlob = ref blobBuilder.ConstructRoot<IntersectionsBlob>();
                blobBuilder.Construct(ref intersectionsBlob.Intersections, intersections);
                IntersectionsBlob.Instance = blobBuilder.CreateBlobAssetReference<IntersectionsBlob>(Allocator.Persistent);
            }
        }

        int numSplines = trackSplineList.Count;
        var trackSplinesBezier = new NativeArray<CubicBezier>(numSplines, Allocator.TempJob);
        var trackSplinesGeometry = new NativeArray<TrackGeometry>(numSplines, Allocator.TempJob);
        var trackSplinesEndIntersection = new NativeArray<ushort>(numSplines, Allocator.TempJob);
        var trackSplinesMeasuredLength = new NativeArray<float>(numSplines, Allocator.TempJob);
        var trackSplinesStartIntersection = new NativeArray<ushort>(numSplines, Allocator.TempJob);
        var trackSplinesCarQueueSize = new NativeArray<float>(numSplines, Allocator.TempJob);
        var trackSplinesMaxCarCount = new NativeArray<int>(numSplines, Allocator.TempJob);
        using (new ProfilerMarker("SetupSplines").Auto())
        {
            TrackSplines.waitingQueues = new List<QueueEntry>[numSplines][];
            for (int i = 0; i < trackSplineList.Count; i++)
            {
                ushort start = trackSplinesStartIntersection[i] = trackSplineList[i].startIntersection;
                ushort end = trackSplinesEndIntersection[i] = trackSplineList[i].endIntersection;
                var b = trackSplinesBezier[i];
                var startP = b.start = intersections[start].Position + .5f * intersectionSize * trackSplineList[i].tangent1;
                var endP = b.end = intersections[end].Position + .5f * intersectionSize * trackSplineList[i].tangent2;

                float dist = math.length(startP - endP);
                b.anchor1 = startP + .5f * dist * trackSplineList[i].tangent1;
                b.anchor2 = endP + .5f * dist * trackSplineList[i].tangent2;
                trackSplinesBezier[i] = b;
                var g = trackSplinesGeometry[i];
                g.startTangent = math.round(trackSplineList[i].tangent1);
                g.endTangent = math.round(trackSplineList[i].tangent2);
                trackSplinesGeometry[i] = g;

                var measuredLength = trackSplinesMeasuredLength[i] = trackSplinesBezier[i].MeasureLength(splineResolution);
                var maxCarCount = trackSplinesMaxCarCount[i] = (int)math.ceil(measuredLength / k_CarSpacing);
                trackSplinesCarQueueSize[i] = 1f / maxCarCount;

                var queues = TrackSplines.waitingQueues[i] = new List<QueueEntry>[4];
                for (int j = 0; j < 4; j++)
                    queues[j] = new List<QueueEntry>();
            }
        }

        Debug.Log(numSplines + " road splines");

        // generate road meshes

        using (new ProfilerMarker("Generate Meshes").Auto())
        {
            TrackUtils.SizeOfMeshData(splineResolution, out int verticesPerSpline, out int indicesPerSpline);
            var vertices = new NativeArray<float3>(verticesPerSpline * numSplines, Allocator.TempJob);
            var uvs = new NativeArray<float2>(verticesPerSpline * numSplines, Allocator.TempJob);
            var triangles = new NativeArray<int>(indicesPerSpline * numSplines, Allocator.TempJob);
            using (vertices)
            using (uvs)
            using (triangles)
            {
                for (int i = 0; i < numSplines; i++)
                {
                    var g = trackSplinesGeometry[i];
                    g.startNormal = intersections[trackSplinesStartIntersection[i]].Normal;
                    g.endNormal = intersections[trackSplinesEndIntersection[i]].Normal;
                    trackSplinesGeometry[i] = g;
                }

                int splinesPerMesh = (3 * trisPerMesh) / indicesPerSpline;
                int numMeshes = numSplines / splinesPerMesh;
                int remaining = numSplines % splinesPerMesh;
                numMeshes += remaining != 0 ? 1 : 0;

                var twistMode = new NativeArray<byte>(numSplines, Allocator.TempJob);
                var job = new GenerateTrackMeshes
                {
                    VerticesPerSpline = verticesPerSpline,
                    IndicesPerSpline = indicesPerSpline,
                    Bezier = trackSplinesBezier,
                    Geometry = trackSplinesGeometry,
                    OutVertices = vertices,
                    OutUVs = uvs,
                    OutTriangles = triangles,
                    OutTwistMode = twistMode,
                    SplinesPerMesh = splinesPerMesh,
                };
                job.Setup(splineResolution, trackRadius, trackThickness);
                job.Schedule(numSplines, 16).Complete();

                using (var blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref var splineArray = ref blobBuilder.ConstructRoot<TrackSplinesBlob>();
                    var arrayBuilder = blobBuilder.Allocate(ref splineArray.Splines, numSplines);

                    JobHandle buildBlobJob;
                    unsafe
                    {
                        buildBlobJob = new BuildTrackSplineBlobJob()
                        {
                            BlobArray = (TrackSpline*)arrayBuilder.GetUnsafePtr(),
                            Bezier = trackSplinesBezier,
                            Geometry = trackSplinesGeometry,
                            EndIntersection = trackSplinesEndIntersection,
                            StartIntersection = trackSplinesStartIntersection,
                            TwistMode = twistMode,
                            CarQueueSize = trackSplinesCarQueueSize,
                            MeasuredLength = trackSplinesMeasuredLength,
                            MaxCarCount = trackSplinesMaxCarCount
                        }.Schedule(numSplines, 16);
                    }

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

                    buildBlobJob.Complete();
                    TrackSplinesBlob.Instance = blobBuilder.CreateBlobAssetReference<TrackSplinesBlob>(Allocator.Persistent);
                }

                Debug.Log($"{triangles.Length} road triangles ({m_RoadMeshes.Count} meshes)");
            }
        }

        // generate intersection matrices for batch-rendering
        {
            int batch = 0;
            m_IntersectionMatrices.Add(new List<Matrix4x4>());
            var scale = new Vector3(intersectionSize, intersectionSize, trackThickness);
            for (int i = 0; i < intersections.Length; i++)
            {
                var matrix = Matrix4x4.TRS(intersections[i].Position, Quaternion.LookRotation(intersections[i].Normal), scale);
                m_IntersectionMatrices[batch].Add(matrix);
                if (m_IntersectionMatrices[batch].Count == k_InstancesPerBatch)
                {
                    batch++;
                    m_IntersectionMatrices.Add(new List<Matrix4x4>());
                }
            }
        }

        // spawn cars
        {
            var world = World.DefaultGameObjectInjectionWorld;
            var em = world.EntityManager;
            const int numCars = 4000;
            var carArchetype = em.CreateArchetype(
                typeof(CarSpeedComponent),
                typeof(SplineDataComponent),
                typeof(OnSplineComponent),
                typeof(LocalIntersectionComponent),
                typeof(CoordinateSystemComponent),
                typeof(LocalToWorld),
                typeof(CarColor)
            );

            var entities = new NativeArray<Entity>(numCars, Allocator.Temp);
            em.CreateEntity(carArchetype, entities);

            for (int i = 0; i < numCars; i++)
            {
                int splineSide = -1 + Random.Range(0, 2) * 2;
                int splineDirection = -1 + Random.Range(0, 2) * 2;
                var roadSpline = Random.Range(0, numSplines);

                var e = entities[i];
                em.SetComponentData(e, new CarSpeedComponent
                {
                    SplineTimer = 1
                });

                var splinePos = new SplinePosition
                {
                    Direction = (sbyte)splineDirection,
                    Spline = (ushort)roadSpline,
                    Side = (sbyte)splineSide,
                }; 
                em.SetComponentData(e, new OnSplineComponent
                {
                    Value = splinePos 
                });

                em.SetComponentData(e, new CarColor
                {
                    Value = (Vector4)Random.ColorHSV()
                });

                TrackSplines.GetQueue(splinePos).Add(new QueueEntry
                {
                    Entity = e,
                    SplineTimer = 1
                });

                em.AddSharedComponentData(e, new Unity.Rendering.RenderMesh
                {
                    mesh = carMesh,
                    material = carMaterial
                });
            }
        }
    }

    void Update()
    {
        using (new ProfilerMarker("DrawMeshes").Auto())
        {
            for (int i = 0; i < m_RoadMeshes.Count; i++)
            {
                Graphics.DrawMesh(m_RoadMeshes[i], Matrix4x4.identity, roadMaterial, 0);
            }

            for (int i = 0; i < m_IntersectionMatrices.Count; i++)
            {
                Graphics.DrawMeshInstanced(intersectionMesh, 0, roadMaterial, m_IntersectionMatrices[i]);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (m_TrackVoxels.IsCreated)
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
    }
}
