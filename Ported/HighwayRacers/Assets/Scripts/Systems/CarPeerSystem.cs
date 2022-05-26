using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[UpdateAfter(typeof(CarSpawningSystem))]
[BurstCompile]
partial struct CarPeerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TrackConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public struct CarGroupingBucket
    {
        public int firstIndex;
        public int carCount;
    }

    public struct CarDistanceInfo
    {
        public Entity Car;
        public float Distance;
        public float WrappedDistance;
        public float CurrentSpeed;
    }

    public struct CarLaneInfo
    {
        public float LaneLength;
        public int CarCount;
        public NativeArray<CarDistanceInfo> CarDistances;
        public NativeArray<CarGroupingBucket> Buckets;
    }

    struct CarDistanceComparer : IComparer<CarDistanceInfo>
    {
        public int Compare(CarDistanceInfo a, CarDistanceInfo b)
        {
            if (a.Car == Entity.Null || b.Car == Entity.Null)
            {
                if (a.Car != b.Car)
                {
                    return a.Car != Entity.Null ? -1 : 1;
                }
                return 0;
            }
            return a.WrappedDistance.CompareTo(b.WrappedDistance);
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TrackConfig track = SystemAPI.GetSingleton<TrackConfig>();

        //NativeArray<CarAspect> cars = CollectionHelper.CreateNativeArray<CarAspect>(track.numberOfCars, Allocator.Temp);
        NativeArray<CarLaneInfo> carLanes = new NativeArray<CarLaneInfo>(4, Allocator.Temp);
        for (int laneIndex = 0; laneIndex < 4; ++laneIndex)
        {
            carLanes[laneIndex] = new CarLaneInfo
            {
                LaneLength = TrackUtilities.GetLaneLength(track.highwaySize, laneIndex),
                CarCount = 0,
                CarDistances = new NativeArray<CarDistanceInfo>(track.numberOfCars, Allocator.Temp)
            };
        }

        // Build list of car distances
        foreach (var car in SystemAPI.Query<CarPositionAspect>())
        {
            float distance = car.Distance;
            CarLaneInfo laneInfo = carLanes[car.Lane];

            laneInfo.CarDistances[laneInfo.CarCount] = new CarDistanceInfo
            {
                Car = car.Entity,
                Distance = distance,
                WrappedDistance = TrackUtilities.WrapDistance(track.highwaySize, distance, car.Lane),
                CurrentSpeed = car.CurrentSpeed
            };
            laneInfo.CarCount++;

            carLanes[car.Lane] = laneInfo;
        }

        // Sort each lane of car distances and cache the distance from the previous car and the distance to the next car
        for (int laneIndex = 0; laneIndex < 4; ++laneIndex)
        {
            CarLaneInfo carLane = carLanes[laneIndex];

            carLane.CarDistances.Sort<CarDistanceInfo, CarDistanceComparer>(new CarDistanceComparer());

            for (int carIndex = 0; carIndex < carLane.CarCount; ++carIndex)
            {
                float distanceAhead = carLane.LaneLength;
                float distanceBehind = distanceAhead;
                float carAheadSpeed = 0.0f;
                Entity carInFront = Entity.Null;

                if (carIndex > 0)
                {
                    distanceBehind = TrackUtilities.WrapDistance(track.highwaySize, carLane.CarDistances[carIndex].Distance - carLane.CarDistances[carIndex - 1].Distance, laneIndex);
                }
                else if (carLane.CarCount > 1)
                {
                    distanceBehind = carLane.LaneLength - TrackUtilities.WrapDistance(track.highwaySize, carLane.CarDistances[carLane.CarCount - 1].Distance - carLane.CarDistances[carIndex].Distance, laneIndex);
                }

                if (carIndex < carLane.CarCount - 1)
                {
                    distanceAhead = TrackUtilities.WrapDistance(track.highwaySize, carLane.CarDistances[carIndex + 1].Distance - carLane.CarDistances[carIndex].Distance, laneIndex);
                    carInFront = carLane.CarDistances[carIndex + 1].Car;
                    carAheadSpeed = carLane.CarDistances[carIndex + 1].CurrentSpeed;
                }
                else if (carLane.CarCount > 1)
                {
                    distanceAhead = carLane.LaneLength - TrackUtilities.WrapDistance(track.highwaySize, carLane.CarDistances[carIndex].Distance - carLane.CarDistances[0].Distance, laneIndex);
                    carInFront = carLane.CarDistances[0].Car;
                    carAheadSpeed = carLane.CarDistances[0].CurrentSpeed;
                }

                // Set the cached AI information to make later systems easier to write
                state.EntityManager.SetComponentData(carLane.CarDistances[carIndex].Car, new CarAICache
                {
                    CarInFront = carInFront,
                    CarInFrontSpeed = carAheadSpeed,
                    CanMergeRight = false,
                    CanMergeLeft = false,
                    DistanceAhead = distanceAhead,
                    DistanceBehind = distanceBehind
                });
            }
        }

        // Build a spatial partition bucketing the lanes into sections approximately 20 units in length
        int numBucketsPerLane = (int)math.ceil(track.highwaySize / 20.0f);
        for (int laneIndex = 0; laneIndex < 4; ++laneIndex)
        {
            CarLaneInfo lane = carLanes[laneIndex];
            lane.Buckets = new NativeArray<CarGroupingBucket>(numBucketsPerLane, Allocator.Temp);

            for (int carIndex = 0; carIndex < lane.CarCount; ++carIndex)
            {
                int bucketIndex = CalculateBucket(ref lane, lane.CarDistances[carIndex].WrappedDistance);
                CarGroupingBucket bucket = lane.Buckets[bucketIndex];

                if (bucket.carCount == 0)
                {
                    bucket.firstIndex = carIndex;
                    bucket.carCount = 1;
                }
                else
                {
                    ++bucket.carCount;
                }

                lane.Buckets[bucketIndex] = bucket;
            }

            carLanes[laneIndex] = lane;
        }

        // Sort each lane of car distances and cache the distance from the previous car and the distance to the next car
        foreach (var car in SystemAPI.Query<CarAICacheAspect>())
        {
            if (car.Lane == 0)
                car.CanMergeRight = false;
            else
            {
                CarLaneInfo targetLane = carLanes[car.Lane - 1];
                car.CanMergeRight = CanMergeToLane(in car, car.Lane - 1, ref targetLane, track.highwaySize);
            }


            if (car.Lane == 3)
                car.CanMergeLeft = false;
            else
            {
                CarLaneInfo targetLane = carLanes[car.Lane + 1];
                car.CanMergeLeft = CanMergeToLane(in car, car.Lane + 1, ref targetLane, track.highwaySize);
            }
        }
    }

    int CalculateBucket(ref CarLaneInfo laneInfo, float wrappedDistance)
    {
        int bucketIndex = (int)math.floor(laneInfo.Buckets.Length * wrappedDistance / laneInfo.LaneLength);
        return bucketIndex;
    }

    bool CanMergeToLane(in CarAICacheAspect car, int targetLane, ref CarLaneInfo targetLaneInfo, float lane0Length)
    {
        float distanceBack = TrackUtilities.GetEquivalentDistance(lane0Length, car.Distance - car.MergeSpace, car.Lane, targetLane);
        float distanceFront = TrackUtilities.GetEquivalentDistance(lane0Length, car.Distance + car.MinDistanceInFront, car.Lane, targetLane);

        distanceBack = TrackUtilities.WrapDistance(lane0Length, distanceBack, targetLane);
        distanceFront = TrackUtilities.WrapDistance(lane0Length, distanceFront, targetLane);

        int backBucket = CalculateBucket(ref targetLaneInfo, distanceBack);
        int frontBucket = CalculateBucket(ref targetLaneInfo, distanceFront);
        int firstLoopFrontBucket = frontBucket >= backBucket ? frontBucket : targetLaneInfo.Buckets.Length - 1;
        float firstLoopDistanceFront = frontBucket >= backBucket ? distanceFront : targetLaneInfo.LaneLength;

        for (int bucketIndex = backBucket; bucketIndex <= firstLoopFrontBucket; ++bucketIndex)
        {
            CarGroupingBucket bucket = targetLaneInfo.Buckets[bucketIndex];

            for (int carIndex = 0; carIndex < bucket.carCount; ++carIndex)
            {
                float distance = targetLaneInfo.CarDistances[bucket.firstIndex + carIndex].WrappedDistance;

                if (distance > distanceBack && distance < firstLoopDistanceFront)
                {
                    return false;
                }
            }
        }

        // If the merge range of distances overlaps the wrap point of the track, we need to do a pass for the remaining buckets
        if (firstLoopFrontBucket != frontBucket)  // Buckets are wrapping around
        {
            for (int bucketIndex = 0; bucketIndex <= frontBucket; ++bucketIndex)
            {
                CarGroupingBucket bucket = targetLaneInfo.Buckets[bucketIndex];

                for (int carIndex = 0; carIndex < bucket.carCount; ++carIndex)
                {
                    float distance = targetLaneInfo.CarDistances[bucket.firstIndex + carIndex].WrappedDistance;

                    if (distance < distanceFront)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
