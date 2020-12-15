using Unity.Entities;

public struct Settings : IComponentData
{
    
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