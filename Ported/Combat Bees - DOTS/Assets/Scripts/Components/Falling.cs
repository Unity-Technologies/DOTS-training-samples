using Unity.Entities;
[GenerateAuthoringComponent]
public struct Falling : IComponentData
{
    public bool shouldFall;
    public float timeToLive;
}
