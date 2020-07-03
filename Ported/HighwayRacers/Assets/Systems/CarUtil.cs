using DataStruct;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace HighwayRacer
{
    public class CarUtil
    {
        
        const float decelerationRate = Road.decelerationRate;
        const float accelerationRate = Road.accelerationRate;
        
        const float mergeLookAhead = Road.mergeLookAhead;
        const float mergeLookBehind = Road.mergeLookBehind;
        
        public static void SetSpeedForUnblocked(ref TargetSpeed targetSpeed, ref Speed speed, float dt, float unblockedSpeed)
        {
            targetSpeed.Val = unblockedSpeed;

            if (targetSpeed.Val < speed.Val)
            {
                speed.Val -= decelerationRate * dt;
                if (speed.Val < targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
            else if (targetSpeed.Val > speed.Val)
            {
                speed.Val += accelerationRate * dt;
                if (speed.Val > targetSpeed.Val)
                {
                    speed.Val = targetSpeed.Val;
                }
            }
        }
        
        
        // todo: account for *speed* of adjacent car ahead and car behind relative to this car?
        public static bool canMerge(float pos, int destLane, int segment, BucketizedCars bucketizedCars,
            float trackLength, int nSegments)
        {
            // find pos and speed of closest car ahead and closest car behind in the destination lane 
            var closestAheadPos = float.MaxValue;
            var closestBehindPos = float.MinValue;

            var wrapAround = (segment == nSegments - 1);
            var secondWrapAround = (segment == nSegments);
            
            var positions = bucketizedCars.GetPositions(destLane, segment);
            var secondPositions =  bucketizedCars.GetPositions(destLane, (wrapAround) ? 0 : segment + 1);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < positions.Length; j++)
                {
                    var otherPos = positions[j].Val + (wrapAround && i == 1 ? trackLength : 0);

                    if (otherPos < closestAheadPos &&
                        otherPos > pos) // found a car ahead that's closer than previous closest
                    {
                        closestAheadPos = otherPos;
                    }
                    else if (otherPos > closestBehindPos &&
                             otherPos <= pos) // found a car behind (or equal) that's closer than previous closest
                    {
                        closestBehindPos = otherPos;
                    }
                }

                positions = secondPositions;
                wrapAround = secondWrapAround;
            }

            // sufficient margin of open space
            return (closestBehindPos + mergeLookBehind) < pos && (closestAheadPos - mergeLookAhead) > pos;
        }


        public static void SetUnblockedSpeed(ref Speed speed, ref TargetSpeed targetSpeed, float dt, float unblockedSpeed)
        {
            var newTargetSpeed = unblockedSpeed;

            if (newTargetSpeed < speed.Val)
            {
                var s = speed.Val - decelerationRate * dt;
                if (s < newTargetSpeed)
                {
                    s = newTargetSpeed;
                }

                speed.Val = s;
            }
            else if (newTargetSpeed > speed.Val)
            {
                var s = speed.Val + accelerationRate * dt;
                if (s > newTargetSpeed)
                {
                    s = newTargetSpeed;
                }

                speed.Val = s;
            }

            targetSpeed.Val = newTargetSpeed;
        }
        
        // public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed, 
        //     NativeArray<UnsafeList<TrackPos>> positions,
        //     NativeArray<UnsafeList<Speed>> speeds,
        //     TrackSegment trackSegment, float trackLength, TrackPos trackPos, int nSegments)
        // {
        //     // find pos and speed of closest car ahead
        //     closestSpeed = 0.0f;
        //     closestPos = float.MaxValue;
        //     for (int i = 0; i < positions.Length; i++)
        //     {
        //         var posSegment = positions[i];
        //         var speedSegment = speeds[i];
        //
        //         var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;
        //
        //         for (int j = 0; j < posSegment.Length; j++)
        //         {
        //             var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
        //             var otherSpeed = speedSegment[j];
        //
        //             if (otherPos < closestPos &&
        //                 otherPos > trackPos.Val) // found a car ahead closer than previous closest
        //             {
        //                 closestPos = otherPos;
        //                 closestSpeed = otherSpeed.Val;
        //             }
        //         }
        //     }
        // }
        
        
        // public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed, 
        //     SegmentizedCars segmentizedCars, int segment, int lane,
        //     float trackLength, TrackPos trackPos, int nSegments)
        // {
        //     // find pos and speed of closest car ahead and closest car behind in the destination lane 
        //     closestPos = float.MaxValue;
        //     closestSpeed = 0.0f;
        //
        //     var wrapAround = (segment == nSegments - 1);
        //     var secondWrapAround = (segment == nSegments);
        //     
        //     var positions = segmentizedCars.GetPositions(lane, segment);
        //     var secondPositions =  segmentizedCars.GetPositions(lane, (wrapAround) ? 0 : segment + 1);
        //     
        //     var speeds = segmentizedCars.GetPositions(lane, segment);
        //     var secondSpeeds =  segmentizedCars.GetPositions(lane, (wrapAround) ? 0 : segment + 1);
        //     
        //     for (int i = 0; i < 2; i++)
        //     {
        //         for (int j = 0; j < positions.Length; j++)
        //         {
        //             var otherPos = positions[j].Val + (wrapAround ? trackLength : 0);
        //             var otherSpeed = speeds[j];
        //
        //             if (otherPos < closestPos &&
        //                 otherPos > trackPos.Val) // found a car ahead closer than previous closest
        //             {
        //                 closestPos = otherPos;
        //                 closestSpeed = otherSpeed.Val;
        //             }
        //         }
        //         
        //         positions = secondPositions;
        //         speeds = secondSpeeds;
        //         wrapAround = secondWrapAround;
        //     }
        // }
        
        public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed, 
            BucketizedCars bucketizedCars, int segment, int lane, int otherLane,
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

                var positions = bucketizedCars.GetPositions(lane, seg);
                var speeds = bucketizedCars.GetSpeeds(lane, seg);
                
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
            BucketizedCars bucketizedCars, int segment, int lane,
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
                var positions = bucketizedCars.GetPositions(lane, segment);
                var speeds = bucketizedCars.GetSpeeds(lane, segment);
                
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

        

        // public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed,
        //     UnsafeList<TrackPos> positionsA, UnsafeList<Speed> speedsA, 
        //     UnsafeList<TrackPos> positionsB, UnsafeList<Speed> speedsB,
        //     TrackSegment trackSegment, float trackLength, TrackPos trackPos, int nSegments)
        // {
        //     // find pos and speed of closest car ahead
        //     closestSpeed = 0.0f;
        //     closestPos = float.MaxValue;
        //
        //     for (int i = 0; i < 2; i++)
        //     {
        //         var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;
        //
        //         for (int j = 0; j < positionsA.Length; j++)
        //         {
        //             var otherPos = positionsA[j].Val + (wrapAround ? trackLength : 0);
        //             var otherSpeed = speedsA[j];
        //
        //             if (otherPos < closestPos &&
        //                 otherPos > trackPos.Val) // found a car ahead closer than previous closest
        //             {
        //                 closestPos = otherPos;
        //                 closestSpeed = otherSpeed.Val;
        //             }
        //         }
        //
        //         positionsA = positionsB;
        //         speedsA = speedsB;
        //     }
        // }
    }
}