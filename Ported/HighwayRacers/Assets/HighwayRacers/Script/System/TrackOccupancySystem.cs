using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Rendering;

public struct LaneOccupancy : IBufferElementData
{
    public bool Occupied;
}

// Update the track-tile occupancy after CarMovementSystem so that we have the latest car offsets.
// CarMovementSystem will use last frame's tile knowledge to avoid collisions.
[UpdateAfter(typeof(CarMovementSystem))]
public class TrackOccupancySystem : SystemBase
{
    // todo both these are readonly for now since we are not dealing with them changing in various systems
    // We would need to recompute the lane tiles, respawn cars etc.
    public readonly float TrackSize = 20;
    public readonly uint LaneCount = 4;
// todo the '64' needs to be the number of tiles we want to subdivide each lane (length of car + circumference of lane?)
    public static readonly uint TilesPerLane = 64;
    public static bool[,] Occupancy = new bool[4,TilesPerLane]; 

    //unsafe void ResetBuffer(ref DynamicBuffer<LaneOccupancy> buffer)
    //{
    //    buffer.ResizeUninitialized( (int)TilesPerLane);
    //    int size = UnsafeUtility.SizeOf<LaneOccupancy>();
    //    UnsafeUtility.MemSet(buffer.GetUnsafePtr(),
    //    0,
    //    buffer.Length * UnsafeUtility.SizeOf<bool>());
    //}

    protected override void OnCreate()
    {
        for (uint i=0; i<LaneCount; i++)
        {
            var entity = EntityManager.CreateEntity(typeof(LaneOccupancy));
            EntityManager.SetName(entity, "Lane" + i);
            DynamicBuffer<LaneOccupancy> buffer = EntityManager.AddBuffer<LaneOccupancy>(entity);
            //ResetBuffer(ref buffer);
        }
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

        // Reset the occupancy for each lane to 0 for all tiles
//        UnsafeUtility.MemSet(Occupancy, 0, UnsafeUtility.SizeOf<bool>()); 
        for(int i = 0; i < LaneCount; ++i)
        {
            for(int j = 0; j < TilesPerLane; ++j)
            {
                Occupancy[i,j] = false;
            }
        }

        //Entities
        //    .ForEach((Entity lane, DynamicBuffer<LaneOccupancy> buffer) =>
        //    {
        //    ResetBuffer(ref buffer);
        //    }).WithoutBurst().Run();

        uint tilesPerLane = TilesPerLane;

        Entities
            .ForEach((Entity vehicle, ref CarMovement movement) =>
            {
                float trackPos = movement.Offset;
                int myLane = (int)movement.Lane;
                int myTile = (int) (trackPos * tilesPerLane);
                Occupancy[myLane, myTile] = true;
                
                //Entities
                //    .ForEach((Entity lane, DynamicBuffer<LaneOccupancy> buffer) =>
                //    {
                //        buffer.ElementAt(myTile).Occupied = true;
                //    }).ScheduleParallel();

                //var lanes = EntityManager.GetBuffer<LaneOccupancy>(vehicle);
                //var myLane = lanes[(int)movement.Lane];
// todo not sure how we set a value in the buffer?
                //buffers[myTile];

            })
//.WithDisposeOnCompletion(lanes)
//.WithDisposeOnCompletion(buffers)
                .ScheduleParallel();

        Entities
            .ForEach((Entity tileEntity, ref TileDebugColor tileDebugColor, ref URPMaterialPropertyBaseColor tileDebugMat) =>
            {
                if(Occupancy[0,tileDebugColor.tileId] == true)
                    tileDebugMat.Value = new Unity.Mathematics.float4(1.0f,0.0f,0.0f,1);
                else
                    tileDebugMat.Value = new Unity.Mathematics.float4(0.5f,0.5f,0.5f,1);
            })
            .ScheduleParallel();


/*
var buffer = EntityManager.GetBuffer<ReferencedBiomes>(layer);
foreach (var element in buffer)
{
    var biomeID = element.BiomeID;
    if (!biomeIds.Contains(biomeID) &&
        m_BiomeIdToEntityAndContentHash.TryGetValue(biomeID, out var entityAndContentHash))
    {
        biomeIds.Add(biomeID);
        biomeEntities.Add(new EntityAndHash { Entity = entityAndContentHash.Entity, BiomeID = biomeID });
    }
}
*/
    }
}
