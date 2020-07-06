using System.Collections.Generic;
using HighwayRacer;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace HighwayRacer
{
    public unsafe struct CarBuckets
    {
        public bool IsCreated;

        private NativeArray<UnsafeList.ParallelWriter> writers;
        private NativeArray<UnsafeList> lists;

        public CarBuckets(int nSegments, int nCarsPerSegment)
        {
            IsCreated = true;

            writers = new NativeArray<UnsafeList.ParallelWriter>(nSegments, Allocator.Persistent);
            lists = new NativeArray<UnsafeList>(nSegments, Allocator.Persistent);

            var ptr = (UnsafeList*) lists.GetUnsafePtr();

            for (int i = 0; i < nSegments; i++)
            {
                var bucket = ptr + i;
                *bucket = new UnsafeList(UnsafeUtility.SizeOf<SortedCar>(), UnsafeUtility.AlignOf<SortedCar>(), nCarsPerSegment, Allocator.Persistent);
                writers[i] = bucket->AsParallelWriter();
            }
        }

        public UnsafeList<SortedCar> GetCars(int segment)
        {
            var bucket = lists[segment];
            return new UnsafeList<SortedCar>((SortedCar*) bucket.Ptr, bucket.Length);
        }

        public void AddCar(TrackSegment trackSegment, TrackPos trackPos, Speed speed, Lane lane)
        {
            var writer = writers[trackSegment.Val];

            writer.AddNoResize(new SortedCar()
            {
                Pos = trackPos.Val,
                Speed = speed.Val,
                Lane = lane.Val,
            });
        }

        public void Clear()
        {
            // clear all the lists
            // (has to be done through writers to properly modify the Length values as stored in each UnsafeList
            for (int i = 0; i < writers.Length; i++)
            {
                var writer = writers[i];
                writer.ListData->Clear();
            }
        }

        // todo: sort in parallel
        public void Sort()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];
                list.Sort<SortedCar, CarCompare>(new CarCompare());
            }
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                for (int i = 0; i < lists.Length; i++)
                {
                    lists[i].Dispose();
                }

                writers.Dispose();
                lists.Dispose();
            }
        }
    }

    public struct CarCompare : IComparer<SortedCar>
    {
        public int Compare(SortedCar x, SortedCar y)
        {
            if (x.Pos < y.Pos)
            {
                return -1;
            }
            else if (x.Pos > y.Pos)
            {
                return 1;
            }

            // lane is tie breaker
            if (x.Lane < y.Lane)
            {
                return -1;
            }
            else if (x.Lane == y.Lane)
            {
                return 0;
            }
            else // no two cars with equal Pos should have equal Lane, so we can assume now that (x.Lane > y.Lane)
            {
                return 1;
            }
        }
    }

    public struct SortedCar
    {
        public float Pos;
        public int Lane;
        public float Speed;
    }
}