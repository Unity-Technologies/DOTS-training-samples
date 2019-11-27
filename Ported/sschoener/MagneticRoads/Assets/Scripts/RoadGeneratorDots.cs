using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
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
        if (m_Dirs.IsCreated)
            m_Dirs.Dispose();
        if (m_FullDirs.IsCreated)
            m_FullDirs.Dispose();
        if (Intersections.Occupied.IsCreated)
            Intersections.Occupied.Dispose();
    }

    void SpawnRoads()
    {
        // first generation pass: plan roads as basic voxel data only
        var trackVoxels = new NativeArray<bool>(voxelCount * voxelCount * voxelCount, Allocator.TempJob);

        // after voxel generation, we'll convert our network into non-voxels
        var intersectionsGrid = new NativeArray<int>(voxelCount * voxelCount * voxelCount, Allocator.TempJob);
        unsafe
        {
            UnsafeUtility.MemSet(intersectionsGrid.GetUnsafePtr(), 0xFF, sizeof(int) * intersectionsGrid.Length);
        }

        var intersections = new NativeList<Intersection>(Allocator.TempJob);
        var intersectionIndices = new NativeList<int3>(Allocator.TempJob);

        using (trackVoxels)
        using (intersectionsGrid)
        using (intersections)
        using (intersectionIndices)
        {
            m_RoadMeshes = new List<Mesh>();
            m_IntersectionMatrices = new List<List<Matrix4x4>>();

            // plan roads broadly: first, as a grid of true/false voxels
            using (new ProfilerMarker("VoxelGeneration").Auto())
            {
                new GenerateVoxelsJob()
                {
                    Rng = new Unity.Mathematics.Random(12345),
                    VoxelCount = voxelCount,
                    VoxelSize = voxelSize,
                    OutputIntersectionIndices = intersectionIndices,
                    OutputIntersections = intersections,
                    OutputIntersectionGrid = intersectionsGrid,
                    Voxels = trackVoxels,
                    Directions = m_Dirs,
                    FullDirections = m_FullDirs
                }.Run();

                Intersections.Occupied = new NativeArray<OccupiedSides>(intersectionIndices.Length, Allocator.Persistent);
            }

            Debug.Log(intersections.Length + " intersections");

            // at this point, we've generated our full layout, but everything
            // is voxels, and we've identified which voxels are intersections.
            // next, we'll reinterpret our voxels as a network of intersections:
            // we'll find all "neighboring intersections" in our voxel map
            // (neighboring intersections are connected by a chain of voxels,
            // which we'll replace with splines)

            var trackSplinePrototypes = new NativeList<TrackSplineCtorData>(Allocator.TempJob);
            var trackSplines = new NativeList<TrackSpline>(Allocator.TempJob);
            var findSplines = new FindSplinesJob
            {
                Intersections = intersections,
                Dirs = m_Dirs,
                IntersectionIndices = intersectionIndices,
                IntersectionsGrid = intersectionsGrid,
                Rng = new Unity.Mathematics.Random(12345),
                OutTrackSplinePrototypes = trackSplinePrototypes,
                TrackVoxels = trackVoxels,
                VoxelCount = voxelCount
            }.Schedule();
            var growList = new GrowListJob
            {
                TrackSplinePrototypes = trackSplinePrototypes,
                TrackSplines = trackSplines
            }.Schedule(findSplines);
            new BuildSplinesJob
            {
                Intersections = intersections,
                CarSpacing = k_CarSpacing,
                IntersectionSize = intersectionSize,
                OutTrackSplines = trackSplines,
                SplineResolution = splineResolution,
                TrackSplinePrototypes = trackSplinePrototypes
            }.Schedule(trackSplinePrototypes, 16, growList).Complete();

            {
                using (var blobBuilder = new BlobBuilder(Allocator.Temp))
                {
                    ref var intersectionsBlob = ref blobBuilder.ConstructRoot<IntersectionsBlob>();
                    var arr = blobBuilder.Allocate(ref intersectionsBlob.Intersections, intersections.Length);
                    unsafe
                    {
                        UnsafeUtility.MemCpy(arr.GetUnsafePtr(), intersections.GetUnsafePtr(), sizeof(Intersection) * intersections.Length);
                    }
                    IntersectionsBlob.Instance = blobBuilder.CreateBlobAssetReference<IntersectionsBlob>(Allocator.Persistent);
                }
            }
            
            int numSplines = trackSplinePrototypes.Length;
            var trackSplinesBezier = new NativeArray<CubicBezier>(numSplines, Allocator.TempJob);
            var trackSplinesGeometry = new NativeArray<TrackGeometry>(numSplines, Allocator.TempJob);
            using (new ProfilerMarker("SetupSplines").Auto())
            {
                TrackSplines.waitingQueues = new List<QueueEntry>[numSplines][];
                for (int i = 0; i < trackSplinePrototypes.Length; i++)
                {
                    trackSplinesBezier[i] = trackSplines[i].Bezier;
                    trackSplinesGeometry[i] = trackSplines[i].Geometry;
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

                    var blobBuilder = new BlobBuilder(Allocator.Temp);
                    JobHandle buildBlobJob;
                    {
                        ref var splineArray = ref blobBuilder.ConstructRoot<TrackSplinesBlob>();
                        var arrayBuilder = blobBuilder.Allocate(ref splineArray.Splines, numSplines);

                        unsafe
                        {
                            buildBlobJob = new BuildTrackSplineBlobJob()
                            {
                                BlobArray = (TrackSpline*)arrayBuilder.GetUnsafePtr(),
                                TrackSplines = trackSplines,
                                TwistMode = twistMode,
                            }.Schedule(numSplines, 16);
                        }
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
                    blobBuilder.Dispose();

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
                var e = em.CreateEntity();
                em.AddComponentData(e, new CarSpawnComponent
                {
                    Count = numCars
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
}
