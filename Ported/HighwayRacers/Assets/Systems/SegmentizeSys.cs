using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(CarSpawnSys))]
    public class SegmentizeSys : SystemBase
    {
        public CarBuckets CarBuckets;
        private int nCars;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            CarBuckets.Dispose();
            base.OnDestroy();
        }

        public static bool mergeLeftFrame = true; // toggles every frame: in a frame, we only initiate merges either left or right, not both

        protected override void OnUpdate()
        {
            if (nCars != RoadSys.numCars)
            {
                CarBuckets.Dispose();
                CarBuckets = new CarBuckets(RoadSys.numCars);
                nCars = RoadSys.numCars;
            }

            CarBuckets.Clear();
            var carBuckets = this.CarBuckets;

            mergeLeftFrame = !mergeLeftFrame;
            var thresholds = RoadSys.thresholds;
            var lastIdx = thresholds.Length - 1;

            // todo:
            // note first index of first car for each
            // segment(-1 if segment has no cars)

            // var jobA = Entities.ForEach(
            //     (ref TrackSegment segment, in TrackPos pos, in Speed speed, in Lane lane) =>
            //     {
            //         carBuckets.AddCar(segment, pos, speed, lane);
            //
            //         for (byte i = 0; i < lastIdx; i++)
            //         {
            //             if (pos.Val < thresholds[i]) // todo: binary search (if num thresholds is large, might make sense)
            //             {
            //                 segment.Val = i;
            //                 return;
            //             }
            //         }
            //
            //         segment.Val = (byte) lastIdx; // last segment gets all the rest (to account for float imprecision)
            //     }).ScheduleParallel(Dependency);
            //
            // jobA.Complete();
            // //sortedCars.Sort();
            // Dependency = jobA;


            // todo: this can't be correct way to disable checks for parallel writes, can it?
            var jobA = Entities.WithReadOnly(carBuckets).WithNone<MergingLeft, MergingRight>().ForEach(
                (ref TrackSegment trackSegment, in TrackPos trackPos, in Speed speed, in Lane lane) =>
                {
                    trackSegment.Val = (byte) lastIdx; // last segment gets all the rest (to account for float imprecision)
                    for (byte i = 0; i < lastIdx; i++)
                    {
                        if (trackPos.Val < thresholds[i]) // todo: binary search (if num thresholds is large, might make sense)
                        {
                            trackSegment.Val = i;
                            break;
                        }
                    }

                    carBuckets.AddCar(trackSegment, trackPos, speed, lane); // todo: can we ensure this gets inlined?
                }).ScheduleParallel(Dependency);

            var jobB = Entities.WithReadOnly(carBuckets).WithAll<MergingLeft>().ForEach(
                (ref TrackSegment trackSegment, in TrackPos trackPos, in Speed speed, in Lane lane) =>
                {
                    trackSegment.Val = (byte) lastIdx; // last segment gets all the rest (to account for float imprecision)
                    for (byte i = 0; i < lastIdx; i++)
                    {
                        if (trackPos.Val < thresholds[i]) // todo: binary search (if num thresholds is large, might make sense)
                        {
                            trackSegment.Val = i;
                            break;
                        }
                    }
                    
                    carBuckets.AddCar(trackSegment, trackPos, speed, lane);
                    carBuckets.AddCar(new TrackSegment() {Val = (ushort) (trackSegment.Val - 1)}, trackPos, speed, lane);
                }).ScheduleParallel(Dependency);

            var jobC = Entities.WithReadOnly(carBuckets).WithAll<MergingRight>().ForEach(
                (ref TrackSegment trackSegment, in TrackPos trackPos, in Speed speed, in Lane lane) =>
                {
                    trackSegment.Val = (byte) lastIdx; // last segment gets all the rest (to account for float imprecision)
                    for (byte i = 0; i < lastIdx; i++)
                    {
                        if (trackPos.Val < thresholds[i]) // todo: binary search (if num thresholds is large, might make sense)
                        {
                            trackSegment.Val = i;
                            break;
                        }
                    }

                    carBuckets.AddCar(trackSegment, trackPos, speed, lane);
                    carBuckets.AddCar(new TrackSegment() {Val = (ushort) (trackSegment.Val + 1)}, trackPos, speed, lane);
                }).ScheduleParallel(Dependency);

            var combined = JobHandle.CombineDependencies(jobA, jobB, jobC);
            combined.Complete(); // todo: ideally, later systems would know to use this as dependency if they read carBuckets, but we have carBuckets erroneously marked read only
            
            carBuckets.Sort();
            Dependency = combined;
        }
    }
}