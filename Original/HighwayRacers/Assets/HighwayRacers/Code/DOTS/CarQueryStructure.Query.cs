using Unity.Collections;
using Unity.Mathematics;

partial struct CarQueryStructure
{
    public CarBasicState GetCarInFront(int carEntityIndex, float carLane)
    {
        int floor = (int)math.floor(carLane);
        int lane = carLane - floor > 0.5f ? floor + 1 : floor;

        UnityEngine.Debug.Assert(lane >= 0 && lane < 4);
        var sortedIndex = OrderedCarLaneIndices[carEntityIndex].x;
        var laneArray = CarLanes.Slice(lane * CarCount, LaneCarCounts[lane]);
        return laneArray[sortedIndex != 0 ? sortedIndex - 1 : laneArray.Length - 1].State;
    }
}
