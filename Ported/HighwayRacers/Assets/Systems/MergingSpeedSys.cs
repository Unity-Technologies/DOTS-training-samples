using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    [UpdateAfter(typeof(BucketizeSys))]
    public class MergingSpeedSys : SystemBase
    {
        const float minDist = Road.minDist;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var nSegments = Road.nSegments;
            var segmentizedCars = World.GetExistingSystem<BucketizeSys>().BucketizedCars;

            var dt = Time.DeltaTime;

            var trackLength = Road.roadLength;

            Entities.WithAll<MergingLeft>()
                .ForEach((ref TargetSpeed targetSpeed, ref Speed speed, ref Lane lane, in TrackPos trackPos,
                    in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
                {
                    CarUtil.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, segmentizedCars,
                        trackSegment.Val, lane.Val, lane.Val - 1, trackLength, trackPos, nSegments);

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

                    CarUtil.SetSpeedForUnblocked(ref targetSpeed, ref speed, dt, desiredSpeed.Unblocked);
                }).Run();

            Entities.WithAll<MergingRight>()
                .ForEach((ref TargetSpeed targetSpeed, ref Speed speed, ref Lane lane, in TrackPos trackPos,
                    in TrackSegment trackSegment, in Blocking blocking, in DesiredSpeed desiredSpeed) =>
                {
                    CarUtil.GetClosestPosAndSpeed(out var closestPos, out var closestSpeed, segmentizedCars,
                        trackSegment.Val, lane.Val, lane.Val + 1, trackLength, trackPos, nSegments);

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

                    CarUtil.SetSpeedForUnblocked(ref targetSpeed, ref speed, dt, desiredSpeed.Unblocked);
                }).Run();
           
        }
    }
}