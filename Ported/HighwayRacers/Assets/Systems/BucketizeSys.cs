using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(SegmentizeSys))]
    public class BucketizeSys : SystemBase
    {
        private int nSegments;

        public CarBuckets CarBuckets;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CarBuckets.Dispose();
        }

        private int nCars;

        protected override void OnUpdate()
        {
            var nSegments = RoadSys.nSegments;
            if (nSegments != this.nSegments)
            {
                if (this.CarBuckets.IsCreated)
                {
                    this.CarBuckets.Dispose();
                }

                this.CarBuckets = new CarBuckets(nSegments);
                this.nSegments = nSegments;
            }

            CarBuckets.Clear();

            var carBuckets = this.CarBuckets;

            // todo: this can't be correct way to disable checks for parallel writes, can it?
            var jobA = Entities.WithReadOnly(carBuckets).WithNone<MergingLeft, MergingRight>().ForEach(
                (Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
                {
                    carBuckets.AddCar(lane.Val, trackSegment, trackPos, speed, nSegments); // todo: can we ensure this gets inlined?
                }).ScheduleParallel(Dependency);

            var jobB = Entities.WithReadOnly(carBuckets).WithAll<MergingLeft>().ForEach(
                (Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
                {
                    carBuckets.AddCar(lane.Val, trackSegment, trackPos, speed, nSegments);
                    carBuckets.AddCar(lane.Val - 1, trackSegment, trackPos, speed, nSegments); // lane to right (the old lane)
                }).ScheduleParallel(Dependency);

            var jobC = Entities.WithReadOnly(carBuckets).WithAll<MergingRight>().ForEach(
                (Entity ent, in TrackPos trackPos, in Speed speed, in Lane lane, in TrackSegment trackSegment) =>
                {
                    carBuckets.AddCar(lane.Val, trackSegment, trackPos, speed, nSegments);
                    carBuckets.AddCar(lane.Val + 1, trackSegment, trackPos, speed, nSegments); // lane to left (the old lane)
                }).ScheduleParallel(Dependency);
            
            var combined = JobHandle.CombineDependencies(jobA, jobB, jobC);
            combined.Complete();  // todo: ideally, later systems would know to use this as dependency if they read carBuckets, but we have carBuckets erroneously marked read only

            Dependency = combined;
        }
    }
}