using System.Collections.Generic;
using HighwayRacer;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    public unsafe struct CarBuckets
    {
        public bool IsCreated;
        private bool mergeLeftFrame; // toggles every frame: in a frame, we only initiate merges either left or right, not both

        private NativeArray<UnsafeList.ParallelWriter> writers;
        private NativeArray<UnsafeList> lists;
        private NativeArray<int> indexMoveAllAfterToNext; // for each list, the index of first car that should be moved to next list
        private UnsafeList<Car> tempList;

        public CarBuckets(int nSegments, int nCarsPerSegment)
        {
            IsCreated = true;
            mergeLeftFrame = true;

            writers = new NativeArray<UnsafeList.ParallelWriter>(nSegments, Allocator.Persistent);
            lists = new NativeArray<UnsafeList>(nSegments, Allocator.Persistent);
            indexMoveAllAfterToNext = new NativeArray<int>(nSegments, Allocator.Persistent);

            var ptr = (UnsafeList*) lists.GetUnsafePtr();

            for (int i = 0; i < nSegments; i++)
            {
                var bucket = ptr + i;
                *bucket = new UnsafeList(UnsafeUtility.SizeOf<Car>(), UnsafeUtility.AlignOf<Car>(), nCarsPerSegment, Allocator.Persistent);
                writers[i] = bucket->AsParallelWriter();
            }

            tempList = new UnsafeList<Car>(nCarsPerSegment, Allocator.Persistent);
        }

        public UnsafeList<Car> GetCars(int segment)
        {
            var bucket = lists[segment];
            return new UnsafeList<Car>((Car*) bucket.Ptr, bucket.Length);
        }

        public void AddCar(ref Car car, int bucketIdx)
        {
            var bucket = GetCars(bucketIdx);

            var lastCar = bucket[bucket.Length - 1];
            car.Lane = lastCar.Lane;
            car.Pos = lastCar.Pos;
            car.SetNextPosAndLane();

            bucket.AddNoResize(car);
        }

        public void AdvanceCars(NativeArray<float> segmentLengths)
        {
            // update car pos based on speed. Also record idx at-and-past which cars in the bucket should move to next bucket
            for (var segmentIdx = 0; segmentIdx < lists.Length; segmentIdx++)
            {
                var segmentLength = segmentLengths[segmentIdx];
                var cars = GetCars(segmentIdx);

                indexMoveAllAfterToNext[segmentIdx] = cars.Length;
                for (var i = 0; i < cars.Length; i++)
                {
                    var car = cars[i];
                    car.Pos += car.Speed;
                    if (car.Pos > segmentLength)
                    {
                        car.Pos -= segmentLength;
                        if (i < indexMoveAllAfterToNext[segmentIdx])
                        {
                            indexMoveAllAfterToNext[segmentIdx] = i;
                        }
                    }
                }
            }

            // cache cars to move from last bucket, then actually remove them from that bucket
            var lastBucket = GetCars(lists.Length - 1);
            var index = indexMoveAllAfterToNext[lists.Length - 1];
            tempList.Clear();
            moveCarsNext(lastBucket, index, tempList);

            // from each bucket, move all cars at or past index to next bucket
            var nextBucket = lastBucket;
            for (int i = lists.Length - 2; i >= 0; i--)
            {
                var bucket = GetCars(i);
                moveCarsNext(bucket, indexMoveAllAfterToNext[i], nextBucket);
                nextBucket = bucket;
            }

            // move cached cars into first bucket
            moveCarsNext(tempList, 0, nextBucket);

            Sort(); // sorts all the buckets
        }

        private void moveCarsNext(UnsafeList<Car> srcBucket, int indexMoveAllAfter, UnsafeList<Car> dstBucket)
        {
            int i = 0;
            for (; i < srcBucket.Length; i++)
            {
                srcBucket[indexMoveAllAfter + i] = dstBucket[dstBucket.Length + 1];
            }

            dstBucket.Length += i + 1;
            srcBucket.Length = indexMoveAllAfter;
        }

        // updates cars in all ways except advancing their position
        // 1. update lane offset of merging cars; cars that complete merge leave merge state
        // 2. sets speed to match target speed unless need to slow for car ahead 
        // 3. blocked cars look to merge
        // 4. overtaking cars look to merge back to the lane they came from
        public void UpdateCars(float dt)
        {
            mergeLeftFrame = !mergeLeftFrame;

            var segmentLengths = RoadSys.segmentLengths;
            var mergeSpeed = dt * RoadSys.mergeTime;

            var nextBucket = GetCars(0);
            for (int bucketIdx = lists.Length - 1; bucketIdx >= 0; bucketIdx--)
            {
                var bucket = GetCars(bucketIdx);
                var segmentLength = segmentLengths[bucketIdx];

                for (int i = 0; i < bucket.Length; i++)
                {
                    var car = bucket[i];
                    car.Merge(mergeSpeed);
                    car.Avoidance(i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                    bucket[i] = car;
                }

                nextBucket = bucket;
            }
        }

        public JobHandle Sort(JobHandle dependency)
        {
            var sortJob = new SortJob()
            {
                Lists = lists,
            };

            return sortJob.Schedule(RoadSys.nSegments, 1, dependency);
        }

        public void Sort()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];
                list.Sort<Car, CarCompare>(new CarCompare());
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
                indexMoveAllAfterToNext.Dispose();
                tempList.Dispose();
            }
        }

        // assumes bucketIdx is valid
        public bool BucketFull(int bucketIdx, NativeArray<RoadSegment> roadSegments)
        {
            var segment = roadSegments[bucketIdx];
            if (segment.IsCurved())
            {
                return true;
            }

            var bucket = GetCars(bucketIdx);

            if (bucket.Length == 0)
            {
                return true;
            }

            // true if current last car is not within 2 * carSpawnDist from end of segment
            var lastCar = bucket[bucket.Length - 1];
            return (lastCar.Pos < (segment.Length - RoadSys.carSpawnDist - RoadSys.carSpawnDist));
        }

        public bool IsBucketIdx(int bucketIdx)
        {
            return bucketIdx >= 0 && bucketIdx < lists.Length;
        }
    }

    public struct CarCompare : IComparer<Car>
    {
        public int Compare(Car x, Car y)
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
            else
            {
                return 1;
            }
        }
    }

    public enum CarState : byte
    {
        Normal,
        OvertakingLeft, // looking to merge right after timer
        OvertakingLeftStart,
        OvertakingLeftEnd,
        OvertakingRight, // looking to merge left after timer
        OvertakingRightStart,
        OvertakingRightEnd,
    }


    public struct CarEnumerator
    {
        private CarBuckets buckets;
        private NativeArray<RoadSegment> segments;

        private UnsafeList<Car> bucket;

        private int carIdx;
        private int bucketIdx;

        public CarEnumerator(CarBuckets buckets, NativeArray<RoadSegment> segments, out RoadSegment segment)
        {
            this.buckets = buckets;
            this.segments = segments;

            bucket = new UnsafeList<Car>();
            segment = segments[0];

            carIdx = 0;
            bucketIdx = 0;
        }

        public void Next(out Car car, ref RoadSegment segment)
        {
            while (carIdx >= bucket.Length)
            {
                carIdx = 0;
                bucketIdx++;

                bucket = buckets.GetCars(bucketIdx);
                segment = segments[bucketIdx];
            }

            car = bucket[carIdx];
            carIdx++;
        }
    }

    public struct SortJob : IJobParallelFor
    {
        public NativeArray<UnsafeList> Lists;

        public void Execute(int index)
        {
            var list = Lists[index];
            list.Sort<Car, CarCompare>(new CarCompare());
        }

        // copied from 
        unsafe static void InsertionSort<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
        {
            int i, j;
            T t;
            for (i = lo; i < hi; i++)
            {
                j = i;
                t = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
                while (j >= lo && comp.Compare(t, UnsafeUtility.ReadArrayElement<T>(array, j)) < 0)
                {
                    UnsafeUtility.WriteArrayElement<T>(array, j + 1, UnsafeUtility.ReadArrayElement<T>(array, j));
                    j--;
                }

                UnsafeUtility.WriteArrayElement<T>(array, j + 1, t);
            }
        }
    }
}