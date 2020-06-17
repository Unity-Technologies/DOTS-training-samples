using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace HighwayRacer
{
    public class CarsByLaneSegmentSys : SystemBase
    {
        // index is (lane * nSegment + segment). Values are the positions within that lane & segment.
        public NativeArray<OtherCars> otherCars;

        const int nSegments = RoadInit.nSegments;
        const int nLanes = RoadInit.nLanes;
        const int initialCarsPerLaneOfSegment = RoadInit.initialCarsPerLaneOfSegment;

        protected override void OnCreate()
        {
            base.OnCreate();

            otherCars = new NativeArray<OtherCars>(nLanes * nSegments, Allocator.Persistent);

            for (int i = 0; i < nLanes * nSegments; i++)
            {
                var oc = otherCars[i];
                oc.positions = new UnsafeList<TrackPos>(initialCarsPerLaneOfSegment, Allocator.Persistent);
                oc.speeds = new UnsafeList<Speed>(initialCarsPerLaneOfSegment, Allocator.Persistent);
                otherCars[i] = oc;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < nLanes * nSegments; i++)
            {
                var oc = otherCars[i];
                oc.positions.Dispose();
                oc.speeds.Dispose();
                otherCars[i] = oc;
            }

            otherCars.Dispose();
        }

        public static void addToLane(NativeArray<OtherCars> otherCars, int lane, TrackSegment trackSegment, TrackPos trackPos, Speed speed)
        {
            var oc = otherCars[lane * nSegments + trackSegment.Val];

            var positions = oc.positions;
            var speeds = oc.speeds;

            positions.Add(trackPos);
            speeds.Add(speed);

            oc.positions = positions;
            oc.speeds = speeds;

            otherCars[lane * nSegments + trackSegment.Val] = oc;
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

                var speedTemp = oc.speeds;
                speedTemp.Clear();

                oc.positions = posTemp;
                oc.speeds = speedTemp;

                otherCars[i] = oc;
            }

            // todo: we may not need speeds
            Entities.WithNone<MergingLeft, MergingRight>().ForEach(
                (Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
                {
                    addToLane(otherCars, lane.Val, trackSegment, trackPos, speed); // todo: can we ensure this gets inlined?
                }).Run();

            Entities.WithAll<MergingLeft>().ForEach((Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
            {
                addToLane(otherCars, lane.Val, trackSegment, trackPos, speed);
                addToLane(otherCars, lane.Val - 1, trackSegment, trackPos, speed); // lane to right (the old lane)  
            }).Run();

            Entities.WithAll<MergingRight>().ForEach((Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
            {
                addToLane(otherCars, lane.Val, trackSegment, trackPos, speed);
                addToLane(otherCars, lane.Val + 1, trackSegment, trackPos, speed); // lane to left (the old lane)
            }).Run();
        }
    }

    public struct OtherCars
    {
        public UnsafeList<TrackPos> positions;
        public UnsafeList<Entity> entities;
        public UnsafeList<Speed> speeds;
    }
}