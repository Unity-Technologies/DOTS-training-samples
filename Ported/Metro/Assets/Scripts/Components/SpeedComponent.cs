using Unity.Entities;

public struct SpeedComponent : IComponentData
{
    public float Current;
    public float Max;
}