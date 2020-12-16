using Unity.Entities;

public struct CommonData : IComponentData
{
    public int FarmerCounter;
    public int DroneCounter;
}

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