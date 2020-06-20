using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CarsByLaneSegmentSys))]
    [UpdateBefore(typeof(AdvanceCarsSys))]
    public class MergingSpeedSys : SystemBase
    {
        private NativeArray<OtherCars> selection; // the OtherCar segments to compare against a particular car

        const int nSegments = RoadInit.nSegments;
        const float minDist = RoadInit.minDist;

        const float decelerationRate = RoadInit.decelerationRate;
        const float accelerationRate = RoadInit.accelerationRate;

        protected override void OnCreate()
        {
            base.OnCreate();

            selection = new NativeArray<OtherCars>(4, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            selection.Dispose();
        }

        protected override void OnUpdate()
        {
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;
            var selection = this.selection;

            var dt = Time.DeltaTime;

            var trackLength = RoadInit.trackLength;

            Entities.WithAll<MergingLeft>()
                .ForEach((ref TargetSpeed targetSpeed, ref Speed speed, ref Lane lane, in TrackPos trackPos,
                    in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
                {
                    var laneBaseIdx = lane.Val * nSegments;

                    var idx = laneBaseIdx + trackSegment.Val;
                    selection[0] = otherCars[idx];

                    // next
                    idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                    selection[1] = otherCars[idx];

                    // check lane to right as well (the lane we're merging from)
                    laneBaseIdx = (lane.Val - 1) * nSegments;
                    idx = laneBaseIdx + trackSegment.Val;
                    selection[2] = otherCars[idx];

                    // next
                    idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                    selection[3] = otherCars[idx];

                    CarSys.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, selection, trackSegment, trackLength, trackPos);

                    if (closestPos != float.MaxValue)
                    {
                        var dist = closestPos - trackPos.Val;
                        if (dist <= blocking.Dist &&
                            speed.Val > closestSpeed) // car is blocked ahead in lane
                        {
                            var closeness = (dist - minDist) / (blocking.Dist - minDist); // 0 is max closeness, 1 is min

                            // closer we get within minDist of leading car, the closer we match speed
                            const float fudge = 2.0f;
                            var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                            if (newSpeed < speed.Val)
                            {
                                speed.Val = newSpeed;
                            }

                            return;
                        }
                    }

                    CarSys.SetSpeedForUnblocked(ref targetSpeed, ref speed, dt, desiredSpeed.Unblocked);
                }).Run();

            Entities.WithAll<MergingRight>()
                .ForEach((ref TargetSpeed targetSpeed, ref Speed speed, ref Lane lane, in TrackPos trackPos,
                    in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
                {
                    var laneBaseIdx = lane.Val * nSegments;

                    var idx = laneBaseIdx + trackSegment.Val;
                    selection[0] = otherCars[idx];

                    // next
                    idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                    selection[1] = otherCars[idx];

                    // check lane to left as well (the lane we're merging from)
                    laneBaseIdx = (lane.Val + 1) * nSegments;
                    idx = laneBaseIdx + trackSegment.Val;
                    selection[2] = otherCars[idx];

                    // next
                    idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                    selection[3] = otherCars[idx];

                    CarSys.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, selection, trackSegment, trackLength, trackPos);

                    if (closestPos != float.MaxValue)
                    {
                        var dist = closestPos - trackPos.Val;
                        if (dist <= blocking.Dist &&
                            speed.Val > closestSpeed) // car is blocked ahead in lane
                        {
                            var closeness = (dist - minDist) / (blocking.Dist - minDist); // 0 is max closeness, 1 is min

                            // closer we get within minDist of leading car, the closer we match speed
                            const float fudge = 2.0f;
                            var newSpeed = math.lerp(closestSpeed, speed.Val + fudge, closeness);
                            if (newSpeed < speed.Val)
                            {
                                speed.Val = newSpeed;
                            }

                            return;
                        }
                    }

                    CarSys.SetSpeedForUnblocked(ref targetSpeed, ref speed, dt, desiredSpeed.Unblocked);
                }).Run();
           
        }
    }
}