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
    public readonly float TrackSize = 20;
    public readonly uint LaneCount = 4;

// todo should be based on length of car + circumference of lane + total cars
    public static readonly uint TilesPerLane = 256;

    public static bool[,] Occupancy = new bool[4,TilesPerLane];
    public static bool[,] ReadOccupancy = new bool[4,TilesPerLane]; 

    static public int GetMyTile(float carOffset)
    {
        return (int)((carOffset * TilesPerLane) + 0.5f) % (int)TilesPerLane;
    }

    protected override void OnUpdate()
    {
// todo read the current 'Offset' and 'Lane' of each vehicle entitiy.
// todo based on that determine the 'tiles' that the car is in and block that tile for other cars.
// todo store this data on '4' "Lane" enitities that use a DynamicBuffer in its component?
// todo for cars NOT switching lanes, we only have to check our lane and the 'tile' in front our current tile.
// todo we can use last frames 'tile' information in "CarMovementSystem"
// todo cars that are going slower than desired (blocked) want to switch lanes and need to check the lane to the right
//      or to the left. We will alternative right and left every other frame so we don't ahve to worry about
//      two cars merging into the same lane.

        // Copy the occupancy before resetting so that CarMovementSystem can read it.
        ReadOccupancy = Occupancy.Clone() as bool[,];

        // Reset the occupancy for each lane to 0 for all tiles
        for(int i = 0; i < LaneCount; ++i)
        {
            for(int j = 0; j < TilesPerLane; ++j)
            {
                Occupancy[i,j] = false;
            }
        }

        uint tilesPerLane = TilesPerLane;

        Entities
            .ForEach((Entity vehicle, ref CarMovement movement) =>
            {
                int myLane = (int)movement.Lane;
                int myTile = GetMyTile(movement.Offset);
                Occupancy[myLane, myTile] = true;
            })
                .ScheduleParallel();

        Entities
            .ForEach((Entity tileEntity, ref TileDebugColor tileDebugColor, ref URPMaterialPropertyBaseColor tileDebugMat) =>
            {
                if(Occupancy[tileDebugColor.laneId, tileDebugColor.tileId] == true)
                    tileDebugMat.Value = new Unity.Mathematics.float4(1.0f,0.0f,0.0f,1);
                else
                    tileDebugMat.Value = new Unity.Mathematics.float4(0.5f,0.5f,0.5f,1);
            })
            .ScheduleParallel();
    }
}
