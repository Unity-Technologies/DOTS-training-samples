using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RoadInit : MonoBehaviour
{
    public static NativeArray<RoadSegment> roadSegments;

    public const int nLanes = 4;
    public const int nSegments = 8;
    public const int initialCarsPerLaneOfSegment = 20; // past a certain track size, this will need to be bigger
    public const float minDist = 4.0f;

    public static float trackLength = 0.0f;
    
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


        const float baseStraightLength = 12.0f;
        const float curvedLength = 48.69f;   // calculated from radius that is midpoint between first and last lane
        const float laneWidth = 1.8f;
        float straightLength = 32.0f;   // can set this to desired length
        float straightScale = straightLength / baseStraightLength;

        roadSegments = new NativeArray<RoadSegment>(transform.childCount, Allocator.Persistent);

        // put the track pieces in place:
        
        var pos = new float3();
        float lengthSum = 0;

        // end point of segment is start point of next
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            var segmentInfo = child.gameObject.GetComponent<SegmentAuth>();
            var cardinal = segmentInfo.direction;
            float length = 0;
            
            float3 endPos = new float3();

            if (segmentInfo.radius > 0)
            {
                endPos = pos + vecFromCardinal[cardinal] * segmentInfo.radius + leftVecFromCardinal[cardinal] * -segmentInfo.radius;
                length = curvedLength;
                lengthSum += curvedLength;
            }
            else
            {
                endPos = pos + vecFromCardinal[cardinal] * straightLength;
                length = straightLength;
                lengthSum += straightLength;

                child.localScale = new Vector3(1, 1, straightScale);
            }
            
            child.position = pos;
            roadSegments[i] = new RoadSegment()
            {
                Position = pos,
                EndPosition = endPos,
                Direction = cardinal,
                DirectionVec = vecFromCardinal[cardinal],
                DirectionRot = rotFromCardinal[cardinal],
                DirectionRotEnd = rightRotFromCardinal[cardinal],
                DirectionLaneOffset = leftVecFromCardinal[cardinal] * laneWidth,
                Length = length,
                Id = (short) i,
                Curved = segmentInfo.radius > 0,
                Radius = segmentInfo.radius,
                Threshold = lengthSum,
            };

            pos = endPos;
        }
        
        RoadInit.trackLength = roadSegments[roadSegments.Length - 1].Threshold;
    }

    private void OnDestroy()
    {
        roadSegments.Dispose();
    }
}


// todo: use float2's instead
public struct RoadSegment
{
    public float3 Position;
    public float3 EndPosition;         // relative from Position 
    public Cardinal Direction;
    public float3 DirectionVec;
    public quaternion DirectionRot;
    public quaternion DirectionRotEnd; // only for curved segments; 90 degrees right of DirectionRot
    public float3 DirectionLaneOffset;
    public float Length;
    public short Id;
    public bool Curved;
    public float Radius; // 0 for straight segments 
    public float Threshold; // end track pos i.e. cummulative length of this segment and all prior
}

public enum Cardinal
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}