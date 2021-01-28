using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Rendering;

// Update the track-tile occupancy after CarMovementSystem so that we have the latest car offsets.
// CarMovementSystem will use last frame's tile knowledge to avoid collisions.
[UpdateAfter(typeof(CarMovementSystem))]
public class TrackOccupancySystem : SystemBase
{
    // todo both these are readonly for now since we are not dealing with them changing in various systems
    // We would need to recompute the lane tiles, respawn cars etc.
    //public readonly float TrackSize = 20;
    public readonly uint LaneCount = 4;

    public static readonly uint TilesPerLane = (uint)((CarMovementSystem.TrackRadius/2.0f) * 4.0f);

    private static uint Frame = 0;

    public static bool[,] GetReadOccupancy()
    {
        if (Frame % 2 == 0) 
            return OccupancyA;
        else
            return OccupancyB;
    }

    public static bool[,] GetWriteOccupancy()
    {
        if (Frame % 2 == 0) 
            return OccupancyB;
        else
            return OccupancyA;
    }

    private static bool[,] OccupancyA = new bool[4,TilesPerLane];
    private static bool[,] OccupancyB = new bool[4,TilesPerLane]; 

    static public int GetMyTile(float carOffset)
    {
        return (int)((carOffset * TilesPerLane) + 0.5f) % (int)TilesPerLane;
    }

    protected override void OnUpdate()
    {
        Frame++;

        // Reset the occupancy for each lane to 0 for all tiles
        for(int i = 0; i < LaneCount; ++i)
        {
            for(int j = 0; j < TilesPerLane; ++j)
            {
                GetWriteOccupancy()[i,j] = false;
            }
        }

        uint tilesPerLane = TilesPerLane;

        Entities
            .ForEach((Entity vehicle, ref CarMovement movement) =>
            {
                int myLane = (int)movement.Lane;
                int myTile = GetMyTile(movement.Offset);
                GetWriteOccupancy()[myLane, myTile] = true;
            })
                .ScheduleParallel();

        Entities
            .ForEach((Entity tileEntity, ref TileDebugColor tileDebugColor, ref URPMaterialPropertyBaseColor tileDebugMat) =>
            {
                if(GetWriteOccupancy()[tileDebugColor.laneId, tileDebugColor.tileId] == true)
                    tileDebugMat.Value = new Unity.Mathematics.float4(1.0f,0.0f,0.0f,1);
                else
                    tileDebugMat.Value = new Unity.Mathematics.float4(0.5f,0.5f,0.5f,1);
            })
            .ScheduleParallel();
    }
}
