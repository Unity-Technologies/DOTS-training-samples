using Unity.Entities;

[GenerateAuthoringComponent]
public struct Ticker : IComponentData
{
    public float TimeRemaining;
}
