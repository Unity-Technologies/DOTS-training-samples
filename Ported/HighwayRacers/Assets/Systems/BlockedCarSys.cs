using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace HighwayRacer
{
    public class BlockedCarSys : SystemBase
    {
        private NativeArray<OtherCar> selection; // the OtherCar segments to compare against a particular car
        
        const int nSegments = RoadInit.nSegments;
        const int nLanes = RoadInit.nLanes;
        const int initialCarsPerLaneOfSegment = RoadInit.initialCarsPerLaneOfSegment;
        const float minDist = RoadInit.minDist;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            selection = new NativeArray<OtherCar>(2, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            var trackLength = RoadInit.trackLength;
            
            var selection = this.selection;
            var otherCars = World.GetExistingSystem<CarsByLaneSegmentSys>().otherCars;
            
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

                for (int i = 0; i < 2; i++)
                {
                    var posSegment = selection[i].positions;
                    var speedSegment = selection[i].speeds;

                    var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;

                    for (int j = 0; j < posSegment.Length; j++)
                    {
                        var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
                        var otherSpeed = speedSegment[j];
                        if (targetSpeed.Val < otherSpeed.Val &&
                            otherPos > trackPos.Val)  // found car ahead
                        {
                            var dist = otherPos - trackPos.Val;
                            if (dist < blockedDist.Val)   // car is blocked ahead in lane
                            {
                                var speedDiff = speed.Val - otherSpeed.Val;
                                var closeness = (dist - minDist) / (blockedDist.Val - minDist);  // 0 is max closeness, 1 is min
                                speed.Val -= math.lerp(speedDiff, 0, closeness);       // deceleration too quick?
                                targetSpeed.Val = otherSpeed.Val;       // this target speed maintained until car is fully in next lane (or car ahead moves out of way)
                            }
                        }
                    }
                }
            }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}