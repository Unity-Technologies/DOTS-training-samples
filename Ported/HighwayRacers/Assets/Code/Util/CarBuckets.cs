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

        private NativeArray<UnsafeList.ParallelWriter> writers;
        private NativeArray<UnsafeList> lists;
        private NativeArray<int> indexMoveAllAfterToNext; // for each list, the index of first car that should be moved to next list
        private UnsafeList<Car> tempList;

        public CarBuckets(int nSegments, int nCarsPerSegment)
        {
            IsCreated = true;

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

        public void AddCar(Segment segment, TrackPos trackPos, Speed speed, Lane lane)
        {
            var writer = writers[segment.Val];

            writer.AddNoResize(new Car()
            {
                Pos = trackPos.Val,
                Speed = speed.Val,
                Lane = lane.Val,
            });
        }

        public JobHandle AdvanceCars(NativeArray<float> segmentLengths, JobHandle dependencies)
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

            // cache cars to move from last bucket and remove them from last bucket
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

            return new JobHandle();
        }

        public void moveCarsNext(UnsafeList<Car> srcBucket, int indexMoveAllAfter, UnsafeList<Car> dstBucket)
        {
            int i = 0;
            for (; i < srcBucket.Length; i++)
            {
                srcBucket[indexMoveAllAfter + i] = dstBucket[dstBucket.Length + 1];
            }

            dstBucket.Length += i + 1;
            srcBucket.Length = indexMoveAllAfter;
        }

        public void AvoidanceNormal(ref Car car, int index, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket,
            bool mergeLeftFrame, float dt)
        {
            CarUtil.GetClosestPosAndSpeed(ref car, out var distance, out var closestSpeed, index, segmentLength, bucket, nextBucket);

            if (distance <= car.BlockingDist &&
                car.Speed > closestSpeed) // car is still blocked ahead in lane
            {
                var closeness = (distance - RoadSys.minDist) / (car.BlockingDist - RoadSys.minDist); // 0 is max closeness, 1 is min

                // closer we get within minDist of leading car, the closer we match speed
                const float fudge = 2.0f;
                var newSpeed = math.lerp(closestSpeed, car.Speed + fudge, closeness);
                if (newSpeed < car.Speed)
                {
                    car.Speed = newSpeed; // todo: will this cause problem in parallel? other cars may care about this car's speed
                }

                if (car.Pos < RoadSys.mergeLookBehind)
                {
                    return;
                }

                // look for opening on left
                if (mergeLeftFrame && !car.IsInLeftmostLane())
                {
                    if (CarUtil.CanMerge(index, ref car, car.LeftOfLane(), segmentLength, bucket, nextBucket))
                    {
                        car.CarState = CarState.MergingLeftStartOvertake;
                        car.LaneOffset = 0; // todo: we're going left, so value starts at 0.0 and will inc to 1.0
                        car.MergeLeftLane();
                    }
                }
                else if (!mergeLeftFrame && !car.IsInRightmostLane()) // look for opening on right
                {
                    if (CarUtil.CanMerge(index, ref car, car.RightOfLane(), segmentLength, bucket, nextBucket))
                    {
                        car.CarState = CarState.MergingRightStartOvertake;
                        car.LaneOffset = 0; // todo: we're going right, so value starts at 0.0 and will dec to -1.0
                        car.MergeRightLane();
                    }
                }

                return;
            }


            car.SetUnblockedSpeed(dt);
        }

        // 1. make sure we don't hit next car ahead
        // 2. if blocked and can merge, trigger overtake state

        // todo: account for all car states, for now just account for Normal
        public void AvoidanceAndSetSpeed(float dt)
        {
            var segmentLengths = RoadSys.segmentLengths;
            var mergeLeftFrame = AdvanceCarsSys.mergeLeftFrame;

            var nextBucket = GetCars(0);

            for (int bucketIdx = lists.Length - 1; bucketIdx >= 0; bucketIdx--)
            {
                var bucket = GetCars(bucketIdx);
                var segmentLength = segmentLengths[bucketIdx];

                for (int i = 0; i < bucket.Length; i++)
                {
                    var car = bucket[i];

                    switch (car.CarState)
                    {
                        case CarState.Normal:
                            AvoidanceNormal(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.Overtake:
                            AvoidanceOvertake(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.MergingLeftStartOvertake:
                            AvoidanceMergingLeftStartOvertake(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.MergingLeftEndOvertake:
                            AvoidanceMergingLeftEndOvertake(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.MergingRightStartOvertake:
                            AvoidanceMergingRightStartOvertake(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.MergingRightEndOvertake:
                            AvoidanceMergingRightEndOvertake(ref car, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                    }

                    bucket[i] = car;
                }

                nextBucket = bucket;
            }
        }

        private void AvoidanceMergingLeftStartOvertake(ref Car car, int i, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, bool mergeLeftFrame, float dt)
        {
            throw new System.NotImplementedException();
        }

        private void AvoidanceOvertake(ref Car car, int i, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, bool mergeLeftFrame, float dt)
        {
            throw new System.NotImplementedException();
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

    // todo: split into SoA?
    public struct Car
    {
        public float Pos;
        public int Lane;
        public float LaneOffset;
        public float Speed;
        public float TargetSpeed;
        public float DesiredSpeed;
        public float BlockingDist;
        public CarState CarState;

        public bool IsInLeftmostLane()
        {
            throw new System.NotImplementedException();
        }

        public int LeftOfLane()
        {
            throw new System.NotImplementedException();
        }

        public void MergeLeftLane()
        {
            throw new System.NotImplementedException();
        }

        public bool IsInRightmostLane()
        {
            throw new System.NotImplementedException();
        }

        public int RightOfLane()
        {
            throw new System.NotImplementedException();
        }

        public void MergeRightLane()
        {
            throw new System.NotImplementedException();
        }

        public void SetUnblockedSpeed(float dt)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum CarState : byte
    {
        Normal,
        MergingLeftStartOvertake,
        MergingLeftEndOvertake,
        MergingRightStartOvertake,
        MergingRightEndOvertake,
        Overtake, // not overtaking
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