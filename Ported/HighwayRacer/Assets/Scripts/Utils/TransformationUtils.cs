using Authoring;
using Unity.Collections;
using Unity.Mathematics;

namespace Utils
{
    public static class TransformationUtils
    {
        public static float4x4 GetWorldTransformation(float distance, float laneLength, float laneRadius, float straightLength)
        {
            // Combined segment is a segment with 1 straight and one curve

            float combinedSegmentLength = laneLength * 0.25f;

            int combinedSegmentIndex = (int)math.floor(distance / combinedSegmentLength);
            float distanceInCombinedSegment = distance - combinedSegmentLength * combinedSegmentIndex;

            bool isOnStraight = (distanceInCombinedSegment <= straightLength);

            var rotation = quaternion.RotateY(math.PI * 0.5f * combinedSegmentIndex);

            if (isOnStraight)
            {
                var translation = new float3(-straightLength * 0.5f - laneRadius, 0.0f, distanceInCombinedSegment - straightLength * 0.5f);
                translation = math.rotate(rotation, translation);
                return float4x4.TRS(translation, rotation, new float3(1f, 1f, 1f));
            }
            else
            {
                float distOnCurve = distanceInCombinedSegment - straightLength;
                float angle = distOnCurve / (laneRadius);

                float4x4 transformation = float4x4.identity;
                transformation = math.mul(float4x4.Translate(new float3(-laneRadius, 0.0f, 0.0f)), transformation);
                transformation = math.mul(float4x4.RotateY(angle), transformation);
                transformation = math.mul(float4x4.Translate(new float3(-straightLength * 0.5f, 0.0f, straightLength * 0.5f)), transformation);
                transformation = math.mul(float4x4.RotateY(math.PI * 0.5f * combinedSegmentIndex), transformation);
                return transformation;
            }
        }

        // Get distance of two cars on the track
        public static float GetDistance(in float3 currentCarPosition, in float3 otherCarPosition)
        {
            return math.length(currentCarPosition - otherCarPosition);
        }

        public static float GetDistanceOnLaneChange(int currentLane, float currentDistance, int laneChangeTo, in NativeArray<Lane> lanes, float straightLength, float laneOffset)
        {
            if (laneChangeTo < 0 || laneChangeTo >= lanes.Length)
            {
                return -1.0f;
            }

            int laneDiff = laneChangeTo - currentLane;
            ;
            // we should only allow one lane change
            if (math.abs(laneDiff) != 1)
            {
                return -1.0f;
            }

            var currentLaneLength = lanes[currentLane].LaneLength;
            float combinedSegmentLength = currentLaneLength * 0.25f;
            int combinedSegmentIndex = (int)math.floor(currentDistance / combinedSegmentLength);
            float distanceInCombinedSegment = currentDistance - combinedSegmentLength * combinedSegmentIndex;

            float baseOffset = laneDiff * laneOffset * 0.5f * math.PI * combinedSegmentIndex;
            bool isOnStraight = (distanceInCombinedSegment <= straightLength);

            if (isOnStraight)
            {
                return currentDistance + baseOffset;
            }
            else
            {
                float distOnCurve = distanceInCombinedSegment - straightLength;
                float angle = distOnCurve / (lanes[currentLane].LaneRadius);
                return lanes[laneChangeTo].LaneLength * 0.25f * combinedSegmentIndex + straightLength + angle * lanes[laneChangeTo].LaneRadius;

            }
        }
    }
}
