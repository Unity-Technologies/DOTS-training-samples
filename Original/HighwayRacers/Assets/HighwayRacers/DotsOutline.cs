#if false
using Unity.Mathematics;
using Unity.Collections;

namespace HighwayRacers
{
    struct HighwayConstants
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
        public float AvgLength;

        public float StartDistance; // distance from highway origin

        public float3 worldPosition;
        public quaternion worldOrientation;

        public float LaneLength(float lane)
        {
            return 0; // TODO
        }

        public void LocalToWorld(
            float avgPos, float lane,
            out float3 worldPos, out quaternion worldRot)
        {
            worldPos = float3.zero; // TODO
            worldRot = quaternion.identity; // TODO
        }

        public float DistanceFromOrigin(float avgPos) { return StartDistance + avgPos; }
    }

    struct DotsHighway
    {
        public NativeArray<HighwaySegment> Segments;

        public int NextSegment(int index) { return (index + 1) % Segments.Length; }
        public int PrevSegment(int index) { return (index + Segments.Length - 1) % Segments.Length; }

        public HighwaySpacePartition SpacePartition;
    }
}
#endif
