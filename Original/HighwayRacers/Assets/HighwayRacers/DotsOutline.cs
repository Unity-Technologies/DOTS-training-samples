using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;


struct HighwaySettings
{
    public const int NUM_LANES = 4;
    public const float LANE_SPACING = 1.9f;
    public const float MID_RADIUS = 31.46f;
    public const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    public const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    public const float MIN_DIST_BETWEEN_CARS = .7f;
}

struct HighwaySegment
{
    public enum SegmentType { Straight, CurveRight }
    public SegmentType Type;
    public float Lane0Length;

    public float StartDistance; // distance from highway origin

    public float3 worldPosition;
    public quaternion worldOrientation;

    public float LaneLength(float lane)
    {
        return 0; // TODO
    }

    public void LocalToWorld(
        float posLaneO, float lane,
        out float3 worldPos, out quaternion worldRot)
    {
        worldPos = float3.zero; // TODO
        worldRot = quaternion.identity; // TODO
    }

    public float DistanceFromOrigin(float posLaneO) { return StartDistance + posLaneO; }
}

struct Highway
{
    public NativeArray<HighwaySegment> Segments;

    public int NextSegment(int index) { return (index + 1) % Segments.Length; }
    public int PrevSegment(int index) { return (index + Segments.Length - 1) % Segments.Length; }

    public struct HighwaySpacePartition
    {
        struct Bucket
        {
            public struct Entry
            {
                public CarID CarID;
                public float2 Pos; // (lane0Distance, lane)
            }
            public NativeArray<Entry> Entries;
        }

        NativeArray<Bucket> Buckets;

        int NextBucket(int index) { return (index + 1) % Buckets.Length; }
        int PrevBucket(int index) { return (index + Buckets.Length - 1) % Buckets.Length; }

        public void MoveCar(int CarId, float2 newPos)
        {
        }

        // Distances are in lane space, not lane0 space
        public struct QueryResult
        {
            public float NearestFrontMyLane;
            public float NearestFrontRight;
            public float NearestFrontLeft;
            public float NearestRearRight;
            public float NearestRearLeft;
        }

        public QueryResult GetNearestCars(
            float lane0Distance, float lane, float maxDistanceFront, float maxDistanceRear)
        {
            return new QueryResult(); // TODO
        }
    }

    public HighwaySpacePartition SpacePartition;
}
