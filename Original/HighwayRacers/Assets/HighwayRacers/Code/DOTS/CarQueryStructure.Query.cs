using Unity.Collections;
using Unity.Mathematics;

partial struct CarQueryStructure
{
    // Returns false if there is no other car on the lane.
    public bool GetCarInFront(int carEntityIndex, in CarBasicState car, float highwayLen, out CarBasicState frontCarBasicState, out float distance)
    {
        int lane1 = (int)math.floor(car.Lane);
        int lane2 = (int)math.ceil(car.Lane);
        UnityEngine.Debug.Assert(lane1 >= 0 && lane1 <= 3 && lane2 >= 0 && lane2 <= 3);

        int numCarsInLane1 = LaneCarCounts[lane1];
        int numCarsInLane2 = LaneCarCounts[lane2];
        UnityEngine.Debug.Assert(numCarsInLane1 >= 1 && numCarsInLane2 >= 1);
        if (numCarsInLane1 == 1 && numCarsInLane2 == 1)
        {
            frontCarBasicState = default;
            distance = float.MaxValue;
            return false;
        }

        var sortedIndices = OrderedCarLaneIndices[carEntityIndex];

        var lane1Array = CarLanes.Slice(lane1 * CarCount, numCarsInLane1);
        // Get the next car in the lane - since the array is sorted ascendingly by position, it should be
        // the car in front (or the car itself if there is no other car in that lane).
        var front1Wrapped = sortedIndices.x >= numCarsInLane1 - 1;
        frontCarBasicState = lane1Array[front1Wrapped ? 0 : sortedIndices.x + 1].State;

        // If the car is inbetween two lanes...compare both to see which one has smaller position.
        if (lane2 != lane1 && numCarsInLane2 > 1)
        {
            var lane2Array = CarLanes.Slice(lane2 * CarCount, numCarsInLane2);
            var front2Wrapped = sortedIndices.y >= numCarsInLane2 - 1;
            var front2 = lane2Array[front2Wrapped ? 0 : sortedIndices.y + 1].State;

            if (numCarsInLane1 == 1)
            {
                // lane2 has a car in front while lane1 is empty: return front2.
                frontCarBasicState = front2;
            }
            // If both or none lane are wrapped - just compare positions
            else if (front1Wrapped == front2Wrapped)
            {
                if (frontCarBasicState.Position > Utilities.ConvertPositionToLane(front2.Position, front2.Lane, frontCarBasicState.Lane, highwayLen))
                    frontCarBasicState = front2;
            }
            // Either front is wrapped - take the one that is not wrapped.
            else
            {
                if (front1Wrapped)
                    frontCarBasicState = front2;
            }
        }

        distance = Utilities.WrapPositionToLane(Utilities.ConvertPositionToLane(frontCarBasicState.Position, frontCarBasicState.Lane, car.Lane, highwayLen) - car.Position, car.Lane, highwayLen);
        return true;
    }
}
