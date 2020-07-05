using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    public class CarUtil
    {
        public static void SetSpeedForUnblocked(ref TargetSpeed targetSpeed, ref Speed speed, float dt, float unblockedSpeed)
        {
            targetSpeed.Val = unblockedSpeed;

            if (targetSpeed.Val < speed.Val)
            {
                speed.Val -= RoadSys.decelerationRate * dt;
                if (speed.Val < targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
            else if (targetSpeed.Val > speed.Val)
            {
                speed.Val += RoadSys.accelerationRate * dt;
                if (speed.Val > targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
        }

        // binary search to find a car's pos in the bucket
        // (we assume the pos is present!)
        public static int findInBucket(UnsafeList<SortedCar> bucket, float pos, float lane)
        {
            int start = 0;
            int end = bucket.Length - 1;
            int idx = end / 2;
            while (true)
            {
                var candidate = bucket[idx];
                var samePos = candidate.Pos == pos; 
                if (samePos && candidate.Lane == lane)
                {
                    break;
                }
                if ((samePos && candidate.Lane < lane) || (candidate.Pos < pos)) // look up 
                {
                    Assert.IsFalse(idx == end); // value should be present in the bucket
                    start = idx + 1;
                    idx = (end - start) / 2 + start;
                }
                else // look down
                {
                    Assert.IsFalse(idx == start); // value should be present in the bucket
                    end = idx - 1;
                    idx = (end - start) / 2 + start;
                }
            }
        }

        // todo: account for *speed* of adjacent car ahead and car behind relative to this car?
        public static bool canMerge(float pos, int destLane, int lane, int segment, CarBuckets carBuckets,
            float trackLength, int nSegments)
        {
            var bucket = carBuckets.GetCars(destLane, segment);
            int idx = findInBucket(bucket, pos, lane);
            
            // return false if a car is behind in dest lane within the mergeBehind range
            for (int backIdx = idx - 1; backIdx < 0; backIdx--)
            {
                var other = bucket[backIdx];

                var inRange = (pos - other.Pos) < RoadSys.mergeLookBehind;
                if (inRange)
                {
                    if (other.Lane == destLane)
                    {
                        return false; // blocked in the same lane
                    }
                }
                else
                {
                    break; // all remaining cars too far back to block us from behind
                }
            }
            
            // return false if a car is ahead in dest lane within the mergeAhead range
            for (int forwardIdx = idx + 1; forwardIdx < bucket.Length; forwardIdx++)
            {
                var other = bucket[forwardIdx];

                var inRange = (other.Pos - pos) < RoadSys.mergeLookAhead;
                if (inRange)
                {
                    if (other.Lane == destLane)
                    {
                        return false;  // blocked in the same lane
                    }
                }
                else
                {
                    return false;   // all remaining cars too far ahead to block us
                }
            }
            
            // same as above, but continue check in second bucket
            var wrapAround = (segment == nSegments - 1);
            bucket = carBuckets.GetCars(destLane, (wrapAround) ? 0 : segment + 1);
            
            for (int forwardIdx = 0; forwardIdx < bucket.Length; forwardIdx++)
            {
                var other = bucket[forwardIdx];

                var otherPos = (wrapAround) ? other.Pos + trackLength : other.Pos;
                var inRange = (other.Pos - pos) < RoadSys.mergeLookAhead;
                if (inRange)
                {
                    if (other.Lane == destLane)
                    {
                        return false;  // blocked in the same lane
                    }
                }
                else
                {
                    return false;   // all remaining cars too far ahead to block us
                }
            }

            return false;    // exhausted the second bucket without finding a blocking car ahead
        }


        public static void SetUnblockedSpeed(ref Speed speed, ref TargetSpeed targetSpeed, float dt, float unblockedSpeed)
        {
            var newTargetSpeed = unblockedSpeed;

            if (newTargetSpeed < speed.Val)
            {
                var s = speed.Val - RoadSys.decelerationRate * dt;
                if (s < newTargetSpeed)
                {
                    s = newTargetSpeed;
                }

                speed.Val = s;
            }
            else if (newTargetSpeed > speed.Val)
            {
                var s = speed.Val + RoadSys.accelerationRate * dt;
                if (s > newTargetSpeed)
                {
                    s = newTargetSpeed;
                }

                speed.Val = s;
            }

            targetSpeed.Val = newTargetSpeed;
        }

        public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed,
            CarBuckets carBuckets, int segment, int lane, int otherLane,
            float trackLength, TrackPos trackPos, int nSegments)
        {
            // find pos and speed of closest car ahead, checking two lanes 
            closestPos = float.MaxValue;
            closestSpeed = 0.0f;

            var wrapAround = segment == nSegments - 1;
            var nextSegment = (wrapAround) ? 0 : segment + 1;

            // in this order:
            //     lane, segment
            //     lane, nextSegment
            //     otherLane, segment
            //     otherLane, nextSegment
            for (int i = 0; i < 4; i++)
            {
                var seg = (i % 2 == 0) ? segment : nextSegment;
                if (i == 2)
                {
                    lane = otherLane;
                }

                var positions = carBuckets.GetPositions(lane, seg);
                var speeds = carBuckets.GetSpeeds(lane, seg);

                for (int j = 0; j < positions.Length; j++)
                {
                    var otherPos = positions[j].Val + (wrapAround && i % 2 == 0 ? trackLength : 0);
                    var otherSpeed = speeds[j];

                    if (otherPos < closestPos &&
                        otherPos > trackPos.Val) // found a car ahead closer than previous closest
                    {
                        closestPos = otherPos;
                        closestSpeed = otherSpeed.Val;
                    }
                }
            }
        }

        public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed,
            CarBuckets carBuckets, int segment, int lane,
            float trackLength, TrackPos trackPos, int nSegments)
        {
            // find pos and speed of closest car ahead, checking two lanes 
            closestPos = float.MaxValue;
            closestSpeed = 0.0f;

            var wrapAround = segment == nSegments - 1;

            // in this order:
            //     lane, segment
            //     lane, nextSegment
            for (int i = 0; i < 2; i++)
            {
                var positions = carBuckets.GetPositions(lane, segment);
                var speeds = carBuckets.GetSpeeds(lane, segment);

                for (int j = 0; j < positions.Length; j++)
                {
                    var otherPos = positions[j].Val + (wrapAround && i == 1 ? trackLength : 0);
                    var otherSpeed = speeds[j];

                    if (otherPos < closestPos &&
                        otherPos > trackPos.Val) // found a car ahead closer than previous closest
                    {
                        closestPos = otherPos;
                        closestSpeed = otherSpeed.Val;
                    }
                }

                segment = wrapAround ? 0 : segment + 1;
            }
        }
    }
}