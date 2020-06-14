using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RoadInit : MonoBehaviour
{
    public NativeArray<RoadSegment> segmentInfos;
    
    void Start()
    {
        var rotFromCardinal = new Dictionary<Cardinal, quaternion>();
        rotFromCardinal[Cardinal.UP] = quaternion.EulerXYZ(0, 0, 0);
        rotFromCardinal[Cardinal.DOWN] = quaternion.EulerXYZ(0, math.radians(180), 0);
        rotFromCardinal[Cardinal.LEFT] = quaternion.EulerXYZ(0, math.radians(-90), 0);
        rotFromCardinal[Cardinal.RIGHT] = quaternion.EulerXYZ(0, math.radians(90), 0);
        
        var vecFromCardinal = new Dictionary<Cardinal, float3>();
        vecFromCardinal[Cardinal.UP] = Vector3.forward;
        vecFromCardinal[Cardinal.DOWN] = Vector3.back;
        vecFromCardinal[Cardinal.LEFT] = Vector3.left;
        vecFromCardinal[Cardinal.RIGHT] = Vector3.right;
        
        var rightRotFromCardinal = new Dictionary<Cardinal, quaternion>();
        rightRotFromCardinal[Cardinal.UP] = quaternion.EulerXYZ(0, math.radians(90), 0);
        rightRotFromCardinal[Cardinal.DOWN] = quaternion.EulerXYZ(0, math.radians(270), 0);
        rightRotFromCardinal[Cardinal.LEFT] = quaternion.EulerXYZ(0, 0, 0);
        rightRotFromCardinal[Cardinal.RIGHT] = quaternion.EulerXYZ(0, math.radians(180), 0);
        
        var leftVecFromCardinal = new Dictionary<Cardinal, float3>();
        leftVecFromCardinal[Cardinal.UP] = Vector3.left;
        leftVecFromCardinal[Cardinal.DOWN] = Vector3.right;
        leftVecFromCardinal[Cardinal.LEFT] = Vector3.back;
        leftVecFromCardinal[Cardinal.RIGHT] = Vector3.forward;

        const float straightLength = 12.0f;
        const float curvedLength = 40.0f;   // todo get real measurement
        const float laneWidth = 1.8f; // todo verify value
        
        segmentInfos = new NativeArray<RoadSegment>(transform.childCount, Allocator.Persistent);
        
        var prev = transform.GetChild(0);

        var segmentInfo = prev.gameObject.GetComponent<SegmentAuth>(); 
        var cardinal = segmentInfo.direction;
        segmentInfos[0] = new RoadSegment()
        {
            Position = prev.position,
            Direction = cardinal,
            DirectionVec = vecFromCardinal[cardinal],
            DirectionRot = rotFromCardinal[cardinal],
            DirectionRotEnd = rightRotFromCardinal[cardinal],
            DirectionLaneOffset = leftVecFromCardinal[cardinal] * laneWidth,
            Length = straightLength,
            Id = 0,
            Curved = false,
            Radius = segmentInfo.radius,
            RadiusSqr = segmentInfo.radius * segmentInfo.radius,
            Threshold = straightLength,
        };
        var lengthSum = straightLength; 
        
        // put the track pieces in place:
        // Move start of child to endpoint of prev (in world coords).
        for (int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            
            segmentInfo = child.gameObject.GetComponent<SegmentAuth>(); 
            cardinal = segmentInfo.direction;

            var pos = prev.Find("EndPoint").position;
            var curved = (i % 2 == 1);

            lengthSum += curved ? curvedLength : straightLength; 
            
            child.position = pos;
            segmentInfos[i] = new RoadSegment()
            {
                Position = pos,
                Direction = cardinal,
                DirectionVec = vecFromCardinal[cardinal],
                DirectionRot = rotFromCardinal[cardinal],
                DirectionRotEnd = rightRotFromCardinal[cardinal],
                DirectionLaneOffset = leftVecFromCardinal[cardinal] * laneWidth,
                Length = curved ? curvedLength : straightLength,
                Id = (short)i,
                Curved = curved,
                Radius = segmentInfo.radius,
                RadiusSqr = segmentInfo.radius * segmentInfo.radius,
                Threshold = lengthSum,
            };
            
            prev = child;
        }
    }

    private void OnDestroy()
    {
        segmentInfos.Dispose();
    }
}


// todo: use float2's instead
public struct RoadSegment
{
    public float3 Position;
    public Cardinal Direction;
    public float3 DirectionVec;
    public quaternion DirectionRot;
    public quaternion DirectionRotEnd;    // only for curved segments; 90 degrees right of DirectionRot
    public float3 DirectionLaneOffset;
    public float Length;
    public short Id;
    public bool Curved;
    public float Radius;     // only used for curved segments; radius for lane 0  // todo: init radius
    public float RadiusSqr;     // only used for curved segments  // todo: init radius
    public float Threshold;        // end track pos i.e. cummulative length of this segment and all prior
}

public enum Cardinal
{
    UP, DOWN, LEFT, RIGHT,
}