using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HighwayRacer;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor.Build;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace HighwayRacer
{
    public unsafe struct CarBuckets
    {
        public bool IsCreated;

        [NativeDisableContainerSafetyRestriction] private NativeArray<UnsafeList.ParallelWriter> writers;
        
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction] 
        private NativeArray<UnsafeList> lists;

        [NativeDisableContainerSafetyRestriction] public NativeArray<int> moveIndexes; // for each list, the index of first car that should be moved to next list
        [NativeDisableContainerSafetyRestriction] private UnsafeList<Car> tempList;

        private int largestBucketLength;
        private int bucketSize;

        public CarBuckets(int nSegments, int nCarsPerSegment)
        {
            IsCreated = true;

            bucketSize = (int)(nCarsPerSegment * 1.6f);   // todo compute exactly how many cars fit in a bucket. This fudge seems to work though, but might be a bit wasteful
            largestBucketLength = 0;
            Debug.Log("bucket capacity: " + bucketSize + "  number of buckets: " + nSegments);

            writers = new NativeArray<UnsafeList.ParallelWriter>(nSegments, Allocator.Persistent);
            lists = new NativeArray<UnsafeList>(nSegments, Allocator.Persistent);
            moveIndexes = new NativeArray<int>(nSegments, Allocator.Persistent);

            var ptr = (UnsafeList*) lists.GetUnsafePtr();

            for (int i = 0; i < nSegments; i++)
            {
                var bucket = ptr + i;
                *bucket = new UnsafeList(UnsafeUtility.SizeOf<Car>(), UnsafeUtility.AlignOf<Car>(), bucketSize, Allocator.Persistent);
                writers[i] = bucket->AsParallelWriter();
            }

            tempList = new UnsafeList<Car>(bucketSize, Allocator.Persistent);
        }

        public int LastIndex()
        {
            return lists.Length - 1;
        }

        public UnsafeList<Car> GetCars(int segmentIdx)
        {
            var bucket = lists[segmentIdx];
            var list = new UnsafeList<Car>((Car*) bucket.Ptr, bucket.Length);
            list.Capacity = bucket.Capacity;
            return list;
        }

        public UnsafeList.ParallelWriter GetWriter(int segmentIdx)
        {
            return writers[segmentIdx];
        }
        
        // todo temp for debugging
        public void CheckSorting()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var prevPos = float.MinValue;
                var bucket = GetCars(i);
                
                for (int carIdx = 0; carIdx < bucket.Length; carIdx++)
                {
                    var car = bucket[carIdx];
                    if (car.Pos < prevPos) 
                    {
                        Debug.Log("car.Pos is lesser than previous car");
                    }
                    prevPos = car.Pos;
                }
            }
        }
        
        public void LargestBucketLength()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var len = lists[i].Length;
                if (len > largestBucketLength)
                {
                    largestBucketLength = len;
                    Debug.Log("new record bucket size: " + largestBucketLength + "  Capacity: " + bucketSize);
                }
            }
        }
        
        // todo temp for debugging
        public void CheckPosAreFinite()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var bucket = GetCars(i);
                for (int carIdx = 0; carIdx < bucket.Length; carIdx++)
                {
                    var car = bucket[carIdx];
                    if (!math.isfinite(car.Pos))
                    {
                        Debug.Log("car.Pos is non-finite");
                    }
                }
            }
        }
        
        // 1. set new pos given speed
        // 2. moves cars between buckets when they move past end
        public void AdvanceCarsJob(NativeArray<float> segmentLengths, float dt, JobHandle dependency)
        {
            // todo experiment with these batch sizes
            const int advanceBatchCount = 8;
            const int sortBatchCount = 4;

            // update car pos based on speed. Also record idx at-and-past which cars in the bucket should move to next bucket
            var advancePosJob = new AdvanceCarPosJob()
            {
                dt = dt,
                segmentLengths = segmentLengths,
                carBuckets = this,
            };

            var advancePosHandle = advancePosJob.Schedule(lists.Length, advanceBatchCount, dependency);
            advancePosHandle.Complete();

            // cache cars to move from last bucket, then actually remove them from that bucket
            pushCarsToCache(moveIndexes[lists.Length - 1], lists.Length - 1);

            // from each bucket, move all cars at or past index to next bucket
            for (int bucketIdx = lists.Length - 2; bucketIdx >= 0; bucketIdx--)
            {
                moveCarsNext(moveIndexes[bucketIdx], bucketIdx, bucketIdx + 1);
            }

            // append cached cars to end of first bucket 
            popCarsFromCache(0);
            
            var sortJob = new SortJob()
            {
                carBuckets = this
            };
            var sortJobHandle = sortJob.Schedule(lists.Length, sortBatchCount);
            sortJobHandle.Complete();
        }

        private void moveCarsNext(int moveIdx, int srcBucketIdx, int dstBucketIdx)
        {
            if (moveIdx == -1)
            {
                return;
            }

            var ptr = (UnsafeList*) lists.GetUnsafePtr();
            var srcBucketPtr = ptr + srcBucketIdx;
            var srcBucket = GetCars(srcBucketIdx);

            var dstBucketPtr = ptr + dstBucketIdx;
            var dstBucket = GetCars(dstBucketIdx);

            var count = srcBucket.Length - moveIdx;
            for (int i = 0; i < count; i++)
            {
                dstBucket[dstBucket.Length + i] = srcBucket[moveIdx + i];
            }

            dstBucketPtr->Length += count;
            srcBucketPtr->Length = moveIdx;
        }


        private void pushCarsToCache(int moveIdx, int srcBucketIdx)
        {
            tempList.Clear();

            if (moveIdx == -1)
            {
                return;
            }

            var ptr = (UnsafeList*) lists.GetUnsafePtr();
            var srcBucketPtr = ptr + srcBucketIdx;
            var srcBucket = GetCars(srcBucketIdx);

            var count = srcBucket.Length - moveIdx;
            for (int i = 0; i < count; i++)
            {
                tempList[i] = srcBucket[moveIdx + i];
            }

            tempList.Length = count;
            srcBucketPtr->Length = moveIdx;
        }

        private void popCarsFromCache(int dstBucketIdx)
        {
            var ptr = (UnsafeList*) lists.GetUnsafePtr();
            var dstBucketPtr = ptr + dstBucketIdx;
            var dstBucket = GetCars(dstBucketIdx);

            for (int i = 0; i < tempList.Length; i++)
            {
                dstBucket[dstBucket.Length + i] = tempList[i];
            }

            dstBucketPtr->Length += tempList.Length;
        }

        private void UpdateCarsBucket(UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, float dt, float mergeSpeed, float segmentLength, bool mergeLeftFrame)
        {
            for (int i = 0; i < bucket.Length; i++)
            {
                var car = bucket[i];
                car.MergingMove(mergeSpeed);
                car.Avoidance(i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                bucket[i] = car;
            }
        }

        public JobHandle UpdateCarsJob(float dt, bool mergeLeftFrame, JobHandle dependency)
        {
            var segmentLengths = RoadSys.segmentLengths;
            var mergeSpeed = dt * RoadSys.mergeTime;

            Assert.IsTrue(lists.Length % 2 == 0); // should always be true as long as our track is a square loop

            const int batchSize = 2; // todo experiment with batch size

            var evens = new UpdateCarsJob()
            {
                dt = dt,
                segmentLengths = segmentLengths,
                mergeSpeed = mergeSpeed,
                mergeLeftFrame = mergeLeftFrame,
                carBuckets = this,
                doOdds = false,
            };
            var evensHandle = evens.Schedule(lists.Length, batchSize, dependency);

            var odds = new UpdateCarsJob()
            {
                dt = dt,
                segmentLengths = segmentLengths,
                mergeSpeed = mergeSpeed,
                mergeLeftFrame = mergeLeftFrame,
                carBuckets = this,
                doOdds = true,
            };
            return odds.Schedule(lists.Length, batchSize, evensHandle);
        }

        public void Sort()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                var list = lists[i];
                InsertionSort<Car, CarCompare>(list.Ptr, 0, list.Length - 1, new CarCompare());
            }
        }

        // copied from Collections
        public void InsertionSort<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
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
                moveIndexes.Dispose();
                tempList.Dispose();
            }
        }

        public bool IsBucketIdx(int bucketIdx)
        {
            return bucketIdx >= 0 && bucketIdx < lists.Length;
        }

        // throws if exhausts buckets
        public void NextNthCar(ref int bucketIdx, ref int carIdx, int nCars)
        {
            // special check for first bucket
            var firstBucket = GetCars(bucketIdx);
            if ((carIdx + nCars) < firstBucket.Length)
            {
                carIdx += nCars;
                return;
            }
            nCars -= firstBucket.Length - carIdx;
            bucketIdx++;

            // all remaining buckets
            for (;; bucketIdx++)
            {
                var bucket = GetCars(bucketIdx);
                
                if (nCars < bucket.Length)
                {
                    carIdx = nCars;
                    return;
                }

                nCars -= bucket.Length;
            }
        }

        public void FirstNonEmptyBucket(out int bucketIdx)
        {
            for (bucketIdx = 0; bucketIdx < lists.Length; bucketIdx++)
            {
                var bucket = GetCars(bucketIdx);
                if (bucket.Length > 0)
                {
                    return;
                }
            }
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

            return 0;
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

    public unsafe struct SortJob : IJobParallelFor
    {
        public CarBuckets carBuckets;

        public void Execute(int idx)
        {
            var bucket = carBuckets.GetCars(idx);
            carBuckets.InsertionSort<Car, CarCompare>(bucket.Ptr, 0, bucket.Length - 1, new CarCompare());
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct UpdateCarsJob : IJobParallelFor
    {
        public float dt;

        public NativeArray<float> segmentLengths;
        public float mergeSpeed;
        public bool mergeLeftFrame;

        public CarBuckets carBuckets;
        public bool doOdds; // used for controlling whether we do odds or evens

        public void Execute(int idx)
        {
            var even = idx % 2 == 0;
            if ((doOdds && even) ||
                (!doOdds && !even))
            {
                return;
            }

            var segmentLength = segmentLengths[idx];
            var bucket = carBuckets.GetCars(idx);
            var nextIdx = (idx == carBuckets.LastIndex()) ? 0 : idx + 1;
            var nextBucket = carBuckets.GetCars(nextIdx);

            for (int i = 0; i < bucket.Length; i++)
            {
                var car = bucket[i];
                car.MergingMove(mergeSpeed);
                car.Avoidance(i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                bucket[i] = car;
            }
        }
    }


    [BurstCompile(CompileSynchronously = true)]
    public struct AdvanceCarPosJob : IJobParallelFor
    {
        public float dt;
        public NativeArray<float> segmentLengths;
        public CarBuckets carBuckets;

        public void Execute(int idx)
        {
            // update car pos based on speed. Also record idx at-and-past which cars in the bucket should move to next bucket
            var segmentLength = segmentLengths[idx];
            var bucket = carBuckets.GetCars(idx);

            var moveIdx = bucket.Length;
            for (var i = 0; i < bucket.Length; i++)
            {
                var car = bucket[i];
                
                car.Pos += car.Speed * dt;
                if (car.Pos > segmentLength)
                {
                    car.Pos -= segmentLength;
                    if (i < moveIdx)
                    {
                        moveIdx = i;
                    }
                }

                bucket[i] = car;
            }

            carBuckets.moveIndexes[idx] = (moveIdx == bucket.Length) ? -1 : moveIdx;
        }
    }
}