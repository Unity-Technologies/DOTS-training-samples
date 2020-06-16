using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CarsByLaneSegmentSys))]
    [UpdateBefore(typeof(AdvanceCarsSys))]
    public class UnblockedCarSys : SystemBase
    {
        private NativeArray<OtherCar> selection; // the OtherCar segments to compare against a particular car

        const int nSegments = RoadInit.nSegments;
        const int nLanes = RoadInit.nLanes;
        const int initialCarsPerLaneOfSegment = RoadInit.initialCarsPerLaneOfSegment;
        const float minDist = RoadInit.minDist;

        const float decelerationRate = 10.0f; // m/s to lose per second
        const float accelerationRate = 15.0f; // m/s to lose per second

        protected override void OnCreate()
        {
            base.OnCreate();

            selection = new NativeArray<OtherCar>(3, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;
            var selection = this.selection;

            var dt = Time.DeltaTime;

            var trackLength = RoadInit.trackLength;

            // make sure we don't hit next car ahead, and trigger overtake state
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.WithNone<Overtaking>().ForEach((Entity ent, ref TargetSpeed targetSpeed, ref Speed speed, in TrackPos trackPos, in Lane lane,
                in TrackSegment trackSegment, in BlockedDist blockedDist) =>
            {
                var laneBaseIdx = lane.Val * nSegments;

                var idx = laneBaseIdx + trackSegment.Val;
                selection[0] = otherCars[idx];

                // next
                idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                selection[1] = otherCars[idx];

                // find pos and speed of closest car ahead
                var closestSpeed = -1.0f;
                var closestPos = trackLength; // max distance away
                for (int i = 0; i < 2; i++)
                {
                    var posSegment = selection[i].positions;
                    var speedSegment = selection[i].speeds;

                    var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;

                    for (int j = 0; j < posSegment.Length; j++)
                    {
                        var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
                        var otherSpeed = speedSegment[j];

                        if (otherPos < closestPos &&
                            otherPos > trackPos.Val) // found car ahead closer than previous closest
                        {
                            closestPos = otherPos;
                            closestSpeed = otherSpeed.Val;
                        }
                    }
                }

                var blocked = false;
                var dist = closestPos - trackPos.Val;
                if (dist <= blockedDist.Val &&
                    speed.Val < closestSpeed) // car is blocked ahead in lane
                {
                    ecb.AddComponent<Blocked>(ent);
                    
                    var speedDiff = speed.Val - closestSpeed;
                    var closeness = (dist - minDist) / (blockedDist.Val - minDist); // 0 is max closeness, 1 is min
                    speed.Val -= math.lerp(speedDiff, 0, closeness); // todo: this is probably too quick a deceleration
                    blocked = true;

                    if (targetSpeed.Val > closestSpeed)
                    {
                        targetSpeed.Val = closestSpeed;
                    }
                }

                // if blocked, we only want to go slower, not faste
                if (targetSpeed.Val < speed.Val)
                {
                    speed.Val -= decelerationRate * dt;
                    if (speed.Val < targetSpeed.Val)
                    {
                        speed.Val = targetSpeed.Val;
                    }
                }
                else if (targetSpeed.Val > speed.Val && !blocked)
                {
                    speed.Val += accelerationRate * dt;
                    if (speed.Val > targetSpeed.Val)
                    {
                        speed.Val = targetSpeed.Val;
                    }
                }
            }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }

    public struct OtherCar
    {
        public UnsafeList<TrackPos> positions;
        public UnsafeList<Entity> entities;
        public UnsafeList<Speed> speeds;
    }
}