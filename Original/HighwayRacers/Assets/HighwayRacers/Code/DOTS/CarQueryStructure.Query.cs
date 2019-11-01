using Unity.Collections;
using Unity.Mathematics;

partial struct CarQueryStructure
{
    public CarBasicState GetCarState(int carEntityIndex)
    {
        var laneIndices = OrderedCarLaneIndices[carEntityIndex];
        var lane1 = (int)math.floor(laneIndices.Lane);
        var laneCars = LaneCars.Slice(lane1 * CarCount, LaneCarCounts[lane1]);
        return laneCars[laneIndices.Lane1Index].State;
    }

    // Returns false if there is no other car on the lane.
    public bool GetCarInFront(int carEntityIndex, in CarBasicState car, out CarBasicState frontCarState, out int frontCarIndex, out float distance)
    {
        int lane1 = (int)math.floor(car.Lane);
        int lane2 = (int)math.ceil(car.Lane);
        UnityEngine.Debug.Assert(lane1 >= 0 && lane1 <= 3 && lane2 >= 0 && lane2 <= 3);

        int numCarsInLane1 = LaneCarCounts[lane1];
        int numCarsInLane2 = LaneCarCounts[lane2];
        UnityEngine.Debug.Assert(numCarsInLane1 >= 1 && numCarsInLane2 >= 1);
        if (numCarsInLane1 == 1 && numCarsInLane2 == 1)
        {
            frontCarState = default;
            frontCarIndex = -1;
            distance = float.MaxValue;
            return false;
        }

        var laneIndices = OrderedCarLaneIndices[carEntityIndex];

        var lane1Array = LaneCars.Slice(lane1 * CarCount, numCarsInLane1);
        // Get the next car in the lane - since the array is sorted ascendingly by position, it should be
        // the car in front (or the car itself if there is no other car in that lane).
        var front1Wrapped = laneIndices.Lane1Index >= numCarsInLane1 - 1;
        var frontCar = lane1Array[front1Wrapped ? 0 : laneIndices.Lane1Index + 1];

        // If the car is inbetween two lanes...compare both to see which one has smaller position.
        if (lane2 != lane1 && numCarsInLane2 > 1)
        {
            var lane2Array = LaneCars.Slice(lane2 * CarCount, numCarsInLane2);
            var front2Wrapped = laneIndices.Lane2Index >= numCarsInLane2 - 1;
            var front2 = lane2Array[front2Wrapped ? 0 : laneIndices.Lane2Index + 1];

            if (numCarsInLane1 == 1)
            {
                // lane2 has a car in front while lane1 is empty: return front2.
                frontCar = front2;
            }
            // If both or none lane are wrapped - just compare positions
            else if (front1Wrapped == front2Wrapped)
            {
                if (frontCar.State.Position > Utilities.ConvertPositionToLane(front2.State.Position, front2.State.Lane, frontCar.State.Lane, HighwayLen))
                    frontCar = front2;
            }
            // Either front is wrapped - take the one that is not wrapped.
            else
            {
                if (front1Wrapped)
                    frontCar = front2;
            }
        }

        frontCarState = frontCar.State;
        frontCarIndex = frontCar.EntityArrayIndex;
        distance = Utilities.WrapPositionToLane(Utilities.ConvertPositionToLane(frontCar.State.Position, frontCar.State.Lane, car.Lane, HighwayLen) - car.Position, car.Lane, HighwayLen);
        return true;
    }

    // Search for the first car with a greater position.
    private static int BinarySearchCars(float position, ref NativeSlice<CarIndexAndState> cars)
    {
        int begin = 0, end = cars.Length;
        while (end > begin + 1)
        {
            int center = (begin + end) / 2;
            var pivot = cars[center].State.Position;
            if (pivot <= position)
                begin = center + 1;
            else
                end = center;
        }

        return begin != end && cars[begin].State.Position <= position ? begin + 1 : begin;
    }

    public bool GetCarInFront(float position, float lane, out CarBasicState frontCarState, out int frontCarIndex, out float distance)
    {
        int lane1 = (int)math.floor(lane);
        int lane2 = (int)math.ceil(lane);
        UnityEngine.Debug.Assert(lane1 >= 0 && lane1 <= 3 && lane2 >= 0 && lane2 <= 3);

        int numCarsInLane1 = LaneCarCounts[lane1];
        int numCarsInLane2 = LaneCarCounts[lane2];
        if (numCarsInLane1 == 0 && numCarsInLane2 == 0)
        {
            frontCarState = default;
            frontCarIndex = -1;
            distance = float.MaxValue;
            return false;
        }

        var lane1Array = LaneCars.Slice(lane1 * CarCount, numCarsInLane1);
        var frontCar1Index = BinarySearchCars(position, ref lane1Array);
        var front1Wrapped = frontCar1Index == numCarsInLane1;
        var frontCar = numCarsInLane1 != 0 ? lane1Array[front1Wrapped ? 0 : frontCar1Index] : default;

        if (lane2 != lane1 && numCarsInLane2 > 0)
        {
            var lane2Array = LaneCars.Slice(lane2 * CarCount, numCarsInLane2);
            var frontCar2Index = BinarySearchCars(position, ref lane2Array);
            var front2Wrapped = frontCar2Index == numCarsInLane2;
            var front2 = lane2Array[front2Wrapped ? 0 : frontCar2Index];

            if (numCarsInLane1 == 0)
            {
                // lane2 has a car in front while lane1 is empty: return front2.
                frontCar = front2;
            }
            // If both or none lane are wrapped - just compare positions
            else if (front1Wrapped == front2Wrapped)
            {
                if (frontCar.State.Position > Utilities.ConvertPositionToLane(front2.State.Position, front2.State.Lane, frontCar.State.Lane, HighwayLen))
                    frontCar = front2;
            }
            // Either front is wrapped - take the one that is not wrapped.
            else
            {
                if (front1Wrapped)
                    frontCar = front2;
            }
        }

        frontCarState = frontCar.State;
        frontCarIndex = frontCar.EntityArrayIndex;
        distance = Utilities.WrapPositionToLane(Utilities.ConvertPositionToLane(frontCar.State.Position, frontCar.State.Lane, lane, HighwayLen) - position, lane, HighwayLen);
        return true;
    }

    public bool CanMergeToLane(in CarBasicState car, float mergeLane)
    {
        return true;
    }
}
