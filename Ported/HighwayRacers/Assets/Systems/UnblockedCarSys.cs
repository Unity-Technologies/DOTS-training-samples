using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CarsByLaneSegmentSys))]
    [UpdateBefore(typeof(AdvanceCarsSys))]
    public class UnblockedCarSys : SystemBase
    {
        private NativeArray<OtherCars> selection; // the OtherCar segments to compare against a particular car

        const int nSegments = RoadInit.nSegments;
        const float minDist = RoadInit.minDist;

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

        protected override void OnUpdate()
        {
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;
            var selection = this.selection;

            var dt = Time.DeltaTime;

            var trackLength = RoadInit.trackLength;

            // make sure we don't hit next car ahead, and trigger overtake state
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            Entities.WithNone<Blocked>().WithNone<Overtaking>()
                .ForEach((Entity ent, ref TargetSpeed targetSpeed, ref Speed speed, in TrackPos trackPos, in Lane lane,
                    in TrackSegment trackSegment, in BlockedDist blockedDist) =>
                {
                    var laneBaseIdx = lane.Val * nSegments;

                    var idx = laneBaseIdx + trackSegment.Val;
                    selection[0] = otherCars[idx];

                    // next
                    idx = laneBaseIdx + ((trackSegment.Val == nSegments - 1) ? 0 : trackSegment.Val + 1);
                    selection[1] = otherCars[idx];

                    // find pos and speed of closest car ahead
                    var closestSpeed = 0.0f;
                    var closestPos = float.MaxValue;
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
                                otherPos > trackPos.Val) // found a car ahead closer than previous closest
                            {
                                closestPos = otherPos;
                                closestSpeed = otherSpeed.Val;
                            }
                        }
                    }

                    if (closestPos != float.MaxValue)
                    {
                        if ((closestPos - trackPos.Val) <= blockedDist.Val &&
                            speed.Val > closestSpeed) // car is blocked ahead in lane
                        {
                            ecb.AddComponent<Blocked>(ent);  // BlockedCarSys will set the speed
                            return;
                        }    
                    }

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
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}