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
    public int m_MaxAdjacentPlatforms = 5;
    public float m_AdjacentDistance = 15.0f;
    private TrainPositioningSystem m_TrainPositioningSytem;
    private PlatformConnectionSystem m_PlatformConnectionSystem;

    public GameObject m_PrefabPlatform;
    public GameObject m_PrefabMover;
    public int m_MoverCount = 1;
    public float m_MoverAcceleration = 5.0f;

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
        DispatcherStopSystem dispatcherStopSystem = world.GetExistingSystem<DispatcherStopSystem>();

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

        dispatcherStopSystem.m_PathStopBits = trainPositioningSystem.m_PathStopBits;
        dispatcherStopSystem.m_PathIndices = trainPositioningSystem.m_StartEndPositionIndicies;

        m_TrainPositioningSytem = trainPositioningSystem;

        InstantiatePlatforms(m_PrefabPlatform, dstManager);
        InstantiatePathMoversOnLoopPaths(m_PrefabMover, m_MoverCount, m_MoverAcceleration);        
    }

    void FindNext(List<int> stationIndices, PlatformConnectionSystem platformConnectionSystem)
    {
         // next platform
        for( int i = 0; i < stationIndices.Count; i++)
        {
            platformConnectionSystem.m_Next[stationIndices[i]] = stationIndices[(i + 1) % stationIndices.Count];
        }
    }

    void FindOpposite(List<int> stationIndices, PlatformConnectionSystem platformConnectionSystem)
    {
         // opposite platform
        for( int i = 0; i < stationIndices.Count; i++)
        {
            float minDist = 9999999999.0f;
            int oppIndex = i;
            for( int j = 0; j < stationIndices.Count; j++)
            {
                if(i != j)
                {
                    float dist = math.distance(platformConnectionSystem.m_PlatformPositions[stationIndices[i]], platformConnectionSystem.m_PlatformPositions[stationIndices[j]]);
                    if(dist < minDist)
                    {
                        minDist = dist;
                        oppIndex = j;
                    }
                }
            }
            Debug.Assert(oppIndex != i);
            platformConnectionSystem.m_Opposite[stationIndices[i]] = stationIndices[oppIndex];
        }
    }

    void FindAdjacent(List<int> stationIndices, PlatformConnectionSystem platformConnectionSystem)
    {
         // opposite platform
        for( int i = 0; i < stationIndices.Count; i++)
        {           
            platformConnectionSystem.m_NumAdjacent[stationIndices[i]] = 0;
            for( int j = 0; j < stationIndices.Count; j++)
            {
                if((i != j) && (platformConnectionSystem.m_Opposite[stationIndices[i]] != stationIndices[j])) // not the same and not opposite
                {
                    float dist = math.distance(platformConnectionSystem.m_PlatformPositions[stationIndices[i]], platformConnectionSystem.m_PlatformPositions[stationIndices[j]]);
                    if(dist < m_AdjacentDistance)
                    {
                        platformConnectionSystem.m_Adjacents[platformConnectionSystem.m_NumAdjacent[stationIndices[i]]] = stationIndices[j];
                        platformConnectionSystem.m_NumAdjacent[stationIndices[i]]++;
                        if(platformConnectionSystem.m_NumAdjacent[stationIndices[i]] == m_MaxAdjacentPlatforms)
                           break;
                    }
                }
            }
        }
    }

    public void InstantiatePlatforms(GameObject prefab, EntityManager dstManager)
    {
        Debug.Assert(prefab != null);
        World world = dstManager.World;
        PlatformConnectionSystem platformConnectionSystem = world.GetExistingSystem<PlatformConnectionSystem>();
        m_PlatformConnectionSystem = platformConnectionSystem;

        // Create entity prefab from the game object hierarchy once
        var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
        var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);

        int platformCount = 0;    
        for (int pathIndex = 0; pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex; ++pathIndex)
        {
            int2 startEndIndices = m_TrainPositioningSytem.m_StartEndPositionIndicies[pathIndex];
            for (int i = startEndIndices.x; i < startEndIndices.y; ++i)
            {
                if (m_TrainPositioningSytem.m_PathStopBits.IsBitSet(i))
                { 
                    platformCount++;
                }
            }
        }

        platformConnectionSystem.m_PlatformPositions = new NativeArray<float3>(platformCount, Allocator.Persistent);
        platformConnectionSystem.m_PlatformRotations = new NativeArray<quaternion>(platformCount, Allocator.Persistent);
        platformConnectionSystem.m_Next = new NativeArray<int>(platformCount, Allocator.Persistent);
        platformConnectionSystem.m_Opposite = new NativeArray<int>(platformCount, Allocator.Persistent);
        platformConnectionSystem.m_NumAdjacent = new NativeArray<int>(platformCount, Allocator.Persistent);
        platformConnectionSystem.m_Adjacents = new NativeArray<int>(platformCount * m_MaxAdjacentPlatforms, Allocator.Persistent);

        // Only spawn platforms at stops in loop paths, not single paths.
        // This seems a little hacky / non-obvious, in that we assume a loop path is specifically a train path that has platforms.
        platformCount = 0;
        for (int pathIndex = 0; pathIndex < m_TrainPositioningSytem.m_SinglePathStartIndex; ++pathIndex)
        {
            int2 startEndIndices = m_TrainPositioningSytem.m_StartEndPositionIndicies[pathIndex];
            List<int> stationIndices = new List<int>();
            for (int i = startEndIndices.x; i < startEndIndices.y; ++i)
            {
                if (!m_TrainPositioningSytem.m_PathStopBits.IsBitSet(i)) { continue; }
                // Encountered a stop vertex. Place a platform for this stop.

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                var entity = dstManager.Instantiate(entityPrefab);

                float3 position = m_TrainPositioningSytem.m_PathPositions[i];

                float3 forward = math.normalize(m_TrainPositioningSytem.m_PathPositions[i + 0] - m_TrainPositioningSytem.m_PathPositions[i + 1]);
                float3 tangent = math.normalize(math.cross(forward, Vector3.up));
                quaternion rotation = quaternion.LookRotation(tangent, Vector3.up);

                stationIndices.Add(platformCount);
                platformConnectionSystem.m_PlatformPositions[platformCount] = position;
                platformConnectionSystem.m_PlatformRotations[platformCount] = rotation;
                platformCount++;

                dstManager.SetComponentData(entity, new LocalToWorld
                {
                    Value = float4x4.TRS(position, rotation, new float3(1f))
                });
                dstManager.SetComponentData(
                    entity,
                    new Translation
                    {
                        Value = position,
                    }
                );
                dstManager.SetComponentData(
                    entity,
                    new Rotation
                    {
                        Value = rotation
                    }
                );
            }

            FindNext(stationIndices, platformConnectionSystem);
            FindOpposite(stationIndices, platformConnectionSystem);
            FindAdjacent(stationIndices, platformConnectionSystem);           
        }
    }

    private void InstantiatePathMoversOnLoopPaths(GameObject prefab, int pathMoverRequestCount, float acceleration)
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
                // Debug.Log("Instantiating Mover: {\nposition: {" + samplePosition.x + ", " + samplePosition.y + ", " + samplePosition.z + "}\n}");
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
                entityManager.SetComponentData(
                    entity,
                    new MovementDerivatives
                    {
                        Speed = 0,
                        Acceleration = acceleration,
                        AccelerationGoal = acceleration
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

    void DrawTrackGizmo()
    {
        if (m_TrainPositioningSytem == null)
        {
            return;
        }

        Color c = Gizmos.color;
        for (int i = 0; i < m_TrainPositioningSytem.m_SinglePathStartIndex; ++i)
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
        Gizmos.color = c;
    }

    void DrawPlatformGizmo()
    {
        if (m_PlatformConnectionSystem == null)
        {
            return;
        }

        Color c = Gizmos.color;        
        for(int i = 0; i < m_PlatformConnectionSystem.m_PlatformPositions.Length; i++)
        {
            float3 current = m_PlatformConnectionSystem.m_PlatformPositions[i];
            float3 next = m_PlatformConnectionSystem.m_PlatformPositions[m_PlatformConnectionSystem.m_Next[i]];
            float3 opposite = m_PlatformConnectionSystem.m_PlatformPositions[m_PlatformConnectionSystem.m_Opposite[i]];            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(current, next);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(current, opposite);
        }
        Gizmos.color = c;
    }

    void OnDrawGizmos()
    {
        DrawTrackGizmo();
        DrawPlatformGizmo();
    }
}
