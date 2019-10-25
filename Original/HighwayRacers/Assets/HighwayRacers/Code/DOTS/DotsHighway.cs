using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace HighwayRacers
{
    public struct DotsHighway
    {
        struct Section
        {
            public float3 Pos;
            public float Lane0Length;
            public float StartRotation;
            public float CurveRadius; // 0 means straight
            public int NumLanes;

            public float LaneLength(float lane)
            {
                return math.select(
                    Lane0Length,
                    (CurveRadius + lane * Highway.LANE_SPACING) * math.PI * 0.5f,
                    CurveRadius > 0);
            }

            public float3 GetLocalPosition(float localDistance, float lane, out float rotY)
            {
                if (CurveRadius == 0)
                {
                    rotY = 0;
                    return new float3(
                        Highway.LANE_SPACING * ((NumLanes - 1) / 2f - lane),
                        0, localDistance);
                }
                float radius = CurveRadius + lane * Highway.LANE_SPACING;
                rotY = localDistance / radius;
                return new float3(
                    Highway.MID_RADIUS - math.cos(rotY) * radius,
                    0, math.sin(rotY) * radius);
            }
        }
        NativeArray<Section> Sections;
        float LastLaneLength;

        public void Create(HighwayPiece[] pieces, int numLanes)
        {
            NumLanes = numLanes;
            Dispose();
            Lane0Length = 0;
            Sections = new NativeArray<Section>(pieces.Length, Allocator.Persistent);
            for (int i = 0; i < pieces.Length; ++i)
            {
                var len = pieces[i].length(0);
                Sections[i] = new Section
                {
                    NumLanes = NumLanes,
                    Pos = pieces[i].transform.localPosition,
                    Lane0Length = len,
                    StartRotation = pieces[i].startRotation,
                    CurveRadius = pieces[i].curveRadiusLane0
                };
                Lane0Length += len;
            }
            LastLaneLength = 0;
            for (int s = 0; s < Sections.Length; ++s)
                LastLaneLength += Sections[s].LaneLength(NumLanes - 1);
        }

        public void Dispose()
        {
            sReaderJob.Complete();
            if (Sections.IsCreated) Sections.Dispose();
        }

        static JobHandle sReaderJob;
        public static void RegisterReaderJob(JobHandle h)
        {
            sReaderJob = h;
        }

        public int NumLanes { get; private set; }

        public float Lane0Length { get; private set; }

        public float LaneLength(float lane)
        {
            return math.lerp(Lane0Length, LastLaneLength, lane / (NumLanes - 1));
        }

        /// <summary>
        /// Wraps distance to be in [0, l), where l is the length of the given lane.
        /// </summary>
        public float WrapDistance(float distance, float lane)
        {
            return distance % LaneLength(lane);
        }

        /// <summary>
        /// Gets world position of a car based on its lane and distance from the start in that lane.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="lane"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="rotation">y rotation of the car, in radians.</param>
        public void GetWorldPosition(
            float distance, float lane, out float3 outPos, out quaternion outRotation)
        {
            // keep distance in [0, length)
            distance = WrapDistance(distance, lane);

            float pieceStartDistance = 0;
            float pieceEndDistance = 0;
            for (int i = 0; i < Sections.Length; i++)
            {
                var section = Sections[i];
                pieceStartDistance = pieceEndDistance;
                pieceEndDistance += section.LaneLength(lane);
                if (distance < pieceEndDistance)
                {
                    // inside section i
                    var localPos = section.GetLocalPosition(
                        distance - pieceStartDistance, lane, out float rotY);

                    // transform
                    var q = quaternion.AxisAngle(
                        new float3(0, 1, 0), section.StartRotation);
                    outPos = math.mul(q, localPos) + section.Pos;
                    outRotation = quaternion.AxisAngle(
                        new float3(0, 1, 0), rotY + section.StartRotation);
                    return;
                }
            }
            outPos = float3.zero;
            outRotation = quaternion.identity;
        }

        /// <summary>
        /// Gets distance in another lane that appears to be the same distance in the given lane.
        /// </summary>
        public float GetEquivalentDistance(float distance, float lane, float otherLane)
        {
            // keep distance in [0, length)
			distance = WrapDistance(distance, lane);
            if (lane == otherLane)
                return distance;

            float pieceStartDistance = 0;
            float pieceEndDistance = 0;
            float pieceStartDistanceOtherLane = 0;
            float pieceEndDistanceOtherLane = 0;

            for (int i = 0; i < Sections.Length; i++)
            {
                var section = Sections[i];
                pieceStartDistance = pieceEndDistance;
                pieceStartDistanceOtherLane = pieceEndDistanceOtherLane;
                pieceEndDistance += section.LaneLength(lane);
                pieceEndDistanceOtherLane += section.LaneLength(otherLane);
                if (distance < pieceEndDistance)
                {
                    // inside piece i
                    if (section.CurveRadius == 0)
                        return pieceStartDistanceOtherLane + distance - pieceStartDistance;
                    // curved piece
                    float radius = section.CurveRadius + Highway.LANE_SPACING * lane;
                    float radiusOtherLane = section.CurveRadius + Highway.LANE_SPACING * otherLane;
                    return pieceStartDistanceOtherLane
                        + (distance - pieceStartDistance) * radiusOtherLane / radius;
                }
            }
            return 0;
        }
    }
}

