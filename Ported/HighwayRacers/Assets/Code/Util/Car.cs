using Unity.Assertions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    public struct Car
    {
        // todo: split into SoA?
        public byte Lane;
        public float Pos;
        public float Speed;
        public half BlockingDist;
        public half DesiredSpeedUnblocked;
        public half DesiredSpeedOvertake;
        public CarState CarState;
        public half LaneOffset;
        public half OvertakeTimer;

        public bool IsInLeftmostLane()
        {
            return (8 & Lane) != 0;
        }

        public bool IsInRightmostLane()
        {
            return (1 & Lane) != 0;
        }

        public int LaneToLeft()
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
                    // Debug.LogError("Invalid: no lane to left.");
                    return 8;
            }
        }

        public int LaneToRight()
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
                    // Debug.LogError("Invalid: no lane to right.");
                    return 1;
            }
        }

        public void RightMostLane()
        {
            Lane = 1;
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
//                    Debug.LogError("Invalid merge left. Lane: " + Lane);
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
                    // Debug.LogError("Invalid merge right. Lane: " + Lane);
                    break;
            }
        }

        // todo: pass in decelerationRate and accelerationRate * with dt so that we can do the multiplication once for all cars
        public void SetSpeed(float dt, float targetSpeed)
        {
            if (Speed > targetSpeed) // then go slower
            {
                var s = Speed - RoadSys.decelerationRate * dt;
                Speed = (s < targetSpeed) ? targetSpeed : s;
            }
            else if (Speed < targetSpeed) // then go faster
            {
                var s = Speed + RoadSys.accelerationRate * dt;
                Speed = (s > targetSpeed) ? targetSpeed : s;
            }
        }

        public void SlowBecauseBlocked(float distToBlockingCar, float blockingCarSpeed)
        {
            var closeness = (distToBlockingCar - RoadSys.minDist) / (BlockingDist - RoadSys.minDist); // 0 is max closeness, 1 is min

            // closer we get within minDist of leading car, the closer we match speed
            const float fudge = 2.0f;
            var newSpeed = math.lerp(blockingCarSpeed, Speed + fudge, closeness);

            // car, so newSpeed should always be slower than current Speed
            if (newSpeed < 0)
            {
                Speed = 0.1f;
            }
            else if (newSpeed < Speed)
            {
                Speed = newSpeed;
            }
        }

        // return true if both lane values overlap
        public bool IsOverlapingLane(int otherLane)
        {
            return (Lane & otherLane) != 0;
        }

        public void CompleteLeftMerge()
        {
            LaneOffset = (half) 0;
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
                    // Debug.LogError("Invalid complete left right.");
                    break;
            }
        }

        public void CompleteRightMerge()
        {
            LaneOffset = (half) 0;
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
                    // Debug.LogError("Invalid CompleteRightMerge.");
                    break;
            }
        }

        public float LaneOffsetDist()
        {
            float baseOffset = 0;

            switch (Lane)
            {
                case 1: // 0001
                case 3: // 0011
                    baseOffset = 0;
                    break;
                case 2: // 0010
                case 6: // 0110
                    baseOffset = 1;
                    break;
                case 4: // 0100
                case 12: // 1100
                    baseOffset = 2;
                    break;
                case 8: // 1000
                    baseOffset = 3;
                    break;
            }

            switch (CarState)
            {
                case CarState.OvertakingRightStart:
                case CarState.OvertakingLeftEnd:
                    baseOffset += 1;
                    break;
            }

            return baseOffset + LaneOffset;
        }


        public void SetNextPosAndLane()
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
                case 4:
                    Lane = 8; // 1000
                    break;
                case 8:
                    Lane = 1; // 1000
                    Pos += RoadSys.carSpawnDist;
                    break;
            }
        }

        public void MergingMove(float mergeSpeed)
        {
            switch (CarState)
            {
                case CarState.Normal:
                case CarState.OvertakingLeft:
                case CarState.OvertakingRight:
                    break;
                case CarState.OvertakingLeftStart:
                    LaneOffset += (half) mergeSpeed;
                    if (LaneOffset > 1.0f)
                    {
                        CompleteLeftMerge();
                        CarState = CarState.OvertakingLeft;
                    }

                    break;
                case CarState.OvertakingRightEnd:
                    LaneOffset += (half) mergeSpeed;
                    if (LaneOffset > 1.0f)
                    {
                        CompleteLeftMerge();
                        CarState = CarState.Normal;
                    }

                    break;
                case CarState.OvertakingRightStart:
                    LaneOffset -= (half) mergeSpeed;
                    if (LaneOffset < -1.0f)
                    {
                        CompleteRightMerge();
                        CarState = CarState.OvertakingRight;
                    }

                    break;
                case CarState.OvertakingLeftEnd:
                    LaneOffset -= (half) mergeSpeed;
                    if (LaneOffset < -1.0f)
                    {
                        CompleteRightMerge();
                        CarState = CarState.Normal;
                    }

                    break;
            }
        }

        public void Avoidance(int idx, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, bool mergeLeftFrame, float dt)
        {
            switch (CarState)
            {
                case CarState.Normal:
                    AvoidanceNormal(idx, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                    break;
                case CarState.OvertakingLeft:
                    OvertakeTimer -= (half) dt;
                    AvoidanceOvertaking(true, idx, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                    break;
                case CarState.OvertakingRight:
                    OvertakeTimer -= (half) dt;
                    AvoidanceOvertaking(false, idx, segmentLength, bucket, nextBucket, mergeLeftFrame, dt);
                    break;
                case CarState.OvertakingLeftStart:
                case CarState.OvertakingRightStart:
                case CarState.OvertakingLeftEnd:
                case CarState.OvertakingRightEnd:
                    OvertakeTimer -= (half) dt;
                    AvoidanceMerging(idx, segmentLength, bucket, nextBucket, dt);
                    break;
            }
        }

        public void AvoidanceNormal(int idx, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket,
            bool mergeLeftFrame, float dt)
        {
            GetClosestPosAndSpeed(out var distance, out var closestSpeed, idx, segmentLength, bucket, nextBucket);

            if (distance <= BlockingDist && Speed > closestSpeed) // car is blocked ahead in lane
            {
                SlowBecauseBlocked(distance, closestSpeed);

                // don't merge if too close to start of segment
                if (Pos < RoadSys.mergeLookBehind)
                {
                    return;
                }

                if (mergeLeftFrame)
                {
                    // look for opening on left
                    if (!IsInLeftmostLane() && CanMerge(idx, LaneToLeft(), segmentLength, bucket, nextBucket))
                    {
                        CarState = CarState.OvertakingLeftStart;
                        OvertakeTimer = (half)RoadSys.overtakeTime;
                        LaneOffset = (half)0; // we're going left, so value starts at 0.0 and will inc to 1.0
                        MergeLeftLane();
                    }
                }
                else
                {
                    // look for opening on right
                    if (!IsInRightmostLane() && CanMerge(idx, LaneToRight(), segmentLength, bucket, nextBucket))
                    {
                        CarState = CarState.OvertakingRightStart;
                        OvertakeTimer = (half)RoadSys.overtakeTime;
                        LaneOffset = (half)0; // we're going right, so value starts at 0.0 and will dec to -1.0
                        MergeRightLane();
                    }
                }

                return;
            }

            SetSpeed(dt, DesiredSpeedUnblocked);
        }

        private void AvoidanceOvertaking(bool overtakeLeft, int idx, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket,
            bool mergeLeftFrame, float dt)
        {
            GetClosestPosAndSpeed(out var distance, out var closestSpeed, idx, segmentLength, bucket, nextBucket);

            // if blocked, go to Normal state
            if (distance <= BlockingDist && Speed > closestSpeed)
            {
                SlowBecauseBlocked(distance, closestSpeed);
                CarState = CarState.Normal;
                return;
            }

            // merging timed out, so end overtake
            if (OvertakeTimer <= 0)
            {
                CarState = CarState.Normal;
                return;
            }

            // try merge back to lane car came from
            if (OvertakeTimer < RoadSys.overtakeTimeTryMerge &&
                ((overtakeLeft && !mergeLeftFrame) || (!overtakeLeft && mergeLeftFrame)) &&
                CanMerge(idx, (overtakeLeft ? LaneToRight() : LaneToLeft()), segmentLength, bucket, nextBucket))
            {
                LaneOffset = (half)0;
                if (overtakeLeft)
                {
                    CarState = CarState.OvertakingLeftEnd;
                    MergeRightLane();
                }
                else
                {
                    CarState = CarState.OvertakingRightEnd;
                    MergeLeftLane();
                }

                return;
            }

            SetSpeed(dt, DesiredSpeedOvertake);
        }

        private void AvoidanceMerging(int idx, float segmentLength, UnsafeList<Car> bucket, UnsafeList<Car> nextBucket, float dt)
        {
            GetClosestPosAndSpeed(out var distance, out var closestSpeed, idx, segmentLength, bucket, nextBucket);

            if (distance <= BlockingDist && Speed > closestSpeed) // car is blocked ahead in lane
            {
                SlowBecauseBlocked(distance, closestSpeed);
                return;
            }

            SetSpeed(dt, DesiredSpeedOvertake);
        }

        public bool CanMerge(int idx, int destLane, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket)
        {
            // return false if a car is behind in dest lane within the mergeBehind range
            for (int behindIdx = idx - 1; behindIdx >= 0; behindIdx--)
            {
                // todo: does this copy whole car regardless that we only access two fields? (moot point if we split off Pos,Lane,and Speed into separate struct in separate buckets)
                var other = bucket[behindIdx];

                if ((Pos - other.Pos) > RoadSys.mergeLookBehind)
                {
                    break; // all remaining cars too far back to block us from behind
                }

                if (other.IsOverlapingLane(destLane))
                {
                    return false; // blocked
                }
            }

            // return false if a car is ahead in dest lane within the mergeAhead range
            for (int aheadIdx = idx + 1; aheadIdx < bucket.Length; aheadIdx++)
            {
                var other = bucket[aheadIdx];

                if ((other.Pos - Pos) > RoadSys.mergeLookAhead)
                {
                    return true; // all remaining cars too far ahead to block us
                }

                if (other.IsOverlapingLane(destLane))
                {
                    return false; // blocked
                }
            }

            // same as above, but continue check in nextBucket
            for (int aheadIdx = 0; aheadIdx < nextBucket.Length; aheadIdx++)
            {
                var other = nextBucket[aheadIdx];
                var otherPos = other.Pos + segmentLength;

                if ((otherPos - Pos) > RoadSys.mergeLookAhead)
                {
                    return true; // all remaining cars too far ahead to block us
                }

                if (other.IsOverlapingLane(destLane))
                {
                    return false; // blocked
                }
            }

            return true; // exhausted nextBucket without finding a blocking car ahead
        }

        // 'index' is the car's index in the bucket; it's valid even when car is not blocked 
        public void GetClosestPosAndSpeed(out float distance, out float closestSpeed, int index, float segmentLength,
            UnsafeList<Car> bucket, UnsafeList<Car> nextBucket)
        {
            distance = float.MaxValue;
            closestSpeed = 0.0f;

            // find pos and speed of car ahead in lane within the mergeAhead range
            for (int i = index + 1; i < bucket.Length; i++)
            {
                var other = bucket[i];
                var dist = (other.Pos - Pos);

                if (dist > RoadSys.mergeLookAhead)
                {
                    return; // all remaining cars too far ahead to block us
                }

                if (IsOverlapingLane(other.Lane)) // blocked in the same lane
                {
                    distance = dist;
                    closestSpeed = other.Speed;
                    return;
                }
            }

            // continue check in second bucket
            for (int forwardIdx = 0; forwardIdx < nextBucket.Length; forwardIdx++)
            {
                var other = nextBucket[forwardIdx];
                var dist = (other.Pos + segmentLength) - Pos;

                if (dist > RoadSys.mergeLookAhead)
                {
                    return; // all remaining cars too far ahead to block us
                }

                if (IsOverlapingLane(other.Lane))
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