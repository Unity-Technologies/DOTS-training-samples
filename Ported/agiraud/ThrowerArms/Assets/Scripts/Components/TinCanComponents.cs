using Unity.Entities;

public enum TinCanState {
    Reset,
    Sitting,
    Falling
}

public struct IsReserved : IComponentData
{
    public bool Value;
}

public struct TinCanStatus : IComponentData
{
    public TinCanState Value;
}


