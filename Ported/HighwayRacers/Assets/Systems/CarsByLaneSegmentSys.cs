using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace HighwayRacer
{
    public class CarsByLaneSegmentSys : SystemBase
    {
        // index is (lane * nSegment + segment). Values are the positions within that lane & segment.
        public NativeArray<OtherCar> otherCars;
        
        const int nSegments = RoadInit.nSegments;
        const int nLanes = RoadInit.nLanes;
        const int initialCarsPerLaneOfSegment = RoadInit.initialCarsPerLaneOfSegment;
        const float minDist = RoadInit.minDist;

        protected override void OnCreate()
        {
            base.OnCreate();

            otherCars = new NativeArray<OtherCar>(nLanes * nSegments, Allocator.Persistent);

            for (int i = 0; i < nLanes * nSegments; i++)
            {
                var oc = otherCars[i];
                oc.positions = new UnsafeList<TrackPos>(initialCarsPerLaneOfSegment, Allocator.Persistent);
                oc.entities = new UnsafeList<Entity>(initialCarsPerLaneOfSegment, Allocator.Persistent);
                oc.speeds = new UnsafeList<Speed>(initialCarsPerLaneOfSegment, Allocator.Persistent);
                otherCars[i] = oc;
            }
        }
        
        protected override void OnUpdate()
        {
            var otherCars = this.otherCars;
            
            // clear all the lists
            for (int i = 0; i < nLanes * nSegments; i++)
            {
                var oc = otherCars[i];

                var posTemp = oc.positions;
                posTemp.Clear();

                var entTemp = oc.entities;
                entTemp.Clear();

                var speedTemp = oc.speeds;
                speedTemp.Clear();

                oc.positions = posTemp;
                oc.entities = entTemp;
                oc.speeds = speedTemp;

                otherCars[i] = oc;
            }

            Entities.ForEach((Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
            {
                var oc = otherCars[lane.Val * nSegments + trackSegment.Val];

                var positions = oc.positions;
                var entities = oc.entities;
                var speeds = oc.speeds;

                positions.Add(trackPos);
                entities.Add(ent);
                speeds.Add(speed);

                oc.positions = positions;
                oc.entities = entities;
                oc.speeds = speeds;

                otherCars[lane.Val * nSegments + trackSegment.Val] = oc;
            }).Run();
        }
    }
}