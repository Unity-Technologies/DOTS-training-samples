using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public enum ETileState
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
    public ETileState Value;
}

public struct Tile : IComponentData
{
    public ETileState State;
}

public struct EmptyTile  : IComponentData {}

public struct GrownTile  : IComponentData {}

public struct RockTile   : IComponentData {}

public struct SeededTile : IComponentData {}

public struct StoreTile  : IComponentData {}

public struct TilledTile : IComponentData {}