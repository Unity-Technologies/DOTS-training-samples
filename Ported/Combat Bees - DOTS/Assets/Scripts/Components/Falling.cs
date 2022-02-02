using Unity.Entities;

public struct Falling : IComponentData
{
    public bool shouldFall;
    public float timeToLive;
}
