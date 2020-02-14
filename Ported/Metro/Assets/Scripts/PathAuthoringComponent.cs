using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PathAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<GameObject> m_PathParents;
    private TrainPositioningSystem m_TrainPositioningSytem;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        int totalPositionsCount = 0;
        for(int i = 0; i < m_PathParents.Count; i++)
        {
            totalPositionsCount += m_PathParents[i].GetComponentsInChildren<RailMarker>().Length;
        }
        Debug.Assert(totalPositionsCount > 0);
        World world = dstManager.World;
        TrainPositioningSystem trainPositioningSystem = world.GetExistingSystem<TrainPositioningSystem>();
        PathMoverSystem pathMoverSystem = world.GetExistingSystem<PathMoverSystem>();
        Debug.Assert(trainPositioningSystem != null);
        trainPositioningSystem.m_PathPositions = new NativeArray<float3>(totalPositionsCount, Allocator.Persistent);
        trainPositioningSystem.m_PathCount = (uint)m_PathParents.Count;
        trainPositioningSystem.m_StartEndPositionIndicies = new NativeArray<int2>(m_PathParents.Count, Allocator.Persistent);
        trainPositioningSystem.m_PathStopBits.Allocate(totalPositionsCount, Allocator.Persistent);
        trainPositioningSystem.m_PathStopBits.ClearAllBits();
        int currIndex = 0;
        for(int i = 0; i < m_PathParents.Count; i++)
        {
            RailMarker[] railMarkers = m_PathParents[i].GetComponentsInChildren<RailMarker>();
            int2 startEnd;
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
            startEnd.y = currIndex;
            trainPositioningSystem.m_StartEndPositionIndicies[i] = startEnd;
        }
        pathMoverSystem.m_PathPositions = trainPositioningSystem.m_PathPositions;
        pathMoverSystem.m_PathIndices = trainPositioningSystem.m_StartEndPositionIndicies;
        m_TrainPositioningSytem = trainPositioningSystem;
    }

    void OnDrawGizmos()
    {
        if (m_TrainPositioningSytem == null)
        {
            Debug.Log("Not converted");
            return;
        }
        Debug.Log("totalPositionsCount = " + m_TrainPositioningSytem.m_PathCount);

        //for (int i = 0, iLen = m_TrainPositioningSytem.m_PathPositions.Length - 1; i < iLen; ++i)
        //for (int i = 0, iLen = m_TrainPositioningSytem.m_StartEndPositionIndicies.Length; i < iLen; ++i)
        for (int i = 0; i < m_TrainPositioningSytem.m_StartEndPositionIndicies.Length; ++i)
        {
            int2 startEndPosition = m_TrainPositioningSytem.m_StartEndPositionIndicies[i];
            
            for (int j = startEndPosition.x; j < (startEndPosition.y - 1); ++j)
            {
                float3 positionStart = m_TrainPositioningSytem.m_PathPositions[j + 0];
                float3 positionEnd = m_TrainPositioningSytem.m_PathPositions[j + 1];
                bool isStop = m_TrainPositioningSytem.m_PathStopBits.IsBitSet(j + 1);
                
                Gizmos.color = Metro.INSTANCE().LineColours[i];
                Gizmos.color *= isStop ? new Color(0.25f, 0.25f, 0.25f, 1.0f) : new Color(1.0f, 1.0f, 1.0f, 1.0f);
                Gizmos.DrawLine(positionStart, positionEnd);
                
            }
        }
    
    }
}
