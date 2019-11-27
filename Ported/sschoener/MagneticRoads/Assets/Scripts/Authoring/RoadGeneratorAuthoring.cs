using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class RoadGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh IntersectionMesh;
    public Material RoadMaterial;
    public float TrackRadius = .2f;
    public float TrackThickness = .05f;
    public float IntersectionSize = .5f;
    public int TrisPerMesh = 4000;
    public int SplineResolution = 20;

    public float CarSpacing = .13f;
    public int VoxelCount = 60;
    public float VoxelSize = 1;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        SpawnRoads(out var intersectionsBlob, out var trackSplinesBlob);
        dstManager.AddComponentData(entity, new RoadSetupComponent
        {
            Splines = trackSplinesBlob,
            Intersections = intersectionsBlob,
            IntersectionSize = IntersectionSize,
            TrackRadius = TrackRadius,
            TrackThickness = TrackThickness,
            TrisPerMesh = TrisPerMesh,
            SplineResolution = SplineResolution,
        });
        dstManager.AddSharedComponentData(entity, new RenderMesh
        {
            mesh = IntersectionMesh,
            material = RoadMaterial,
        });
    } 
    
    void SpawnRoads(out BlobAssetReference<IntersectionsBlob> outIntersections, out BlobAssetReference<TrackSplinesBlob> outTrackSplines)
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
                var intersectionsGrid = new NativeArray<int>(VoxelCount * VoxelCount * VoxelCount, Allocator.TempJob);
                unsafe
                {
                    UnsafeUtility.MemSet(intersectionsGrid.GetUnsafePtr(), 0xFF, sizeof(int) * intersectionsGrid.Length);
                }

                var trackVoxels = new NativeArray<bool>(VoxelCount * VoxelCount * VoxelCount, Allocator.TempJob);
                var intersectionIndices = new NativeList<int3>(Allocator.TempJob);

                // plan roads broadly: first, as a grid of true/false voxels
                new GenerateVoxelsJob()
                {
                    Rng = new Unity.Mathematics.Random(12345),
                    VoxelCount = VoxelCount,
                    VoxelSize = VoxelSize,
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
                    VoxelCount = VoxelCount
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
                    CarSpacing = CarSpacing,
                    IntersectionSize = IntersectionSize,
                    OutTrackSplines = trackSplines,
                    SplineResolution = SplineResolution,
                    TrackSplinePrototypes = trackSplinePrototypes
                }.Schedule(trackSplinePrototypes, 16, growList);
                trackSplinePrototypes.Dispose(buildSplines);

                new DetermineTwistModeJob
                {
                    Resolution = SplineResolution,
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

                outIntersections = blobBuilder.CreateBlobAssetReference<IntersectionsBlob>(Allocator.Persistent);
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

                outTrackSplines = blobBuilder.CreateBlobAssetReference<TrackSplinesBlob>(Allocator.Persistent);
            }

            #endregion

            Debug.Log(numSplines + " road splines");

            TrackSplines.waitingQueues = new List<QueueEntry>[numSplines][];
            for (int i = 0; i < trackSplines.Length; i++)
            {
                var queues = TrackSplines.waitingQueues[i] = new List<QueueEntry>[4];
                for (int j = 0; j < 4; j++)
                    queues[j] = new List<QueueEntry>();
            }

            Intersections.Occupied = new NativeArray<OccupiedSides>(intersections.Length, Allocator.Persistent);
        }
    }
}