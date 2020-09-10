using Unity.Entities;

[GenerateAuthoringComponent]
public struct Score : IComponentData
{
    public int Value;
}

public struct ScoreEvent : IComponentData
{
    public int Addition;
    public float Scale;
}
