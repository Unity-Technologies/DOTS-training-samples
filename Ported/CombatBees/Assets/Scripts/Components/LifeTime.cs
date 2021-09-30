using Unity.Entities;

[GenerateAuthoringComponent]
public struct LifeTime : IComponentData
{
    public float TimeRemaining;
    public float TotalTime;
}