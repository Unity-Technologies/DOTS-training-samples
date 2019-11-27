using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

public class RoadGeneratorDots : MonoBehaviour
{
    public int voxelCount = 60;
    public float voxelSize = 1f;
    public int trisPerMesh = 4000;
    public Material roadMaterial;
    public Mesh intersectionMesh;

    public const float intersectionSize = .5f;
    public const float trackRadius = .2f;
    public const float trackThickness = .05f;
    public const int splineResolution = 20;
    const float k_CarSpacing = .13f;
    const int k_InstancesPerBatch = 1023;

    Mesh[] m_RoadMeshes;
    List<List<Matrix4x4>> m_IntersectionMatrices;

    void OnDestroy()
    {
        if (Intersections.Occupied.IsCreated)
            Intersections.Occupied.Dispose();
    }

    public void SpawnRoads()
    {
        // first generation pass: plan roads as basic voxel data only
        // after voxel generation, we'll convert our network into non-voxels
        var intersections = new NativeList<Intersection>(Allocator.TempJob);
        var trackSplines = new NativeList<TrackSpline>(Allocator.TempJob);
        using (intersections)
        using (trackSplines)
        {
            #region build splines

            {
                var intersectionsGrid = new NativeArray<int>(voxelCount * voxelCount * voxelCount, Allocator.TempJob);
                unsafe
                {
                    UnsafeUtility.MemSet(intersectionsGrid.GetUnsafePtr(), 0xFF, sizeof(int) * intersectionsGrid.Length);
                }

                var trackVoxels = new NativeArray<bool>(voxelCount * voxelCount * voxelCount, Allocator.TempJob);
                var intersectionIndices = new NativeList<int3>(Allocator.TempJob);

                // plan roads broadly: first, as a grid of true/false voxels
                new GenerateVoxelsJob()
                {
                    Rng = new Unity.Mathematics.Random(12345),
                    VoxelCount = voxelCount,
                    VoxelSize = voxelSize,
                    OutputIntersectionIndices = intersectionIndices,
                    OutputIntersections = intersections,
                    OutputIntersectionGrid = intersectionsGrid,
                    Voxels = trackVoxels,
                }.Run();

                Debug.Log(intersections.Length + " intersections");

                // at this point, we've generated our full layout, but everything
                // is voxels, and we've identified which voxels are intersections.
                // next, we'll reinterpret our voxels as a network of intersections:
                // we'll find all "neighboring intersections" in our voxel map
                // (neighboring intersections are connected by a chain of voxels,
                // which we'll replace with splines)

                var trackSplinePrototypes = new NativeList<TrackSplineCtorData>(Allocator.TempJob);
                var findSplines = new FindSplinesJob
                {
                    Intersections = intersections,
                    IntersectionIndices = intersectionIndices,
                    IntersectionsGrid = intersectionsGrid,
                    Rng = new Unity.Mathematics.Random(12345),
                    OutTrackSplinePrototypes = trackSplinePrototypes,
                    TrackVoxels = trackVoxels,
                    VoxelCount = voxelCount
                }.Schedule();
                trackVoxels.Dispose(findSplines);
                intersectionIndices.Dispose(findSplines);
                intersectionsGrid.Dispose(findSplines);
                var growList = new GrowListJob
                {
                    TrackSplinePrototypes = trackSplinePrototypes,
                    TrackSplines = trackSplines
                }.Schedule(findSplines);
                var buildSplines = new BuildSplinesJob
                {
                    Intersections = intersections,
                    CarSpacing = k_CarSpacing,
                    IntersectionSize = intersectionSize,
                    OutTrackSplines = trackSplines,
                    SplineResolution = splineResolution,
                    TrackSplinePrototypes = trackSplinePrototypes
                }.Schedule(trackSplinePrototypes, 16, growList);
                trackSplinePrototypes.Dispose(buildSplines);

                new DetermineTwistModeJob
                {
                    Resolution = splineResolution,
                    Splines = trackSplines
                }.Schedule(trackSplines, 16, buildSplines).Complete();
            }

            #endregion

            #region build blobs

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var intersectionsBlob = ref blobBuilder.ConstructRoot<IntersectionsBlob>();
                var arr = blobBuilder.Allocate(ref intersectionsBlob.Intersections, intersections.Length);
                unsafe
                {
                    UnsafeUtility.MemCpy(
                        arr.GetUnsafePtr(),
                        intersections.GetUnsafePtr(),
                        sizeof(Intersection) * intersections.Length
                    );
                }

                IntersectionsBlob.Instance = blobBuilder.CreateBlobAssetReference<IntersectionsBlob>(Allocator.Persistent);
            }

            int numSplines = trackSplines.Length;
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var splineArray = ref blobBuilder.ConstructRoot<TrackSplinesBlob>();
                var arrayBuilder = blobBuilder.Allocate(ref splineArray.Splines, numSplines);
                unsafe
                {
                    UnsafeUtility.MemCpy(
                        arrayBuilder.GetUnsafePtr(),
                        trackSplines.GetUnsafePtr(),
                        sizeof(TrackSpline) * trackSplines.Length
                    );
                }

                TrackSplinesBlob.Instance = blobBuilder.CreateBlobAssetReference<TrackSplinesBlob>(Allocator.Persistent);
            }

            #endregion

            Debug.Log(numSplines + " road splines");

            #region generate meshes

            {
                TrackUtils.SizeOfMeshData(splineResolution, out int verticesPerSpline, out int indicesPerSpline);
                var vertices = new NativeArray<float3>(verticesPerSpline * numSplines, Allocator.TempJob);
                var triangles = new NativeArray<int>(indicesPerSpline * numSplines, Allocator.TempJob);
                using (vertices)
                using (triangles)
                {
                    int splinesPerMesh = 3 * trisPerMesh / indicesPerSpline;
                    var job = new GenerateTrackMeshes
                    {
                        VerticesPerSpline = verticesPerSpline,
                        IndicesPerSpline = indicesPerSpline,
                        TrackSplines = trackSplines,
                        OutVertices = vertices,
                        OutTriangles = triangles,
                        SplinesPerMesh = splinesPerMesh,
                    };
                    job.Setup(splineResolution, trackRadius, trackThickness);
                    job.Schedule(numSplines, 16).Complete();

                    using (new ProfilerMarker("CreateMeshes").Auto())
                    {
                        int numMeshes = numSplines / splinesPerMesh;
                        int remaining = numSplines % splinesPerMesh;
                        numMeshes += remaining != 0 ? 1 : 0;
                        m_RoadMeshes = new Mesh[numMeshes];
                        for (int i = 0; i < numMeshes; i++)
                        {
                            int splines = i < numMeshes - 1 ? splinesPerMesh : remaining;
                            Mesh mesh = new Mesh();
                            mesh.name = "Generated Road Mesh";
                            mesh.SetVertices(vertices, i * splinesPerMesh * verticesPerSpline, splines * verticesPerSpline);
                            mesh.SetIndices(triangles, i * splinesPerMesh * indicesPerSpline, splines * indicesPerSpline, MeshTopology.Triangles, 0);
                            mesh.RecalculateNormals();
                            mesh.RecalculateBounds();
                            m_RoadMeshes[i] = mesh;
                        }
                    }

                    Debug.Log($"{triangles.Length} road triangles ({m_RoadMeshes.Length} meshes)");
                }
            }

            #endregion

            TrackSplines.waitingQueues = new List<QueueEntry>[numSplines][];
            int total = 0;
            for (int i = 0; i < trackSplines.Length; i++)
            {
                var queues = TrackSplines.waitingQueues[i] = new List<QueueEntry>[4];
                total += trackSplines[i].MaxCarCount;
                for (int j = 0; j < 4; j++)
                    queues[j] = new List<QueueEntry>();
            }

            Debug.Log(total);

            Intersections.Occupied = new NativeArray<OccupiedSides>(intersections.Length, Allocator.Persistent);

            // generate intersection matrices for batch-rendering
            {
                int batch = 0;
                m_IntersectionMatrices = new List<List<Matrix4x4>>();
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
        }
    }

    void Update()
    {
        using (new ProfilerMarker("DrawMeshes").Auto())
        {
            for (int i = 0; i < m_RoadMeshes.Length; i++)
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
