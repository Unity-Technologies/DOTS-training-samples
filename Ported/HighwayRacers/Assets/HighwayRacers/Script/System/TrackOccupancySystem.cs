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

// todo should be based on length of car + circumference of lane + total cars
    public static readonly uint TilesPerLane = (uint)((CarMovementSystem.TrackRadius/2.0f) * 4.0f);

    public static bool[,] Occupancy = new bool[4,TilesPerLane];
    public static bool[,] ReadOccupancy = new bool[4,TilesPerLane]; 

    static public int GetMyTile(float carOffset)
    {
        return (int)((carOffset * TilesPerLane) + 0.5f) % (int)TilesPerLane;
    }

    protected override void OnUpdate()
    {
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
