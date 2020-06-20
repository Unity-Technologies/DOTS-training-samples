using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    // update cars that aren't merging or overtaking 
    [UpdateBefore(typeof(AdvanceCarsSys))]
    [UpdateAfter(typeof(MergingSpeedSys))]
    public class CarSys : SystemBase
    {
        private NativeArray<OtherCars> selection; // the OtherCar segments to compare against a particular car

        const int nSegments = RoadInit.nSegments;
        const int nLanes = RoadInit.nLanes;

        const float minDist = RoadInit.minDist;

        const float mergeLookAhead = RoadInit.mergeLookAhead;
        const float mergeLookBehind = RoadInit.mergeLookBehind;

        const float decelerationRate = RoadInit.decelerationRate;
        const float accelerationRate = RoadInit.accelerationRate;

        protected override void OnCreate()
        {
            base.OnCreate();

            selection = new NativeArray<OtherCars>(2, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            selection.Dispose();
        }

        public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed, NativeArray<OtherCars> selection,
            TrackSegment trackSegment, float trackLength, TrackPos trackPos)
        {
            // find pos and speed of closest car ahead
            closestSpeed = 0.0f;
            closestPos = float.MaxValue;
            for (int i = 0; i < selection.Length; i++)
            {
                var posSegment = selection[i].positions;
                var speedSegment = selection[i].speeds;

                var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;

                for (int j = 0; j < posSegment.Length; j++)
                {
                    var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
                    var otherSpeed = speedSegment[j];

                    if (otherPos < closestPos &&
                        otherPos > trackPos.Val) // found a car ahead closer than previous closest
                    {
                        closestPos = otherPos;
                        closestSpeed = otherSpeed.Val;
                    }
                }
            }
        }

        // todo: account for *speed* of adjacent car ahead and car behind relative to this car?
        public static bool canMerge(float pos, int destLane, int segment, NativeArray<OtherCars> otherCars, float trackLength)
        {
            var laneBaseIdx = destLane * nSegments;

            var idx = laneBaseIdx + segment;
            var adjacentLane = otherCars[idx];

            var wrapAround = (segment == nSegments - 1);

            idx = laneBaseIdx + (wrapAround ? 0 : segment + 1);
            var adjacentLaneNextSegment = otherCars[idx];

            // find pos and speed of closest car ahead and closest car behind 
            var closestAheadPos = float.MaxValue;
            var closestAheadSpeed = 0.0f;

            var closestBehindPos = float.MinValue;
            var closestBehindSpeed = 0.0f;

            var posSegment = adjacentLane.positions;
            var speedSegment = adjacentLane.speeds;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < posSegment.Length; j++)
                {
                    var otherPos = posSegment[j].Val + (wrapAround && i == 1 ? trackLength : 0);
                    var otherSpeed = speedSegment[j];

                    if (otherPos < closestAheadPos &&
                        otherPos > pos) // found a car ahead that's closer than previous closest
                    {
                        closestAheadPos = otherPos;
                        closestAheadSpeed = otherSpeed.Val;
                    }
                    else if (otherPos > closestBehindPos &&
                             otherPos <= pos) // found a car behind (or equal) that's closer than previous closest
                    {
                        closestBehindPos = otherPos;
                        closestBehindSpeed = otherSpeed.Val;
                    }
                }

                posSegment = adjacentLaneNextSegment.positions;
                speedSegment = adjacentLaneNextSegment.speeds;
            }

            // sufficient margin of open space
            return (closestBehindPos + mergeLookBehind) < pos && (closestAheadPos - mergeLookAhead) > pos;
        }

        public static void SetSpeedForUnblocked(ref TargetSpeed targetSpeed, ref Speed speed, float dt, float unblockedSpeed)
        {
            targetSpeed.Val = unblockedSpeed;

            if (targetSpeed.Val < speed.Val)
            {
                speed.Val -= decelerationRate * dt;
                if (speed.Val < targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
            else if (targetSpeed.Val > speed.Val)
            {
                speed.Val += accelerationRate * dt;
                if (speed.Val > targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
        }

        protected override void OnUpdate()
        {
            var trackLength = RoadInit.trackLength;
            var roadSegments = RoadInit.roadSegments;

            var selection = this.selection;
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;

            var mergeLeftFrame = SegmentizeSys.mergeLeftFrame;

            var dt = Time.DeltaTime;

            // make sure we don't hit next car ahead, and trigger overtake state
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.WithNone<MergingLeft, MergingRight>().WithNone<OvertakingLeft, OvertakingRight>().ForEach((Entity ent, ref TargetSpeed targetSpeed,
                ref Speed speed, ref Lane lane, in TrackPos trackPos, in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
            {
                var laneBaseIdx = lane.Val * nSegments;

                var idx = laneBaseIdx + trackSegment.Val;
                selection[0] = otherCars[idx];

                // next
                idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                selection[1] = otherCars[idx];

                GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, selection, trackSegment, trackLength, trackPos);

                if (closestPos != float.MaxValue)
                {
                    var dist = closestPos - trackPos.Val;
                    if (dist <= blocking.Dist &&
                        speed.Val > closestSpeed) // car is still blocked ahead in lane
                    {
                        var closeness = (dist - minDist) / (blocking.Dist - minDist); // 0 is max closeness, 1 is min

                        // closer we get within minDist of leading car, the closer we match speed
                        const float fudge = 2.0f;
                        var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                        if (newSpeed < speed.Val)
                        {
                            speed.Val = newSpeed;
                        }

                        // to spare us from having to check prior segment, can't merge if too close to start of segment
                        var threshold = roadSegments[(trackSegment.Val > 0) ? trackSegment.Val - 1 : nSegments - 1].Threshold;
                        var segmentPos = trackPos.Val - threshold;
                        if (segmentPos < mergeLookBehind)
                        {
                            return;
                        }

                        // look for opening on left
                        if (mergeLeftFrame && lane.Val < nLanes - 1)
                        {
                            var leftLaneIdx = lane.Val + 1;
                            if (canMerge(trackPos.Val, leftLaneIdx, trackSegment.Val, otherCars, trackLength))
                            {
                                ecb.AddComponent<MergingLeft>(ent);
                                ecb.AddComponent<LaneOffset>(ent, new LaneOffset() {Val = -1.0f});
                                lane.Val = (byte) leftLaneIdx;
                            }
                        }
                        else if (lane.Val > 0) // look for opening on right
                        {
                            var rightLaneIdx = lane.Val - 1;
                            if (canMerge(trackPos.Val, rightLaneIdx, trackSegment.Val, otherCars, trackLength))
                            {
                                ecb.AddComponent<MergingRight>(ent);
                                ecb.AddComponent<LaneOffset>(ent, new LaneOffset() {Val = 1.0f});
                                lane.Val = (byte) rightLaneIdx;
                            }
                        }

                        return;
                    }
                }

                SetSpeedForUnblocked(ref targetSpeed, ref speed, dt, desiredSpeed.Unblocked);
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}