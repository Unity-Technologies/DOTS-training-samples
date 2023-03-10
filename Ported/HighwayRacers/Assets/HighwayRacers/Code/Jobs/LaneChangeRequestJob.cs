using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Aspects;
using Unity.Mathematics;

struct SortByDistance : IComparer<CarPosition>
{
    public int Compare(CarPosition a, CarPosition b) => a.Distance.CompareTo(b.Distance);
}

[BurstCompile]
public partial struct LaneChangeRequestJob : IJobEntity
{
    [ReadOnly]
    public Config config;
    [ReadOnly]
    public NativeArray<CarPosition> carPositions;
    [ReadOnly]
    public bool changeUpAlternator;

    [BurstCompile]
    public void Execute(ref LaneChangeState laneChange, in CarPosition carPos)
    {
        int carPositionIndex = carPositions.BinarySearch(carPos, new SortByDistance());

        laneChange.myIndex = carPositionIndex;

        int startIndex = (carPositionIndex + carPositions.Length - config.HighwaySliceSize) % carPositions.Length;
        int endIndex = (carPositionIndex + config.HighwaySliceSize) % carPositions.Length;
        if (changeUpAlternator)
        {
            if (laneChange.requestChangeUp)
            {
                if (startIndex > endIndex)
                {
                    // if the search area wrapped around the highway, we have two separate segments to check
                    laneChange.approveChangeUp = IsLaneChangeOk(carPos.Distance, carPos.CurrentLane + 1, startIndex, carPositions.Length) &&
                                            IsLaneChangeOk(carPos.Distance, carPos.CurrentLane + 1, 0, endIndex);
                }
                else
                {
                    if (laneChange.requestChangeUp)
                        laneChange.approveChangeUp = IsLaneChangeOk(carPos.Distance, carPos.CurrentLane + 1, startIndex, endIndex);
                }
            }
        }
        else
        {
            if (laneChange.requestChangeDown)
            {
                if (startIndex > endIndex)
                {
                    // if the search area wrapped around the highway, we have two separate segments to check
                    laneChange.approveChangeDown = IsLaneChangeOk(carPos.Distance, carPos.CurrentLane - 1, startIndex, carPositions.Length) &&
                                            IsLaneChangeOk(carPos.Distance, carPos.CurrentLane - 1, 0, endIndex);
                }
                else
                {
                    laneChange.approveChangeDown = IsLaneChangeOk(carPos.Distance, carPos.CurrentLane - 1, startIndex, endIndex);

                }
            }
        }

        laneChange.distFromCarInFront = GetDistToCarAhead(carPositionIndex, carPos);
    }

    bool IsLaneChangeOk(float distance, float lane, int startIndex, int endIndex)
    {
        for (int index = startIndex; index < endIndex; ++index)
        {
            CarPosition obstacle = carPositions[index];
            if (math.floor(obstacle.CurrentLane) == lane || math.ceil(obstacle.CurrentLane) == lane) // in the lane I want to move into?
            {
                if (math.abs((distance + config.HighwayMaxSize - obstacle.Distance) % config.HighwayMaxSize) < config.CarLength) // close enough to block me?
                {
                    // blocked
                    return false;
                }
            }
        }
        return true;
    }

    float GetDistToCarAhead(int carPositionIndex, CarPosition carPos)
    {
        int lookaheadStart = (carPositionIndex + 1) % carPositions.Length;
        int lookaheadEnd = lookaheadStart+1;// (carPositionIndex + 1 + config.HighwaySliceSize*2) % carPositions.Length;
        if (lookaheadStart > lookaheadEnd)
        {
            for (int index = lookaheadStart; index < carPositions.Length; ++index)
            {
                CarPosition obstacle = carPositions[index];
                if (math.abs(obstacle.CurrentLane - carPos.CurrentLane) < 1.0f) // if the obstacle car is moving into/out of my lane, or me his
                {
                    if (obstacle.Distance > carPos.Distance)
                        return obstacle.Distance - carPos.Distance;
                    else
                        return (obstacle.Distance + config.HighwayMaxSize - carPos.Distance) % config.HighwayMaxSize;
                }
            }

            lookaheadStart = 0;
        }

        for (int index = lookaheadStart; index < lookaheadEnd; ++index)
        {
            CarPosition obstacle = carPositions[index];
            if (math.abs(obstacle.CurrentLane - carPos.CurrentLane) < 1.0f) // if the obstacle car is moving into/out of my lane, or me his
            {
                if (obstacle.Distance > carPos.Distance)
                    return obstacle.Distance - carPos.Distance;
                else
                    return (obstacle.Distance + config.HighwayMaxSize - carPos.Distance) % config.HighwayMaxSize;
            }
        }

        return 10.0f; // an arbitrary "lane is clear as far as you can see" distance
    }
}