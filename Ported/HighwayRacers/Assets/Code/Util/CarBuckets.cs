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

        public void AddCar(ref Car car, int bucketIdx)
        {
            var bucket = GetCars(bucketIdx);
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

            if (distance <= car.BlockingDist && car.Speed > closestSpeed) // car is still blocked ahead in lane
            {
                car.SlowForBlock(distance, closestSpeed);

                if (car.Pos < RoadSys.mergeLookBehind)
                {
                    return;
                }

                // look for opening on left
                if (mergeLeftFrame && !car.IsInLeftmostLane())
                {
                    if (CarUtil.CanMerge(index, ref car, car.LeftOfLane(), segmentLength, bucket, nextBucket))
                    {
                        car.CarState = CarState.OvertakingLeftStart;
                        car.LaneOffset = 0; // todo: we're going left, so value starts at 0.0 and will inc to 1.0
                        car.MergeLeftLane();
                    }
                }
                else if (!mergeLeftFrame && !car.IsInRightmostLane()) // look for opening on right
                {
                    if (CarUtil.CanMerge(index, ref car, car.RightOfLane(), segmentLength, bucket, nextBucket))
                    {
                        car.CarState = CarState.OvertakingLeftEnd;
                        car.LaneOffset = 0; // todo: we're going right, so value starts at 0.0 and will dec to -1.0
                        car.MergeRightLane();
                    }
                }

                return;
            }

            car.SetSpeed(dt, car.DesiredSpeedUnblocked);
        }

        // 1. update LaneOffset of all merging cars
        // 2. cars that complete merge change Lane and CarState
        public void Merge(float dt)
        {
            var mergeSpeed = RoadSys.mergeTime * dt;

            for (int bucketIdx = lists.Length - 1; bucketIdx >= 0; bucketIdx--)
            {
                var bucket = GetCars(bucketIdx);

                for (int i = 0; i < bucket.Length; i++)
                {
                    var car = bucket[i];

                    switch (car.CarState)
                    {
                        case CarState.Normal:
                            continue;
                        case CarState.OvertakingLeft:
                            continue;
                        case CarState.OvertakingRight:
                            continue;
                        case CarState.OvertakingLeftStart:
                            car.LaneOffset += mergeSpeed;
                            if (car.LaneOffset > 1.0f)
                            {
                                car.CompleteRightMerge();
                            }

                            car.CarState = CarState.OvertakingLeft;
                            break;
                        case CarState.OvertakingRightEnd:
                            car.LaneOffset += mergeSpeed;
                            if (car.LaneOffset > 1.0f)
                            {
                                car.CompleteRightMerge();
                            }

                            car.CarState = CarState.Normal;
                            break;
                        case CarState.OvertakingRightStart:
                            car.LaneOffset -= mergeSpeed;
                            if (car.LaneOffset < -1.0f)
                            {
                                car.CompleteLeftMerge();
                            }

                            car.CarState = CarState.OvertakingRight;
                            break;
                        case CarState.OvertakingLeftEnd:
                            car.LaneOffset -= mergeSpeed;
                            if (car.LaneOffset < -1.0f)
                            {
                                car.CompleteLeftMerge();
                            }

                            car.CarState = CarState.Normal;
                            break;
                    }

                    bucket[i] = car;
                }
            }
        }

        // 1. make sure we don't hit next car ahead
        // 2. triggers most state changes (merging / overtaking)
        // 3. sets speed
        public void Avoidance(float dt)
        {
            var segmentLengths = RoadSys.segmentLengths;
            var mergeLeftFrame = AvoidanceAndSpeedSys.mergeLeftFrame;

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
                        case CarState.OvertakingLeft:
                            AvoidanceOvertaking(ref car, true, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.OvertakingRight:
                            AvoidanceOvertaking(ref car, false, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.OvertakingLeftStart:
                            AvoidanceOvertakingStart(ref car, true, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.OvertakingRightStart:
                            AvoidanceOvertakingStart(ref car, false, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.OvertakingLeftEnd:
                            AvoidanceOvertakingEnd(ref car, true, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                        case CarState.OvertakingRightEnd:
                            AvoidanceOvertakingEnd(ref car, false, i, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                            break;
                    }

                    bucket[i] = car;
                }

                nextBucket = bucket;
            }
        }

        private void AvoidanceOvertakingEnd(ref Car car, bool overtakeLeft, int index, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, bool mergeLeftFrame, float dt)
        {
            CarUtil.GetClosestPosAndSpeed(ref car, out var distance, out var closestSpeed, index, segmentLength, bucket, nextBucket);

            if (distance <= car.BlockingDist &&
                car.Speed > closestSpeed) // car is blocked ahead in lane
            {
                car.SlowForBlock(distance, closestSpeed);
                return;
            }

            car.SetSpeed(dt, car.DesiredSpeedOvertake);
        }

        private void AvoidanceOvertakingStart(ref Car car, bool overtakeLeft, int index, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, bool mergeLeftFrame, float dt)
        {
            car.OvertakeTimer -= dt;

            CarUtil.GetClosestPosAndSpeed(ref car, out var distance, out var closestSpeed, index, segmentLength, bucket, nextBucket);

            if (distance <= car.BlockingDist &&
                car.Speed > closestSpeed) // car is blocked ahead in lane
            {
                car.SlowForBlock(distance, closestSpeed);
                return;
            }

            car.SetSpeed(dt, car.DesiredSpeedOvertake);
        }

        private void AvoidanceOvertaking(ref Car car, bool overtakeLeft, int index, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket,
            bool mergeLeftFrame, float dt)
        {
            car.OvertakeTimer -= dt;

            CarUtil.GetClosestPosAndSpeed(ref car, out var distance, out var closestSpeed, index, segmentLength, bucket, nextBucket);

            // if blocked, leave OvertakingLeft state
            if (distance <= car.BlockingDist && car.Speed > closestSpeed)
            {
                car.SlowForBlock(distance, closestSpeed);
                car.CarState = CarState.Normal;
                return;
            }

            // merging timed out, so end overtake
            if (car.OvertakeTimer <= 0)
            {
                car.CarState = CarState.Normal;
                return;
            }

            // try merge
            var tryMerge = (overtakeLeft && !mergeLeftFrame) || (!overtakeLeft && mergeLeftFrame);
            int destLane = overtakeLeft ? car.RightOfLane() : car.LeftOfLane();

            if (car.OvertakeTimer < RoadSys.overtakeTimeTryMerge && tryMerge)
            {
                if (CarUtil.CanMerge(index, ref car, destLane, segmentLength, bucket, nextBucket))
                {
                    car.LaneOffset = 0;
                    if (overtakeLeft)
                    {
                        car.CarState = CarState.OvertakingLeftEnd;
                        car.MergeRightLane();
                    }
                    else
                    {
                        car.CarState = CarState.OvertakingRightEnd;
                        car.MergeLeftLane();
                    }

                    return;
                }
            }

            car.SetSpeed(dt, car.DesiredSpeedOvertake);
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

        public bool IsBucket(int bucketIdx)
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

    // todo: split into SoA?
    public struct Car
    {
        public float Pos;
        public byte Lane;
        public float Speed;
        public float DesiredSpeedUnblocked;
        public float DesiredSpeedOvertake;
        public CarState CarState;

        // todo: these fields could be in an Entity referenced from each Car (but that saves just 8 bytes per Car?)
        public float LaneOffset;
        public float BlockingDist;
        public float OvertakeTimer;

        public bool IsInLeftmostLane()
        {
            return (8 & Lane) != 0;
        }


        public bool IsInRightmostLane()
        {
            return (1 & Lane) != 0;
        }

        public int LeftOfLane()
        {
            switch (Lane)
            {
                case 1: // 0001
                    return 2; // 0010
                case 2: // 0010
                    return 4; // 0100
                case 4: // 0100
                    return 8; // 1000
                default:
                    Debug.LogError("Invalid: no lane to left.");
                    return 8;
            }
        }

        public int RightOfLane()
        {
            switch (Lane)
            {
                case 2: // 0010
                    return 1; // 0001
                case 4: // 0100
                    return 2; // 0010
                case 8: // 1000
                    return 4; // 0100
                default:
                    Debug.LogError("Invalid: no lane to right.");
                    return 1;
            }
        }

        public void MergeLeftLane()
        {
            switch (Lane)
            {
                case 1: // 0001
                    Lane = 3; // 0011 
                    break;
                case 2: // 0010
                    Lane = 6; // 0110
                    break;
                case 4: // 0100
                    Lane = 12; // 1100
                    break;
                default:
                    Debug.LogError("Invalid merge left.");
                    break;
            }
        }


        public void MergeRightLane()
        {
            switch (Lane)
            {
                case 2: // 0010
                    Lane = 3; // 0011 
                    break;
                case 4: // 0100
                    Lane = 6; // 0110
                    break;
                case 8: // 1000
                    Lane = 12; // 1100
                    break;
                default:
                    Debug.LogError("Invalid merge right.");
                    break;
            }
        }

        public void SetSpeed(float dt, float targetSpeed)
        {
            if (targetSpeed < Speed)
            {
                var s = Speed - RoadSys.decelerationRate * dt;
                Speed = (s < targetSpeed) ? targetSpeed : s;
            }
            else if (targetSpeed > Speed)
            {
                var s = Speed + RoadSys.accelerationRate * dt;
                Speed = (s > targetSpeed) ? targetSpeed : s;
            }
        }

        public void SlowForBlock(float blockingCarDist, float blockingCarSpeed)
        {
            var closeness = (blockingCarDist - RoadSys.minDist) / (BlockingDist - RoadSys.minDist); // 0 is max closeness, 1 is min

            // closer we get within minDist of leading car, the closer we match speed
            const float fudge = 2.0f;
            var newSpeed = math.lerp(blockingCarSpeed, Speed + fudge, closeness);
            if (newSpeed < Speed)
            {
                Speed = newSpeed;
            }
        }

        // return true if both lane values overlap
        public bool IsOccupyingLane(int otherLane)
        {
            return (Lane & otherLane) != 0;
        }

        public void CompleteLeftMerge()
        {
            switch (Lane)
            {
                case 3: // 0011
                    Lane = 2; // 0010 
                    break;
                case 6: // 0110
                    Lane = 4; // 0100
                    break;
                case 12: // 1100
                    Lane = 8; // 1000
                    break;
                default:
                    Debug.LogError("Invalid complete left right.");
                    break;
            }
        }

        public void CompleteRightMerge()
        {
            switch (Lane)
            {
                case 3: // 0011
                    Lane = 1; // 0001 
                    break;
                case 6: // 0110
                    Lane = 2; // 0010
                    break;
                case 12: // 1100
                    Lane = 4; // 0100
                    break;
                default:
                    Debug.LogError("Invalid complete merge right.");
                    break;
            }
        }

        public float LaneOffsetDist()
        {
            throw new System.NotImplementedException();
        }

        public void SetLaneByIdx(byte currentLane)
        {
            switch (Lane)
            {
                case 0: 
                    Lane = 1; // 0001 
                    break;
                case 1: 
                    Lane = 2; // 0010
                    break;
                case 2: 
                    Lane = 4; // 0100
                    break;
                case 3: 
                    Lane = 8; // 1000
                    break;
                default:
                    Debug.LogError("Invalid complete merge right.");
                    break;
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