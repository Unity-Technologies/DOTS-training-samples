using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class PathAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> m_ParentsLoopedPaths;
    public List<GameObject> m_ParentsSinglePaths;
    public float m_LoopPathOffset = 1;
    private TrainPositioningSystem m_TrainPositioningSytem;

    public GameObject prefabPlatform;
    public GameObject prefabMover;
    public int moverCount = 1;

    void CreatPathPositionData(ref int currIndex, TrainPositioningSystem trainPositioningSystem, List<GameObject> pathParents, bool isLooped)
    {     
        for(int i = 0; i < pathParents.Count; i++)
        {
            int2 startEnd;
            RailMarker[] railMarkers = pathParents[i].GetComponentsInChildren<RailMarker>();            
            startEnd.x = currIndex;
            for (int railMarkerIndex = 0; railMarkerIndex < railMarkers.Length; railMarkerIndex++)
            {
                trainPositioningSystem.m_PathPositions[currIndex] = railMarkers[railMarkerIndex].transform.position;
                Debug.Assert(railMarkers[railMarkerIndex].pointIndex == railMarkerIndex);

                if (railMarkers[railMarkerIndex].railMarkerType == RailMarkerType.PLATFORM_END)
                {
                    trainPositioningSystem.m_PathStopBits.SetBit(currIndex);
                }
                currIndex++;
            }
            
            if(isLooped)
            {
                float3 prevTan;
                float3 currTan;
                int railMarkerIndex = startEnd.x + railMarkers.Length - 1;                
                prevTan = math.normalize(math.cross(math.normalize(trainPositioningSystem.m_PathPositions[railMarkerIndex] - trainPositioningSystem.m_PathPositions[railMarkerIndex - 1]), Vector3.up));
                for (; railMarkerIndex >= startEnd.x; railMarkerIndex--)
                {
                    currTan = railMarkerIndex == startEnd.x ? prevTan : math.normalize(math.cross(math.normalize(trainPositioningSystem.m_PathPositions[railMarkerIndex] - trainPositioningSystem.m_PathPositions[railMarkerIndex - 1]), Vector3.up));
                    float3 offset = math.normalize(currTan + prevTan);
                    trainPositioningSystem.m_PathPositions[currIndex] = trainPositioningSystem.m_PathPositions[railMarkerIndex] + offset * m_LoopPathOffset;
                    if(trainPositioningSystem.m_PathStopBits.IsBitSet(railMarkerIndex + 1))
                        trainPositioningSystem.m_PathStopBits.SetBit(currIndex);
                    prevTan = currTan;                   
                    currIndex++;
                }
            }
            startEnd.y = currIndex;
            trainPositioningSystem.m_StartEndPositionIndicies[i] = startEnd;
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        int totalPositionsCount = 0;
        for(int i = 0; i < m_ParentsLoopedPaths.Count; i++)
        {
            totalPositionsCount += m_ParentsLoopedPaths[i].GetComponentsInChildren<RailMarker>().Length;
        }
        totalPositionsCount *= 2;
        for(int i = 0; i < m_ParentsSinglePaths.Count; i++)
        {
            totalPositionsCount += m_ParentsSinglePaths[i].GetComponentsInChildren<RailMarker>().Length;
        }

        Debug.Assert(totalPositionsCount > 0);
        World world = dstManager.World;
        TrainPositioningSystem trainPositioningSystem = world.GetExistingSystem<TrainPositioningSystem>();
        PathMoverSystem pathMoverSystem = world.GetExistingSystem<PathMoverSystem>();

        Debug.Assert(trainPositioningSystem != null);
        trainPositioningSystem.m_PathPositions = new NativeArray<float3>(totalPositionsCount, Allocator.Persistent);
        trainPositioningSystem.m_PathCount = m_ParentsLoopedPaths.Count + m_ParentsSinglePaths.Count;
        trainPositioningSystem.m_SinglePathStartIndex = m_ParentsLoopedPaths.Count;
        trainPositioningSystem.m_StartEndPositionIndicies = new NativeArray<int2>(trainPositioningSystem.m_PathCount, Allocator.Persistent);
        trainPositioningSystem.m_PathStopBits.Allocate(totalPositionsCount, Allocator.Persistent);
        trainPositioningSystem.m_PathStopBits.ClearAllBits();

        int currIndex = 0;
        CreatPathPositionData(ref currIndex, trainPositioningSystem, m_ParentsLoopedPaths, true);
        CreatPathPositionData(ref currIndex, trainPositioningSystem, m_ParentsSinglePaths, false);

        pathMoverSystem.m_PathPositions = trainPositioningSystem.m_PathPositions;
        pathMoverSystem.m_PathIndices = trainPositioningSystem.m_StartEndPositionIndicies;

        m_TrainPositioningSytem = trainPositioningSystem;

        InstantiatePlatforms(prefabPlatform);
        InstantiatePathMoversOnLoopPaths(prefabMover, moverCount);
    }

    private void InstantiatePlatforms(GameObject prefab)
    {
        Debug.Assert(prefab != null);

        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Only spawn platforms at stops in loop paths, not single paths.
        // This seems a little hacky / non-obvious, in that we assume a loop path is specifically a train path that has platforms.
        for (int pathIndex = 0; pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex; ++pathIndex)
        {
            int2 startEndIndices = m_TrainPositioningSytem.m_StartEndPositionIndicies[pathIndex];

            for (int i = startEndIndices.x; i < startEndIndices.y; ++i)
            {
                if (!m_TrainPositioningSytem.m_PathStopBits.IsBitSet(i)) { continue; }
                // Encountered a stop vertex. Place a platform for this stop.

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                var entity = entityManager.Instantiate(entityPrefab);

                float3 position = m_TrainPositioningSytem.m_PathPositions[i];

                float3 forward = math.normalize(m_TrainPositioningSytem.m_PathPositions[i + 0] - m_TrainPositioningSytem.m_PathPositions[i + 1]);
                float3 tangent = math.normalize(math.cross(forward, Vector3.up));
                quaternion rotation = quaternion.LookRotation(tangent, Vector3.up);

                entityManager.SetComponentData(
                    entity,
                    new Translation
                    {
                        Value = position,
                    }
                );
                entityManager.SetComponentData(
                    entity,
                    new Rotation
                    {
                        Value = rotation
                    }
                );
            }
        }
    }

    private void InstantiatePathMoversOnLoopPaths(GameObject prefab, int pathMoverRequestCount)
    {
        Debug.Assert(pathMoverRequestCount > 0);

        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Perform stratified sampling of paths to on average evenly distribute movers with some variation.
        float pathLengthTotal = ComputePathLengthTotalFromPathRange(0, m_TrainPositioningSytem.m_SinglePathStartIndex);
        float samplePDF = pathLengthTotal / pathMoverRequestCount;
        float cdf = 0.0f;

        int sampleIndex = 0;
        int pathIndex = 0;
        while (sampleIndex < pathMoverRequestCount && pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex)
        {
            float pathLengthCurrent = ComputePathLength(pathIndex);
            Debug.Assert(pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex);

            float pathStartCDF = cdf;
            float pathEndCDF = cdf + pathLengthCurrent;

            int2 startEndIndices = m_TrainPositioningSytem.m_StartEndPositionIndicies[pathIndex];

            int pathPositionIndexCurrent = startEndIndices.x;
            Entity entityPreviousOnPath = Entity.Null;
            Entity entityFirstOnPath = Entity.Null;
            while (pathPositionIndexCurrent < startEndIndices.y && cdf < pathEndCDF)
            {
                float sampleCDF = UnityEngine.Random.value * samplePDF + cdf;

                float3 samplePosition = float3.zero;
                quaternion sampleRotation = quaternion.identity;
                int samplePositionIndex = -1;
                while (cdf < sampleCDF && pathPositionIndexCurrent < startEndIndices.y)
                {
                    int pathPositionIndex0 = pathPositionIndexCurrent;
                    int pathPositionIndex1 = ((pathPositionIndexCurrent + 1 - startEndIndices.x) % (startEndIndices.y - startEndIndices.x)) + startEndIndices.x;
                    float3 pathSegmentV0 = m_TrainPositioningSytem.m_PathPositions[pathPositionIndex0];
                    float3 pathSegmentV1 = m_TrainPositioningSytem.m_PathPositions[pathPositionIndex1];
                    
                    float pathSegmentLength = math.length(pathSegmentV1 - pathSegmentV0);
                    float offsetCDF = math.min(pathSegmentLength, sampleCDF - cdf);
                    float t = offsetCDF / pathSegmentLength;

                    samplePosition = math.lerp(pathSegmentV0, pathSegmentV1, t);

                    float3 forward = math.normalize(pathSegmentV1 - pathSegmentV0);
                    float3 tangent = math.normalize(math.cross(forward, Vector3.up));
                    sampleRotation = quaternion.LookRotation(forward, Vector3.up);

                    samplePositionIndex = pathPositionIndexCurrent;

                    cdf += offsetCDF;
                    ++pathPositionIndexCurrent;
                }

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                Debug.Log("Instantiating Mover: {\nposition: {" + samplePosition.x + ", " + samplePosition.y + ", " + samplePosition.z + "}\n}");
                var entity = entityManager.Instantiate(entityPrefab);
                entityManager.SetComponentData(
                    entity,
                    new Translation
                    {
                        Value = samplePosition,
                    }
                );
                entityManager.SetComponentData(
                    entity,
                    new Rotation
                    {
                        Value = sampleRotation
                    }
                );
                entityManager.SetComponentData(
                    entity,
                    new PathMoverComponent
                    {
                        m_TrackIndex = pathIndex,
                        AccelerationIdx = -1, // TODO: What is this?
                        CurrentPointIndex = samplePositionIndex

                    }
                );

                if (entityPreviousOnPath != Entity.Null)
                {
                    entityManager.SetComponentData(
                        entityPreviousOnPath,
                        new PathMoverSafeFollowComponent
                        {
                            m_FollowEntity = entity
                        }
                    );
                }
                else
                {
                    entityFirstOnPath = entity;
                }
                entityPreviousOnPath = entity;


                ++sampleIndex;
                cdf += samplePDF;
            }

            // Close loop: link the last entity on the current path to follow the first entity on the current path.
            if (entityPreviousOnPath != Entity.Null && entityPreviousOnPath != entityFirstOnPath)
            {
                entityManager.SetComponentData(
                    entityPreviousOnPath,
                    new PathMoverSafeFollowComponent
                    {
                        m_FollowEntity = entityFirstOnPath
                    }
                );
            }

            ++pathIndex;
        }
    }

    private float ComputePathLengthTotalFromPathRange(int pathIndexStart, int pathIndexEnd)
    {
        float pathLength = 0.0f;
        for (int pathIndex = pathIndexStart; pathIndex < pathIndexEnd; ++pathIndex)
        {
            pathLength += ComputePathLength(pathIndex);
        }

        return pathLength;
    }

    private float ComputePathLength(int pathIndex)
    {
        int2 startEndIndices = m_TrainPositioningSytem.m_StartEndPositionIndicies[pathIndex];

        float pathLength = 0.0f;
        for (int i = startEndIndices.x, iLen = (startEndIndices.y - 1); i < iLen; ++i)
        {
            pathLength += math.length(m_TrainPositioningSytem.m_PathPositions[i + 1] - m_TrainPositioningSytem.m_PathPositions[i + 0]);
        }

        if (ComputePathIsLoopFromIndex(pathIndex))
        {
            // Handle loop closure segment.
            pathLength += math.length(m_TrainPositioningSytem.m_PathPositions[startEndIndices.x]
                - m_TrainPositioningSytem.m_PathPositions[startEndIndices.y - 1]);
        }

        return pathLength;
    }

    private bool ComputePathIsLoopFromIndex(int pathIndex)
    {
        return pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex;
    }

    void OnDrawGizmos()
    {
        if (m_TrainPositioningSytem == null)
        {
            Debug.Log("Not converted");
            return;
        }

        for (int i = 0; i < m_TrainPositioningSytem.m_StartEndPositionIndicies.Length; ++i)
        {
            int2 startEndPosition = m_TrainPositioningSytem.m_StartEndPositionIndicies[i];
            
            int pathSize = startEndPosition.y - startEndPosition.x;
            for (int j = 0; j < pathSize; ++j)
            {
                float3 positionStart = m_TrainPositioningSytem.m_PathPositions[startEndPosition.x + j];
                float3 positionEnd = m_TrainPositioningSytem.m_PathPositions[startEndPosition.x + ((j + 1) % pathSize)];
                bool isStop = m_TrainPositioningSytem.m_PathStopBits.IsBitSet(startEndPosition.x + ((j + 1) % pathSize));
                
                Gizmos.color = Metro.INSTANCE().LineColours[i];
                Gizmos.color *= isStop ? new Color(0.25f, 0.25f, 0.25f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Gizmos.DrawLine(positionStart, positionEnd);
                
            }
        }
    }
}
