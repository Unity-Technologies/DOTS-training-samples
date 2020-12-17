using Unity.Entities;

public struct CommonData : IComponentData
{
    public int FarmerCount;
    public int DroneCount;
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