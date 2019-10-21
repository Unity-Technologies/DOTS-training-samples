using Unity.Entities;

public enum RockState 
{
    Conveyor,
    Held,
    Thrown
}

public struct RockStatus : IComponentData
{
    public RockState Value;
}
