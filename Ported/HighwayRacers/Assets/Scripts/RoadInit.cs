using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class RoadInit : MonoBehaviour
{
    public NativeArray<RoadSegmentInfo> segmentInfos;
    public NativeArray<float> segmentThresholds;
    
    void Start()
    {
        const float straightLength = 12.0f;
        const float curvedLength = 40.0f;   // todo get real measurement
        
        segmentInfos = new NativeArray<RoadSegmentInfo>(transform.childCount, Allocator.Persistent);
        segmentThresholds = new NativeArray<float>(transform.childCount, Allocator.Persistent);
        
        var prev = transform.GetChild(0);

        var direction = (prev.Find("EndPoint").position - prev.position).normalized; 
        segmentInfos[0] = new RoadSegmentInfo()
        {
            position = prev.position,
            direction = direction,
            laneOffsetDir = Quaternion.Euler(0, -90, 0) * direction,  // todo: check if should be negative to rotate left
            length = straightLength,
            id = 0,
            curved = false,
        };
        segmentThresholds[0] = straightLength; 
        
        // put the track pieces in place:
        // Move start of child to endpoint of prev (in world coords).
        for (int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            var startPos = prev.Find("EndPoint").position;
            var endPos = child.Find("EndPoint").position;
            
            var curved = (i % 2 == 1);
            direction = (child.Find("EndPoint").position - child.position).normalized;
            if (curved)
            {
                // rotate left 45 degrees (we want the initial direction of travel for the curved piece
                direction = Quaternion.Euler(0, 45, 0) * direction; // todo: positive or negative 45?
                direction.Normalize();
            }

            child.position = startPos;
            segmentInfos[i] = new RoadSegmentInfo()
            {
                position = startPos,
                direction = direction,
                laneOffsetDir = Quaternion.Euler(0, -90, 0) * direction,
                length = curved ? curvedLength : straightLength,
                id = (short)i,
                curved = curved,
            };
            segmentThresholds[i] = segmentThresholds[i - 1] + segmentInfos[i].length;

            prev = child;
        }
    }

    private void OnDestroy()
    {
        segmentThresholds.Dispose();
        segmentInfos.Dispose();
    }
}

public struct RoadSegmentInfo
{
    public Vector3 position;
    public Vector3 direction;
    public Vector3 laneOffsetDir;
    public float length;
    public short id;
    public bool curved;
}