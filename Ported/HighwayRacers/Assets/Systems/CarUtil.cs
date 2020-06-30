using Unity.Collections;

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
        public static bool canMerge(float pos, int destLane, int segment, NativeArray<OtherCars> otherCars, float trackLength, int nSegments)
        {
            var laneBaseIdx = destLane * nSegments;

            var idx = laneBaseIdx + segment;
            var adjacentLane = otherCars[idx];

            var wrapAround = (segment == nSegments - 1);

            idx = laneBaseIdx + (wrapAround ? 0 : segment + 1);
            var adjacentLaneNextSegment = otherCars[idx];

            // find pos and speed of closest car ahead and closest car behind 
            var closestAheadPos = float.MaxValue;
            var closestBehindPos = float.MinValue;

            var posSegment = adjacentLane.positions;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < posSegment.Length; j++)
                {
                    var otherPos = posSegment[j].Val + (wrapAround && i == 1 ? trackLength : 0);

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

                posSegment = adjacentLaneNextSegment.positions;
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
        
        public static void GetClosestPosAndSpeed(out float closestPos, out float closestSpeed, NativeArray<OtherCars> selection,
            TrackSegment trackSegment, float trackLength, TrackPos trackPos, int nSegments)
        {
            // find pos and speed of closest car ahead
            closestSpeed = 0.0f;
            closestPos = float.MaxValue;
            for (int i = 0; i < selection.Length; i++)
            {
                var posSegment = selection[i].positions;
                var speedSegment = selection[i].speeds;

                var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;

                for (int j = 0; j < posSegment.Length; j++)
                {
                    var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
                    var otherSpeed = speedSegment[j];

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
            OtherCars selectionA, OtherCars selectionB,
            TrackSegment trackSegment, float trackLength, TrackPos trackPos, int nSegments)
        {
            // find pos and speed of closest car ahead
            closestSpeed = 0.0f;
            closestPos = float.MaxValue;
            
            var posSegment = selectionA.positions;
            var speedSegment = selectionA.speeds;
            
            for (int i = 0; i < 2; i++)
            {
                var wrapAround = (trackSegment.Val == nSegments - 1) && i == 1;

                for (int j = 0; j < posSegment.Length; j++)
                {
                    var otherPos = posSegment[j].Val + (wrapAround ? trackLength : 0);
                    var otherSpeed = speedSegment[j];

                    if (otherPos < closestPos &&
                        otherPos > trackPos.Val) // found a car ahead closer than previous closest
                    {
                        closestPos = otherPos;
                        closestSpeed = otherSpeed.Val;
                    }
                }
                
                posSegment = selectionB.positions;
                speedSegment = selectionB.speeds;
            }
        }
        
    }
}