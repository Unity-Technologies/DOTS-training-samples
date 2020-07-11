using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    public class CarUtil
    {
        public static bool CanMerge(int index, ref Car car, int destLane, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket)
        {
            // return false if a car is behind in dest lane within the mergeBehind range
            for (int behindIdx = index - 1; behindIdx >= 0; behindIdx--)
            {
                var other = bucket[behindIdx];

                if ((car.Pos - other.Pos) > RoadSys.mergeLookBehind)
                {
                    break; // all remaining cars too far back to block us from behind
                }

                if (other.Lane == destLane)
                {
                    return false; // blocked
                }
            }

            // return false if a car is ahead in dest lane within the mergeAhead range
            for (int aheadIdx = index + 1; aheadIdx < bucket.Length; aheadIdx++)
            {
                var other = bucket[aheadIdx];

                if ((other.Pos - car.Pos) > RoadSys.mergeLookAhead)
                {
                    return true; // all remaining cars too far ahead to block us
                }

                if (other.Lane == destLane)
                {
                    return false; // blocked
                }
            }

            // same as above, but continue check in second bucket


            for (int aheadIdx = 0; aheadIdx < nextBucket.Length; aheadIdx++)
            {
                var other = nextBucket[aheadIdx];
                var otherPos = other.Pos + segmentLength;

                if ((otherPos - car.Pos) > RoadSys.mergeLookAhead)
                {
                    return true; // all remaining cars too far ahead to block us
                }

                if (other.Lane == destLane)
                {
                    return false; // blocked
                }
            }

            return true; // exhausted the second bucket without finding a blocking car ahead
        }

        // 'index' is the car's index in the bucket; it's valid even when car is not blocked 
        public static void GetClosestPosAndSpeed(ref Car car, out float distance, out float closestSpeed, int index, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket)
        {
            distance = float.MaxValue;
            closestSpeed = 0.0f;

            // find pos and speed of car ahead in lane within the mergeAhead range
            for (int i = index + 1; i < bucket.Length; i++)
            {
                var other = bucket[i];
                var dist = (other.Pos - car.Pos);
                
                if (dist > RoadSys.mergeLookAhead)
                {
                    return; // all remaining cars too far ahead to block us
                }

                if (car.IsOccupyingLane(other.Lane)) // blocked in the same lane
                {
                    distance = dist;
                    closestSpeed = other.Speed;
                    return;
                }
            }

            // continue check in second bucket

            for (int forwardIdx = 0; forwardIdx < bucket.Length; forwardIdx++)
            {
                var other = bucket[forwardIdx];
                var otherPos = other.Pos + segmentLength;
                var dist = otherPos - car.Pos;
                
                if (dist > RoadSys.mergeLookAhead)
                {
                    return; // all remaining cars too far ahead to block us
                }

                if (car.IsOccupyingLane(other.Lane))
                {
                    distance = dist;
                    closestSpeed = other.Speed;
                    return; // blocked in the same lane
                }
            }

            // exhausted the second bucket without finding a blocking car ahead
        }
    }
}