using Unity.Entities;
using Unity.Mathematics;

public struct Settings : IComponentData
{
    public int2 GridSize;
}

public enum TileStates
{
    Empty,
    Store,
    Rock,
    Tilled,
    Seeded,
    Grown
}

public struct TileState : IBufferElementData
{
    public TileStates Value;
}