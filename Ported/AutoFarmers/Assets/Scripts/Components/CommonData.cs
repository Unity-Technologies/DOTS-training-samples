using Unity.Entities;

public struct CommonData : IComponentData
{
    public int FarmerCounter;
    public int DroneCounter;
    public int MoneyForDrones;
    public float MoveSmoothForDrones;
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
