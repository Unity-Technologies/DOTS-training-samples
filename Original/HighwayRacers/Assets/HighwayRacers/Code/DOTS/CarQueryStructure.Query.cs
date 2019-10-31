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
        var highwayLen = HighwayLen;
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

        // TEST for binary search
        int p = BinarySearchCars(car.Position, ref lane1Array, 0, numCarsInLane1);
        //if (p != lane)

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
                if (frontCar.State.Position > Utilities.ConvertPositionToLane(front2.State.Position, front2.State.Lane, frontCar.State.Lane, highwayLen))
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
        distance = Utilities.WrapPositionToLane(Utilities.ConvertPositionToLane(frontCar.State.Position, frontCar.State.Lane, car.Lane, highwayLen) - car.Position, car.Lane, highwayLen);
        return true;
    }

    // Search for the first car with a greater position.
    private static int BinarySearchCars(float position, ref NativeSlice<CarIndexAndState> cars, int begin, int end)
    {
        //if (end < begin + 1)
        //    UnityEngine.Debug.Log($"{begin}, {end}");

        if (end <= begin + 1)
            //return cars[begin].State.Position > position ? begin : begin + 1;
            return begin;

        int center = (begin + end) / 2;
        var pivot = cars[center].State.Position;
        return pivot <= position
            ? BinarySearchCars(position, ref cars, center + 1, end)
            : BinarySearchCars(position, ref cars, begin, center);
    }

    //public bool GetCarInFront(float position, float lane, out CarBasicState frontCarState, out int frontCarIndex, out float distance)
    //{
    //    int lane1 = (int)math.floor(lane);
    //    int lane2 = (int)math.ceil(lane);
    //    UnityEngine.Debug.Assert(lane1 >= 0 && lane1 <= 3 && lane2 >= 0 && lane2 <= 3);

    //    int numCarsInLane1 = LaneCarCounts[lane1];
    //    int numCarsInLane2 = LaneCarCounts[lane2];
    //    if (numCarsInLane1 == 0 && numCarsInLane2 == 0)
    //    {
    //        frontCarState = default;
    //        frontCarIndex = -1;
    //        distance = float.MaxValue;
    //        return false;
    //    }

    //    // Binary search the lane1.
    //    var frontCar1Index = BinarySearchCars(position, ref LaneCars, lane1 * CarCount, lane1 * CarCount + numCarsInLane1);
    //    if (frontCar1Index == )
    //    //var lane1Array = LaneCars.Slice(lane1 * CarCount, numCarsInLane1);
    //}

    public bool CanMergeToLane(in CarBasicState car, float mergeLane)
    {
        return false;
    }
}
