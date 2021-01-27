using Unity.Entities;

public struct TileDebugColor : IComponentData
{
    public uint laneId;
    public uint tileId;
}
