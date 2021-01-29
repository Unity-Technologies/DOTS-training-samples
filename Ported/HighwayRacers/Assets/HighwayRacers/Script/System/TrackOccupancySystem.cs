using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Rendering;

public struct LaneOccupancy : IBufferElementData
{
    // The 8 bits in Occupied each represent one lane.
    // So if bit 1 is set it means lane 1 is occupied
    public byte Occupied;
}

// Update the track-tile occupancy after CarMovementSystem so that we have the latest car offsets.
// CarMovementSystem will use last frame's tile knowledge to avoid collisions.
[UpdateAfter(typeof(CarMovementSystem))]
public class TrackOccupancySystem : SystemBase
{
    // todo both these are readonly for now since we are not dealing with them changing in various systems
    // We would need to recompute the lane tiles, respawn cars etc.
    public readonly uint LaneCount = 4;
    public static readonly bool ShowDebugTiles = false;
    public static readonly uint TilesPerLane = (uint)((CarMovementSystem.TrackRadius/2.0f) * 4.0f);
    private CarMovementSystem m_CarMovementSystem;

    public DynamicBuffer<LaneOccupancy> GetReadBuffer(uint frame)
    {
        var occupancyEntities = GetEntityQuery(typeof(LaneOccupancy)).ToEntityArray(Allocator.Temp);
        if (frame % 2 == 0)
            return EntityManager.GetBuffer<LaneOccupancy>(occupancyEntities[0]);
        else
            return EntityManager.GetBuffer<LaneOccupancy>(occupancyEntities[1]);
    }

    public DynamicBuffer<LaneOccupancy> GetWriteBuffer(uint frame)
    {
        var occupancyEntities = GetEntityQuery(typeof(LaneOccupancy)).ToEntityArray(Allocator.Temp);
        if (frame % 2 == 0)
            return EntityManager.GetBuffer<LaneOccupancy>(occupancyEntities[1]);
        else
            return EntityManager.GetBuffer<LaneOccupancy>(occupancyEntities[0]);
    }

    unsafe void ResetBuffer(ref DynamicBuffer<LaneOccupancy> buffer)
    {
        buffer.ResizeUninitialized( (int)TilesPerLane);
        int elementSize = UnsafeUtility.SizeOf<LaneOccupancy>();
        UnsafeUtility.MemSet(buffer.GetUnsafePtr(), 0, buffer.Length * elementSize);
    }

    static public int GetMyTile(float carOffset)
    {
        return (int)((carOffset * TilesPerLane) + 0.5f) % (int)TilesPerLane;
    }
    
    static public bool IsTileOccupied(ref DynamicBuffer<LaneOccupancy> buffer, int lane, int tile)
    {
        byte tiles = buffer[tile].Occupied;
        byte laneByte = (byte) (1 << (int)lane);

        return (tiles & laneByte) != 0;
    }

    protected override void OnCreate()
    {
        m_CarMovementSystem = World.GetExistingSystem<CarMovementSystem>();

        var readEntity = EntityManager.CreateEntity(typeof(LaneOccupancy));
        DynamicBuffer<LaneOccupancy> readBuffer = EntityManager.AddBuffer<LaneOccupancy>(readEntity);
        ResetBuffer(ref readBuffer);

        var writeEntity = EntityManager.CreateEntity(typeof(LaneOccupancy));
        DynamicBuffer<LaneOccupancy> writeBuffer = EntityManager.AddBuffer<LaneOccupancy>(writeEntity);
        ResetBuffer(ref writeBuffer);
    }

    protected override void OnUpdate()
    {
        // Reset the write occupancy for each lane to 0 for all tiles
        var writeBuffer = GetWriteBuffer(m_CarMovementSystem.Frame);
        ResetBuffer(ref writeBuffer);

        Entities
            .WithNativeDisableContainerSafetyRestriction(writeBuffer)
            .ForEach((Entity vehicle, in CarMovement movement) =>
            {
                int myLane = (int)movement.Lane;
                int myTile = GetMyTile(movement.Offset);
                writeBuffer.ElementAt(myTile).Occupied |= (byte) (1 << myLane);
            })
            .ScheduleParallel();

        if (ShowDebugTiles)
        {
            Entities
                .WithNativeDisableContainerSafetyRestriction(writeBuffer)
                .ForEach((Entity tileEntity, ref URPMaterialPropertyBaseColor tileDebugMat, in TileDebugColor tileDebugColor) =>
                {
                    byte tiles = writeBuffer[(int)tileDebugColor.tileId].Occupied;
                    byte lane = (byte) (1 << (int)tileDebugColor.laneId);

                    if((tiles & lane) == 0)
                        tileDebugMat.Value = new Unity.Mathematics.float4(0.5f,0.5f,0.5f,1);
                    else
                        tileDebugMat.Value = new Unity.Mathematics.float4(1.0f,0.0f,0.0f,1);
                })
                .ScheduleParallel();
        }
    }
}
