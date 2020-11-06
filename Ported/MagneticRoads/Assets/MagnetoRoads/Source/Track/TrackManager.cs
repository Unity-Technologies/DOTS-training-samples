using System.Collections.Generic;
using Magneto.Collections;
using Magneto.JobBank;
using Magneto.Track.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Magneto.Track
{
    public static class TrackManager
    {
        public const int RANDOM_SEED = 42;
        public const int DIRECTIONS_LENGTH = 26;
        public const int LIMITED_DIRECTIONS_LENGTH = 6;

        public const int JOB_EXECUTION_MAXIMUM_FRAMES = 3;
        public const int VOXEL_COUNT = 100;
        public const float VOXEL_SIZE = 1f;
        public const int TRI_PER_MESH = 4000;


        public const float intersectionSize = .5f;
        public const float trackRadius = .2f;
        public const float trackThickness = .05f;
        public const int splineResolution = 20;
        public const float carSpacing = .13f;

        private const int instancesPerBatch = 1023;


        public static TrackGenerationSystem System;


        // Grid split based X y sizingf to make voxels

        // build map of grid cells that will used

        // split into sub cells and map

        //

        public class TrackGenerationSystem
        {
            private NativeArray<int3> _cachedNeighbourIndexOffsets;
            private NativeArray<int3> _cachedNeighbourIndexOffsetsLimited;
            public NativeStrideGridArray<int> _intersectionIndicesByGrid;

            public NativeList<IntersectionData> _intersections;
            //private NativeArray<int> _splineCount;


            public NativeList<SplineData> _splines;
            private NativeStrideGridArray<bool> _trackVoxels;

            public JobHandle Stage1JobHandle;
            public JobHandle Stage2JobHandle;
            public bool IsRunning { get; private set; }

            public void Init()
            {
                // Initialize Native Arrays
                _cachedNeighbourIndexOffsets = new NativeArray<int3>(DIRECTIONS_LENGTH, Allocator.Persistent);
                _cachedNeighbourIndexOffsetsLimited =
                    new NativeArray<int3>(LIMITED_DIRECTIONS_LENGTH, Allocator.Persistent);
                _intersections = new NativeList<IntersectionData>(Allocator.Persistent);
                _intersectionIndicesByGrid = new NativeStrideGridArray<int>(VOXEL_COUNT, Allocator.Persistent,
                    NativeArrayOptions.ClearMemory);
                _trackVoxels =
                    new NativeStrideGridArray<bool>(VOXEL_COUNT, Allocator.Persistent, NativeArrayOptions.ClearMemory);

                _splines = new NativeList<SplineData>(32, Allocator.Persistent);
                //_splineCount = new NativeArray<int>(1, Allocator.Persistent);


                // Prefill One Manually
                _cachedNeighbourIndexOffsetsLimited[0] = new int3(1, 0, 0);
                _cachedNeighbourIndexOffsetsLimited[1] = new int3(-1, 0, 0);
                _cachedNeighbourIndexOffsetsLimited[2] = new int3(0, 1, 0);
                _cachedNeighbourIndexOffsetsLimited[3] = new int3(0, -1, 0);
                _cachedNeighbourIndexOffsetsLimited[4] = new int3(0, 0, 1);
                _cachedNeighbourIndexOffsetsLimited[5] = new int3(0, 0, -1);
            }

            public void Schedule()
            {
                IsRunning = true;

                #region Stage 1 - Setup

                // Create Cached Directional Offsets
                var cacheNeighbourIndexOffsetsJobHandle = new CachedNeighbourIndexOffsetJob
                {
                    W_Buffer = _cachedNeighbourIndexOffsets
                }.Schedule();

                // We need to exploit having indices as -1 for not found values
                var fillIntersectionsIndicesGridJobHandle = new IntegerBufferFillJob
                {
                    Buffer = _intersectionIndicesByGrid.array,
                    FillValue = -1
                }.Schedule(VOXEL_COUNT * VOXEL_COUNT * VOXEL_COUNT, 128);

                #endregion

                // Create a combined dependency for the next stage to rely on
                Stage1JobHandle = JobHandle.CombineDependencies(cacheNeighbourIndexOffsetsJobHandle,
                    fillIntersectionsIndicesGridJobHandle);

                #region Stage 2 - Full Layout

                // Build Voxel Map
                var buildVoxelMapJobHandle = new BuildVoxelMapJob
                {
                    R_CachedNeighbourIndexOffsets = _cachedNeighbourIndexOffsets,
                    R_LimitedCachedNeighbourIndexOffsets = _cachedNeighbourIndexOffsetsLimited,
                    RW_TrackVoxels = _trackVoxels,

                    RW_Intersections = _intersections,
                    W_IntersectionsGrid = _intersectionIndicesByGrid
                }.Schedule(Stage1JobHandle);

                // Build the connective tissues
                Stage2JobHandle = new BuildVoxelNetworkJob
                {
                    RW_Intersections = _intersections,
                    R_IntersectionsGrid = _intersectionIndicesByGrid,
                    R_LimitedCachedNeighbourIndexOffsets = _cachedNeighbourIndexOffsetsLimited,
                    R_TrackVoxels = _trackVoxels,

                    W_OutSplines = _splines
                }.Schedule(buildVoxelMapJobHandle);

                #endregion

                // GO!
                JobHandle.ScheduleBatchedJobs();
            }

            public void Complete(out IntersectionData[] intersectionData, out SplineData[] splineData,
                out bool[] activeVoxels)
            {
                Stage2JobHandle.Complete();
                IsRunning = false;


                // Write output
                intersectionData = _intersections.ToArray();
                splineData = _splines.ToArray();
                activeVoxels = _trackVoxels.array.ToArray();

                
                // Were going to push the data to the traffic system
                
                TrafficSpawnerSystem trafficSpawnerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TrafficSpawnerSystem>();
                trafficSpawnerSystem.m_SpawnFromGenerator = true;
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityArchetype laneArchetype = entityManager.CreateArchetype(typeof(Lane), typeof(Spline), typeof(CarBufferElement));

                
                Intersection[] intersections = new Intersection[_intersections.Length];
                for (int i = 0; i < _intersections.Length; i++)
                {
                    var iData = _intersections[i];
                    intersections[i] = new Intersection(
                        new Vector3Int(iData.Index.x, iData.Index.y, iData.Index.z),
                        iData.Position.GetVector3(),
                        new Vector3Int(iData.Normal.x, iData.Normal.y, iData.Normal.z));
                    
                    trafficSpawnerSystem.m_Intersections.Add( intersections[i]);
                }
                
                foreach (var spline in _splines)
                {
                    var startIntersection = intersections[_intersectionIndicesByGrid[spline.StartIntersectionPosition]];
                    var endIntersection = intersections[_intersectionIndicesByGrid[spline.EndIntersectionPosition]];
                    
                    if(startIntersection.neighbors.Contains(endIntersection)) continue;
                    
                    
                    Entity laneOut = entityManager.CreateEntity(laneArchetype);
                    Entity laneIn = entityManager.CreateEntity(laneArchetype);
                    
                    entityManager.SetComponentData(laneOut, 
                        new Spline {startPos = spline.StartIntersectionPosition, endPos = spline.EndIntersectionPosition});
                    
                    entityManager.SetComponentData(laneIn, 
                        new Spline {startPos = spline.EndIntersectionPosition, endPos = spline.StartIntersectionPosition});

                    float length = (spline.EndIntersectionPosition.GetVector3() - spline.StartIntersectionPosition.GetVector3()).magnitude;
                    Lane lane = new Lane {Length = length};
							
                    entityManager.SetComponentData(laneOut, lane);
                    entityManager.SetComponentData(laneIn, lane);
                    
                    startIntersection.lanes.Add(laneIn);
                    startIntersection.lanes.Add(laneOut);
                    endIntersection.lanes.Add(laneOut);
                    endIntersection.lanes.Add(laneIn);

                    startIntersection.neighbors.Add(endIntersection);
                    endIntersection.neighbors.Add(startIntersection);
                    
                    var trackSpline = new TrackSplineObject(
                        spline.StartPosition.GetVector3(),
                        startIntersection.normal,
                        spline.StartTangent.GetVector3(),
                        spline.EndPosition.GetVector3(),
                        endIntersection.normal,
                        spline.EndTangent.GetVector3());

                    List<Vector3> vertices = new List<Vector3>();
                    List<Vector2> uvs = new List<Vector2>();
                    List<int> triangles = new List<int>();

                    trackSpline.GenerateMesh(vertices, uvs, triangles);

                    var mesh = new Mesh();
                    mesh.SetVertices(vertices);
                    mesh.SetUVs(0,uvs);
                    mesh.SetTriangles(triangles,0);
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    trafficSpawnerSystem.SplineMeshes.Add(mesh);
                    trafficSpawnerSystem.SplineTranslations.Add(spline.StartPosition);
                }
                
                
                _cachedNeighbourIndexOffsets.Dispose();
                _cachedNeighbourIndexOffsetsLimited.Dispose();
                _intersectionIndicesByGrid.Dispose();
                _intersections.Dispose();
                _trackVoxels.Dispose();


                trafficSpawnerSystem.isReady = true;
                _splines.Dispose();
                
            }
        }
    }
}